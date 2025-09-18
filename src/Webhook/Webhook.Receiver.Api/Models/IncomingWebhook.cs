using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webhook.Receiver.Api.Models
{
    public record IncomingWebhook(Guid PaymentId, string Status);
}
