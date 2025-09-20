using MCA.SharedKernel.Infrastructure.Configurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> builder)
    {
        builder.ToTable("Features", "recreation");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .ValueGeneratedNever();

        builder.Property(f => f.CategoryId)
            .IsRequired(false);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(f => f.IconClass)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(f => f.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(f => f.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(f => f.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(f => f.DefaultValue)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(f => f.ValidationRules)
            .HasMaxLength(1000)
            .IsRequired(false);


        builder.HasOne(f => f.Category)
            .WithMany(fc => fc.Features)
            .HasForeignKey(f => f.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(f => f.CategoryId);
        builder.HasIndex(f => new { f.CategoryId, f.Name }).IsUnique();
        builder.HasIndex(f => f.DisplayOrder);
        builder.HasIndex(f => f.IsActive);
    }
}