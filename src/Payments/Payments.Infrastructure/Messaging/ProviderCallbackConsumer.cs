using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using Confluent.Kafka;
using MassTransit;
using Payments.Application.UseCases.ProcessCallback;

namespace Payments.Infrastructure.Messaging
{
    public class ProviderCallbackConsumer(IProcessProviderCallbackUseCase useCase)
        : IConsumer<ProviderCallback>
    {
        public async Task Consume(ConsumeContext<ProviderCallback> context)
        {
            await useCase.ExecuteAsync(context.Message, context.CancellationToken);
        }
    }
}
