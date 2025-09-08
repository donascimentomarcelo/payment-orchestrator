using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Api.DTOs
{
    public record CreatePaymentDto(decimal Amount, string Currency);
}