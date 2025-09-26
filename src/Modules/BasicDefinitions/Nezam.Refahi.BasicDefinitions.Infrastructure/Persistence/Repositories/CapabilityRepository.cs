using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Capability entity
/// </summary>
public class CapabilityRepository : EfRepository<BasicDefinitionsDbContext, Capability, string>, ICapabilityRepository
{
    public CapabilityRepository(BasicDefinitionsDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Capability>> GetActiveCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive && !c.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Capability>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Name.Contains(name))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(c => c.Id == key, cancellationToken);
    }

    public async Task<IEnumerable<Capability>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => keys.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Capability>> GetValidCapabilitiesAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive && 
                       !c.IsDeleted &&
                       (!c.ValidFrom.HasValue || c.ValidFrom.Value <= date) &&
                       (!c.ValidTo.HasValue || c.ValidTo.Value >= date))
            .ToListAsync(cancellationToken);
    }
}
