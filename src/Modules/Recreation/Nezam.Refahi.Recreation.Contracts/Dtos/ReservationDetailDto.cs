using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Detailed tour reservation data transfer object
/// </summary>
public class ReservationDetailDto : IStaticMapper<TourReservation, ReservationDetailDto>
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public ReservationStatus Status { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public decimal? TotalAmountRials { get; set; }
    public string? Notes { get; set; }
    public Guid? CapacityId { get; set; }

    // Capacity information
    public TourCapacityDetailDto? Capacity { get; set; }

    // Tour information
    public TourSummaryDto Tour { get; set; } = null!;

    // Participants
    public List<ParticipantDto> Participants { get; set; } = new();

    // Summary information
    public int ParticipantCount { get; set; }
    public int MainParticipantCount { get; set; }
    public int GuestParticipantCount { get; set; }
    public bool IsExpired { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsPending { get; set; }

    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Maps from TourReservation entity to ReservationDetailDto
    /// Note: This is a basic mapping. Use the query handler for complete mapping with context
    /// </summary>
    /// <param name="entity">The source reservation entity</param>
    /// <returns>Mapped ReservationDetailDto</returns>
    public static ReservationDetailDto MapFrom(TourReservation entity)
    {
        return new ReservationDetailDto
        {
            Id = entity.Id,
            TourId = entity.TourId,
            TrackingCode = entity.TrackingCode,
            Status = entity.Status,
            ReservationDate = entity.ReservationDate,
            ExpiryDate = entity.ExpiryDate,
            ConfirmationDate = entity.ConfirmationDate,
            TotalAmountRials = entity.TotalAmount?.AmountRials,
            Notes = entity.Notes,
            CapacityId = entity.CapacityId,
            Participants = entity.Participants.Select(ParticipantDto.MapFrom).ToList(),
            ParticipantCount = entity.GetParticipantCount(),
            MainParticipantCount = entity.Participants.Count(p => p.ParticipantType == ParticipantType.Member),
            GuestParticipantCount = entity.Participants.Count(p => p.ParticipantType == ParticipantType.Guest),
            IsExpired = entity.IsExpired(),
            IsConfirmed = entity.IsConfirmed(),
            IsPending = entity.IsPending(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.LastModifiedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedBy = entity.LastModifiedBy
        };
    }

    /// <summary>
    /// Maps from ReservationDetailDto to TourReservation entity
    /// Note: This operation is not supported as DTOs should not create entities
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>Not supported</returns>
    public static TourReservation MapTo(ReservationDetailDto dto)
    {
        throw new NotSupportedException("Mapping from ReservationDetailDto to TourReservation is not supported. Use domain services to create entities.");
    }
}