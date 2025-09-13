using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;

namespace Routing.Engine.Application.Contracts
{
    public interface IChooseProviderUseCase
    {
        ProviderChosen Execute(
            Guid paymentId,
            decimal amount,
            string currency,
            DateTime requestedAtUtc
        );
    }
}
