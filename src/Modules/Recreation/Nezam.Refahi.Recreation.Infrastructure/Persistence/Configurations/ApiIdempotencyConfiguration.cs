using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class ApiIdempotencyConfiguration : IEntityTypeConfiguration<ApiIdempotency>
{
    public void Configure(EntityTypeBuilder<ApiIdempotency> builder)
    {
        builder.ToTable("ApiIdempotency", "recreation", t =>
        {
            t.HasCheckConstraint("CK_ApiIdempotency_ExpiresAfterCreated", "[ExpiresAt] > [CreatedAt]");
            t.HasCheckConstraint("CK_ApiIdempotency_StatusCodeValid", "[StatusCode] >= 100 AND [StatusCode] <= 599");
        });

        builder.HasKey(ai => ai.Id);
        builder.Property(ai => ai.Id)
            .ValueGeneratedNever();

        builder.Property(ai => ai.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ai => ai.Endpoint)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ai => ai.RequestPayloadHash)
            .HasMaxLength(64); // SHA256 hash

        builder.Property(ai => ai.ResponseData)
            .HasColumnType("nvarchar(max)"); // JSON response

        builder.Property(ai => ai.StatusCode)
            .IsRequired();

        builder.Property(ai => ai.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ai => ai.ExpiresAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(ai => ai.IsProcessed)
            .IsRequired()
            .HasDefaultValue(false);

        // Multi-tenancy support
        builder.Property(ai => ai.TenantId)
            .IsRequired(false)
            .HasMaxLength(50);

        // User context
        builder.Property(ai => ai.UserId)
            .HasMaxLength(50);

        builder.Property(ai => ai.UserAgent)
            .HasMaxLength(500);

        builder.Property(ai => ai.ClientIp)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(ai => ai.CorrelationId)
            .HasMaxLength(100);

        // Unique constraint for idempotency key per tenant per endpoint
        builder.HasIndex(ai => new { ai.TenantId, ai.Endpoint, ai.IdempotencyKey })
            .HasDatabaseName("UX_ApiIdempotency_TenantEndpointKey")
            .IsUnique();

        // Performance indexes
        builder.HasIndex(ai => new { ai.TenantId, ai.CreatedAt })
            .HasDatabaseName("IX_ApiIdempotency_TenantCreatedAt");

        builder.HasIndex(ai => ai.ExpiresAt)
            .HasDatabaseName("IX_ApiIdempotency_ExpiresAt");

        builder.HasIndex(ai => new { ai.TenantId, ai.IsProcessed, ai.CreatedAt })
            .HasDatabaseName("IX_ApiIdempotency_TenantProcessedCreated");

        builder.HasIndex(ai => ai.UserId)
            .HasDatabaseName("IX_ApiIdempotency_UserId")
            .HasFilter("[UserId] IS NOT NULL");
    }
}
