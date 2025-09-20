using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Shared.Domain.Entities;

namespace Nezam.Refahi.Shared.Domain.Repositories;

/// <summary>
/// Repository interface for Province entity
/// </summary>
public interface IProvinceRepository : IRepository<Province,Guid>
{
    Task<Province?> GetByCodeAsync(string code);
    Task<IEnumerable<Province>> FindByNameAsync(string name);
}
