using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for RoleClaim entity
/// </summary>
public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims", "identity");
        
        builder.HasKey(rc => rc.Id);
        builder.Property(rc => rc.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        builder.Property(rc => rc.RoleId)
            .IsRequired();

  

        // Configure Claim as owned type
        builder.OwnsOne(rc => rc.Claim, claim =>
        {
            claim.Property(c => c.Type)
                .HasColumnName("ClaimType")
                .HasMaxLength(100)
                .IsRequired();
                
            claim.Property(c => c.Value)
                .HasColumnName("ClaimValue")
                .HasMaxLength(1000)
                .IsRequired();
                
            claim.Property(c => c.ValueType)
                .HasColumnName("ClaimValueType")
                .HasMaxLength(50);
                
            claim.WithOwner();
            
            // Indexes for owned type properties inside OwnsOne
            claim.HasIndex(c => c.Type)
                .HasDatabaseName("IX_RoleClaims_ClaimType");
                
            claim.HasIndex(c => c.Value)
                .HasDatabaseName("IX_RoleClaims_ClaimValue");
        });

        // Configure relationships
        builder.HasOne(rc => rc.Role)
            .WithMany(r => r.Claims)
            .HasForeignKey(rc => rc.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(rc => rc.RoleId)
            .HasDatabaseName("IX_RoleClaims_RoleId");

        // Indexes for owned type properties are now inside OwnsOne configuration above
        
        // Note: Composite indexes with owned type properties need to be handled through migration
    }
}
