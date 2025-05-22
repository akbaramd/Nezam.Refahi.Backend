using System;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;

namespace Nezam.Refahi.Domain.BoundedContexts.Accommodation.Services;

/// <summary>
/// Factory service for creating LocationReference objects from Location bounded context entities
/// This service acts as an Anti-Corruption Layer (ACL) between the two bounded contexts
/// </summary>
public class LocationReferenceFactory
{
    private readonly ICityRepository _cityRepository;
    private readonly IProvinceRepository _provinceRepository;
    
    public LocationReferenceFactory(
        ICityRepository cityRepository,
        IProvinceRepository provinceRepository)
    {
        _cityRepository = cityRepository ?? throw new ArgumentNullException(nameof(cityRepository));
        _provinceRepository = provinceRepository ?? throw new ArgumentNullException(nameof(provinceRepository));
    }
    
    /// <summary>
    /// Creates a LocationReference from City and Province entities
    /// </summary>
    public LocationReference CreateFromEntities(City city, Province province,string address)
    {
        if (city == null) throw new ArgumentNullException(nameof(city));
        if (province == null) throw new ArgumentNullException(nameof(province));
        
        return new LocationReference(
            city.Id,
            province.Id,
            city.Name,
            province.Name,
          address
        );
    }
    
    /// <summary>
    /// Asynchronously creates a LocationReference from City and Province IDs
    /// </summary>
    public async Task<LocationReference> CreateFromIdsAsync(Guid cityId, Guid provinceId,string address)
    {
        var city = await _cityRepository.GetByIdAsync(cityId);
        if (city == null) throw new ArgumentException($"City with ID {cityId} not found", nameof(cityId));
        
        var province = await _provinceRepository.GetByIdAsync(provinceId);
        if (province == null) throw new ArgumentException($"Province with ID {provinceId} not found", nameof(provinceId));
        
        return CreateFromEntities(city, province,address);
    }
    
    /// <summary>
    /// Converts the legacy Location value object to a LocationReference
    /// This is a helper method for data migration and should be used cautiously
    /// </summary>
    public async Task<LocationReference> CreateFromLegacyLocationAsync(string cityName, string provinceName,string address)
    {
        var provinces = await _provinceRepository.FindByNameAsync(provinceName);
        var province = provinces.FirstOrDefault();
        if (province == null)
            throw new InvalidOperationException($"Province '{provinceName}' not found");
            
        var cities = await _cityRepository.FindByNameAsync(cityName);
        var city = cities.FirstOrDefault(c => c.ProvinceId == province.Id);
        if (city == null)
            throw new InvalidOperationException($"City '{cityName}' not found in province '{provinceName}'");
            
        return CreateFromEntities(city, province,address);
    }
}
