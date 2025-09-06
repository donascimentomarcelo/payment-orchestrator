using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payments.Infrastructure;
using Polly;

namespace Payments.Api.Publisher
{
    public class OutboxPublisher : BackgroundService
    {

        private readonly ILogger<OutboxPublisher> _logger;
        private readonly IServiceProvider _sp;

        public OutboxPublisher(ILogger<OutboxPublisher> logger, IServiceProvider sp)
        {
            _logger = logger;
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxPublisher started");

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    attempt => TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, attempt))),
                    (ex, ts) => _logger.LogWarning(ex, "Retrying outbox publish in {Delay}", ts));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        using var scope = _sp.CreateScope();

                        var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                        // ✅ Resolva o producer DENTRO do escopo
                        var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<PaymentRequested>>();

                        var batch = await db.Outbox
                            .Where(o => !o.Published)
                            .OrderBy(o => o.CreatedAt)
                            .Take(50)
                            .ToListAsync(stoppingToken);

                        if (batch.Count == 0)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                            return;
                        }

                        foreach (var msg in batch)
                        {
                            switch (msg.Type)
                            {
                                case nameof(PaymentRequested):
                                    var evt = JsonSerializer.Deserialize<PaymentRequested>(msg.Payload)!;
                                    await producer.Produce(evt, stoppingToken);
                                    msg.Published = true;
                                    break;

                                default:
                                    _logger.LogWarning("Unknown outbox Type: {Type}", msg.Type);
                                    msg.Published = true; // ou manter false para investigar
                                    break;
                            }
                        }

                        await db.SaveChangesAsync(stoppingToken);
                    });
                }
                catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fatal error in OutboxPublisher loop");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("OutboxPublisher stopped");
        }

    }
}