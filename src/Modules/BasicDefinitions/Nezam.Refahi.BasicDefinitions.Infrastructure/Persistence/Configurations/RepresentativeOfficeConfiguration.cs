using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for RepresentativeOffice entity
/// </summary>
public class RepresentativeOfficeConfiguration : IEntityTypeConfiguration<RepresentativeOffice>
{
    public void Configure(EntityTypeBuilder<RepresentativeOffice> builder)
    {
        builder.ToTable("RepresentativeOffices", "definitions");

        // Primary key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ExternalCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ManagerName)
            .HasMaxLength(100);

        builder.Property(x => x.ManagerPhone)
            .HasMaxLength(20);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.EstablishedDate)
            .HasColumnType("date");

        // Indexes
        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("IX_RepresentativeOffices_Code");

        builder.HasIndex(x => x.ExternalCode)
            .IsUnique()
            .HasDatabaseName("IX_RepresentativeOffices_ExternalCode");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_RepresentativeOffices_IsActive");

        builder.HasIndex(x => x.ManagerName)
            .HasDatabaseName("IX_RepresentativeOffices_ManagerName");
    }
}
