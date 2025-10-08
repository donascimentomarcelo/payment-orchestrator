using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Application.UseCases.RefundPayment
{
    public interface IRefundPaymentUseCase
    {
        Task ExecuteAsync(Guid paymentId, CancellationToken ct);
    }
}
