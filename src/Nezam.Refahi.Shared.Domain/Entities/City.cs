using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Shared.Domain.Entities;

/// <summary>
/// Represents a city in the system
/// </summary>
public class City : Entity<Guid>
{
    public string Name { get; private set; } = null!;
    
    // Relationship with parent Province
    public Guid ProvinceId { get; private set; }
    
    // Navigation property for EF Core
    public Province? Province { get; private set; }
    
    // Private constructor for EF Core
    private City() : base() { }
    
    public City(string name) : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty", nameof(name));
            
        Name = name;
    }
    
    public void SetProvince(Province province)
    {
        if (province == null)
            throw new ArgumentNullException(nameof(province));
            
        Province = province;
        ProvinceId = province.Id;
    }
    
    public void UpdateDetails(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty", nameof(name));
            
        Name = name;
    }
    
    public override string ToString()
    {
        return Name;
    }
}
