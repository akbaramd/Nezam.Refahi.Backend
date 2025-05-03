using System;
using System.Collections.Generic;
using System.Linq;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Aggregates;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;

/// <summary>
/// Represents a hotel in the system
/// </summary>
public class Hotel : BaseEntity
{
    // Private backing fields for collections with proper initialization for EF Core
    private readonly List<HotelFeature> _features = new();
    private readonly List<HotelPhoto> _photos = new();
    
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public LocationReference Location { get; private set; } = null!;
    public Money PricePerNight { get; private set; } = null!;
    public int Capacity { get; private set; }
    public bool IsAvailable { get; private set; }
    
    // Use IReadOnlyCollection for encapsulation but List for EF Core
    public IReadOnlyCollection<HotelFeature> Features => _features.AsReadOnly();
    public IReadOnlyCollection<HotelPhoto> Photos => _photos.AsReadOnly();
    
    // Navigation property for reservations (for EF Core relationship mapping)
    public ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();
    
    // Private constructor for EF Core
    private Hotel() : base() { }
    
    public Hotel(Guid id, string name, string description, LocationReference location, Money pricePerNight, int capacity) 
        : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Hotel name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
            
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be positive", nameof(capacity));
        Id = id;       
        Name = name;
        Description = description;
        Location = location ?? throw new ArgumentNullException(nameof(location));
        PricePerNight = pricePerNight ?? throw new ArgumentNullException(nameof(pricePerNight));
        Capacity = capacity;
        IsAvailable = true;
    }
    
    public void UpdateDetails(string name, string description, Money pricePerNight, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Hotel name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));
            
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be positive", nameof(capacity));
            
        Name = name;
        Description = description;
        PricePerNight = pricePerNight ?? throw new ArgumentNullException(nameof(pricePerNight));
        Capacity = capacity;
        UpdateModifiedAt();
    }
    
    public void UpdateLocation(LocationReference location)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
        UpdateModifiedAt();
    }
    
    public void AddFeature(HotelFeature feature)
    {
        if (feature == null)
            throw new ArgumentNullException(nameof(feature));
            
        if (_features.Any(f => f.Name == feature.Name))
            throw new InvalidOperationException($"Feature with name '{feature.Name}' already exists");
            
        // Set the relationship
        feature.SetHotel(this);
        _features.Add(feature);
        UpdateModifiedAt();
    }
    
    public void RemoveFeature(string featureName)
    {
        var feature = _features.FirstOrDefault(f => f.Name == featureName);
        if (feature != null)
        {
            _features.Remove(feature);
            UpdateModifiedAt();
        }
    }
    
    public void AddPhoto(HotelPhoto photo)
    {
        if (photo == null)
            throw new ArgumentNullException(nameof(photo));
            
        // Set the relationship
        photo.SetHotel(this);
        _photos.Add(photo);
        UpdateModifiedAt();
    }
    
    public void RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);
        if (photo != null)
        {
            _photos.Remove(photo);
            UpdateModifiedAt();
        }
    }
    
    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        UpdateModifiedAt();
    }
    
    public bool IsFree => PricePerNight.IsFree;
    
    // Adding a domain behavior method to check availability for specific dates
    public bool IsAvailableForDates(DateRange dateRange)
    {
        if (!IsAvailable)
            return false;
            
        // This would be implemented with repository in the application service
        // For now, just return basic availability status
        return IsAvailable;
    }
    
    // Calculate total price for a stay
    public Money CalculateTotalPrice(DateRange stayPeriod)
    {
        return PricePerNight * stayPeriod.NightCount;
    }
}
