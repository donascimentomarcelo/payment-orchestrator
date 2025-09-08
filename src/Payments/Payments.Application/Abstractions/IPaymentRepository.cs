using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payments.Domain;

namespace Payments.Application.Abstractions
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment, CancellationToken cancellationToken);
        Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
