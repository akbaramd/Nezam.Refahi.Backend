using System;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;

/// <summary>
/// Represents a feature of a hotel as an entity
/// </summary>
public class HotelFeature : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public string? Description { get; private set; }
    
    // Relationship with parent Hotel
    public Guid HotelId { get; private set; }
    
    // Navigation property for EF Core
    public Hotel? Hotel { get; private set; }

    // Required parameterless constructor for EF Core
    private HotelFeature() : base() { }

    public HotelFeature(string name, string value, string? description = null) 
        : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Feature name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Feature value cannot be empty", nameof(value));
            
        Name = name;
        Value = value;
        Description = description;
    }
    
    public void SetHotel(Hotel hotel)
    {
        if (hotel == null)
            throw new ArgumentNullException(nameof(hotel));
            
        Hotel = hotel;
        HotelId = hotel.Id;
        UpdateModifiedAt();
    }
    
    public void UpdateValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Feature value cannot be empty", nameof(value));
            
        Value = value;
        UpdateModifiedAt();
    }
    
    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdateModifiedAt();
    }
    
    // Common hotel features factory methods
    public static HotelFeature BedroomCount(int count) => 
        new HotelFeature("BedroomCount", count.ToString(), "تعداد اتاق خواب");
        
    public static HotelFeature HasWifi(bool hasWifi) => 
        new HotelFeature("HasWifi", hasWifi.ToString(), "دسترسی به اینترنت");
        
    public static HotelFeature HasBreakfast(bool hasBreakfast) => 
        new HotelFeature("HasBreakfast", hasBreakfast.ToString(), "صبحانه");
        
    public static HotelFeature HasParking(bool hasParking) => 
        new HotelFeature("HasParking", hasParking.ToString(), "پارکینگ");
}
