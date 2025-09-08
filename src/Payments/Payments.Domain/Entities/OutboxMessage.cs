using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string Payload { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Published { get; set; } = false;
    }
}
