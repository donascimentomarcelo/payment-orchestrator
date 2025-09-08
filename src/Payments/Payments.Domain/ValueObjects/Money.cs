using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Domain.ValueObjects
{
    public readonly record struct Money(decimal Amount, string Currency)
    {
        // public Money(decimal Amount, string Currency)
        // {
        //     if (Amount < 0)
        //         throw new ArgumentOutOfRangeException(nameof(Amount), "Amount cannot be negative.");

        //     if (string.IsNullOrWhiteSpace(Currency))
        //         throw new ArgumentException("Currency cannot be null or empty.", nameof(Currency));

        //     this.Amount = Amount;
        //     this.Currency = Currency.ToUpperInvariant();
        // }
    }
}