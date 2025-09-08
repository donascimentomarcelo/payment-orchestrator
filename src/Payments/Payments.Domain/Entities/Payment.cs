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

        public Payment(decimal amount, string currency)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
                throw new ArgumentException("Currency must be 3 letters.", nameof(currency));

            Id = Guid.NewGuid();
            Amount = amount;
            Currency = currency.ToUpperInvariant();
            Status = PaymentStatus.Requested;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
