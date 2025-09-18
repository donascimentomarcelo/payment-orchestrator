using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging
{
    public record ProviderCallback(
        [property: JsonPropertyName("paymentId")] Guid PaymentId,
        [property: JsonPropertyName("provider")] string Provider,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("receivedAtUtc")] DateTime ReceivedAtUtc
    );
}
