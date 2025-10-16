using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Shared.Domain.Services;

namespace Nezam.Refahi.Shared.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for EventIdempotency
/// </summary>
public class EventIdempotencyConfiguration : IEntityTypeConfiguration<EventIdempotency>
{
    public void Configure(EntityTypeBuilder<EventIdempotency> builder)
    {
        builder.ToTable("EventIdempotencies", "shared");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.AggregateId)
            .HasColumnType("uniqueidentifier");

        builder.Property(x => x.IsProcessed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.ProcessedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(x => x.Error)
            .HasMaxLength(2000)
            .HasColumnType("nvarchar(2000)");

        // Indexes
        builder.HasIndex(x => x.IdempotencyKey)
            .IsUnique()
            .HasDatabaseName("IX_EventIdempotencies_IdempotencyKey");

        builder.HasIndex(x => x.AggregateId)
            .HasDatabaseName("IX_EventIdempotencies_AggregateId")
            .HasFilter("[AggregateId] IS NOT NULL");

        builder.HasIndex(x => x.ProcessedAt)
            .HasDatabaseName("IX_EventIdempotencies_ProcessedAt");

        builder.HasIndex(x => x.IsProcessed)
            .HasDatabaseName("IX_EventIdempotencies_IsProcessed");
    }
}
