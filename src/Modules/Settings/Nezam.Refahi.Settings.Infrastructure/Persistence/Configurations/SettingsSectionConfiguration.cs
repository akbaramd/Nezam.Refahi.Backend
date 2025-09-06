using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Nezam.Refahi.Settings.Domain.Entities;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SettingsSection entity.
/// </summary>
public class SettingsSectionConfiguration : IEntityTypeConfiguration<SettingsSection>
{
    public void Configure(EntityTypeBuilder<SettingsSection> builder)
    {
        // Table configuration
        builder.ToTable("SettingsSections", "settings");
        
        // Primary key - Client-generated GUID
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        
        // Properties configuration
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.Description)
            .HasMaxLength(500);
            
        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(s => s.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);
            
        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
            
        // Relationships
        builder.HasMany(s => s.Categories)
            .WithOne(c => c.Section)
            .HasForeignKey(c => c.SectionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure backing field for collections
        builder.Metadata.FindNavigation(nameof(SettingsSection.Categories))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        // Create indexes for commonly queried fields
        builder.HasIndex(s => s.Name)
            .IsUnique();
            
        builder.HasIndex(s => s.IsActive);
            
        builder.HasIndex(s => s.DisplayOrder);
            
        builder.HasIndex(s => new { s.IsActive, s.DisplayOrder });
    }
}
