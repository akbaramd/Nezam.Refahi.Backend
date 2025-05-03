using System;
using System.Collections.Generic;
using System.Linq;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Location.Entities;

/// <summary>
/// Represents a province (state/region) in the system
/// </summary>
public class Province : BaseEntity
{
    private readonly List<City> _cities = new();
    
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    
    // Navigation property for EF Core
    public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();
    
    // Private constructor for EF Core
    private Province() : base() { }
    
    public Province(string name, string code) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Province name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Province code cannot be empty", nameof(code));
            
        Name = name;
        Code = code;
    }
    
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Province name cannot be empty", nameof(name));
            
        Name = name;
        UpdateModifiedAt();
    }
    
    public void AddCity(City city)
    {
        if (city == null)
            throw new ArgumentNullException(nameof(city));
            
        if (_cities.Any(c => c.Name.Equals(city.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"City with name '{city.Name}' already exists in this province");
            
        city.SetProvince(this);
        _cities.Add(city);
        UpdateModifiedAt();
    }
    
    public override string ToString() => $"{Name} ({Code})";
}
