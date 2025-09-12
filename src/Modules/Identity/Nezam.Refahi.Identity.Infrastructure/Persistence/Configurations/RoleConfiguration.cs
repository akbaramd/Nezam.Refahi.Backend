using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Role entity
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", "identity");
        
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(r => r.Description)
            .HasMaxLength(500);
            
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(r => r.IsSystemRole)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(r => r.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
            
        // Base aggregate root properties
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(r => r.ModifiedAt)
            .IsRequired(false)
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(r => r.CreatedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
        builder.Property(r => r.ModifiedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
            
        // Soft delete - SHOULD: Global query filter
        builder.HasQueryFilter(r => !r.IsDeleted);

        // Configure relationships - DDD Cascade Rules
        // Within Aggregate: Cascade (Role owns its claims)
        builder.HasMany(r => r.Claims)
            .WithOne(rc => rc.Role)
            .HasForeignKey(rc => rc.RoleId)
            .OnDelete(DeleteBehavior.Cascade); // Within aggregate

        // Between Aggregates: Restrict (UserRole is a join entity between UserDetail and Role aggregates)
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // Between aggregates

        // Configure indexes
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        builder.HasIndex(r => r.IsActive)
            .HasDatabaseName("IX_Roles_IsActive");

        builder.HasIndex(r => r.IsSystemRole)
            .HasDatabaseName("IX_Roles_IsSystemRole");

        builder.HasIndex(r => r.DisplayOrder)
            .HasDatabaseName("IX_Roles_DisplayOrder");

        builder.HasIndex(r => new { r.IsActive, r.DisplayOrder })
            .HasDatabaseName("IX_Roles_IsActive_DisplayOrder");
    }
}
