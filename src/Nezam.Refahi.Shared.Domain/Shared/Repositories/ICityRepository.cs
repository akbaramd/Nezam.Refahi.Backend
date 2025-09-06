using MCA.SharedKernel.Domain.Contracts;
using Nezam.Refahi.Shared.Domain.Shared.Entities;

namespace Nezam.Refahi.Shared.Domain.Shared.Repositories;

/// <summary>
/// Repository interface for City entity
/// </summary>
public interface ICityRepository : IRepository<City,Guid>
{
    Task<IEnumerable<City>> GetByProvinceIdAsync(Guid provinceId);
    Task<IEnumerable<City>> FindByNameAsync(string name);
}
