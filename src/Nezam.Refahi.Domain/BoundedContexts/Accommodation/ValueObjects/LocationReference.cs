using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

/// <summary>
/// A value object that references a city and province from the Location bounded context
/// This provides a clean separation between bounded contexts 
/// </summary>
public class LocationReference
{
    // References to the Location bounded context entities
    public Guid CityId { get; private set; }
    public Guid ProvinceId { get; private set; }
    
    // Cached values to avoid unnecessary cross-boundary lookups
    public string CityName { get; private set; } = null!;
    public string ProvinceName { get; private set; } = null!;
    
    // Detailed address information
    public string Address { get; private set; } = null!;
    
    // Required parameterless constructor for EF Core
    protected LocationReference() { }
    
    public LocationReference(Guid cityId, Guid provinceId, string cityName, string provinceName, string address)
    {
        if (cityId == Guid.Empty)
            throw new ArgumentException("City ID cannot be empty", nameof(cityId));
            
        if (provinceId == Guid.Empty)
            throw new ArgumentException("Province ID cannot be empty", nameof(provinceId));
            
        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("City name cannot be empty", nameof(cityName));
            
        if (string.IsNullOrWhiteSpace(provinceName))
            throw new ArgumentException("Province name cannot be empty", nameof(provinceName));
            
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));
            
        CityId = cityId;
        ProvinceId = provinceId;
        CityName = cityName;
        ProvinceName = provinceName;
        Address = address;
    }
    
    public override string ToString() => $"{Address}, {CityName}, {ProvinceName}";
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is LocationReference other && 
               CityId.Equals(other.CityId) && 
               ProvinceId.Equals(other.ProvinceId) &&
               Address.Equals(other.Address);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(CityId, ProvinceId, Address);
    }
    
    public static bool operator ==(LocationReference? left, LocationReference? right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(LocationReference? left, LocationReference? right)
    {
        return !Equals(left, right);
    }
}
