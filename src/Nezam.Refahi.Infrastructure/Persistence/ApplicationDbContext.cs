using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates;
using System.Reflection;

namespace Nezam.Refahi.Infrastructure.Persistence
{
    /// <summary>
    /// Main database context for the application
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Accommodation bounded context
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        // Identity bounded context
        public DbSet<User> Users { get; set; }

        // Location bounded context
        public DbSet<City> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }

        // Payment bounded context
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
