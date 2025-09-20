using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for Tour entity operations
/// </summary>
public interface ITourRepository : IRepository<Tour, Guid>
{
    /// <summary>
    /// Gets active tours only
    /// </summary>
    /// <returns>Collection of active tours</returns>
    Task<IEnumerable<Tour>> GetActiveToursAsync();


    /// <summary>
    /// Gets upcoming tours (starting after the specified date)
    /// </summary>
    /// <param name="fromDate">Date to filter tours from</param>
    /// <returns>Collection of upcoming tours</returns>
    Task<IEnumerable<Tour>> GetUpcomingToursAsync(DateTime fromDate);

    /// <summary>
    /// Gets tours within a date range
    /// </summary>
    /// <param name="startDate">Start date range</param>
    /// <param name="endDate">End date range</param>
    /// <returns>Collection of tours within the date range</returns>
    Task<IEnumerable<Tour>> GetToursByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Searches tours by title (partial match)
    /// </summary>
    /// <param name="searchTerm">Search term for title</param>
    /// <returns>Collection of tours matching the search term</returns>
    Task<IEnumerable<Tour>> SearchToursByTitleAsync(string searchTerm);

    /// <summary>
    /// Gets paginated list of tours
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="activeOnly">Filter only active tours</param>
    /// <returns>Paginated list of tours</returns>
    Task<(IEnumerable<Tour> Tours, int TotalCount)> GetToursPaginatedAsync(
        int pageNumber,
        int pageSize,
        bool activeOnly = true);

   
    /// <summary>
    /// Gets tours with their photos included
    /// </summary>
    /// <param name="tourId">Tour ID</param>
    /// <returns>Tour with photos if found, null otherwise</returns>
    Task<Tour?> GetTourWithPhotosAsync(Guid tourId);

  
    /// <summary>
    /// Gets tours by price range
    /// </summary>
    /// <param name="minPriceRials">Minimum price in rials</param>
    /// <param name="maxPriceRials">Maximum price in rials</param>
    /// <returns>Collection of tours within the price range</returns>
    Task<IEnumerable<Tour>> GetToursByPriceRangeAsync(long minPriceRials, long maxPriceRials);


    /// <summary>
    /// Gets tours by age requirements
    /// </summary>
    /// <param name="age">Participant age</param>
    /// <returns>Collection of tours suitable for the age</returns>
    Task<IEnumerable<Tour>> GetToursByAgeAsync(int age);

    /// <summary>
    /// Gets tours with age restrictions
    /// </summary>
    /// <returns>Collection of tours that have min/max age requirements</returns>
    Task<IEnumerable<Tour>> GetToursWithAgeRestrictionsAsync();
}