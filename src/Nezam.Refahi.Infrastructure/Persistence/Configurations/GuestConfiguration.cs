using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;

namespace Nezam.Refahi.Infrastructure.Persistence.Configurations
{
    public class GuestConfiguration : IEntityTypeConfiguration<Guest>
    {
        public void Configure(EntityTypeBuilder<Guest> builder)
        {
            // Primary key
            builder.HasKey(g => g.Id);

            // Properties
            builder.Property(g => g.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(g => g.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(g => g.Age)
                .IsRequired();

            // Value object: NationalId
            builder.OwnsOne(g => g.NationalId, nationalId =>
            {
                nationalId.Property(n => n.Value)
                    .HasColumnName("NationalId")
                    .HasMaxLength(20)
                    .IsRequired();
            });
        }
    }
}
