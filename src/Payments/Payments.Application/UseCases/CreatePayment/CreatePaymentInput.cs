using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Application.UseCases.CreatePayment
{
    public record CreatePaymentInput(decimal Amount, string Currency);
}
