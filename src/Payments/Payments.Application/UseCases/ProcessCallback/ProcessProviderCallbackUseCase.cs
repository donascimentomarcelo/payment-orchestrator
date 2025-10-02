using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Messaging;
using Microsoft.Extensions.Logging;
using Payments.Application.Abstractions;
using Payments.Domain;

namespace Payments.Application.UseCases.ProcessCallback
{
    public class ProcessProviderCallbackUseCase(
        IPaymentRepository repo,
        IUnitOfWork uow,
        ILogger<ProcessProviderCallbackUseCase> logger
    ) : IProcessProviderCallbackUseCase
    {
        private readonly IPaymentRepository _repo = repo;
        private readonly IUnitOfWork _uow = uow;

        public async Task ExecuteAsync(ProviderCallback callback, CancellationToken ct)
        {
            var payment = await _repo.GetByIdAsync(callback.PaymentId, ct);
            if (payment == null)
            {
                logger.LogInformation("No payment found");
                return;
            }

            switch (callback.Status.ToLowerInvariant())
            {
                case "authorized":
                    if (payment.Status == PaymentStatus.Requested)
                        payment.MarkAuthorized();
                    break;
                case "failed":
                    if (payment.Status == PaymentStatus.Requested)
                        payment.MarkFailed();
                    break;
            }

            await _uow.SaveChangesAsync(ct);
        }
    }
}
