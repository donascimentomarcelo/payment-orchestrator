using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payments.Domain;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Persistence
{
    public class PaymentsDbContext : DbContext
    {
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(p =>
            {
                p.ToTable("Payments");
                p.HasKey(p => p.Id);
                p.Property(p => p.Amount).IsRequired();
                p.Property(p => p.Currency).IsRequired();
                p.Property(p => p.Status).IsRequired();
                p.Property(p => p.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<OutboxMessage>().ToTable("Outbox").HasKey(o => o.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
