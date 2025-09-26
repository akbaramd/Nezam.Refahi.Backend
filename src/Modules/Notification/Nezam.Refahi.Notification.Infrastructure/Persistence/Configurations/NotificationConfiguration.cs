using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Notifications.Domain.Entities;

namespace Nezam.Refahi.Notifications.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for Notification
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Table configuration
        builder.ToTable("Notifications", "notification");
        
        // Primary key
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id)
            .ValueGeneratedNever(); // Client generates ID
        
        // Properties
        builder.Property(n => n.ExternalUserId)
            .IsRequired();
            
        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(n => n.Context)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(n => n.Action)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(n => n.IsRead)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(n => n.CreatedAt)
            .IsRequired();
            
        builder.Property(n => n.ExpiresAt)
            .IsRequired(false);
            
        builder.Property(n => n.Data)
            .IsRequired(false)
            .HasColumnType("NVARCHAR(MAX)");
        
        // Indexes
        builder.HasIndex(n => n.ExternalUserId)
            .HasDatabaseName("IX_Notifications_ExternalUserId");
            
        builder.HasIndex(n => n.Context)
            .HasDatabaseName("IX_Notifications_Context");
            
        builder.HasIndex(n => n.Action)
            .HasDatabaseName("IX_Notifications_Action");
            
        builder.HasIndex(n => n.IsRead)
            .HasDatabaseName("IX_Notifications_IsRead");
            
        builder.HasIndex(n => n.CreatedAt)
            .HasDatabaseName("IX_Notifications_CreatedAt");
            
        builder.HasIndex(n => n.ExpiresAt)
            .HasDatabaseName("IX_Notifications_ExpiresAt");
            
        // Composite index for user and read status
        builder.HasIndex(n => new { n.ExternalUserId, n.IsRead })
            .HasDatabaseName("IX_Notifications_ExternalUserId_IsRead");
            
        // Composite index for user and context
        builder.HasIndex(n => new { n.ExternalUserId, n.Context })
            .HasDatabaseName("IX_Notifications_ExternalUserId_Context");
            
        // Composite index for user and action
        builder.HasIndex(n => new { n.ExternalUserId, n.Action })
            .HasDatabaseName("IX_Notifications_ExternalUserId_Action");
    }
}