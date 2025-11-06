using Nezam.Refahi.Facilities.Domain.Repositories;

namespace Nezam.Refahi.Facilities.Domain.Services;

/// <summary>
/// Domain Service for validating facility cycle business rules
/// Implements cross-aggregate validation logic
/// </summary>
public sealed class FacilityCycleValidationService : IFacilityCycleValidationService
{
    private readonly IFacilityCycleRepository _cycleRepository;

    public FacilityCycleValidationService(IFacilityCycleRepository cycleRepository)
    {
        _cycleRepository = cycleRepository ?? throw new ArgumentNullException(nameof(cycleRepository));
    }

    /// <summary>
    /// بررسی اینکه آیا نام دوره در یک تسهیلات یکتا است
    /// </summary>
    public async Task<bool> IsCycleNameUniqueAsync(
        Guid facilityId,
        string cycleName,
        Guid? excludeCycleId = null,
        CancellationToken cancellationToken = default)
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (string.IsNullOrWhiteSpace(cycleName))
            throw new ArgumentException("Cycle name cannot be empty", nameof(cycleName));

        return await _cycleRepository.IsNameUniqueAsync(
            facilityId,
            cycleName,
            excludeCycleId,
            cancellationToken);
    }

    /// <summary>
    /// بررسی اینکه آیا دوره جدید با دوره‌های موجود تداخل زمانی دارد
    /// </summary>
    public async Task<IEnumerable<Guid>> GetOverlappingCyclesAsync(
        Guid facilityId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeCycleId = null,
        CancellationToken cancellationToken = default)
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));

        var overlappingCycles = await _cycleRepository.GetOverlappingCyclesAsync(
            facilityId,
            startDate,
            endDate,
            excludeCycleId,
            cancellationToken);

        return overlappingCycles.Select(c => c.Id);
    }

}

