using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        // Table configuration
        builder.ToTable("Members", "membership");
        
        // Primary key - Client-generated GUID
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor
        
        // UserId - Optional link to Auth module
        builder.Property(m => m.UserId)
            .IsRequired(false);
            
        // Configure NationalCode as owned value object
        builder.OwnsOne(m => m.NationalCode, nationalId =>
        {
            nationalId.Property(n => n.Value)
                .HasColumnName("NationalCode")
                .IsRequired()
                .HasMaxLength(20);
            // Indexes for performance and uniqueness
            nationalId.HasIndex(x => x.Value)
              .IsUnique()
              .HasDatabaseName("IX_Members_NationalCode");
            nationalId.WithOwner();
        });
        
        // Configure FullName as owned value object
        builder.OwnsOne(m => m.FullName, fullName =>
        {
            fullName.Property(fn => fn.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(100);
                
            fullName.Property(fn => fn.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(100);
                
            // Composite index for name searches
            fullName.HasIndex(x => x.FirstName);
            fullName.HasIndex(x => x.LastName);
            fullName.WithOwner();
        });
        
        // Configure Email as owned value object
        builder.OwnsOne(m => m.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(254);
                           
            // Index on Email for lookups
            email.HasIndex(x => x.Value)
              .IsUnique();
            
            
             

            email.WithOwner();
        });
        
        // Configure PhoneNumber as owned value object
        builder.OwnsOne(m => m.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(20);
            // Composite index for name searches
            phoneNumber.HasIndex(x => x.Value).IsUnique();
            phoneNumber.WithOwner();
        });
        
        // BirthDate as DateOnly
        builder.Property(m => m.BirthDate)
            .HasColumnType("date")
            .IsRequired(false);
            
        // Indexes for performance
        builder.HasIndex(m => m.UserId)
            .IsUnique()
            .HasFilter("[UserId] IS NOT NULL")
            .HasDatabaseName("IX_Members_UserId");
 
        // Audit properties (inherited from FullyAuditableAggregateRoot)
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");
            
        builder.Property(m => m.CreatedBy)
            .HasMaxLength(100);
            
        builder.Property(m => m.ModifiedAt)
            .HasColumnType("datetime2");
            
        builder.Property(m => m.ModifiedBy)
            .HasMaxLength(100);
            
        builder.Property(m => m.DeletedAt)
            .HasColumnType("datetime2");
            
        builder.Property(m => m.DeletedBy)
            .HasMaxLength(100);
            
        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        // Soft delete filter
        builder.HasQueryFilter(m => !m.IsDeleted);
        
        // Configure relationship with MemberRole
        builder.HasMany(m => m.Roles)
            .WithOne(mr => mr.Member)
            .HasForeignKey(mr => mr.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Configure relationship with MemberClaim
        builder.HasMany(m => m.Claims)
            .WithOne(mc => mc.Member)
            .HasForeignKey(mc => mc.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}