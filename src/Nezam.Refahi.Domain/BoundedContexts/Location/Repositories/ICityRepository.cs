using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;

/// <summary>
/// Repository interface for City entity
/// </summary>
public interface ICityRepository
{
    Task<City?> GetByIdAsync(Guid id);
    Task<IEnumerable<City>> GetAllAsync();
    Task<IEnumerable<City>> GetByProvinceIdAsync(Guid provinceId);
    Task<IEnumerable<City>> FindByNameAsync(string name);
    Task<IEnumerable<City>> FindByPostalCodeAsync(string postalCode);
    Task AddAsync(City city);
    Task UpdateAsync(City city);
    Task DeleteAsync(City city);
}
