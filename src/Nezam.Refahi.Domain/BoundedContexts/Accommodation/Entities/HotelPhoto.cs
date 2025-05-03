using System;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;

/// <summary>
/// Represents a photo of a hotel
/// </summary>
public class HotelPhoto : BaseEntity
{
    public string Url { get; private set; } = null!;
    public string? Caption { get; private set; }
    public string? AltText { get; private set; }
    public bool IsMainPhoto { get; private set; }
    
    // Relationship with parent Hotel
    public Guid HotelId { get; private set; }
    
    // Navigation property for EF Core
    public Hotel? Hotel { get; private set; }
    
    // Private constructor for EF Core
    private HotelPhoto() : base() { }
    
    public HotelPhoto(string url, string? caption = null, string? altText = null, bool isMainPhoto = false) 
        : base()
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo URL cannot be empty", nameof(url));
            
        Url = url;
        Caption = caption;
        AltText = altText;
        IsMainPhoto = isMainPhoto;
    }
    
    public void SetHotel(Hotel hotel)
    {
        if (hotel == null)
            throw new ArgumentNullException(nameof(hotel));
            
        Hotel = hotel;
        HotelId = hotel.Id;
        UpdateModifiedAt();
    }
    
    public void UpdateDetails(string url, string? caption, string? altText)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Photo URL cannot be empty", nameof(url));
            
        Url = url;
        Caption = caption;
        AltText = altText;
        UpdateModifiedAt();
    }
    
    public void SetAsMainPhoto()
    {
        IsMainPhoto = true;
        UpdateModifiedAt();
    }
    
    public void UnsetAsMainPhoto()
    {
        IsMainPhoto = false;
        UpdateModifiedAt();
    }
}
