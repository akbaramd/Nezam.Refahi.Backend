using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Configurations;

public class MemberRoleConfiguration : IEntityTypeConfiguration<MemberRole>
{
    public void Configure(EntityTypeBuilder<MemberRole> builder)
    {
        // Table configuration
        builder.ToTable("MemberRoles", "membership");
        
        // Primary key - Client-generated GUID
        builder.HasKey(mr => mr.Id);
        builder.Property(mr => mr.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor
        
        // Configure MemberId as foreign key
        builder.Property(mr => mr.MemberId)
            .IsRequired();
            
        // Configure RoleId as foreign key
        builder.Property(mr => mr.RoleId)
            .IsRequired();
        
        // Assignment timestamp
        builder.Property(mr => mr.AssignedAt)
            .IsRequired()
            .HasColumnType("datetime2");
        
        // Validity period
        builder.Property(mr => mr.ValidFrom)
            .HasColumnType("datetime2");
            
        builder.Property(mr => mr.ValidTo)
            .HasColumnType("datetime2");
        
        // Assignment metadata
        builder.Property(mr => mr.AssignedBy)
            .HasMaxLength(100);
            
        builder.Property(mr => mr.Notes)
            .HasMaxLength(1000);
        
        // Boolean properties
        builder.Property(mr => mr.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        
        // Indexes for performance
        builder.HasIndex(mr => mr.MemberId)
            .HasDatabaseName("IX_MemberRoles_MemberId");
            
        builder.HasIndex(mr => mr.RoleId)
            .HasDatabaseName("IX_MemberRoles_RoleId");
            
        builder.HasIndex(mr => mr.IsActive)
            .HasDatabaseName("IX_MemberRoles_IsActive");
            
        builder.HasIndex(mr => mr.ValidTo)
            .HasDatabaseName("IX_MemberRoles_ValidTo");
            
        // Composite indexes
        builder.HasIndex(mr => new { mr.MemberId, mr.RoleId })
            .HasDatabaseName("IX_MemberRoles_Member_Role");
            
        builder.HasIndex(mr => new { mr.MemberId, mr.IsActive })
            .HasDatabaseName("IX_MemberRoles_Member_IsActive");
            
        builder.HasIndex(mr => new { mr.RoleId, mr.IsActive })
            .HasDatabaseName("IX_MemberRoles_Role_IsActive");
            
        builder.HasIndex(mr => new { mr.MemberId, mr.ValidTo })
            .HasDatabaseName("IX_MemberRoles_Member_ValidTo");
        
        // Configure relationship with Member
        builder.HasOne(mr => mr.Member)
            .WithMany(m => m.Roles)
            .HasForeignKey(mr => mr.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure relationship with Role
        builder.HasOne(mr => mr.Role)
            .WithMany(r => r.MemberRoles)
            .HasForeignKey(mr => mr.RoleId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete member roles when role is deleted
    }
}