using MCA.SharedKernel.Domain.Events;

namespace Nezam.Refahi.Recreation.Domain.Events;

/// <summary>
/// Domain event raised when a tour capacity is marked as special (VIP only)
/// </summary>
public sealed class TourCapacityMarkedAsSpecialEvent : DomainEvent
{
    public Guid TourCapacityId { get; }
    public Guid TourId { get; }
    public string? TenantId { get; }
    public DateTime MarkedAt { get; }

    public TourCapacityMarkedAsSpecialEvent(
        Guid tourCapacityId, 
        Guid tourId, 
        string? tenantId = null)
    {
        TourCapacityId = tourCapacityId;
        TourId = tourId;
        TenantId = tenantId;
        MarkedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Domain event raised when special status is removed from a tour capacity
/// </summary>
public sealed class TourCapacitySpecialStatusRemovedEvent : DomainEvent
{
    public Guid TourCapacityId { get; }
    public Guid TourId { get; }
    public string? TenantId { get; }
    public DateTime RemovedAt { get; }

    public TourCapacitySpecialStatusRemovedEvent(
        Guid tourCapacityId, 
        Guid tourId, 
        string? tenantId = null)
    {
        TourCapacityId = tourCapacityId;
        TourId = tourId;
        TenantId = tenantId;
        RemovedAt = DateTime.UtcNow;
    }
}
