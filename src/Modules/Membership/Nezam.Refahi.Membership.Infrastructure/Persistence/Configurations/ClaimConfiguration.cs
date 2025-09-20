using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence.Configurations;

public class ClaimConfiguration : IEntityTypeConfiguration<Features>
{
    public void Configure(EntityTypeBuilder<Features> builder)
    {
        // Table configuration
        builder.ToTable("Features", "membership");
        
        // Primary key - Client-generated GUID
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Id)
            .ValueGeneratedNever(); // Client generates ID in constructor
        
        // Key property - unique identifier
        // Title property
        builder.Property(ct => ct.Title)
            .IsRequired()
            .HasMaxLength(200);
        


    }
}