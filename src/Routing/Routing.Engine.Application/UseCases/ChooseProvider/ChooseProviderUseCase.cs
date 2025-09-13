using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using Routing.Engine.Application.Contracts;
using Routing.Engine.Application.Strategy;

namespace Routing.Engine.Application.UseCases.ChooseProvider
{
    public class ChooseProviderUseCase(IProviderRoutingStrategy strategy) : IChooseProviderUseCase
    {
        private readonly IProviderRoutingStrategy _strategy = strategy;

        public ProviderChosen Execute(
            Guid paymentId,
            decimal amount,
            string currency,
            DateTime requestedAtUtc
        )
        {
            var provider = _strategy.ChooseProvider(amount, currency);
            return new ProviderChosen(paymentId, provider, DateTime.UtcNow);
        }
    }
}
