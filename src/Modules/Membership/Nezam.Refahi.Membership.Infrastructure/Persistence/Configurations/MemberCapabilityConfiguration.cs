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

        builder.Property(mc => mc.CapabilityKey)
            .IsRequired()
            .HasMaxLength(100);

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(mc => new { mc.MemberId, mc.CapabilityKey })
            .IsUnique()
            .HasDatabaseName("IX_MemberCapabilities_MemberId_CapabilityKey");

        // Indexes for performance
        builder.HasIndex(mc => mc.MemberId)
            .HasDatabaseName("IX_MemberCapabilities_MemberId");

        builder.HasIndex(mc => mc.CapabilityKey)
            .HasDatabaseName("IX_MemberCapabilities_CapabilityKey");

        // Configure relationship with Member
        builder.HasOne(mc => mc.Member)
            .WithMany(m => m.Capabilities)
            .HasForeignKey(mc => mc.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
