using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class TourCapacityConfiguration : IEntityTypeConfiguration<TourCapacity>
{
    public void Configure(EntityTypeBuilder<TourCapacity> builder)
    {
        builder.ToTable("TourCapacities", "recreation", t =>
        {
            t.HasCheckConstraint("CK_TourCapacity_MaxParticipants_Positive", "[MaxParticipants] > 0");
            t.HasCheckConstraint("CK_TourCapacity_RemainingParticipants_Valid", "[RemainingParticipants] >= 0 AND [RemainingParticipants] <= [MaxParticipants]");
            t.HasCheckConstraint("CK_TourCapacity_ParticipantLimits_Valid", "[MinParticipantsPerReservation] > 0 AND [MaxParticipantsPerReservation] >= [MinParticipantsPerReservation]");
            t.HasCheckConstraint("CK_TourCapacity_RegistrationDates_Valid", "[RegistrationStart] < [RegistrationEnd]");
        });

        builder.HasKey(tc => tc.Id);
        builder.Property(tc => tc.Id)
            .ValueGeneratedNever();

        builder.Property(tc => tc.TourId)
            .IsRequired();

        builder.Property(tc => tc.MaxParticipants)
            .IsRequired();

        builder.Property(tc => tc.RemainingParticipants)
            .IsRequired();

        builder.Property(tc => tc.MinParticipantsPerReservation)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(tc => tc.MaxParticipantsPerReservation)
            .IsRequired()
            .HasDefaultValue(10);

        builder.Property(tc => tc.RegistrationStart)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(tc => tc.RegistrationEnd)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(tc => tc.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(tc => tc.Description)
            .IsRequired(false)
            .HasMaxLength(500);

        // Special capacity for VIP members only
        builder.Property(tc => tc.IsSpecial)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Indicates if this capacity is only visible to special VIP members");

        // Multi-tenancy support
        builder.Property(tc => tc.TenantId)
            .IsRequired(false)
            .HasMaxLength(50);

        // Concurrency control
        builder.Property(tc => tc.RowVersion)
            .IsRequired()
            .IsRowVersion();

 

        // Configure relationship with Tour
        builder.HasOne(tc => tc.Tour)
            .WithMany(t => t.Capacities)
            .HasForeignKey(tc => tc.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraints with tenant support
        builder.HasIndex(tc => new { tc.TenantId, tc.TourId, tc.RegistrationStart, tc.RegistrationEnd })
            .HasDatabaseName("UX_TourCapacity_TenantTourPeriod")
            .IsUnique();

        // Performance indexes
        builder.HasIndex(tc => new { tc.TenantId, tc.TourId })
            .HasDatabaseName("IX_TourCapacity_TenantTour");

        builder.HasIndex(tc => new { tc.TenantId, tc.TourId, tc.IsActive })
            .HasDatabaseName("IX_TourCapacity_TenantTourActive");

        builder.HasIndex(tc => new { tc.TenantId, tc.RegistrationStart, tc.RegistrationEnd })
            .HasDatabaseName("IX_TourCapacity_TenantRegistrationWindow");

        builder.HasIndex(tc => tc.RemainingParticipants)
            .HasDatabaseName("IX_TourCapacity_RemainingParticipants");

        // Index for special capacities
        builder.HasIndex(tc => new { tc.TenantId, tc.TourId, tc.IsSpecial })
            .HasDatabaseName("IX_TourCapacity_TenantTourSpecial");
    }
}