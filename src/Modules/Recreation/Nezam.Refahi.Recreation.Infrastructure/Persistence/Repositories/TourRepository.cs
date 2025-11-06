using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Tour entities
/// </summary>
public class TourRepository : EfRepository<RecreationDbContext, Tour, Guid>, ITourRepository
{
    public TourRepository(RecreationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Tour>> GetActiveToursAsync()
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.IsActive)
            .OrderBy(t => t.TourStart)
            .ToListAsync();
    }


  

    public async Task<IEnumerable<Tour>> GetUpcomingToursAsync(DateTime fromDate)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.IsActive && t.TourStart > fromDate)
            .OrderBy(t => t.TourStart)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tour>> GetToursByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.TourStart >= startDate && t.TourStart <= endDate)
            .OrderBy(t => t.TourStart)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tour>> SearchToursByTitleAsync(string searchTerm)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.Title.Contains(searchTerm))
            .OrderBy(t => t.TourStart)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Tour> Tours, int TotalCount)> GetToursPaginatedAsync(
        int pageNumber, int pageSize, bool activeOnly = true)
    {
        var query = PrepareQuery(_dbSet);

        if (activeOnly)
            query = query.Where(t => t.IsActive);

        var totalCount = await query.CountAsync();

        var tours = await query
            .OrderBy(t => t.TourStart)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (tours, totalCount);
    }



    public async Task<Tour?> GetTourWithPhotosAsync(Guid tourId)
    {
        return await PrepareQuery(_dbSet)
            .FirstOrDefaultAsync(t => t.Id == tourId);
    }

   

    public async Task<IEnumerable<Tour>> GetToursByPriceRangeAsync(long minPriceRials, long maxPriceRials)
    {
        // This would need to be implemented with pricing logic
        // For now, returning all tours - would need to join with TourPricing table
        return await PrepareQuery(_dbSet)
            .Where(t => t.IsActive)
            .OrderBy(t => t.TourStart)
            .ToListAsync();
    }


    public async Task<IEnumerable<Tour>> GetToursByAgeAsync(int age)
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.IsActive &&
                       (t.MinAge == null || age >= t.MinAge) &&
                       (t.MaxAge == null || age <= t.MaxAge))
            .OrderBy(t => t.TourStart)
            .ToListAsync();
    }

    public Task<IEnumerable<Tour>> GetAvailableToursForParticipantAsync(IEnumerable<Guid> participantTourIds)
    {
      throw new NotImplementedException();
    }


    public async Task<IEnumerable<Tour>> GetToursWithAgeRestrictionsAsync()
    {
        return await PrepareQuery(_dbSet)
            .Where(t => t.MinAge != null || t.MaxAge != null)
            .OrderBy(t => t.TourStart)
            .ToListAsync();
    }

    protected override IQueryable<Tour> PrepareQuery(IQueryable<Tour> query)
    {
      query = query.Include(x => x.Photos)
        .Include(v => v.MemberCapabilities)
        .Include(c => c.MemberFeatures)
        .Include(c => c.Pricing)
        .ThenInclude(c => c.Capabilities)
        .Include(c => c.Pricing)
        .ThenInclude(c => c.Features)
        .Include(c => c.Capacities)
        .Include(c => c.Reservations)
        .Include(x => x.TourFeatures).ThenInclude(x=>x.Feature);
      return query;
    }
}