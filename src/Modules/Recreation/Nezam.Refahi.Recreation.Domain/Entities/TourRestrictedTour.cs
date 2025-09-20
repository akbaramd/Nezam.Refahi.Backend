using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Middle table entity for Tour-RestrictedTour many-to-many relationship
/// Defines which tours are restricted for participants of a specific tour
/// </summary>
public sealed class TourRestrictedTour : Entity<Guid>
{
    public Guid TourId { get; private set; }
    public Guid RestrictedTourId { get; private set; }

    // Navigation properties
    public Tour Tour { get; private set; } = null!;
    public Tour RestrictedTour { get; private set; } = null!;

    // Private constructor for EF Core
    private TourRestrictedTour() : base() { }

    /// <summary>
    /// Creates a new tour restriction relationship
    /// </summary>
    public TourRestrictedTour(Guid tourId, Guid restrictedTourId)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty", nameof(tourId));
        if (restrictedTourId == Guid.Empty)
            throw new ArgumentException("Restricted Tour ID cannot be empty", nameof(restrictedTourId));
        if (tourId == restrictedTourId)
            throw new InvalidOperationException("Tour cannot be restricted to itself");

        TourId = tourId;
        RestrictedTourId = restrictedTourId;
    }
}