using MCA.SharedKernel.Domain.Contracts;
using Nezam.Refahi.Shared.Domain.Shared.Entities;

namespace Nezam.Refahi.Shared.Domain.Shared.Repositories;

/// <summary>
/// Repository interface for Province entity
/// </summary>
public interface IProvinceRepository : IRepository<Province,Guid>
{
    Task<Province?> GetByCodeAsync(string code);
    Task<IEnumerable<Province>> FindByNameAsync(string name);
}
