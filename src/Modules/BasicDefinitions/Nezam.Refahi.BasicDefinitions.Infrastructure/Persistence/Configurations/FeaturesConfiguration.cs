using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Configurations;

public class FeaturesConfiguration : IEntityTypeConfiguration<Features>
{
    public void Configure(EntityTypeBuilder<Features> builder)
    {
        // Table configuration
        builder.ToTable("Features", "definitions");

        // Primary key - Client-generated string
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor

        // Title configuration
        builder.Property(f => f.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Type configuration
        builder.Property(f => f.Type)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes for performance
        builder.HasIndex(f => f.Title)
            .HasDatabaseName("IX_Features_Title");

        builder.HasIndex(f => f.Type)
            .HasDatabaseName("IX_Features_Type");

        builder.HasIndex(f => new { f.Type, f.Title })
            .HasDatabaseName("IX_Features_Type_Title");

        // Configure relationship with Capabilities
        builder.HasMany(f => f.Capabilities)
            .WithMany(c => c.Features)
            .UsingEntity(
                "CapabilityFeatures",
                l => l.HasOne(typeof(Capability)).WithMany().HasForeignKey("CapabilityId").HasPrincipalKey(nameof(Capability.Id)),
                r => r.HasOne(typeof(Features)).WithMany().HasForeignKey("ClaimTypeId").HasPrincipalKey(nameof(Features.Id)),
                j => j.ToTable("CapabilityFeatures", "definitions"));
    }
}
