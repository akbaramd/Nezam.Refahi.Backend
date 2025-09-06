// -----------------------------------------------------------------------------
// Infrastructure.Persistence.Configurations.UserTokensConfiguration.cs
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserTokensConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        // 1) Table & Primary Key
        builder.ToTable("UserTokens", "identity");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor

        // 2) Scalar Properties
        builder.Property(t => t.TokenValue)
               .IsRequired()
               .HasMaxLength(2048)          // JWT tokens can be up to 2KB
               .IsUnicode(false);

        builder.Property(t => t.TokenType)
               .IsRequired()
               .HasMaxLength(16)            // "RefreshToken" longest
               .IsUnicode(false);

        builder.Property(t => t.DeviceFingerprint)
               .HasMaxLength(256)           // Device fingerprint can be longer
               .IsUnicode(false);

        builder.Property(t => t.IpAddress)
               .HasMaxLength(45)            // IPv6 max length
               .IsUnicode(false);

        builder.Property(t => t.UserAgent)
               .HasMaxLength(512)           // User agent can be quite long
               .IsUnicode(false);

        builder.Property(t => t.SessionFamilyId)
               .IsRequired(false);

        builder.Property(t => t.ParentTokenId)
               .IsRequired(false);

        builder.Property(t => t.LastUsedAt)
               .IsRequired(false)
               .HasConversion(
                   v => v,
                   v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        builder.Property(t => t.Salt)
               .HasMaxLength(64)            // Base64 encoded salt
               .IsUnicode(false);

        // همه DateTime ها را به UTC نگاه می‌داریم
        builder.Property(t => t.ExpiresAt)
               .IsRequired()
               .HasConversion(
                   v => v,                  // DateTime → DateTime (UTC enforced در Domain)
                   v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        // 3) Relationships
        builder.HasOne(t => t.User)
               .WithMany(u => u.Tokens)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // 4) Indexes – Critical for performance
        //    a) Token validation: (TokenValue, TokenType) → unique
        builder.HasIndex(t => new { t.TokenValue, t.TokenType })
               .IsUnique()
#if NET8_0_OR_GREATER
               .HasFillFactor(90);         // SQL Server: reduce Page-Split
#else
               ;
#endif

        //    b) Batch retrieval/revocation: (UserId, TokenType)
        builder.HasIndex(t => new { t.UserId, t.TokenType });

        //    c) Periodic cleanup: Expiration
        builder.HasIndex(t => t.ExpiresAt);

        //    d) Fast filtering for "active tokens" (common at runtime)
        builder.HasIndex(t => new { t.IsRevoked, t.IsUsed })
               .HasFilter("[IsRevoked] = 0 AND [IsUsed] = 0"); // SQL Server syntax
               // PostgreSQL: use .HasFilter("\"IsRevoked\" = FALSE AND \"IsUsed\" = FALSE");

        //    e) Session family management for refresh token rotation
        builder.HasIndex(t => t.SessionFamilyId)
               .HasFilter("[SessionFamilyId] IS NOT NULL");

        //    f) Device-based token management
        builder.HasIndex(t => new { t.UserId, t.DeviceFingerprint, t.TokenType })
               .HasFilter("[DeviceFingerprint] IS NOT NULL");

        //    g) Idle timeout cleanup
        builder.HasIndex(t => t.LastUsedAt)
               .HasFilter("[LastUsedAt] IS NOT NULL");

        //    h) Parent-child token relationships
        builder.HasIndex(t => t.ParentTokenId)
               .HasFilter("[ParentTokenId] IS NOT NULL");

   

        // 5) Concurrency-Token (Optimistic Locking) - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
            
 
    }
}
