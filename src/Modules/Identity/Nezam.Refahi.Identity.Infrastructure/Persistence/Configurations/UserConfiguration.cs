using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Enums;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table configuration
        builder.ToTable("Users", "identity");
        
        // Primary key - Client-generated GUID (Version 7 Sequential)
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        // Properties
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        // Configure PhoneNumber as a value object
        builder.OwnsOne(u => u.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
                
            phoneNumber.WithOwner();
                
            // Create unique index on the owned type property
            phoneNumber.HasIndex(p => p.Value)
                .IsUnique();
        });
        
        // Configure NationalId as a value object
        builder.OwnsOne(u => u.NationalId, nationalId =>
        {
            nationalId.Property(n => n.Value)
                .HasColumnName("NationalId")
                .HasMaxLength(20)
                .IsRequired(false); // Can be null for phone-only users
                
            nationalId.WithOwner();
                
            // Create index on the owned type property
            nationalId.HasIndex(n => n.Value)
                .IsUnique()
                .HasFilter("[NationalId] IS NOT NULL");
        });
        
        // Role is now managed through UserRole entity (many-to-many relationship)
            
        // Configure LastLoginAt
        builder.Property(u => u.LastLoginAt)
            .IsRequired(false);
            
        // Configure OTP Authentication fields
        builder.Property(u => u.IsPhoneVerified)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(u => u.PhoneVerifiedAt)
            .IsRequired(false);
            
        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(u => u.LastAuthenticatedAt)
            .IsRequired(false);
            
        builder.Property(u => u.FailedAttempts)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(u => u.LockedAt)
            .IsRequired(false);
            
        builder.Property(u => u.LockReason)
            .HasMaxLength(500)
            .IsRequired(false);
            
        builder.Property(u => u.UnlockAt)
            .IsRequired(false);
            
        // Configure DeviceFingerprint as a value object
        builder.OwnsOne(u => u.LastDeviceFingerprint, deviceFingerprint =>
        {
            deviceFingerprint.Property(d => d.Value)
                .HasColumnName("LastDeviceFingerprint")
                .HasMaxLength(500)
                .IsRequired(false);
                
            deviceFingerprint.WithOwner();
            
     
        });
        
        builder.Property(u => u.LastIpAddress)
            .HasMaxLength(45) // IPv6 max length
            .IsRequired(false);
            
        builder.Property(u => u.LastUserAgent)
            .HasMaxLength(500)
            .IsRequired(false);
            
        // Configure navigation properties - DDD Cascade Rules
        // Within Aggregate: Cascade (User owns these entities)
        builder.HasMany(u => u.Tokens)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate
            
        // Configure UserPreferences relationship - Within aggregate
        builder.HasMany(u => u.Preferences)
            .WithOne(up => up.User)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate
            
        // Between Aggregates: Restrict (UserRole is a join entity between User and Role aggregates)
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Between aggregates
            
        // Configure UserClaims relationship - Within aggregate
        builder.HasMany(u => u.UserClaims)
            .WithOne(uc => uc.User)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate
            
        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
            
        // Base aggregate root properties
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(u => u.ModifiedAt)
            .IsRequired(false)
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(u => u.CreatedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
        builder.Property(u => u.ModifiedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
            
        // Soft delete - SHOULD: Global query filter
        builder.HasQueryFilter(u => !u.IsDeleted);
        
        // Create indexes for commonly queried fields
        // The following indexes are now handled within OwnsOne blocks
        // builder.HasIndex("PhoneNumber")
        //     .IsUnique()
        //     .HasDatabaseName("IX_Users_PhoneNumber");
            
        // builder.HasIndex("NationalId")
        //     .IsUnique()
        //     .HasDatabaseName("IX_Users_NationalId")
        //     .HasFilter("[NationalId] IS NOT NULL");
            
        builder.HasIndex(u => u.IsActive);
            
        builder.HasIndex(u => u.IsPhoneVerified);
    }
}
