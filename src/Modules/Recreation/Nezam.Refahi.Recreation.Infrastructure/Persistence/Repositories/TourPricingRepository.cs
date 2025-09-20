using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TourPricing entities
/// </summary>
public class TourPricingRepository : EfRepository<RecreationDbContext, TourPricing, Guid>, ITourPricingRepository
{
    public TourPricingRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TourPricing>> GetByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.TourId == tourId)
            .OrderBy(p => p.ParticipantType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourPricing>> GetActiveByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.TourId == tourId && p.IsActive)
            .OrderBy(p => p.ParticipantType)
            .ToListAsync(cancellationToken);
    }

    public async Task<TourPricing?> GetByTourIdAndParticipantTypeAsync(Guid tourId, ParticipantType participantType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(p => p.TourId == tourId &&
                                    p.ParticipantType == participantType &&
                                    p.IsActive, cancellationToken:cancellationToken);
    }

    public async Task<TourPricing?> GetValidPricingAsync(Guid tourId, ParticipantType participantType, DateTime date, int quantity = 1, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.TourId == tourId &&
                       p.ParticipantType == participantType &&
                       p.IsActive &&
                       (p.ValidFrom == null || date >= p.ValidFrom) &&
                       (p.ValidTo == null || date <= p.ValidTo) &&
                       (p.MinQuantity == null || quantity >= p.MinQuantity) &&
                       (p.MaxQuantity == null || quantity <= p.MaxQuantity))
            .OrderByDescending(p => p.ValidFrom) // Get the most recent pricing
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TourPricing>> GetValidForDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .Where(p => p.IsActive &&
                       (p.ValidFrom == null || p.ValidFrom <= endDate) &&
                       (p.ValidTo == null || p.ValidTo >= startDate))
            .OrderBy(p => p.TourId)
            .ThenBy(p => p.ParticipantType)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForTourAndTypeAsync(Guid tourId, ParticipantType participantType, CancellationToken cancellationToken = default)
    {
        return await PrepareQuery(_dbSet)
            .AnyAsync(p => p.TourId == tourId &&
                          p.ParticipantType == participantType &&
                          p.IsActive, cancellationToken:cancellationToken);
    }

    protected override IQueryable<TourPricing> PrepareQuery(IQueryable<TourPricing> query)
    {
        return query;
    }
}