using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payments.Application.UseCases.CreatePayment;
using Payments.Domain;

namespace Payments.Application.Abstractions
{
    public interface ICreatePaymentUseCase
    {
        Task<Payment> ExecuteAsync(CreatePaymentInput input, CancellationToken cancellationToken);
    }
}
