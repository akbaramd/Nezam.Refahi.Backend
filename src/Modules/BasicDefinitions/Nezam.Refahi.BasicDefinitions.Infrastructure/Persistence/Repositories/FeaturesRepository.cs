using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Features entity
/// </summary>
public class FeaturesRepository : EfRepository<BasicDefinitionsDbContext, Features, string>, IFeaturesRepository
{
    public FeaturesRepository(BasicDefinitionsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Features>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => f.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Features>> GetActiveFeaturesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => !f.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(f => f.Id == key, cancellationToken);
    }

    public async Task<IEnumerable<Features>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(f => keys.Contains(f.Id))
            .ToListAsync(cancellationToken);
    }
}
