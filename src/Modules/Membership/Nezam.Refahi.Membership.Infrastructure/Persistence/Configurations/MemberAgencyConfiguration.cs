using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Configurations;

public class MemberAgencyConfiguration : IEntityTypeConfiguration<MemberAgency>
{
    public void Configure(EntityTypeBuilder<MemberAgency> builder)
    {
        // Table configuration
        builder.ToTable("MemberAgencies", "membership");

        // Primary key - Client-generated GUID
        builder.HasKey(ma => ma.Id);
        builder.Property(ma => ma.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor

        // Foreign keys
        builder.Property(ma => ma.MemberId)
            .IsRequired();

        builder.Property(ma => ma.AgencyId)
            .IsRequired();

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(ma => new { ma.MemberId, ma.AgencyId })
            .IsUnique()
            .HasDatabaseName("IX_MemberAgencies_MemberId_AgencyId");

        // Indexes for performance
        builder.HasIndex(ma => ma.MemberId)
            .HasDatabaseName("IX_MemberAgencies_MemberId");

        builder.HasIndex(ma => ma.AgencyId)
            .HasDatabaseName("IX_MemberAgencies_AgencyId");

        // Configure relationship with Member
        builder.HasOne(ma => ma.Member)
            .WithMany(m => m.Agencies)
            .HasForeignKey(ma => ma.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
