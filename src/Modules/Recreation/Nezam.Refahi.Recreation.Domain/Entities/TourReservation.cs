using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Events;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Entity representing a tour reservation with participants and tracking
/// </summary>
public sealed class TourReservation : FullAggregateRoot<Guid>
{
    public Guid TourId { get; private set; }
    public string TrackingCode { get; private set; } = null!;
    public ReservationStatus Status { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime? ConfirmationDate { get; private set; }
    public DateTime? CancellationDate { get; private set; }
    public Money? TotalAmount { get; private set; }
    public Money? PaidAmount { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }
    public Guid? BillId { get; private set; }
    public Guid? CapacityId { get; private set; }
    public Guid? MemberId { get; private set; }
    public Guid ExternalUserId { get; private set; }
    
    // Multi-tenancy support
    public string? TenantId { get; private set; }
    
    // Concurrency control

    // Navigation properties
    public Tour Tour { get; private set; } = null!;
    public TourCapacity? Capacity { get; private set; }

    private readonly List<Participant> _participants = new();
    public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

    private readonly List<ReservationPriceSnapshot> _priceSnapshots = new();
    public IReadOnlyCollection<ReservationPriceSnapshot> PriceSnapshots => _priceSnapshots.AsReadOnly();

    // Private constructor for EF Core
    private TourReservation() : base() { }

    /// <summary>
    /// Creates a new tour reservation
    /// </summary>
    public TourReservation(
        Guid tourId,
        string trackingCode,
        Guid externalUserId,
        Guid? capacityId = null,
        Guid? memberId = null,
        DateTime? expiryDate = null,
        string? notes = null,
        string? tenantId = null)
        : base(Guid.NewGuid())
    {
        if (tourId == Guid.Empty)
            throw new ArgumentException("شناسه تور الزامی است", nameof(tourId));

        if (externalUserId == Guid.Empty)
            throw new ArgumentException("شناسه کاربر خارجی الزامی است", nameof(externalUserId));

        ValidateTrackingCode(trackingCode);

        TourId = tourId;
        TrackingCode = trackingCode.Trim().ToUpperInvariant();
        ExternalUserId = externalUserId;
        Status = ReservationStatus.Draft; // Start in Draft state
        ReservationDate = DateTime.UtcNow;
        ExpiryDate = expiryDate ?? DateTime.UtcNow.AddHours(2); // Default 15 minutes expiry
        Notes = notes?.Trim();
        CapacityId = capacityId;
        MemberId = memberId;
        TenantId = tenantId;
    }

    /// <summary>
    /// Adds a participant to this reservation
    /// </summary>
    public void AddParticipant(Participant participant)
    {
        if (participant == null)
            throw new ArgumentNullException(nameof(participant));
        if (participant.ReservationId != Id)
            throw new ArgumentException("شرکت‌کننده متعلق به این رزرو نمی‌باشد", nameof(participant));
        
        // Only allow adding participants in Draft or Held state
        if (Status != ReservationStatus.Draft && Status != ReservationStatus.Held)
            throw new InvalidOperationException($"امکان اضافه کردن شرکت‌کننده در وضعیت {Status} وجود ندارد");

        // Check if national number already exists in this reservation
        if (_participants.Any(p => p.NationalNumber == participant.NationalNumber))
            throw new InvalidOperationException("شرکت‌کننده با این شماره ملی قبلاً در رزرو ثبت شده است");

        _participants.Add(participant);
    }

    /// <summary>
    /// Removes a participant from this reservation
    /// </summary>
    public void RemoveParticipant(Guid participantId)
    {
        if (Status != ReservationStatus.Draft && Status != ReservationStatus.Held)
            throw new InvalidOperationException("امکان حذف شرکت‌کننده از رزروهای غیردر انتظار وجود ندارد");

        var participant = _participants.FirstOrDefault(p => p.Id == participantId);
        if (participant != null)
        {
            _participants.Remove(participant);
        }
    }

    /// <summary>
    /// Transitions from Draft to Held state (reserves capacity)
    /// </summary>
    public void Hold()
    {
        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Held, "Hold");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        if (!_participants.Any())
            throw new InvalidOperationException("رزرو بدون شرکت‌کننده قابل نگهداری نیست");

        Status = ReservationStatus.Held;
        ExpiryDate ??= DateTime.UtcNow.AddMinutes(15); // Set expiry if not already set
        
        // Publish domain event
        AddDomainEvent(new TourReservationCreatedEvent
        {
            ReservationId = Id,
            TourId = TourId,
            TourTitle = Tour?.Title ?? string.Empty,
            TrackingCode = TrackingCode,
            ExternalUserId = ExternalUserId,
            ReservationDate = ReservationDate,
            TourStartDate = Tour?.TourStart ?? DateTime.MinValue,
            TourEndDate = Tour?.TourEnd ?? DateTime.MinValue,
            ParticipantCount = _participants.Count,
            TotalPrice = TotalAmount?.AmountRials ?? 0,
            Status = Status.ToString()
        });
    }

    /// <summary>
    /// Sets the reservation status to paying (payment in progress)
    /// </summary>
    public void SetToPaying()
    {
        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Paying, "SetToPaying");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        if (ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value)
            throw new InvalidOperationException("رزرو منقضی شده است و امکان پردازش پرداخت وجود ندارد");
        if (!_participants.Any())
            throw new InvalidOperationException("رزرو بدون شرکت‌کننده است و امکان پردازش پرداخت وجود ندارد");

        Status = ReservationStatus.Paying;
    }

    /// <summary>
    /// Confirms the reservation
    /// </summary>
    public void Confirm(Money? totalAmount = null, bool skipExpiryCheck = false)
    {
        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Confirmed, "Confirm");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        // Skip expiry check for payment-confirmed reservations
        if (!skipExpiryCheck && ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value)
            throw new InvalidOperationException("رزرو منقضی شده است و امکان تأیید وجود ندارد");
        if (!_participants.Any())
            throw new InvalidOperationException("رزرو بدون شرکت‌کننده است و امکان تأیید وجود ندارد");

        Status = ReservationStatus.Confirmed;
        ConfirmationDate = DateTime.UtcNow;
        TotalAmount = totalAmount;
        ExpiryDate = null; // Clear expiry date when confirmed
        
        // Publish domain event
        AddDomainEvent(new TourReservationConfirmedEvent
        {
            ReservationId = Id,
            TourId = TourId,
            TourTitle = Tour?.Title ?? string.Empty,
            TrackingCode = TrackingCode,
            ExternalUserId = ExternalUserId,
            TourStartDate = Tour?.TourStart ?? DateTime.MinValue,
            TourEndDate = Tour?.TourEnd ?? DateTime.MinValue,
            ParticipantCount = _participants.Count,
            TotalPrice = TotalAmount?.AmountRials ?? 0,
            ConfirmedAt = ConfirmationDate.Value
        });
    }

    /// <summary>
    /// Cancels the reservation by user
    /// </summary>
    public void Cancel(string? reason = null)
    {
        if (Status == ReservationStatus.Cancelled)
            return; // Already cancelled

        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Cancelled, "Cancel");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        Status = ReservationStatus.Cancelled;
        CancellationDate = DateTime.UtcNow;
        CancellationReason = reason?.Trim();
        
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Notes = string.IsNullOrWhiteSpace(Notes) ? reason : $"{Notes} | Cancelled: {reason}";
        }
        
        // Publish domain event
        AddDomainEvent(new TourReservationCancelledEvent
        {
            ReservationId = Id,
            TourId = TourId,
            TourTitle = Tour?.Title ?? string.Empty,
            TrackingCode = TrackingCode,
            ExternalUserId = ExternalUserId,
            CancellationReason = reason ?? "UserRequest",
            CancelledAt = CancellationDate.Value,
            RefundAmount = TotalAmount?.AmountRials ?? 0,
            RefundProcessed = false // Will be updated when refund is processed
        });
    }

    /// <summary>
    /// Cancels the reservation by system (e.g., tour cancelled)
    /// </summary>
    public void SystemCancel(string reason)
    {
        if (Status == ReservationStatus.SystemCancelled)
            return; // Already system cancelled

        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.SystemCancelled, "SystemCancel");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        Status = ReservationStatus.SystemCancelled;
        CancellationDate = DateTime.UtcNow;
        CancellationReason = reason?.Trim();
        
        Notes = string.IsNullOrWhiteSpace(Notes) ? $"System cancelled: {reason}" : $"{Notes} | System cancelled: {reason}";
    }

    /// <summary>
    /// Marks the reservation as expired
    /// </summary>
    public void MarkAsExpired()
    {
        if (ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value)
        {
            var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Expired, "MarkAsExpired");
            if (isValid)
            {
                Status = ReservationStatus.Expired;
            }
        }
    }

    /// <summary>
    /// Reactivates an expired reservation with new expiry date
    /// </summary>
    public void Reactivate(DateTime newExpiryDate, string? reason = null)
    {
        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Held, "Reactivate");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        if (Status != ReservationStatus.Expired)
            throw new InvalidOperationException("فقط رزروهای منقضی شده قابل فعال‌سازی مجدد می‌باشند");

        if (newExpiryDate <= DateTime.UtcNow)
            throw new ArgumentException("تاریخ انقضای جدید باید در آینده تعیین شود", nameof(newExpiryDate));

        Status = ReservationStatus.Held;
        ExpiryDate = newExpiryDate;
        
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Notes = string.IsNullOrWhiteSpace(Notes) ? $"Reactivated: {reason}" : $"{Notes} | Reactivated: {reason}";
        }
    }

    /// <summary>
    /// Marks payment as failed
    /// </summary>
    public void MarkPaymentFailed(string? reason = null)
    {
        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.PaymentFailed, "MarkPaymentFailed");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        Status = ReservationStatus.PaymentFailed;
        CancellationReason = reason?.Trim();
        
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Notes = string.IsNullOrWhiteSpace(Notes) ? $"Payment failed: {reason}" : $"{Notes} | Payment failed: {reason}";
        }
    }

    /// <summary>
    /// Initiates refund process
    /// </summary>
    public void InitiateRefund(string? reason = null)
    {
        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Refunding, "InitiateRefund");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        Status = ReservationStatus.Refunding;
        
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Notes = string.IsNullOrWhiteSpace(Notes) ? $"Refund initiated: {reason}" : $"{Notes} | Refund initiated: {reason}";
        }
    }

    /// <summary>
    /// Marks refund as completed
    /// </summary>
    public void CompleteRefund(Money? refundAmount = null)
    {
        var (isValid, errorMessage) = ReservationStateMachine.ValidateTransition(Status, ReservationStatus.Refunded, "CompleteRefund");
        if (!isValid)
            throw new InvalidOperationException(errorMessage);

        Status = ReservationStatus.Refunded;
        if (refundAmount != null)
        {
            PaidAmount = Money.Zero; // Clear paid amount after refund
        }
    }

    /// <summary>
    /// Extends the expiry date
    /// </summary>
    public void ExtendExpiry(DateTime newExpiryDate)
    {
        if (Status != ReservationStatus.Draft && Status != ReservationStatus.Held)
            throw new InvalidOperationException("امکان تمدید تاریخ انقضا فقط برای رزروهای در انتظار وجود دارد");
        if (newExpiryDate <= DateTime.UtcNow)
            throw new ArgumentException("تاریخ انقضای جدید باید در آینده تعیین شود", nameof(newExpiryDate));

        ExpiryDate = newExpiryDate;
    }

    /// <summary>
    /// Updates reservation notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
    }

    /// <summary>
    /// Sets the bill ID for this reservation
    /// </summary>
    public void SetBillId(Guid billId)
    {
        if (billId == Guid.Empty)
            throw new ArgumentException("شناسه فاکتور الزامی است", nameof(billId));
        if (BillId.HasValue)
            throw new InvalidOperationException("شناسه فاکتور قبلاً برای این رزرو تعریف شده است");

        BillId = billId;
    }

    /// <summary>
    /// Gets the main participant of the reservation
    /// </summary>
    public Participant? GetMainParticipant()
    {
        return _participants.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member);
    }

    /// <summary>
    /// Gets all guest participants
    /// </summary>
    public IEnumerable<Participant> GetGuestParticipants()
    {
        return _participants.Where(p => p.ParticipantType == ParticipantType.Guest);
    }

    /// <summary>
    /// Gets total participant count
    /// </summary>
    public int GetParticipantCount()
    {
        return _participants.Count;
    }

    /// <summary>
    /// Checks if reservation is expired or should be expired
    /// </summary>
    public bool IsExpired()
    {
        return Status == ReservationStatus.Expired || 
               (ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value && ReservationStateMachine.CanBeCancelled(Status));
    }

    /// <summary>
    /// بررسی اینکه آیا رزرو تأیید شده و منقضی نشده است
    /// </summary>
    public bool IsConfirmed()
    {
        return Status == ReservationStatus.Confirmed && !IsExpired();
    }

    /// <summary>
    /// بررسی اینکه آیا رزرو در انتظار است (Held یا Paying) و منقضی نشده
    /// </summary>
    public bool IsPending()
    {
        return (Status == ReservationStatus.Held || Status == ReservationStatus.Paying) && !IsExpired();
    }

    /// <summary>
    /// Checks if reservation is in draft state
    /// </summary>
    public bool IsDraft()
    {
        return Status == ReservationStatus.Draft;
    }

    /// <summary>
    /// Checks if reservation is being processed for payment
    /// </summary>
    public bool IsPaying()
    {
        return Status == ReservationStatus.Paying;
    }

    /// <summary>
    /// Checks if reservation is cancelled (by user or system)
    /// </summary>
    public bool IsCancelled()
    {
        return Status == ReservationStatus.Cancelled || Status == ReservationStatus.SystemCancelled;
    }

    /// <summary>
    /// Checks if reservation is in active state (counts towards capacity) and not expired
    /// </summary>
    public bool IsActive()
    {
        return ReservationStateMachine.IsActiveState(Status) && !IsExpired();
    }

    /// <summary>
    /// Checks if reservation is in terminal state (no further transitions)
    /// </summary>
    public bool IsTerminal()
    {
        return ReservationStateMachine.IsTerminalState(Status);
    }

    /// <summary>
    /// Gets available next states for this reservation
    /// </summary>
    public IEnumerable<ReservationStatus> GetAvailableTransitions()
    {
        return ReservationStateMachine.GetValidNextStates(Status);
    }

    /// <summary>
    /// Checks if this reservation conflicts with creating a new reservation
    /// Business rules: Only Paying and Confirmed reservations prevent new reservations
    /// </summary>
    public bool HasConflictingReservation()
    {
        // Paying and Confirmed reservations always prevent new reservations
        if (Status == ReservationStatus.Paying || Status == ReservationStatus.Confirmed)
        {
            return true;
        }

        // All other states (Draft, Held, Expired, Cancelled, SystemCancelled, PaymentFailed, 
        // Refunding, Refunded, Waitlisted, CancelRequested, AmendRequested, NoShow, Rejected)
        // do not prevent new reservations - they can be renewed instead
        return false;
    }

    /// <summary>
    /// Checks if this reservation can be reused to add new participants
    /// Business rules: Draft, Held, and Expired reservations can be reused
    /// </summary>
    public bool CanBeReused()
    {
        // Draft reservations can always be reused
        if (Status == ReservationStatus.Draft)
        {
            return true;
        }

        // Expired reservations can be reused (will be renewed to Held)
        if (Status == ReservationStatus.Expired)
        {
            return true;
        }

        // Held reservations can be reused if not expired
        if (Status == ReservationStatus.Held)
        {
            return IsExpired();
        }

        // Confirmed reservations can be reused if not expired
        if (Status == ReservationStatus.Confirmed)
        {
            return false;
        }

        // All other states cannot be reused
        return false;
    }

    /// <summary>
    /// Checks if this reservation can accommodate additional participants
    /// </summary>
    public bool CanAddParticipants(int additionalCount, Tour tour)
    {
        if (additionalCount <= 0)
            return true;

        if (tour == null)
            throw new ArgumentNullException(nameof(tour));

        // Check if reservation can be reused
        if (!CanBeReused())
            return false;

        // Check tour capacity limits
        var currentParticipantCount = _participants.Count;
        var totalAfterAddition = currentParticipantCount + additionalCount;
        
        // Check overall tour capacity
        if (totalAfterAddition > tour.MaxParticipants)
            return false;

        // Check guest limits per reservation
        var currentGuestCount = _participants.Count(p => p.ParticipantType == ParticipantType.Guest);
        var newGuestCount = additionalCount; // Assuming all new participants are guests for now
        
        if (tour.MaxGuestsPerReservation.HasValue && 
            currentGuestCount + newGuestCount > tour.MaxGuestsPerReservation.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Gets the number of additional participants this reservation can accommodate
    /// </summary>
    public int GetAvailableParticipantSlots(Tour tour)
    {
        if (tour == null)
            throw new ArgumentNullException(nameof(tour));

        if (!CanBeReused())
            return 0;

        // Calculate based on tour capacity
        var currentCount = _participants.Count;
        var maxFromTour = tour.MaxParticipants - currentCount;

        // Calculate based on guest limits
        var currentGuestCount = _participants.Count(p => p.ParticipantType == ParticipantType.Guest);
        var maxGuests = tour.MaxGuestsPerReservation ?? int.MaxValue;
        var maxFromGuestLimit = maxGuests - currentGuestCount;

        // Return the minimum of both limits
        return Math.Max(0, Math.Min(maxFromTour, maxFromGuestLimit));
    }

    /// <summary>
    /// Finds the best reusable reservation from a collection of reservations
    /// Priority: Draft > Expired > Held > Confirmed (most recent first within each status)
    /// Always validates capacity before selecting
    /// </summary>
    public static TourReservation? FindBestReusableReservation(IEnumerable<TourReservation> reservations, Tour tour, int requiredSlots)
    {
        if (reservations == null)
            throw new ArgumentNullException(nameof(reservations));
        if (tour == null)
            throw new ArgumentNullException(nameof(tour));

        var reusableReservations = reservations
            .Where(r => 
            {
                // Check if reservation can be reused
                if (!r.CanBeReused())
                    return false;

                // For expired reservations, check if they can be renewed with capacity
                if (r.Status == ReservationStatus.Expired)
                {
                    return r.CanBeRenewed(tour, requiredSlots);
                }

                // For other states, check if they can add participants
                return r.CanAddParticipants(requiredSlots, tour);
            })
            .OrderBy(r => r.Status switch
            {
                ReservationStatus.Draft => 1,      // Highest priority - can always be renewed
                ReservationStatus.Expired => 2,    // Second priority - can be renewed if capacity allows
                ReservationStatus.Held => 3,       // Third priority - can be renewed if not expired
                ReservationStatus.Confirmed => 4,  // Lowest priority - can be renewed if not expired
                _ => 999
            })
            .ThenByDescending(r => r.ReservationDate) // Most recent first within same status
            .ToList();

        return reusableReservations.FirstOrDefault();
    }

    /// <summary>
    /// Updates the expiry date of the reservation
    /// </summary>
    public void UpdateExpiryDate(DateTime newExpiryDate)
    {
        if (newExpiryDate <= DateTime.UtcNow)
            throw new ArgumentException("Expiry date must be in the future", nameof(newExpiryDate));

        ExpiryDate = newExpiryDate;
    }

    /// <summary>
    /// Checks if this reservation can be renewed considering tour capacity
    /// </summary>
    public bool CanBeRenewed(Tour tour, int additionalParticipants = 0)
    {
        if (tour == null)
            throw new ArgumentNullException(nameof(tour));

        // Only expired reservations can be renewed
        if (Status != ReservationStatus.Expired)
            return false;

        // Check if tour has capacity for current participants + additional participants
        var totalParticipants = _participants.Count + additionalParticipants;
        
        // Check overall tour capacity
        if (totalParticipants > tour.MaxParticipants)
            return false;

        // Check guest limits per reservation
        var currentGuestCount = _participants.Count(p => p.ParticipantType == ParticipantType.Guest);
        var additionalGuests = additionalParticipants; // Assuming all additional are guests for now
        
        if (tour.MaxGuestsPerReservation.HasValue && 
            currentGuestCount + additionalGuests > tour.MaxGuestsPerReservation.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Renews an expired reservation by changing its status to Held
    /// </summary>
    public void Renew()
    {
        if (Status != ReservationStatus.Expired)
            throw new InvalidOperationException($"Cannot renew reservation in {Status} state. Only expired reservations can be renewed.");

        // Change status to Held and update expiry date
        Status = ReservationStatus.Held;
        ExpiryDate = DateTime.UtcNow.AddMinutes(15); // Default 15 minutes hold time
    }

    /// <summary>
    /// Adds a price snapshot for this reservation
    /// </summary>
    public void AddPriceSnapshot(ReservationPriceSnapshot priceSnapshot)
    {
        if (priceSnapshot == null)
            throw new ArgumentNullException(nameof(priceSnapshot));
        if (priceSnapshot.ReservationId != Id)
            throw new ArgumentException("تصویر قیمت متعلق به این رزرو نمی‌باشد", nameof(priceSnapshot));

        // Remove existing snapshot for same participant type
        var existingSnapshot = _priceSnapshots.FirstOrDefault(ps => ps.ParticipantType == priceSnapshot.ParticipantType);
        if (existingSnapshot != null)
        {
            _priceSnapshots.Remove(existingSnapshot);
        }

        _priceSnapshots.Add(priceSnapshot);
    }

    /// <summary>
    /// Gets price snapshot for a participant type
    /// </summary>
    public ReservationPriceSnapshot? GetPriceSnapshot(ParticipantType participantType)
    {
        return _priceSnapshots.FirstOrDefault(ps => ps.ParticipantType == participantType);
    }

    /// <summary>
    /// Calculates total amount from price snapshots
    /// </summary>
    public Money CalculateTotalFromSnapshots()
    {
        var mainSnapshot = GetPriceSnapshot(ParticipantType.Member);
        var guestSnapshot = GetPriceSnapshot(ParticipantType.Guest);
        
        var mainPrice = mainSnapshot?.FinalPrice ?? Money.Zero;
        var guestCount = GetGuestParticipants().Count();
        var guestPrice = guestSnapshot != null ? guestSnapshot.FinalPrice.Multiply(guestCount) : Money.Zero;
        
        return mainPrice.Add(guestPrice);
    }

    /// <summary>
    /// Records payment for this reservation
    /// </summary>
    public void RecordPayment(Money amount)
    {
        PaidAmount = amount ?? throw new ArgumentNullException(nameof(amount));
    }

    /// <summary>
    /// Checks if reservation is fully paid
    /// </summary>
    public bool IsFullyPaid => PaidAmount != null && TotalAmount != null && PaidAmount.AmountRials >= TotalAmount.AmountRials;

    /// <summary>
    /// Gets remaining amount to be paid
    /// </summary>
    public Money RemainingAmount => TotalAmount?.Subtract(PaidAmount ?? Money.Zero) ?? Money.Zero;

    /// <summary>
    /// Generates a unique tracking code
    /// </summary>
    public static string GenerateTrackingCode()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomPart = Random.Shared.Next(1000, 9999).ToString();
        return $"TR{timestamp}{randomPart}";
    }

    // Private validation methods
    private static void ValidateTrackingCode(string trackingCode)
    {
        if (string.IsNullOrWhiteSpace(trackingCode))
            throw new ArgumentException("کد پیگیری الزامی است", nameof(trackingCode));
        if (trackingCode.Length > 50)
            throw new ArgumentException("کد پیگیری نمی‌تواند بیش از 50 کاراکتر باشد", nameof(trackingCode));
    }
}