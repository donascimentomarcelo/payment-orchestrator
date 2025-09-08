using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payments.Application.Abstractions;
using Payments.Domain;

namespace Payments.Application.UseCases.CreatePayment
{
    public class CreatePaymentUseCase : ICreatePaymentUseCase
    {
        private readonly IPaymentRepository _repo;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePaymentUseCase(IPaymentRepository repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Payment> ExecuteAsync(CreatePaymentInput input, CancellationToken cancellationToken)
        {
            var payment = new Payment(input.Amount, input.Currency);

            await _repo.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return payment;
        }
    }
}