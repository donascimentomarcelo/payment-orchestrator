using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payments.Domain;
using Payments.Domain.Entities;

namespace Payments.Infrastructure
{
    public class PaymentsDbContext : DbContext
    {
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>().ToTable("Payments").HasKey(p => p.Id);
            modelBuilder.Entity<OutboxMessage>().ToTable("Outbox").HasKey(o => o.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
