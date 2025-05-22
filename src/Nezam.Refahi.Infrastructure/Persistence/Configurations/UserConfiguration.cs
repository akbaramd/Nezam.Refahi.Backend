using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary key
            builder.HasKey(u => u.Id);
            
            // Properties
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);
                
            // Configure NationalId as a primitive type
            // This is a simplification for the value object to make migrations work
            // In a production environment, we would use a more sophisticated approach
            builder.OwnsOne(g => g.NationalId, nationalId =>
            {
                nationalId.Property(n => n.Value)
                    .HasColumnName("NationalId")
                    .HasMaxLength(20)
                    .IsRequired();
            });
            
            // Configure Role as a string in the database
            builder.Property(u => u.Role)
                .HasColumnName("Role")
                .HasConversion(
                    v => v.ToString(),  // Convert enum to string when saving to database
                    v => (Role)Enum.Parse(typeof(Role), v)  // Convert string back to enum when reading from database
                )
                .HasMaxLength(200)  // Set an appropriate max length for the string representation
                .IsRequired();
        
        }
    }
