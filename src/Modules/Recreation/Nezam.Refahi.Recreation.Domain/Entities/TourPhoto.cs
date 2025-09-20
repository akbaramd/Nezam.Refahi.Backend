using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Entity representing a photo associated with a tour
/// </summary>
public sealed class TourPhoto : Entity<Guid>
{
    public string Url { get; private set; } = null!;
    public string? Caption { get; private set; }
    public string FileName { get; set; }= null!;
    public string FilePath { get; set; }= null!;
    public int DisplayOrder { get; private set; }
    public Tour Tour { get; private set; } = null!;
    public Guid TourId { get; private set; } 

    // Private constructor for EF Core
    private TourPhoto() : base() { }

    /// <summary>
    /// Creates a new tour photo
    /// </summary>
    public TourPhoto(Guid tourId, string url, string? caption = null, int displayOrder = 0)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("Tour ID cannot be empty", nameof(tourId));
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo URL cannot be empty", nameof(url));

        TourId = tourId;
        Url = url.Trim();
        Caption = caption?.Trim();
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Updates photo details
    /// </summary>
    public void UpdateDetails(string url, string? caption = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo URL cannot be empty", nameof(url));

        Url = url.Trim();
        Caption = caption?.Trim();
    }

    /// <summary>
    /// Updates the display order
    /// </summary>
    public void UpdateDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}