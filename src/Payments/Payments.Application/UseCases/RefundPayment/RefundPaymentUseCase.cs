using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Payments.Application.Abstractions;
using Payments.Domain;
using Payments.Infrastructure.Outbox;

namespace Payments.Application.UseCases.RefundPayment
{
    public class RefundPaymentUseCase(
        IPaymentRepository repo,
        IUnitOfWork uow,
        IOutboxWriter outbox
    ) : IRefundPaymentUseCase
    {
        public async Task ExecuteAsync(Guid paymentId, CancellationToken ct)
        {
            var payment = await repo.GetByIdAsync(paymentId, ct);

            if (
                payment == null
                || payment.Status != PaymentStatus.Authorized
                    && payment.Status != PaymentStatus.Captured
            )
                return;

            var evt = new
            {
                PaymentId = payment.Id,
                payment.Amount,
                payment.Currency,
            };
            var payload = JsonSerializer.Serialize(evt);

            await outbox.EnqueueAsync("PaymentRefundRequested", payload, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
}
