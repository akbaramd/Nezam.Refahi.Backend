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
    
    // Required parameterless constructor for EF Core
    protected LocationReference() { }
    
    public LocationReference(Guid cityId, Guid provinceId, string cityName, string provinceName)
    {
        if (cityId == Guid.Empty)
            throw new ArgumentException("City ID cannot be empty", nameof(cityId));
            
        if (provinceId == Guid.Empty)
            throw new ArgumentException("Province ID cannot be empty", nameof(provinceId));
            
        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("City name cannot be empty", nameof(cityName));
            
        if (string.IsNullOrWhiteSpace(provinceName))
            throw new ArgumentException("Province name cannot be empty", nameof(provinceName));
            
        CityId = cityId;
        ProvinceId = provinceId;
        CityName = cityName;
        ProvinceName = provinceName;
    }
    
    public override string ToString() => $"{CityName}, {ProvinceName}";
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is LocationReference other && 
               CityId.Equals(other.CityId) && 
               ProvinceId.Equals(other.ProvinceId);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(CityId, ProvinceId);
    }
    
    public static bool operator ==(LocationReference? left, LocationReference? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }
    
    public static bool operator !=(LocationReference? left, LocationReference? right)
    {
        return !(left == right);
    }
}
