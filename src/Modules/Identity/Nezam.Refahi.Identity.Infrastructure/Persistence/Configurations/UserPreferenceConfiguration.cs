using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.Services;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Configurations;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        // Table configuration
        builder.ToTable("UserPreferences", "identity");
        
        // Primary key - Client-generated GUID
        builder.HasKey(up => up.Id);
        builder.Property(up => up.Id)
            .ValueGeneratedNever(); // MUST: Client generates ID in constructor
        
        // Foreign key to UserDetail
        builder.Property(up => up.UserId)
            .IsRequired();
            
        // Configure PreferenceKey as a value object
        builder.OwnsOne(up => up.Key, key =>
        {
            key.Property(k => k.Value)
                .HasColumnName("Key")
                .HasMaxLength(100)
                .IsRequired();
                
            key.WithOwner();
        });
        
        // Configure PreferenceValue as a value object
        builder.OwnsOne(up => up.Value, value =>
        {
            value.Property(v => v.RawValue)
                .HasColumnName("Value")
                .HasMaxLength(2000) // Large enough for JSON values
                .IsRequired();
                
            value.Property(v => v.Type)
                .HasColumnName("ValueType")
                .HasConversion(
                    v => v.ToString(),
                    v => (PreferenceType)Enum.Parse(typeof(PreferenceType), v)
                )
                .HasMaxLength(50)
                .IsRequired();
                
            value.WithOwner();
            
            // Index for ValueType inside OwnsOne
            value.HasIndex(v => v.Type)
                .HasDatabaseName("IX_UserPreferences_ValueType");
        });
        
        // Description
        builder.Property(up => up.Description)
            .IsRequired()
            .HasMaxLength(500);
            
        // IsActive
        builder.Property(up => up.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // DisplayOrder
        builder.Property(up => up.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);
            
        // Category
        builder.Property(up => up.Category)
            .HasColumnName("Category")
            .HasConversion(
                v => v.ToString(),
                v => (PreferenceCategory)Enum.Parse(typeof(PreferenceCategory), v)
            )
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue(PreferenceCategory.General)
            .HasSentinel(PreferenceCategory.General);
            
        // Concurrency control - MUST: Rowversion for critical clusters
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();
            
 
            
        // Configure navigation property to UserDetail
        builder.HasOne(up => up.User)
            .WithMany(u => u.Preferences)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Note: Unique index on UserId + Key combination will be created through migration
        // as it involves owned type properties
            
        // Create indexes for commonly queried fields
        builder.HasIndex(up => up.UserId);
        builder.HasIndex(up => up.IsActive);
        builder.HasIndex(up => up.DisplayOrder);
        builder.HasIndex(up => up.Category);
        
        // Index for ValueType is now inside OwnsOne configuration above
        
 
        // Create composite index for filtering by user and category
        builder.HasIndex(up => new { up.UserId, up.Category })
            .HasDatabaseName("IX_UserPreferences_UserId_Category");
    }
}
