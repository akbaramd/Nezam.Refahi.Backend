using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Configurations;

public class FeatureCategoryConfiguration : IEntityTypeConfiguration<FeatureCategory>
{
    public void Configure(EntityTypeBuilder<FeatureCategory> builder)
    {
        builder.ToTable("FeatureCategories", "recreation");

        builder.HasKey(fc => fc.Id);
        builder.Property(fc => fc.Id)
            .ValueGeneratedNever();

        builder.Property(fc => fc.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(fc => fc.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(fc => fc.IconClass)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(fc => fc.ColorCode)
            .HasMaxLength(10)
            .IsRequired(false);

        builder.Property(fc => fc.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(fc => fc.IsActive)
            .IsRequired()
            .HasDefaultValue(true);



        builder.HasMany(fc => fc.Features)
            .WithOne(f => f.Category)
            .HasForeignKey(f => f.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(fc => fc.Name).IsUnique();
        builder.HasIndex(fc => fc.DisplayOrder);
        builder.HasIndex(fc => fc.IsActive);
    }
}