using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Payments.Application.Abstractions;

namespace Payments.Infrastructure.Persistence
{
    public class UnitOfWork(PaymentsDbContext dbContext) : IUnitOfWork
    {
        private readonly PaymentsDbContext _dbContext = dbContext;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
