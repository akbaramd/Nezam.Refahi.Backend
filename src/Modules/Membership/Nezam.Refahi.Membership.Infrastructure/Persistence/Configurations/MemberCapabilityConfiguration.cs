using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Configurations;

public class MemberCapabilityConfiguration : IEntityTypeConfiguration<MemberCapability>
{
    public void Configure(EntityTypeBuilder<MemberCapability> builder)
    {
        // Table configuration
        builder.ToTable("MemberCapabilities", "membership");

        // Primary key - Client-generated GUID
        builder.HasKey(mc => mc.Id);
        builder.Property(mc => mc.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor

        // Foreign keys
        builder.Property(mc => mc.MemberId)
            .IsRequired();

        builder.Property(mc => mc.CapabilityId)
            .IsRequired();

        // IsActive configuration
        builder.Property(mc => mc.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // ValidFrom configuration
        builder.Property(mc => mc.ValidFrom)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // ValidTo configuration
        builder.Property(mc => mc.ValidTo)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // AssignedBy configuration
        builder.Property(mc => mc.AssignedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        // AssignedAt configuration
        builder.Property(mc => mc.AssignedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Notes configuration
        builder.Property(mc => mc.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        // Cached capability information configuration
        builder.Property(mc => mc.CapabilityName)
            .HasMaxLength(200)
            .IsRequired();

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(mc => new { mc.MemberId, mc.CapabilityId })
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("IX_MemberCapabilities_Member_Capability_Active");

        // Indexes for performance
        builder.HasIndex(mc => mc.MemberId)
            .HasDatabaseName("IX_MemberCapabilities_MemberId");

        builder.HasIndex(mc => mc.CapabilityId)
            .HasDatabaseName("IX_MemberCapabilities_CapabilityId");

        builder.HasIndex(mc => mc.IsActive)
            .HasDatabaseName("IX_MemberCapabilities_IsActive");

        builder.HasIndex(mc => new { mc.ValidFrom, mc.ValidTo })
            .HasDatabaseName("IX_MemberCapabilities_ValidityPeriod");

        builder.HasIndex(mc => mc.AssignedAt)
            .HasDatabaseName("IX_MemberCapabilities_AssignedAt");

        // Configure relationship with Member
        builder.HasOne(mc => mc.Member)
            .WithMany(m => m.Capabilities)
            .HasForeignKey(mc => mc.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        // Capability relation removed - only store the key
        // No foreign key relationship with Capability entity
    }
}