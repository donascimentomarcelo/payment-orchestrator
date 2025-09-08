using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Payments.Application.Abstractions;
using Payments.Domain;
using Payments.Infrastructure.Outbox;

namespace Payments.Application.UseCases.CreatePayment
{
    public class CreatePaymentUseCase(
        IPaymentRepository repo,
        IUnitOfWork unitOfWork,
        IOutboxWriter outboxWriter
    ) : ICreatePaymentUseCase
    {
        private readonly IPaymentRepository _repo = repo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOutboxWriter _outboxWriter = outboxWriter;

        public async Task<Payment> ExecuteAsync(CreatePaymentInput input, CancellationToken ct)
        {
            var payment = new Payment(input.Amount, input.Currency);

            await _repo.AddAsync(payment, ct);

            var evt = new
            {
                Type = "PaymentRequested",
                payment.Id,
                payment.Amount,
                payment.Currency,
                payment.CreatedAt,
            };
            var payload = JsonSerializer.Serialize(evt);
            await _outboxWriter.EnqueueAsync("PaymentRequested", payload, ct);

            await _unitOfWork.SaveChangesAsync(ct);
            return payment;
        }
    }
}
