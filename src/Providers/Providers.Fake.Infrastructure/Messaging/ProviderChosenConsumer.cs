using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Providers.Fake.Infrastructure.Messaging
{
    public class ProviderChosenConsumer(
        ITopicProducer<ProviderCallback> producer,
        ILogger<ProviderChosenConsumer> log
    ) : IConsumer<ProviderChosen>
    {
        public async Task Consume(ConsumeContext<ProviderChosen> context)
        {
            var chosen = context.Message;

            log.LogInformation(
                "ProviderChosen received: PaymentId={PaymentId}, Provider={Provider}",
                chosen.PaymentId,
                chosen.Provider
            );

            var callback = new ProviderCallback(
                chosen.PaymentId,
                chosen.Provider,
                Status: "authorized",
                ReceivedAtUtc: DateTime.UtcNow
            );

            await producer.Produce(callback, context.CancellationToken);
            log.LogInformation("ProviderCallback sent for PaymentId={PaymentId}", chosen.PaymentId);
        }
    }
}
