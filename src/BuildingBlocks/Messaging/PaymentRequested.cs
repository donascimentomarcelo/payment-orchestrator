using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging
{
    public record PaymentRequested(
        Guid PaymentId,
        decimal Amount,
        string Currency,
        DateTime RequestedAtUtc
    );
}