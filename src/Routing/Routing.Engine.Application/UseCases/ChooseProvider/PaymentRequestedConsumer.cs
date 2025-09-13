using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.Extensions.Logging;
using Routing.Engine.Application.Contracts;

namespace Routing.Engine.Application.UseCases.ChooseProvider
{
    public class PaymentRequestedConsumer(
        IChooseProviderUseCase useCase,
        ITopicProducer<ProviderChosen> producer,
        ILogger<PaymentRequestedConsumer> logger
    ) : IConsumer<PaymentRequested>
    {
        public async Task Consume(ConsumeContext<PaymentRequested> context)
        {
            var msg = context.Message;

            logger.LogInformation("Routing started: PaymentId={PaymentId}", msg.PaymentId);
            var chosen = useCase.Execute(
                msg.PaymentId,
                msg.Amount,
                msg.Currency,
                msg.RequestedAtUtc
            );
            await producer.Produce(chosen, context.CancellationToken);
        }
    }
}
