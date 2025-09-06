using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for UserRole entity
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", "identity");
        
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        builder.Property(ur => ur.UserId)
            .IsRequired();
            
        builder.Property(ur => ur.RoleId)
            .IsRequired();
            
        builder.Property(ur => ur.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(ur => ur.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(ur => ur.ExpiresAt);
            
        builder.Property(ur => ur.AssignedBy)
            .HasMaxLength(100);
            
        builder.Property(ur => ur.Notes)
            .HasMaxLength(500);

        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
   

        // Join entity relationships - Both sides are Restrict (between aggregates)
        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // Between aggregates
            
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Between aggregates

        // Configure indexes
        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("IX_UserRoles_UserId");

        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("IX_UserRoles_RoleId");

        builder.HasIndex(ur => ur.IsActive)
            .HasDatabaseName("IX_UserRoles_IsActive");

        builder.HasIndex(ur => ur.AssignedAt)
            .HasDatabaseName("IX_UserRoles_AssignedAt");

        builder.HasIndex(ur => ur.ExpiresAt)
            .HasDatabaseName("IX_UserRoles_ExpiresAt");

        builder.HasIndex(ur => ur.AssignedBy)
            .HasDatabaseName("IX_UserRoles_AssignedBy");

        // Composite indexes
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("IX_UserRoles_UserId_RoleId");

        builder.HasIndex(ur => new { ur.UserId, ur.IsActive })
            .HasDatabaseName("IX_UserRoles_UserId_IsActive");

        builder.HasIndex(ur => new { ur.RoleId, ur.IsActive })
            .HasDatabaseName("IX_UserRoles_RoleId_IsActive");

        builder.HasIndex(ur => new { ur.IsActive, ur.ExpiresAt })
            .HasDatabaseName("IX_UserRoles_IsActive_ExpiresAt");
    }
}
