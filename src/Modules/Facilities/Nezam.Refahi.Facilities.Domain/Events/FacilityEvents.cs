using MCA.SharedKernel.Domain.Events;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Events;

/// <summary>
/// رویداد ایجاد تسهیلات جدید
/// </summary>
public sealed class FacilityCreatedEvent : DomainEvent
{
    public Guid FacilityId { get; }
    public string Name { get; }
    public string Code { get; }
    public FacilityType Type { get; }
    public Money? DefaultAmount { get; }
    public DateTime CreatedAt { get; }

    public FacilityCreatedEvent(
        Guid facilityId,
        string name,
        string code,
        FacilityType type,
        Money? defaultAmount,
        DateTime createdAt)
    {
        FacilityId = facilityId;
        Name = name;
        Code = code;
        Type = type;
        DefaultAmount = defaultAmount;
        CreatedAt = createdAt;
    }
}

/// <summary>
/// رویداد تغییر وضعیت تسهیلات
/// </summary>
public sealed class FacilityStatusChangedEvent : DomainEvent
{
    public Guid FacilityId { get; }
    public string Name { get; }
    public string Code { get; }
    public FacilityStatus PreviousStatus { get; }
    public FacilityStatus NewStatus { get; }
    public string Reason { get; }

    public FacilityStatusChangedEvent(
        Guid facilityId,
        string name,
        string code,
        FacilityStatus previousStatus,
        FacilityStatus newStatus,
        string reason)
    {
        FacilityId = facilityId;
        Name = name;
        Code = code;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}

/// <summary>
/// رویداد اضافه کردن ویژگی به تسهیلات
/// </summary>
public sealed class FacilityFeatureAddedEvent : DomainEvent
{
    public Guid FacilityId { get; }
    public string Name { get; }
    public string Code { get; }
    public string FeatureId { get; }
    public FacilityFeatureRequirementType RequirementType { get; }
    public string? Notes { get; }

    public FacilityFeatureAddedEvent(
        Guid facilityId,
        string name,
        string code,
        string featureId,
        FacilityFeatureRequirementType requirementType,
        string? notes)
    {
        FacilityId = facilityId;
        Name = name;
        Code = code;
        FeatureId = featureId;
        RequirementType = requirementType;
        Notes = notes;
    }
}

/// <summary>
/// رویداد حذف ویژگی از تسهیلات
/// </summary>
public sealed class FacilityFeatureRemovedEvent : DomainEvent
{
    public Guid FacilityId { get; }
    public string Name { get; }
    public string Code { get; }
    public string FeatureId { get; }

    public FacilityFeatureRemovedEvent(
        Guid facilityId,
        string name,
        string code,
        string featureId)
    {
        FacilityId = facilityId;
        Name = name;
        Code = code;
        FeatureId = featureId;
    }
}

/// <summary>
/// رویداد اضافه کردن سیاست قابلیت به تسهیلات
/// </summary>
public sealed class FacilityCapabilityPolicyAddedEvent : DomainEvent
{
    public Guid FacilityId { get; }
    public string Name { get; }
    public string Code { get; }
    public string CapabilityId { get; }

    public FacilityCapabilityPolicyAddedEvent(
        Guid facilityId,
        string name,
        string code,
        string capabilityId)
    {
        FacilityId = facilityId;
        Name = name;
        Code = code;
        CapabilityId = capabilityId;
    }
}

/// <summary>
/// رویداد حذف سیاست قابلیت از تسهیلات
/// </summary>
public sealed class FacilityCapabilityPolicyRemovedEvent : DomainEvent
{
    public Guid FacilityId { get; }
    public string Name { get; }
    public string Code { get; }
    public string CapabilityId { get; }

    public FacilityCapabilityPolicyRemovedEvent(
        Guid facilityId,
        string name,
        string code,
        string capabilityId)
    {
        FacilityId = facilityId;
        Name = name;
        Code = code;
        CapabilityId = capabilityId;
    }
}

/// <summary>
/// رویداد ایجاد دوره تسهیلات
/// </summary>
public sealed class FacilityCycleCreatedEvent : DomainEvent
{
    public Guid CycleId { get; }
    public Guid FacilityId { get; }
    public string FacilityName { get; }
    public string FacilityCode { get; }
    public string CycleName { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public int Quota { get; }

    public FacilityCycleCreatedEvent(
        Guid cycleId,
        Guid facilityId,
        string facilityName,
        string facilityCode,
        string cycleName,
        DateTime startDate,
        DateTime endDate,
        int quota)
    {
        CycleId = cycleId;
        FacilityId = facilityId;
        FacilityName = facilityName;
        FacilityCode = facilityCode;
        CycleName = cycleName;
        StartDate = startDate;
        EndDate = endDate;
        Quota = quota;
    }
}

/// <summary>
/// رویداد تغییر وضعیت دوره تسهیلات
/// </summary>
public sealed class FacilityCycleStatusChangedEvent : DomainEvent
{
    public Guid CycleId { get; }
    public Guid FacilityId { get; }
    public string CycleName { get; }
    public FacilityCycleStatus PreviousStatus { get; }
    public FacilityCycleStatus NewStatus { get; }
    public string Reason { get; }

    public FacilityCycleStatusChangedEvent(
        Guid cycleId,
        Guid facilityId,
        string cycleName,
        FacilityCycleStatus previousStatus,
        FacilityCycleStatus newStatus,
        string reason)
    {
        CycleId = cycleId;
        FacilityId = facilityId;
        CycleName = cycleName;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}

/// <summary>
/// رویداد اضافه کردن درخواست به دوره
/// </summary>
public sealed class FacilityRequestAddedToCycleEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid CycleId { get; }
    public Guid FacilityId { get; }
    public int UsedQuota { get; }
    public int TotalQuota { get; }

    public FacilityRequestAddedToCycleEvent(
        Guid applicationId,
        Guid cycleId,
        Guid facilityId,
        int usedQuota,
        int totalQuota)
    {
        ApplicationId = applicationId;
        CycleId = cycleId;
        FacilityId = facilityId;
        UsedQuota = usedQuota;
        TotalQuota = totalQuota;
    }
}

/// <summary>
/// رویداد ایجاد درخواست تسهیلات
/// </summary>
public sealed class FacilityRequestCreatedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid FacilityId { get; }
    public Guid FacilityCycleId { get; }
    public Guid ExternalUserId { get; }
    public string? UserFullName { get; }
    public Money RequestedAmount { get; }
    public string ApplicationNumber { get; }
    public DateTime CreatedAt { get; }

    public FacilityRequestCreatedEvent(
        Guid applicationId,
        Guid facilityId,
        Guid facilityCycleId,
        Guid externalUserId,
        string? userFullName,
        Money requestedAmount,
        string applicationNumber,
        DateTime createdAt)
    {
        ApplicationId = applicationId;
        FacilityId = facilityId;
        FacilityCycleId = facilityCycleId;
        ExternalUserId = externalUserId;
        UserFullName = userFullName;
        RequestedAmount = requestedAmount;
        ApplicationNumber = applicationNumber;
        CreatedAt = createdAt;
    }
}

/// <summary>
/// رویداد تغییر وضعیت درخواست تسهیلات
/// </summary>
public sealed class FacilityRequestStatusChangedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid FacilityId { get; }
    public Guid ExternalUserId { get; }
    public string ApplicationNumber { get; }
    public FacilityRequestStatus PreviousStatus { get; }
    public FacilityRequestStatus NewStatus { get; }
    public string Reason { get; }

    public FacilityRequestStatusChangedEvent(
        Guid applicationId,
        Guid facilityId,
        Guid externalUserId,
        string applicationNumber,
        FacilityRequestStatus previousStatus,
        FacilityRequestStatus newStatus,
        string reason)
    {
        ApplicationId = applicationId;
        FacilityId = facilityId;
        ExternalUserId = externalUserId;
        ApplicationNumber = applicationNumber;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = reason;
    }
}

/// <summary>
/// رویداد تأیید درخواست تسهیلات
/// </summary>
public sealed class FacilityRequestApprovedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid FacilityId { get; }
    public Guid ExternalUserId { get; }
    public string ApplicationNumber { get; }
    public Money RequestedAmount { get; }
    public Money ApprovedAmount { get; }
    public DateTime ApprovedAt { get; }

    public FacilityRequestApprovedEvent(
        Guid applicationId,
        Guid facilityId,
        Guid externalUserId,
        string applicationNumber,
        Money requestedAmount,
        Money approvedAmount,
        DateTime approvedAt)
    {
        ApplicationId = applicationId;
        FacilityId = facilityId;
        ExternalUserId = externalUserId;
        ApplicationNumber = applicationNumber;
        RequestedAmount = requestedAmount;
        ApprovedAmount = approvedAmount;
        ApprovedAt = approvedAt;
    }
}

/// <summary>
/// رویداد رد درخواست تسهیلات
/// </summary>
public sealed class FacilityRequestRejectedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid FacilityId { get; }
    public Guid ExternalUserId { get; }
    public string ApplicationNumber { get; }
    public Money RequestedAmount { get; }
    public string RejectionReason { get; }
    public DateTime RejectedAt { get; }

    public FacilityRequestRejectedEvent(
        Guid applicationId,
        Guid facilityId,
        Guid externalUserId,
        string applicationNumber,
        Money requestedAmount,
        string rejectionReason,
        DateTime rejectedAt)
    {
        ApplicationId = applicationId;
        FacilityId = facilityId;
        ExternalUserId = externalUserId;
        ApplicationNumber = applicationNumber;
        RequestedAmount = requestedAmount;
        RejectionReason = rejectionReason;
        RejectedAt = rejectedAt;
    }
}

/// <summary>
/// رویداد لغو درخواست تسهیلات
/// </summary>
public sealed class FacilityRequestCancelledEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid FacilityId { get; }
    public Guid ExternalUserId { get; }
    public string ApplicationNumber { get; }
    public Money RequestedAmount { get; }
    public string? CancellationReason { get; }
    public DateTime CancelledAt { get; }

    public FacilityRequestCancelledEvent(
        Guid applicationId,
        Guid facilityId,
        Guid externalUserId,
        string applicationNumber,
        Money requestedAmount,
        string? cancellationReason,
        DateTime cancelledAt)
    {
        ApplicationId = applicationId;
        FacilityId = facilityId;
        ExternalUserId = externalUserId;
        ApplicationNumber = applicationNumber;
        RequestedAmount = requestedAmount;
        CancellationReason = cancellationReason;
        CancelledAt = cancelledAt;
    }
}


/// <summary>
/// رویداد پردازش درخواست توسط بانک
/// </summary>
public sealed class FacilityRequestProcessedByBankEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid FacilityId { get; }
    public Guid ExternalUserId { get; }
    public string ApplicationNumber { get; }
    public Money ApprovedAmount { get; }
    public DateTime ProcessedAt { get; }

    public FacilityRequestProcessedByBankEvent(
        Guid applicationId,
        Guid facilityId,
        Guid externalUserId,
        string applicationNumber,
        Money approvedAmount,
        DateTime processedAt)
    {
        ApplicationId = applicationId;
        FacilityId = facilityId;
        ExternalUserId = externalUserId;
        ApplicationNumber = applicationNumber;
        ApprovedAmount = approvedAmount;
        ProcessedAt = processedAt;
    }
}

/// <summary>
/// رویداد تکمیل درخواست تسهیلات
/// </summary>
public sealed class FacilityRequestCompletedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public Guid FacilityId { get; }
    public Guid ExternalUserId { get; }
    public string ApplicationNumber { get; }
    public Money ApprovedAmount { get; }
    public DateTime CompletedAt { get; }

    public FacilityRequestCompletedEvent(
        Guid applicationId,
        Guid facilityId,
        Guid externalUserId,
        string applicationNumber,
        Money approvedAmount,
        DateTime completedAt)
    {
        ApplicationId = applicationId;
        FacilityId = facilityId;
        ExternalUserId = externalUserId;
        ApplicationNumber = applicationNumber;
        ApprovedAmount = approvedAmount;
        CompletedAt = completedAt;
    }
}

