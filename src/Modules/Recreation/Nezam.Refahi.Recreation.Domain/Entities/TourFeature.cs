using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Junction entity representing the many-to-many relationship between Tours and Features
/// </summary>
public sealed class TourFeature : Entity<Guid>
{
    public Guid TourId { get; private set; }
    public Guid FeatureId { get; private set; }
    public string? Notes { get; private set; }
    public bool IsHighlighted { get; private set; }
    public DateTime AssignedAt { get; private set; }

    // Navigation properties
    public Tour Tour { get; private set; } = null!;
    public Feature Feature { get; private set; } = null!;

    // Private constructor for EF Core
    private TourFeature() : base() { }

    /// <summary>
    /// Creates a new tour-feature relationship
    /// </summary>
    public TourFeature(Guid tourId, Guid featureId, string? notes = null, bool isHighlighted = false)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty", nameof(tourId));
        if (featureId == Guid.Empty)
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        TourId = tourId;
        FeatureId = featureId;
        Notes = notes?.Trim();
        IsHighlighted = isHighlighted;
        AssignedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the relationship notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Sets whether this feature should be highlighted for the tour
    /// </summary>
    public void SetHighlighted(bool isHighlighted)
    {
        IsHighlighted = isHighlighted;
    }
}