using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Domain.Repositories;

/// <summary>
/// Repository interface for tour pricing
/// </summary>
public interface ITourPricingRepository : IRepository<TourPricing, Guid>
{
    /// <summary>
    /// Gets all pricing rules for a specific tour
    /// </summary>
    Task<IEnumerable<TourPricing>> GetByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active pricing rules for a specific tour
    /// </summary>
    Task<IEnumerable<TourPricing>> GetActiveByTourIdAsync(Guid tourId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pricing for a specific tour and participant type
    /// </summary>
    Task<TourPricing?> GetByTourIdAndParticipantTypeAsync(Guid tourId, ParticipantType participantType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets valid pricing for a specific tour, participant type, and date
    /// </summary>
    Task<TourPricing?> GetValidPricingAsync(Guid tourId, ParticipantType participantType, DateTime date, int quantity = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pricing rules that are valid for a specific date range
    /// </summary>
    Task<IEnumerable<TourPricing>> GetValidForDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if pricing exists for a tour and participant type
    /// </summary>
    Task<bool> ExistsForTourAndTypeAsync(Guid tourId, ParticipantType participantType, CancellationToken cancellationToken = default);
}