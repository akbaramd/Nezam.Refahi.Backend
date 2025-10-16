using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Agency
/// </summary>
public class AgencyRepository : EfRepository<BasicDefinitionsDbContext, Agency, Guid>, IAgencyRepository
{
    public AgencyRepository(BasicDefinitionsDbContext context) : base(context)
    {
    }

    public async Task<Agency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<Agency?> GetByExternalCodeAsync(string externalCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.ExternalCode == externalCode, cancellationToken);
    }

    public async Task<IEnumerable<Agency>> GetActiveOfficesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agency>> GetByManagerAsync(string managerName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(x => x.ManagerName == managerName && x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, Guid excludeOfficeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(x => x.Code == code && x.Id != excludeOfficeId, cancellationToken);
    }

    public async Task<bool> ExternalCodeExistsAsync(string externalCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(x => x.ExternalCode == externalCode, cancellationToken);
    }

    public async Task<bool> ExternalCodeExistsAsync(string externalCode, Guid excludeOfficeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(x => x.ExternalCode == externalCode && x.Id != excludeOfficeId, cancellationToken);
    }
}
