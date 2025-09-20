using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table configuration
        builder.ToTable("Roles", "membership");
        
        // Primary key - Client-generated GUID
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor
        
        // Key property - unique identifier
        builder.Property(r => r.Key)
            .IsRequired()
            .HasMaxLength(100);
            
        // Title property
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        // Description
        builder.Property(r => r.Description)
            .HasMaxLength(1000);
            
        // Employer information
        builder.Property(r => r.EmployerName)
            .HasMaxLength(200);
            
        builder.Property(r => r.EmployerCode)
            .HasMaxLength(50);
        
        // Boolean properties
        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Sort order for display
        builder.Property(r => r.SortOrder)
            .IsRequired(false);
        
        // Unique constraint on Key
        builder.HasIndex(r => r.Key)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Key");
            
        // Index for active roles
        builder.HasIndex(r => r.IsActive)
            .HasDatabaseName("IX_Roles_IsActive");
            
        // Index for employer lookups
        builder.HasIndex(r => r.EmployerName)
            .HasDatabaseName("IX_Roles_EmployerName");
            
        builder.HasIndex(r => r.EmployerCode)
            .HasDatabaseName("IX_Roles_EmployerCode");
            
        // Index for sorting
        builder.HasIndex(r => new { r.SortOrder, r.Title })
            .HasDatabaseName("IX_Roles_SortOrder_Title");
        
    
        
        // Configure relationship with MemberRole
        builder.HasMany(r => r.MemberRoles)
            .WithOne(mr => mr.Role)
            .HasForeignKey(mr => mr.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}