using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Domain
{
    public class Payment
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentStatus Status { get; set; }

        private Payment() { }

        public Payment(decimal amount, string currency)
        {
            Id = Guid.NewGuid();
            Amount = amount;
            Currency = currency;
            Status = PaymentStatus.Created;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
