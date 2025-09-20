using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for UserClaim entity
/// </summary>
public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaims", "identity");
        
        builder.HasKey(uc => uc.Id);
        builder.Property(uc => uc.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        builder.Property(uc => uc.UserId)
            .IsRequired();
            
        builder.Property(uc => uc.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(uc => uc.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(uc => uc.ExpiresAt);
            
        builder.Property(uc => uc.AssignedBy)
            .HasMaxLength(100);
            
        builder.Property(uc => uc.Notes)
            .HasMaxLength(500);

  


        // Configure Claim as owned type
        builder.OwnsOne(uc => uc.Claim, claim =>
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
                .HasDatabaseName("IX_UserClaims_ClaimType");
                
            claim.HasIndex(c => c.Value)
                .HasDatabaseName("IX_UserClaims_ClaimValue");
        });

        // Configure relationships
        builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserClaims)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(uc => uc.UserId)
            .HasDatabaseName("IX_UserClaims_UserId");

        builder.HasIndex(uc => uc.IsActive)
            .HasDatabaseName("IX_UserClaims_IsActive");

        builder.HasIndex(uc => uc.AssignedAt)
            .HasDatabaseName("IX_UserClaims_AssignedAt");

        builder.HasIndex(uc => uc.ExpiresAt)
            .HasDatabaseName("IX_UserClaims_ExpiresAt");

        builder.HasIndex(uc => uc.AssignedBy)
            .HasDatabaseName("IX_UserClaims_AssignedBy");

        // Composite indexes
        builder.HasIndex(uc => new { uc.UserId, uc.IsActive })
            .HasDatabaseName("IX_UserClaims_UserId_IsActive");

        builder.HasIndex(uc => new { uc.IsActive, uc.ExpiresAt })
            .HasDatabaseName("IX_UserClaims_IsActive_ExpiresAt");

        // Indexes for owned type properties are now inside OwnsOne configuration above

   
    }
}
