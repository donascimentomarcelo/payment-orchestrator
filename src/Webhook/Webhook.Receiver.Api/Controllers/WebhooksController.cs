using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Webhook.Receiver.Api.Models;

namespace Webhook.Receiver.Api.Controllers
{
    [ApiController]
    [Route("webhook/{provider}")]
    public class WebhooksController(ITopicProducer<ProviderCallback> producer, IMemoryCache cache)
        : ControllerBase
    {
        private readonly ITopicProducer<ProviderCallback> _producer = producer;
        private readonly IMemoryCache _cache = cache;
        private const int IdempotencySeconds = 120;

        [HttpPost]
        public async Task<IActionResult> Receive(
            [FromRoute] string provider,
            [FromBody] IncomingWebhook body
        )
        {
            var key = Request.Headers.TryGetValue("Idepotency-Key", out var idk)
                ? $"webhook:{provider}:{idk}"
                : $"webhook:{provider}:{body.PaymentId}:{body.Status}";

            if (_cache.TryGetValue(key, out _))
                return Accepted(new { duplicated = true });

            _cache.Set(key, true, TimeSpan.FromSeconds(IdempotencySeconds));

            var normalized = new ProviderCallback(
                PaymentId: body.PaymentId,
                Provider: provider,
                Status: body.Status?.ToLowerInvariant() ?? "authorized",
                ReceivedAtUtc: DateTime.UtcNow
            );

            await _producer.Produce(normalized);
            return Accepted(new { published = true });
        }
    }
}
