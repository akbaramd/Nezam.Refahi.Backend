using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Shared.Domain.Entities;

namespace Nezam.Refahi.Shared.Domain.Repositories;

/// <summary>
/// Repository interface for City entity
/// </summary>
public interface ICityRepository : IRepository<City,Guid>
{
    Task<IEnumerable<City>> GetByProvinceIdAsync(Guid provinceId);
    Task<IEnumerable<City>> FindByNameAsync(string name);
}
