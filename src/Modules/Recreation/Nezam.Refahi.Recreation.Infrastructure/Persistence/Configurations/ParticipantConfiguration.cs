using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("Participants", "recreation");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.ReservationId)
            .IsRequired();

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.NationalNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(p => p.Email)
            .HasMaxLength(256);

        builder.Property(p => p.ParticipantType)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(ParticipantType.Guest);

        builder.Property(p => p.BirthDate)
            .HasColumnType("date");

        builder.Property(p => p.EmergencyContactName)
            .HasMaxLength(200);

        builder.Property(p => p.EmergencyContactPhone)
            .HasMaxLength(15);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.RegistrationDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(p => p.PaymentDate)
            .HasColumnType("datetime2");

        // Configure Money value object for RequiredAmount
        builder.OwnsOne(p => p.RequiredAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("RequiredAmountRials")
                .HasColumnType("bigint")
                .IsRequired();
        });

        // Configure Money value object for PaidAmount
        builder.OwnsOne(p => p.PaidAmount, money =>
        {
            money.Property(m => m.AmountRials)
                .HasColumnName("PaidAmountRials")
                .HasColumnType("bigint");
        });

        // Configure relationship with TourReservation
        builder.HasOne(p => p.Reservation)
            .WithMany(r => r.Participants)
            .HasForeignKey(p => p.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Note: Complex tour-level uniqueness constraint for national numbers will be enforced in business logic
        // SQL Server doesn't support EXISTS in filtered indexes, so we handle this in the application layer

        // Performance indexes
        builder.HasIndex(p => p.ReservationId)
            .HasDatabaseName("IX_Participants_ReservationId");

        builder.HasIndex(p => p.NationalNumber)
            .HasDatabaseName("IX_Participants_NationalNumber");

        builder.HasIndex(p => p.ParticipantType)
            .HasDatabaseName("IX_Participants_ParticipantType");

        builder.HasIndex(p => p.RegistrationDate)
            .HasDatabaseName("IX_Participants_RegistrationDate");

        // Composite index for reservation + national number (for uniqueness within reservation)
        builder.HasIndex(p => new { p.ReservationId, p.NationalNumber })
            .IsUnique()
            .HasDatabaseName("UX_Participants_ReservationNationalNumber");

    }
}