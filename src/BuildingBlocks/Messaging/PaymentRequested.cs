using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging
{
    public record PaymentRequested(
        [property: JsonPropertyName("paymentId")] Guid PaymentId,
        [property: JsonPropertyName("amount")] decimal Amount,
        [property: JsonPropertyName("currency")] string Currency,
        [property: JsonPropertyName("requestedAtUtc")] DateTime RequestedAtUtc
    );
}
