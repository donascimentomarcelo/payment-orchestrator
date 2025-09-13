using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging
{
    public record ProviderChosen(Guid PaymentId, string Provider, DateTime ChosenAtUtc);
}
