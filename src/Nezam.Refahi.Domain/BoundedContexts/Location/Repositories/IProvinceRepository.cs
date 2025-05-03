using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;

/// <summary>
/// Repository interface for Province entity
/// </summary>
public interface IProvinceRepository
{
    Task<Province?> GetByIdAsync(Guid id);
    Task<Province?> GetByCodeAsync(string code);
    Task<IEnumerable<Province>> GetAllAsync();
    Task<IEnumerable<Province>> FindByNameAsync(string name);
    Task AddAsync(Province province);
    Task UpdateAsync(Province province);
    Task DeleteAsync(Province province);
}
