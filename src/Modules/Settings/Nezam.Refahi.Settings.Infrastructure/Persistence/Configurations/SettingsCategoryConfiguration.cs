using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Nezam.Refahi.Settings.Domain.Entities;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SettingsCategory entity.
/// </summary>
public class SettingsCategoryConfiguration : IEntityTypeConfiguration<SettingsCategory>
{
    public void Configure(EntityTypeBuilder<SettingsCategory> builder)
    {
        // Table configuration
        builder.ToTable("SettingsCategories", "settings");
        
        // Primary key - Client-generated GUID
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        // Properties configuration
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(c => c.Description)
            .HasMaxLength(500);
            
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(c => c.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(c => c.SectionId)
            .IsRequired();
            
  
            
        // Relationships
        builder.HasOne(c => c.Section)
            .WithMany(s => s.Categories)
            .HasForeignKey(c => c.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(c => c.Settings)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure backing field for collections
        builder.Metadata.FindNavigation(nameof(SettingsCategory.Settings))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        // Create indexes for commonly queried fields
        builder.HasIndex(c => new { c.SectionId, c.Name })
            .IsUnique();
            
        builder.HasIndex(c => c.SectionId);
            
        builder.HasIndex(c => c.IsActive);
            
        builder.HasIndex(c => new { c.SectionId, c.IsActive, c.DisplayOrder });
    }
}
