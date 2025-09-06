using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Nezam.Refahi.Settings.Domain.Entities;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SettingChangeEvent entity.
/// </summary>
public class SettingChangeEventConfiguration : IEntityTypeConfiguration<SettingChangeEvent>
{
    public void Configure(EntityTypeBuilder<SettingChangeEvent> builder)
    {
        // Table configuration
        builder.ToTable("SettingChangeEvents", "settings");
        
        // Primary key - Client-generated GUID
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        // Base aggregate root properties
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(e => e.ModifiedAt)
            .IsRequired(false)
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(e => e.CreatedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
        builder.Property(e => e.ModifiedBy)
            .IsRequired(false)
            .HasDefaultValue("00000000-0000-0000-0000-000000000001");
            
        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
        
        // Properties configuration
        builder.Property(e => e.SettingId)
            .IsRequired();
            
        builder.Property(e => e.ChangedByUserId)
            .IsRequired();
            
        builder.Property(e => e.ChangeReason)
            .HasMaxLength(500)
            .IsRequired(false);
            
        // Configure SettingKey as a value object
        builder.OwnsOne(e => e.SettingKey, key =>
        {
            key.Property(k => k.Value)
                .HasColumnName("SettingKey")
                .HasMaxLength(200)
                .IsRequired();
                
            key.WithOwner();
                
            // Create index on the owned type property for performance
            key.HasIndex(k => k.Value);
        });
        
        // Configure OldValue as a value object
        builder.OwnsOne(e => e.OldValue, value =>
        {
            value.Property(v => v.RawValue)
                .HasColumnName("OldValue")
                .HasMaxLength(4000)
                .IsRequired();
                
            value.Property(v => v.Type)
                .HasColumnName("OldValueType")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
                
            value.WithOwner();
                
            // Create index on the value type for filtering
            value.HasIndex(v => v.Type);
        });
        
        // Configure NewValue as a value object
        builder.OwnsOne(e => e.NewValue, value =>
        {
            value.Property(v => v.RawValue)
                .HasColumnName("NewValue")
                .HasMaxLength(4000)
                .IsRequired();
                
            value.Property(v => v.Type)
                .HasColumnName("NewValueType")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
                
            value.WithOwner();
                
            // Create index on the value type for filtering
            value.HasIndex(v => v.Type);
        });
            
        // Relationships
        builder.HasOne(e => e.Setting)
            .WithMany(s => s.ChangeEvents)
            .HasForeignKey(e => e.SettingId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure backing field for collections
        builder.Metadata.FindNavigation(nameof(SettingChangeEvent.Setting))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        // Create indexes for commonly queried fields
        builder.HasIndex(e => e.SettingId);
            
        builder.HasIndex(e => e.ChangedByUserId);
            
        builder.HasIndex(e => e.CreatedAt);
            
        builder.HasIndex(e => new { e.SettingId, e.CreatedAt });
            
        builder.HasIndex(e => new { e.ChangedByUserId, e.CreatedAt });
            
        // Composite index for audit trail queries
        builder.HasIndex(e => new { e.SettingId, e.ChangedByUserId, e.CreatedAt });
    }
}
