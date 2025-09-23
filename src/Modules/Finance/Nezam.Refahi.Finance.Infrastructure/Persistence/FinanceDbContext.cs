using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence;

/// <summary>
/// Database context for Finance bounded context
/// Handles all financial entities including bills, payments, refunds, and transactions
/// </summary>
public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    // Finance bounded context entities
    public DbSet<Bill> Bills { get; set; } = default!;
    public DbSet<BillItem> BillItems { get; set; } = default!;
    public DbSet<Payment> Payments { get; set; } = default!;
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = default!;
    public DbSet<Refund> Refunds { get; set; } = default!;
    
    // Wallet entities
    public DbSet<Wallet> Wallets { get; set; } = default!;
    public DbSet<WalletTransaction> WalletTransactions { get; set; } = default!;
    public DbSet<WalletDeposit> WalletDeposits { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        // This automatically discovers and applies all IEntityTypeConfiguration<T> implementations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Set default schema for Finance module
        modelBuilder.HasDefaultSchema("finance");
    }
}