using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence;

public class RecreationDbContext : DbContext
{
    public RecreationDbContext(DbContextOptions<RecreationDbContext> options) : base(options)
    {
    }

    // DbSets for all Recreation entities
    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<TourCapacity> TourCapacities { get; set; } = null!;
    public DbSet<TourReservation> TourReservations { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
    public DbSet<ReservationPriceSnapshot> ReservationPriceSnapshots { get; set; } = null!;
    public DbSet<ApiIdempotency> ApiIdempotency { get; set; } = null!;
    public DbSet<TourPhoto> TourPhotos { get; set; } = null!;
    public DbSet<TourPricing> TourPricing { get; set; } = null!;
    public DbSet<TourFeature> TourFeatures { get; set; } = null!;
    public DbSet<Feature> Features { get; set; } = null!;
    public DbSet<FeatureCategory> FeatureCategories { get; set; } = null!;
    public DbSet<TourMemberCapability> TourMemberCapabilities { get; set; } = null!;
    public DbSet<TourMemberFeature> TourMemberFeatures { get; set; } = null!;
    public DbSet<TourRestrictedTour> TourRestrictedTours { get; set; } = null!;
    public DbSet<TourAgency> TourAgencies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RecreationDbContext).Assembly);

        // Set default schema for Recreation module
        modelBuilder.HasDefaultSchema("recreation");
    }
}
