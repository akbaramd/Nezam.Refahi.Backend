using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Shared.Domain.Entities;

namespace Nezam.Refahi.Shared.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for OutboxMessage with comprehensive failure handling
/// </summary>
public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "shared");

        // Primary key
        builder.HasKey(x => x.Id);

        // Basic event information
        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.FullTypeName)
            .IsRequired()
            .HasMaxLength(1000)
            .HasColumnType("nvarchar(1000)");

        builder.Property(x => x.AssemblyName)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.OccurredOn)
            .IsRequired()
            .HasColumnType("datetime2");

        // Processing status
        builder.Property(x => x.ProcessedOn)
            .HasColumnType("datetime2");

        builder.Property(x => x.Error)
            .HasMaxLength(2000)
            .HasColumnType("nvarchar(2000)");

        builder.Property(x => x.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.MaxRetries)
            .IsRequired()
            .HasDefaultValue(3);

        // Enhanced failure handling
        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.AggregateId)
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.SchemaVersion)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.NextRetryAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.FailureReason)
            .HasMaxLength(2000)
            .HasColumnType("nvarchar(2000)");

        builder.Property(x => x.IsPoisonMessage)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.PoisonedAt)
            .HasColumnType("datetime2");

        // DLQ management
        builder.Property(x => x.MovedToDlqAt)
            .HasColumnType("datetime2");

        builder.Property(x => x.DlqReason)
            .HasMaxLength(2000)
            .HasColumnType("nvarchar(2000)");

        // Indexes for performance
        builder.HasIndex(x => x.ProcessedOn)
            .HasDatabaseName("IX_OutboxMessages_ProcessedOn");

        builder.HasIndex(x => x.OccurredOn)
            .HasDatabaseName("IX_OutboxMessages_OccurredOn");

        builder.HasIndex(x => x.Type)
            .HasDatabaseName("IX_OutboxMessages_Type");

        builder.HasIndex(x => x.FullTypeName)
            .HasDatabaseName("IX_OutboxMessages_FullTypeName");

        builder.HasIndex(x => x.AssemblyName)
            .HasDatabaseName("IX_OutboxMessages_AssemblyName");

        // Composite index for processing
        builder.HasIndex(x => new { x.ProcessedOn, x.RetryCount })
            .HasDatabaseName("IX_OutboxMessages_ProcessedOn_RetryCount");

        // Indexes for new fields
        builder.HasIndex(x => x.IdempotencyKey)
            .HasDatabaseName("IX_OutboxMessages_IdempotencyKey")
            .HasFilter("[IdempotencyKey] IS NOT NULL");

        builder.HasIndex(x => x.AggregateId)
            .HasDatabaseName("IX_OutboxMessages_AggregateId")
            .HasFilter("[AggregateId] IS NOT NULL");

        builder.HasIndex(x => x.CorrelationId)
            .HasDatabaseName("IX_OutboxMessages_CorrelationId")
            .HasFilter("[CorrelationId] IS NOT NULL");

        builder.HasIndex(x => x.NextRetryAt)
            .HasDatabaseName("IX_OutboxMessages_NextRetryAt")
            .HasFilter("[NextRetryAt] IS NOT NULL");

        builder.HasIndex(x => x.MovedToDlqAt)
            .HasDatabaseName("IX_OutboxMessages_MovedToDlqAt")
            .HasFilter("[MovedToDlqAt] IS NOT NULL");

        builder.HasIndex(x => x.IsPoisonMessage)
            .HasDatabaseName("IX_OutboxMessages_IsPoisonMessage");

        // Composite indexes for common queries
        builder.HasIndex(x => new { x.Type, x.ProcessedOn, x.RetryCount })
            .HasDatabaseName("IX_OutboxMessages_Type_ProcessedOn_RetryCount");

        builder.HasIndex(x => new { x.AggregateId, x.OccurredOn })
            .HasDatabaseName("IX_OutboxMessages_AggregateId_OccurredOn")
            .HasFilter("[AggregateId] IS NOT NULL");

        builder.HasIndex(x => new { x.CorrelationId, x.OccurredOn })
            .HasDatabaseName("IX_OutboxMessages_CorrelationId_OccurredOn")
            .HasFilter("[CorrelationId] IS NOT NULL");
    }
}
