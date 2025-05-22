using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;

namespace Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;

/// <summary>
/// Repository interface for Province entity
/// </summary>
public interface IProvinceRepository : IGenericRepository<Province>
{
    Task<Province?> GetByCodeAsync(string code);
    Task<IEnumerable<Province>> FindByNameAsync(string name);
}
