using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for RefreshSession entity
/// </summary>
public class RefreshSessionConfiguration : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        // Table name
        builder.ToTable("RefreshSessions");

        // Primary key - Client-generated GUID
        builder.HasKey(rs => rs.Id);
        builder.Property(rs => rs.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor

        // Properties
        builder.Property(rs => rs.UserId)
            .IsRequired();

        builder.Property(rs => rs.ClientId)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(rs => rs.Rotation)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(rs => rs.LastUsedAt)
            .IsRequired();

        builder.Property(rs => rs.RevokedAt)
            .IsRequired(false);

        builder.Property(rs => rs.RevokeReason)
            .HasMaxLength(500)
            .IsRequired(false);

        // Configure owned value objects
        builder.OwnsOne(rs => rs.DeviceFingerprint, deviceFingerprint =>
        {
            deviceFingerprint.Property(df => df.Value)
                .HasColumnName("DeviceFingerprintValue")
                .HasMaxLength(100)
                .IsRequired(false);
        });

        builder.OwnsOne(rs => rs.CurrentTokenHash, tokenHash =>
        {
            tokenHash.Property(th => th.Hash)
                .HasColumnName("TokenHash")
                .HasMaxLength(255)
                .IsRequired();
            
            tokenHash.Property(th => th.Algorithm)
                .HasColumnName("TokenAlgorithm")
                .HasMaxLength(50)
                .IsRequired();
            
            tokenHash.Property(th => th.Salt)
                .HasColumnName("TokenSalt")
                .HasMaxLength(255)
                .IsRequired();
        });

        // Base aggregate root properties
        builder.Property(rs => rs.CreatedAt).IsRequired();
        
        // Indexes
        builder.HasIndex(rs => rs.UserId)
            .HasDatabaseName("IX_RefreshSessions_UserId");

        builder.HasIndex(rs => rs.ClientId)
            .HasDatabaseName("IX_RefreshSessions_ClientId");

        builder.HasIndex(rs => rs.LastUsedAt)
            .HasDatabaseName("IX_RefreshSessions_LastUsedAt");

        builder.HasIndex(rs => new { rs.UserId, rs.ClientId })
            .HasDatabaseName("IX_RefreshSessions_UserId_ClientId");

        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();

        // Relationships - Within aggregate (UserDetail owns RefreshSession)
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(rs => rs.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate
    }
}
