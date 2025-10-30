using MediatR;

namespace Nezam.Refahi.Recreation.Contracts.IntegrationEvents;

/// <summary>
/// Published when a reservation's bill is created and the reservation can proceed to completion (awaiting payment).
/// </summary>
public class ReservationReadyToCompleteIntegrationEvent : INotification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    public string EventVersion { get; set; } = "1.0";

    // Reservation identity
    public Guid ReservationId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    // Bill identity
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;

    // Amounts
    public decimal TotalAmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    // User
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Optional metadata
    public Dictionary<string, string> Metadata { get; set; } = new();
}


