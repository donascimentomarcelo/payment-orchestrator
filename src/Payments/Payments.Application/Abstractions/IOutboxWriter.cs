using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Infrastructure.Outbox
{
    public interface IOutboxWriter
    {
        Task EnqueueAsync(string type, string payload, CancellationToken cancellationToken);
    }
}
