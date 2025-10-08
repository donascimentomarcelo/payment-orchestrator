using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Payments.Application.Abstractions;
using Payments.Domain;
using Payments.Infrastructure.Outbox;

namespace Payments.Application.UseCases.CapturePayment
{
    public class CapturePaymentUseCase(
        IPaymentRepository repo,
        IUnitOfWork unitOfWork,
        IOutboxWriter outbox
    ) : ICapturePaymentUseCase
    {
        public async Task ExecuteAsync(Guid paymentId, CancellationToken ct)
        {
            var payment = await repo.GetByIdAsync(paymentId, ct);
            if (payment == null || payment.Status != PaymentStatus.Authorized)
                return;

            var evt = new
            {
                PaymentId = payment.Id,
                payment.Amount,
                payment.Currency,
            };
            var payload = JsonSerializer.Serialize(evt);

            await outbox.EnqueueAsync("PaymentCaptureRequested", payload, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
