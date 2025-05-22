using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;

namespace Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;

/// <summary>
/// Repository interface for City entity
/// </summary>
public interface ICityRepository : IGenericRepository<City>
{
    Task<IEnumerable<City>> GetByProvinceIdAsync(Guid provinceId);
    Task<IEnumerable<City>> FindByNameAsync(string name);
    Task<IEnumerable<City>> FindByPostalCodeAsync(string postalCode);
}
