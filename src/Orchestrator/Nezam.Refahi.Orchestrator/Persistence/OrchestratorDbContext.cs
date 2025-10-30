using Microsoft.EntityFrameworkCore;
using MassTransit;
using Nezam.Refahi.Orchestrator.Sagas.ReservationPayment;
using Nezam.Refahi.Orchestrator.Sagas.WalletDeposit;
using MassTransit.EntityFrameworkCoreIntegration;

namespace Nezam.Refahi.Orchestrator.Persistence;

/// <summary>
/// DbContext for Orchestrator to persist saga state and MassTransit EF Outbox
/// </summary>
public class OrchestratorDbContext : DbContext
{
    public OrchestratorDbContext(DbContextOptions<OrchestratorDbContext> options) : base(options)
    {
    }

    public DbSet<ReservationPaymentSagaState> ReservationPaymentSagas { get; set; } = default!;
    public DbSet<WalletDepositSagaState> WalletDepositSagas { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Default schema for orchestrator components
        modelBuilder.HasDefaultSchema("orchestrator");

        modelBuilder.Entity<ReservationPaymentSagaState>(entity =>
        {
            entity.ToTable("ReservationPaymentSagas", "orchestrator");
            entity.HasKey(x => x.CorrelationId);
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.TrackingCode).HasMaxLength(100);
            entity.Property(x => x.TourTitle).HasMaxLength(500);
            entity.Property(x => x.BillNumber).HasMaxLength(100);
            entity.Property(x => x.Gateway).HasMaxLength(100);
            entity.Property(x => x.GatewayReference).HasMaxLength(200);
            entity.Property(x => x.GatewayTransactionId).HasMaxLength(200);
            entity.Property(x => x.FailureReason).HasMaxLength(1000);
            entity.Property(x => x.ErrorCode).HasMaxLength(100);
            entity.Property(x => x.TotalAmountRials).HasPrecision(18, 2);
        });

        // MassTransit Outbox/Inbox entities
        modelBuilder.AddInboxStateEntity(c=>c.ToTable("InboxState", "orchestrator"));
        modelBuilder.AddOutboxMessageEntity(c=>c.ToTable("OutboxMessage", "orchestrator"));
        modelBuilder.AddOutboxStateEntity(c=>c.ToTable("OutboxState", "orchestrator"));

        modelBuilder.Entity<WalletDepositSagaState>(entity =>
        {
            entity.ToTable("WalletDepositSagas", "orchestrator");
            entity.HasKey(x => x.CorrelationId);
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.UserFullName).HasMaxLength(200);
            entity.Property(x => x.Currency).HasMaxLength(10);
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.TrackingCode).HasMaxLength(100);
            entity.Property(x => x.BillNumber).HasMaxLength(100);
            entity.Property(x => x.AmountRials).HasPrecision(18, 2);
        });
    }
}


