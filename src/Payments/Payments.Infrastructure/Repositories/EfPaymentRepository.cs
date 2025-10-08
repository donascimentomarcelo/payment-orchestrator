using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payments.Application.Abstractions;
using Payments.Domain;
using Payments.Infrastructure.Persistence;

namespace Payments.Infrastructure.Repositories
{
    public class EfPaymentRepository(PaymentsDbContext context) : IPaymentRepository
    {
        private readonly PaymentsDbContext _context = context;

        public Task AddAsync(Payment payment, CancellationToken cancellationToken)
        {
            _context.Payments.Add(payment);
            return Task.CompletedTask;
        }

        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            _context.Payments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
