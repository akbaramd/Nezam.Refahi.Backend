using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Nezam.Refahi.Settings.Domain.Entities;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Setting aggregate root.
/// </summary>
public class SystemSettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        // Table configuration
        builder.ToTable("Settings", "settings");
        
        // Primary key - Client-generated GUID
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        // Base aggregate root properties
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(s => s.ModifiedAt)
            .IsRequired(false)
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(s => s.CreatedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
        builder.Property(s => s.ModifiedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
            
        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
        
        // Properties configuration
        builder.Property(s => s.Description)
            .HasMaxLength(500);
            
        builder.Property(s => s.IsReadOnly)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(s => s.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(s => s.CategoryId)
            .IsRequired();
            
        // Configure SettingKey as a value object
        builder.OwnsOne(s => s.Key, key =>
        {
            key.Property(k => k.Value)
                .HasColumnName("Key")
                .HasMaxLength(200)
                .IsRequired();
                
            key.WithOwner();
                
            // Create index on the owned type property
            key.HasIndex(k => k.Value)
                .IsUnique();
        });
        
        // Configure SettingValue as a value object
        builder.OwnsOne(s => s.Value, value =>
        {
            value.Property(v => v.RawValue)
                .HasColumnName("Value")
                .HasMaxLength(4000)
                .IsRequired();
                
            value.Property(v => v.Type)
                .HasColumnName("ValueType")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
                
            value.WithOwner();
                
            // Create index on the value type for filtering
            value.HasIndex(v => v.Type);
        });
            
        // Relationships
        builder.HasOne(s => s.Category)
            .WithMany(c => c.Settings)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure backing field for collections
        builder.Metadata.FindNavigation(nameof(Setting.ChangeEvents))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        // Create indexes for commonly queried fields
        builder.HasIndex(s => s.CategoryId);
            
        builder.HasIndex(s => s.IsActive);
            
        builder.HasIndex(s => s.IsReadOnly);
            
        builder.HasIndex(s => new { s.CategoryId, s.IsActive, s.DisplayOrder });
            
        builder.HasIndex(s => new { s.IsActive, s.IsReadOnly });
    }
}
