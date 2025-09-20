using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Enums;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the OtpChallenge entity
/// </summary>
public class OtpChallengeConfiguration : IEntityTypeConfiguration<OtpChallenge>
{
    public void Configure(EntityTypeBuilder<OtpChallenge> builder)
    {
        // Table configuration
        builder.ToTable("OtpChallenges", "identity");
        
        // Primary key - Client-generated GUID
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
    
            
        builder.Property(o => o.OtpHash)
            .IsRequired()
            .HasMaxLength(256)
            .IsUnicode(false);
            
        builder.Property(o => o.Nonce)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);
            
        builder.Property(o => o.ExpiresAt)
            .IsRequired()
            .HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
            
        builder.Property(o => o.LastSentAt)
            .IsRequired(false)
            .HasConversion(
                v => v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );
            
        builder.Property(o => o.VerifiedAt)
            .IsRequired(false)
            .HasConversion(
                v => v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );
            
        builder.Property(o => o.ConsumedAt)
            .IsRequired(false)
            .HasConversion(
                v => v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );
            
        builder.Property(o => o.LockedAt)
            .IsRequired(false)
            .HasConversion(
                v => v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );
            
        builder.Property(o => o.AttemptsLeft)
            .IsRequired()
            .HasDefaultValue(3);
            
        builder.Property(o => o.ResendLeft)
            .IsRequired()
            .HasDefaultValue(3);
            
        builder.Property(o => o.UserId)
            .IsRequired(false);
            
        // Configure PhoneNumber as a value object
        builder.OwnsOne(o => o.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
                
            // FK را Shadow نگه دارید؛ نیازی به تعریف Property برای OtpChallengeId نیست
            phoneNumber.WithOwner(); // FK سایه را نگه می‌دارد
            
            // Create index on the owned type property
            phoneNumber.HasIndex(p => p.Value)
                .HasDatabaseName("IX_OtpChallenges_PhoneNumber");
        });
        
        // Configure ClientId as a value object
        builder.OwnsOne(o => o.ClientId, clientId =>
        {
            clientId.Property(c => c.Value)
                .HasColumnName("ClientId")
                .HasMaxLength(50)
                .IsRequired();
                
            // FK را Shadow نگه دارید
            clientId.WithOwner();
                
            // Create index on the owned type property
            clientId.HasIndex(c => c.Value)
                .HasDatabaseName("IX_OtpChallenges_ClientId");
        });
        
        // Configure DeviceFingerprint as a value object
        builder.OwnsOne(o => o.DeviceFingerprint, deviceFingerprint =>
        {
            deviceFingerprint.Property(d => d.Value)
                .HasColumnName("DeviceFingerprint")
                .HasMaxLength(500)
                .IsRequired(false)
                .HasDefaultValue(string.Empty);
                
            deviceFingerprint.WithOwner();
        });
        
        // Configure IpAddress as a value object
        builder.OwnsOne(o => o.IpAddress, ipAddress =>
        {
            ipAddress.Property(i => i.Value)
                .HasColumnName("IpAddress")
                .HasMaxLength(45) // IPv6 max length
                .IsRequired(false);
                
            ipAddress.WithOwner();
        });
        
        // Configure OtpPolicy as a value object
        builder.OwnsOne(o => o.Policy, policy =>
        {
            policy.Property(p => p.Length)
                .HasColumnName("OtpLength")
                .IsRequired();
                
            policy.Property(p => p.TtlSeconds)
                .HasColumnName("TtlSeconds")
                .IsRequired();
                
            policy.Property(p => p.MaxVerifyAttempts)
                .HasColumnName("MaxVerifyAttempts")
                .IsRequired();
                
            policy.Property(p => p.MaxResends)
                .HasColumnName("MaxResends")
                .IsRequired();
                
            policy.Property(p => p.MaxPerPhonePerHour)
                .HasColumnName("MaxPerPhonePerHour")
                .IsRequired();
                
            policy.Property(p => p.MaxPerIpPerHour)
                .HasColumnName("MaxPerIpPerHour")
                .IsRequired();
        });
            
        // Configure Status as a string in the database
        builder.Property(o => o.Status)
            .HasColumnName("ChallengeStatus")
            .HasConversion(
                v => v.ToString(),
                v => (ChallengeStatus)Enum.Parse(typeof(ChallengeStatus), v)
            )
            .HasMaxLength(50)
            .IsRequired();
            
        // Configure DeliveryStatus as a string in the database
        builder.Property(o => o.DeliveryStatus)
            .HasColumnName("DeliveryStatus")
            .HasConversion(
                v => v.ToString(),
                v => (DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), v)
            )
            .HasMaxLength(50)
            .IsRequired();
            
        // Base aggregate root properties
        builder.Property(o => o.CreatedAt).IsRequired();
     
        

            
        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_OtpChallenges_Status");
            
        builder.HasIndex(o => o.ExpiresAt)
            .HasDatabaseName("IX_OtpChallenges_ExpiresAt");
            
        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("IX_OtpChallenges_CreatedAt");
            
        // Composite index for cleanup operations
        builder.HasIndex(o => new { o.Status, o.ExpiresAt })
            .HasDatabaseName("IX_OtpChallenges_Status_ExpiresAt");
  

    }
}

