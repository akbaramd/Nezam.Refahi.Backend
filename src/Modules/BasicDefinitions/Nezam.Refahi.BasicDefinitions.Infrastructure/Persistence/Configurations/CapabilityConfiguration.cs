using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Configurations;

public class CapabilityConfiguration : IEntityTypeConfiguration<Capability>
{
    public void Configure(EntityTypeBuilder<Capability> builder)
    {
        // Table configuration
        builder.ToTable("Capabilities", "definitions");

        // Primary key - Client-generated GUID
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor

        // Name configuration
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Description configuration
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(1000);

        // IsActive configuration
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // ValidFrom configuration
        builder.Property(c => c.ValidFrom)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // ValidTo configuration
        builder.Property(c => c.ValidTo)
            .HasColumnType("datetime2")
            .IsRequired(false);



        // Indexes for performance
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("IX_Capabilities_Name");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_Capabilities_IsActive");

        builder.HasIndex(c => new { c.ValidFrom, c.ValidTo })
            .HasDatabaseName("IX_Capabilities_ValidityPeriod");


        // Configure relationship with ClaimTypes
        builder.HasMany(c => c.Features)
            .WithMany(ct => ct.Capabilities)
            .UsingEntity(
                "CapabilityFeatures",
                l => l.HasOne(typeof(Features)).WithMany().HasForeignKey("ClaimTypeId").HasPrincipalKey(nameof(Features.Id)),
                r => r.HasOne(typeof(Capability)).WithMany().HasForeignKey("CapabilityId").HasPrincipalKey(nameof(Capability.Id)),
                j => j.ToTable("CapabilityFeatures", "definitions"));
    }
}
