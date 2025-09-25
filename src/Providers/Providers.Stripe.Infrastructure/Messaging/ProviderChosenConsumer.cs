using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Providers.Stripe.Infrastructure.Messaging
{
    public class ProviderChosenConsumer(
        PaymentIntentService paymentIntents,
        ILogger<ProviderChosenConsumer> logger
    ) : IConsumer<ProviderChosen>
    {
        private readonly PaymentIntentService _paymentIntents = paymentIntents;

        public async Task Consume(ConsumeContext<ProviderChosen> context)
        {
            Console.WriteLine($"[DEBUG] Consumer started");
            var chosen = context.Message;

            if (!string.Equals(chosen.Provider, "stripe", StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                var createOptions = new PaymentIntentCreateOptions
                {
                    Amount = (long)chosen.Amount,
                    Currency = chosen.Currency,
                    CaptureMethod = "manual",
                };

                Console.WriteLine($"[DEBUG] Striper object to read {createOptions}", createOptions);

                var reqOpts = new RequestOptions { IdempotencyKey = chosen.PaymentId.ToString() };

                Console.WriteLine($"[DEBUG] Stripe Service Invoked");
                var intent = await _paymentIntents.CreateAsync(
                    createOptions,
                    reqOpts,
                    context.CancellationToken
                );
                Console.WriteLine($"[DEBUG] Stripe Service ran successfully");

                var callback = new ProviderCallback(
                    PaymentId: chosen.PaymentId,
                    Provider: "stripe",
                    Status: "authorized",
                    ReceivedAtUtc: DateTime.UtcNow
                );

                await context.Publish(callback);
            }
            catch (StripeException se)
            {
                await context.Publish(
                    new ProviderCallback(chosen.PaymentId, "stripe", "failed", DateTime.UtcNow)
                );
                throw;
            }
            catch (Exception)
            {
                await context.Publish(
                    new ProviderCallback(chosen.PaymentId, "stripe", "failed", DateTime.UtcNow)
                );
                throw;
            }
        }
    }
}
