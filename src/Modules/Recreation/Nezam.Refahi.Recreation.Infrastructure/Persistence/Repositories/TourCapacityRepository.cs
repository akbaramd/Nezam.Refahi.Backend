using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for tour capacity management
/// </summary>
public class TourCapacityRepository : EfRepository<RecreationDbContext,TourCapacity, Guid>, ITourCapacityRepository
{
    public TourCapacityRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TourCapacity>> GetByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(tc => tc.TourId == tourId)
            .OrderBy(tc => tc.RegistrationStart)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourCapacity>> GetActiveBytourIdAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(tc => tc.TourId == tourId && tc.IsActive)
            .OrderBy(tc => tc.RegistrationStart)
            .ToListAsync(cancellationToken);
    }

    public async Task<TourCapacity?> GetEffectiveCapacityAsync(Guid tourId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(tc => tc.TourId == tourId && 
                        tc.IsActive && 
                        tc.RegistrationStart <= date && 
                        tc.RegistrationEnd >= date)
            .OrderBy(tc => tc.RegistrationStart)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourCapacity>> GetOpenForRegistrationAsync(Guid tourId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(tc => tc.TourId == tourId && 
                        tc.IsActive && 
                        tc.RegistrationStart <= date && 
                        tc.RegistrationEnd >= date)
            .OrderBy(tc => tc.RegistrationStart)
            .ToListAsync(cancellationToken);
    }

    public async Task<TourCapacity?> GetWithTourAsync(Guid capacityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Include(tc => tc.Tour)
            .FirstOrDefaultAsync(tc => tc.Id == capacityId, cancellationToken);
    }

    public async Task<bool> IsActiveCapacityAsync(Guid capacityId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(tc => tc.Id == capacityId && tc.IsActive, cancellationToken);
    }
}
