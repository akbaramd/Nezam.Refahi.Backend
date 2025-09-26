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

        builder.Property(ma => ma.RepresentativeOfficeId)
            .IsRequired();

        // IsActive configuration
        builder.Property(ma => ma.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // ValidFrom configuration
        builder.Property(ma => ma.ValidFrom)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // ValidTo configuration
        builder.Property(ma => ma.ValidTo)
            .HasColumnType("datetime2")
            .IsRequired(false);

        // AssignedBy configuration
        builder.Property(ma => ma.AssignedBy)
            .HasMaxLength(100)
            .IsRequired(false);

        // AssignedAt configuration
        builder.Property(ma => ma.AssignedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Notes configuration
        builder.Property(ma => ma.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        // AccessLevel configuration
        builder.Property(ma => ma.AccessLevel)
            .HasMaxLength(50)
            .IsRequired(false);

        // Cached office information configuration
        builder.Property(ma => ma.OfficeCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ma => ma.OfficeTitle)
            .HasMaxLength(200)
            .IsRequired();

        // Composite unique index to prevent duplicate assignments
        builder.HasIndex(ma => new { ma.MemberId, ma.RepresentativeOfficeId })
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("IX_MemberAgencies_Member_Office_Active");

        // Indexes for performance
        builder.HasIndex(ma => ma.MemberId)
            .HasDatabaseName("IX_MemberAgencies_MemberId");

        builder.HasIndex(ma => ma.RepresentativeOfficeId)
            .HasDatabaseName("IX_MemberAgencies_RepresentativeOfficeId");

        builder.HasIndex(ma => ma.IsActive)
            .HasDatabaseName("IX_MemberAgencies_IsActive");

        builder.HasIndex(ma => new { ma.ValidFrom, ma.ValidTo })
            .HasDatabaseName("IX_MemberAgencies_ValidityPeriod");

        builder.HasIndex(ma => ma.AssignedAt)
            .HasDatabaseName("IX_MemberAgencies_AssignedAt");

        builder.HasIndex(ma => ma.AccessLevel)
            .HasDatabaseName("IX_MemberAgencies_AccessLevel");

        // Configure relationship with Member
        builder.HasOne(ma => ma.Member)
            .WithMany(m => m.Agencies)
            .HasForeignKey(ma => ma.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        // RepresentativeOffice relation - only store the key
        // No foreign key relationship with RepresentativeOffice entity (cross-module)
        // But we can add a check constraint to ensure the office exists
        builder.HasCheckConstraint("CK_MemberAgencies_RepresentativeOfficeId_NotEmpty", 
            "[RepresentativeOfficeId] != '00000000-0000-0000-0000-000000000000'");
    }
}
