using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// User reservation data transfer object with participant information
/// </summary>
public class UserReservationDto : IStaticMapper<TourReservation, UserReservationDto>
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public string TourTitle { get; set; } = string.Empty;
    public string TourDescription { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public ReservationStatus Status { get; set; }
    public string StatusDisplayName { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public DateTime? CancellationDate { get; set; }
    public decimal? TotalAmountRials { get; set; }
    public decimal? PaidAmountRials { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public Guid? BillId { get; set; }
    public Guid? CapacityId { get; set; }
    public Guid? MemberId { get; set; }

    // Participant information for the current user
    public string UserNationalNumber { get; set; } = string.Empty;
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public ParticipantType UserParticipantType { get; set; }
    public decimal UserRequiredAmountRials { get; set; }
    public decimal? UserPaidAmountRials { get; set; }
    public DateTime? UserPaymentDate { get; set; }
    public bool UserHasPaid { get; set; }
    public bool UserIsFullyPaid { get; set; }
    public decimal UserRemainingAmountRials { get; set; }

    // Tour information
    public DateTime? TourStartDate { get; set; }
    public DateTime? TourEndDate { get; set; }
    public string? TourLocation { get; set; }
    public int? TourCapacity { get; set; }
    public int? TourAvailableCapacity { get; set; }

    // Calculated properties
    public bool IsExpired { get; set; }
    public bool CanCancel { get; set; }
    public bool CanPay { get; set; }
    public bool CanConfirm { get; set; }
    public bool IsPaymentPending { get; set; }
    public bool IsPaymentOverdue { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;

  public static UserReservationDto MapFrom(TourReservation entity)
  {
    throw new NotImplementedException();
  }

  public static TourReservation MapTo(UserReservationDto dto)
  {
    throw new NotImplementedException();
  }
}
