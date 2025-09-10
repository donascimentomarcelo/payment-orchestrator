using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Payments.Infrastructure.Persistence;
using Polly;

namespace Payments.Infrastructure.Messaging
{
    public class OutboxPublisher(ILogger<OutboxPublisher> logger, IServiceProvider sp)
        : BackgroundService
    {
        private readonly ILogger<OutboxPublisher> _logger = logger;
        private readonly IServiceProvider _sp = sp;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxPublisher started");

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryForeverAsync(
                    attempt => TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, attempt))),
                    (ex, ts) => _logger.LogWarning(ex, "Retrying outbox publish in {Delay}", ts)
                );

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        using var scope = _sp.CreateScope();

                        var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                        var producer = scope.ServiceProvider.GetRequiredService<
                            ITopicProducer<PaymentRequested>
                        >();

                        var batch = await db
                            .Outbox.Where(o => !o.Published)
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
                                    var evt = JsonSerializer.Deserialize<PaymentRequested>(
                                        msg.Payload
                                    )!;
                                    await producer.Produce(evt, stoppingToken);
                                    msg.Published = true;
                                    break;

                                default:
                                    _logger.LogWarning("Unknown outbox Type: {Type}", msg.Type);
                                    msg.Published = true;
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
