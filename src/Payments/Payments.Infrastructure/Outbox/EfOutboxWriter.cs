using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payments.Domain.Entities;
using Payments.Infrastructure.Persistence;

namespace Payments.Infrastructure.Outbox
{
    public class EfOutboxWriter(PaymentsDbContext db) : IOutboxWriter
    {
        private readonly PaymentsDbContext _db = db;

        public Task EnqueueAsync(string type, string payload, CancellationToken cancellationToken)
        {
            _db.Outbox.Add(
                new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = type,
                    Payload = payload,
                    CreatedAt = DateTime.UtcNow,
                    Published = false,
                }
            );

            return Task.CompletedTask;
        }
    }
}
