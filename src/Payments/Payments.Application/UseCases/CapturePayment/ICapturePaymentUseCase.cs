using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.Application.UseCases.CapturePayment
{
    public interface ICapturePaymentUseCase
    {
        Task ExecuteAsync(Guid paymentId, CancellationToken cancellationToken);
    }
}
