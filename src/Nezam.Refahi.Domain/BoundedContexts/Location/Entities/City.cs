using System;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Location.Entities;

/// <summary>
/// Represents a city in the system
/// </summary>
public class City : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? PostalCode { get; private set; }
    
    // Relationship with parent Province
    public Guid ProvinceId { get; private set; }
    
    // Navigation property for EF Core
    public Province? Province { get; private set; }
    
    // Private constructor for EF Core
    private City() : base() { }
    
    public City(string name, string? postalCode = null) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty", nameof(name));
            
        Name = name;
        PostalCode = postalCode;
    }
    
    public void SetProvince(Province province)
    {
        if (province == null)
            throw new ArgumentNullException(nameof(province));
            
        Province = province;
        ProvinceId = province.Id;
        UpdateModifiedAt();
    }
    
    public void UpdateDetails(string name, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty", nameof(name));
            
        Name = name;
        PostalCode = postalCode;
        UpdateModifiedAt();
    }
    
    public override string ToString()
    {
        return PostalCode != null 
            ? $"{Name} ({PostalCode})" 
            : Name;
    }
}
