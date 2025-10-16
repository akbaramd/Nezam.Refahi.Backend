using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// وابستگی دوره تسهیلات به تسهیلات دیگر
/// در دنیای واقعی: برخی تسهیلات نیاز به تکمیل تسهیلات قبلی دارند
/// </summary>
public sealed class FacilityCycleDependency : Entity<Guid>
{
    public Guid CycleId { get; private set; }
    public Guid RequiredFacilityId { get; private set; }
    public string RequiredFacilityName { get; private set; } = null!;
    public bool MustBeCompleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation properties
    public FacilityCycle FacilityCycle { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityCycleDependency() : base() { }

    public FacilityCycleDependency(
        Guid cycleId,
        Guid requiredFacilityId,
        string requiredFacilityName,
        bool mustBeCompleted = true)
        : base(Guid.NewGuid())
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle ID cannot be empty", nameof(cycleId));
        if (requiredFacilityId == Guid.Empty)
            throw new ArgumentException("Required facility ID cannot be empty", nameof(requiredFacilityId));
        if (string.IsNullOrWhiteSpace(requiredFacilityName))
            throw new ArgumentException("Required facility name cannot be empty", nameof(requiredFacilityName));

        CycleId = cycleId;
        RequiredFacilityId = requiredFacilityId;
        RequiredFacilityName = requiredFacilityName.Trim();
        MustBeCompleted = mustBeCompleted;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تغییر وضعیت وابستگی
    /// </summary>
    public void ChangeRequirement(bool mustBeCompleted)
    {
        MustBeCompleted = mustBeCompleted;
    }
}
