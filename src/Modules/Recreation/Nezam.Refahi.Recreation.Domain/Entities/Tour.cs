using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Domain.Entities;

/// <summary>
/// Tour aggregate root representing a recreational tour with registration management
/// </summary>
public sealed class Tour : FullAggregateRoot<Guid>
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public DateTime TourStart { get; private set; }
    public DateTime TourEnd { get; private set; }

    /// <summary>
    /// Gets the maximum participants from all active capacities
    /// </summary>
    public int MaxParticipants => _capacities.Where(c => c.IsActive).Sum(c => c.MaxParticipants);

    public TourStatus Status { get; private set; }
    public TourDifficulty Difficulty { get; private set; } = TourDifficulty.Easy;
    public bool IsActive { get; private set; }

    // Age restrictions
    public int? MinAge { get; private set; }
    public int? MaxAge { get; private set; }

    // Guest limitations per reservation
    public int? MaxGuestsPerReservation { get; private set; }

    // Navigation properties
    private readonly List<TourPhoto> _photos = new();
    public IReadOnlyCollection<TourPhoto> Photos => _photos.AsReadOnly();

    // Participants are now managed through reservations only

    private readonly List<TourPricing> _pricing = new();
    public IReadOnlyCollection<TourPricing> Pricing => _pricing.AsReadOnly();



    // Navigation property for restricted tours (many-to-many through TourRestrictedTour)
    private readonly List<TourRestrictedTour> _tourRestrictedTours = new();
    public IReadOnlyCollection<TourRestrictedTour> TourRestrictedTours => _tourRestrictedTours.AsReadOnly();

    private readonly List<TourMemberCapability> _memberCapabilities = new();
    public IReadOnlyCollection<TourMemberCapability> MemberCapabilities => _memberCapabilities.AsReadOnly();

    private readonly List<TourMemberFeature> _memberFeatures = new();
    public IReadOnlyCollection<TourMemberFeature> MemberFeatures => _memberFeatures.AsReadOnly();

    private readonly List<TourFeature> _tourFeatures = new();
    public IReadOnlyCollection<TourFeature> TourFeatures => _tourFeatures.AsReadOnly();

    private readonly List<TourReservation> _reservations = new();
    public IReadOnlyCollection<TourReservation> Reservations => _reservations.AsReadOnly();

    private readonly List<TourCapacity> _capacities = new();
    public IReadOnlyCollection<TourCapacity> Capacities => _capacities.AsReadOnly();

    private readonly List<TourAgency> _tourAgencies = new();
    public IReadOnlyCollection<TourAgency> TourAgencies => _tourAgencies.AsReadOnly();

    // Private constructor for EF Core
    private Tour() : base() { }

    /// <summary>
    /// Creates a new tour
    /// </summary>
    public Tour(
        string title,
        string description,
        DateTime tourStart,
        DateTime tourEnd,
        int? minAge = null,
        int? maxAge = null,
        TourDifficulty difficulty = TourDifficulty.Easy,
        int? maxGuestsPerReservation = null,
        IEnumerable<TourPhoto>? photos = null)
        : base(Guid.NewGuid())
    {
        ValidateTitle(title);
        ValidateTourDates(tourStart, tourEnd);
        ValidateAgeRestrictions(minAge, maxAge);
        ValidateMaxGuestsPerReservation(maxGuestsPerReservation);

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        TourStart = tourStart;
        TourEnd = tourEnd;
        Status = TourStatus.Draft;
        IsActive = true;
        MinAge = minAge;
        MaxAge = maxAge;
        Difficulty = difficulty;
        MaxGuestsPerReservation = maxGuestsPerReservation;

        if (photos != null)
            _photos.AddRange(photos);
    }

    /// <summary>
    /// Updates tour basic details
    /// </summary>
    public void UpdateDetails(string title, string description)
    {
        ValidateTitle(title);
        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
    }


    /// <summary>
    /// Updates tour dates
    /// </summary>
    public void UpdateTourDates(DateTime tourStart, DateTime tourEnd)
    {
        ValidateTourDates(tourStart, tourEnd);

        // Validate that no capacity has registration end after new tour start
        foreach (var capacity in _capacities.Where(c => c.IsActive))
        {
            capacity.ValidateAgainstTourStartDate(tourStart);
        }

        TourStart = tourStart;
        TourEnd = tourEnd;
    }

    /// <summary>
    /// Sets age restrictions for the tour
    /// </summary>
    public void SetAgeRestrictions(int? minAge, int? maxAge)
    {
        ValidateAgeRestrictions(minAge, maxAge);
        MinAge = minAge;
        MaxAge = maxAge;
    }

    /// <summary>
    /// Sets the maximum number of guests allowed per reservation
    /// </summary>
    public void SetMaxGuestsPerReservation(int? maxGuests)
    {
        ValidateMaxGuestsPerReservation(maxGuests);
        MaxGuestsPerReservation = maxGuests;
    }

    // Capacity management methods

    /// <summary>
    /// Adds a new capacity to the tour
    /// </summary>
    public void AddCapacity(int maxParticipants, DateTime registrationStart, DateTime registrationEnd, string? description = null)
    {
        var capacity = new TourCapacity(Id, maxParticipants, registrationStart, registrationEnd, description);

        // Validate against tour start date
        capacity.ValidateAgainstTourStartDate(TourStart);

        // Check for overlaps with existing active capacities
        var overlappingCapacity = _capacities.FirstOrDefault(c => c.IsActive && c.OverlapsWith(capacity));
        if (overlappingCapacity != null)
            throw new InvalidOperationException("New capacity overlaps with existing capacity registration period");

        _capacities.Add(capacity);
    }

    /// <summary>
    /// Updates an existing capacity
    /// </summary>
    public void UpdateCapacity(Guid capacityId, int maxParticipants, DateTime registrationStart, DateTime registrationEnd, string? description = null)
    {
        var capacity = _capacities.FirstOrDefault(c => c.Id == capacityId);
        if (capacity == null)
            throw new ArgumentException("Capacity not found", nameof(capacityId));

        capacity.UpdateCapacity(maxParticipants, description);
        capacity.UpdateRegistrationPeriod(registrationStart, registrationEnd);
        capacity.ValidateAgainstTourStartDate(TourStart);

        // Check for overlaps with other active capacities
        var overlappingCapacity = _capacities.FirstOrDefault(c => c.IsActive && c.Id != capacityId && c.OverlapsWith(capacity));
        if (overlappingCapacity != null)
            throw new InvalidOperationException("Updated capacity overlaps with existing capacity registration period");
    }

    /// <summary>
    /// Removes a capacity from the tour
    /// </summary>
    public void RemoveCapacity(Guid capacityId)
    {
        var capacity = _capacities.FirstOrDefault(c => c.Id == capacityId);
        if (capacity != null)
        {
            capacity.Deactivate();
        }
    }

    /// <summary>
    /// Gets capacity that is effective for a specific date
    /// </summary>
    public TourCapacity? GetEffectiveCapacity(DateTime date)
    {
        return _capacities.FirstOrDefault(c => c.IsEffectiveFor(date));
    }

    /// <summary>
    /// Gets all active capacities
    /// </summary>
    public IEnumerable<TourCapacity> GetActiveCapacities()
    {
        return _capacities.Where(c => c.IsActive);
    }

    /// <summary>
    /// Gets available spots for a specific date based on capacity
    /// </summary>
    public int GetAvailableSpotsForDate(DateTime date)
    {
        var capacity = GetEffectiveCapacity(date);
        if (capacity == null)
            return 0;

        var reservedCount = GetConfirmedReservationCount() + GetPendingReservationCount();
        return Math.Max(0, capacity.MaxParticipants - reservedCount);
    }

    /// <summary>
    /// Adds a tour to the restricted list
    /// </summary>
    public void AddRestrictedTour(Guid restrictedTourId)
    {
        if (restrictedTourId == Guid.Empty)
            throw new ArgumentException("Restricted Tour ID cannot be empty", nameof(restrictedTourId));
        if (restrictedTourId == Id)
            throw new InvalidOperationException("Tour cannot be restricted to itself");

        // Check if restriction already exists
        if (_tourRestrictedTours.Any(trt => trt.RestrictedTourId == restrictedTourId))
            return;

        var tourRestrictedTour = new TourRestrictedTour(Id, restrictedTourId);
        _tourRestrictedTours.Add(tourRestrictedTour);
    }

    /// <summary>
    /// Removes a tour from the restricted list
    /// </summary>
    public void RemoveRestrictedTour(Guid restrictedTourId)
    {
        var restrictionToRemove = _tourRestrictedTours.FirstOrDefault(trt => trt.RestrictedTourId == restrictedTourId);
        if (restrictionToRemove != null)
        {
            _tourRestrictedTours.Remove(restrictionToRemove);
        }
    }

    /// <summary>
    /// Gets all restricted tour IDs
    /// </summary>
    public IEnumerable<Guid> GetRestrictedTourIds()
    {
        return _tourRestrictedTours.Select(trt => trt.RestrictedTourId);
    }


 

    /// <summary>
    /// Adds a photo to the tour
    /// </summary>
    public void AddPhoto(TourPhoto photo)
    {
        if (photo == null)
            throw new ArgumentNullException(nameof(photo));
    

        _photos.Add(photo);
    }

    /// <summary>
    /// Removes a photo from the tour
    /// </summary>
    public void RemovePhoto(Guid photoId)
    {
        var photo = _photos.FirstOrDefault(p => p.Id == photoId);
        if (photo != null)
            _photos.Remove(photo);
    }

 


    // Reservation management methods

    /// <summary>
    /// Creates a new reservation for this tour
    /// </summary>
    public TourReservation CreateReservation(Guid externalUserId, Guid capacityId, Guid memberId, string? trackingCode = null, DateTime? expiryDate = null, string? notes = null)
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot create reservations for inactive tours");
        if (!IsRegistrationOpen(DateTime.UtcNow))
            throw new InvalidOperationException("Registration is not open for this tour");

        var code = trackingCode ?? TourReservation.GenerateTrackingCode();
        var reservation = new TourReservation(Id, code, externalUserId, capacityId, memberId, expiryDate, notes);

        _reservations.Add(reservation);
        return reservation;
    }

    /// <summary>
    /// Gets a reservation by tracking code
    /// </summary>
    public TourReservation? GetReservationByTrackingCode(string trackingCode)
    {
        if (string.IsNullOrWhiteSpace(trackingCode))
            return null;

        return _reservations.FirstOrDefault(r => r.TrackingCode.Equals(trackingCode.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all confirmed reservations (excluding expired ones)
    /// </summary>
    public IEnumerable<TourReservation> GetConfirmedReservations()
    {
        return _reservations.Where(r => r.IsConfirmed());
    }

    /// <summary>
    /// Gets all active reservations (confirmed + pending, excluding expired ones)
    /// </summary>
    public IEnumerable<TourReservation> GetActiveReservations()
    {
        return _reservations.Where(r => r.IsActive());
    }

    /// <summary>
    /// Gets all pending reservations
    /// </summary>
    public IEnumerable<TourReservation> GetPendingReservations()
    {
        return _reservations.Where(r => r.IsPending());
    }

    /// <summary>
    /// تعداد کل شرکت‌کنندگان در رزروهای تأیید شده
    /// </summary>
    public int GetConfirmedReservationCount()
    {
        return _reservations
            .Where(r => r.IsConfirmed())
            .Sum(r => r.GetParticipantCount());
    }

    /// <summary>
    /// تعداد کل شرکت‌کنندگان در رزروهای OnHold
    /// </summary>
    public int GetHeldReservationCount()
    {
        return _reservations
            .Where(r => r.Status == ReservationStatus.OnHold)
            .Sum(r => r.GetParticipantCount());
    }

    /// <summary>
    /// تعداد کل شرکت‌کنندگان در رزروهای در انتظار (OnHold)
    /// Note: OnHold means waiting for payment and confirmation
    /// </summary>
    public int GetPendingReservationCount()
    {
        return _reservations
            .Where(r => r.Status == ReservationStatus.OnHold)
            .Where(r => !r.IsExpired()) // حذف رزروهای منقضی شده
            .Sum(r => r.GetParticipantCount());
    }

    /// <summary>
    /// بررسی اینکه آیا ظرفیت خالی وجود دارد (با در نظر گیری رزروهای تأیید شده + در انتظار)
    /// </summary>
    public bool HasAvailableSpotsForReservation()
    {
        var totalReserved = GetConfirmedReservationCount() + GetPendingReservationCount();
        return totalReserved < MaxParticipants;
    }

    /// <summary>
    /// تعداد ظرفیت‌های خالی موجود (با در نظر گیری رزروهای تأیید شده + در انتظار)
    /// </summary>
    public int GetAvailableSpots()
    {
        var totalReserved = GetConfirmedReservationCount() + GetPendingReservationCount();
        return Math.Max(0, MaxParticipants - totalReserved);
    }

    /// <summary>
    /// Marks expired reservations as expired
    /// </summary>
    public void MarkExpiredReservations()
    {
        var expiredReservations = _reservations.Where(r => r.IsExpired()).ToList();
        foreach (var reservation in expiredReservations)
        {
            reservation.MarkAsExpired();
        }
    }

    // Pricing management methods

    /// <summary>
    /// Adds a pricing rule to the tour
    /// </summary>
    public void AddPricing(TourPricing pricing)
    {
        if (pricing == null)
            throw new ArgumentNullException(nameof(pricing));
        if (pricing.TourId != Id)
            throw new ArgumentException("Pricing does not belong to this tour", nameof(pricing));

        // If setting as default, ensure no other default exists for this participant type
        if (pricing.IsDefault)
        {
            var existingDefault = _pricing.FirstOrDefault(p =>
                p.ParticipantType == pricing.ParticipantType &&
                p.IsDefault &&
                p.IsActive &&
                p.Id != pricing.Id);

            if (existingDefault != null)
            {
                throw new InvalidOperationException($"A default pricing already exists for {pricing.ParticipantType} participants.");
            }
        }

        _pricing.Add(pricing);
    }

    /// <summary>
    /// Removes a pricing rule from the tour
    /// </summary>
    public void RemovePricing(Guid pricingId)
    {
        var pricing = _pricing.FirstOrDefault(p => p.Id == pricingId);
        if (pricing != null)
        {
            pricing.Deactivate();
        }
    }

    /// <summary>
    /// Sets a pricing as default for its participant type.
    /// Ensures only one default exists per participant type.
    /// </summary>
    public void SetPricingAsDefault(Guid pricingId)
    {
        var pricing = _pricing.FirstOrDefault(p => p.Id == pricingId);
        if (pricing == null)
            throw new ArgumentException("Pricing not found", nameof(pricingId));

        if (!pricing.IsActive)
            throw new InvalidOperationException("Cannot set inactive pricing as default");

        // Unset other defaults for the same participant type
        var otherDefaults = _pricing
            .Where(p => p.ParticipantType == pricing.ParticipantType &&
                       p.Id != pricingId &&
                       p.IsDefault &&
                       p.IsActive)
            .ToList();

        foreach (var otherDefault in otherDefaults)
        {
            otherDefault.SetAsDefault(false);
        }

        pricing.SetAsDefault(true);
    }

    /// <summary>
    /// Gets the default pricing for a specific participant type
    /// </summary>
    public TourPricing? GetDefaultPricing(ParticipantType participantType)
    {
        return _pricing
            .FirstOrDefault(p => p.ParticipantType == participantType &&
                                p.IsDefault &&
                                p.IsActive);
    }

    /// <summary>
    /// Gets pricing for a specific participant type (basic overload, uses default or first available)
    /// </summary>
    public TourPricing? GetPricing(ParticipantType participantType, DateTime? date = null)
    {
        return GetPricing(participantType, date, null, null);
    }

    /// <summary>
    /// Gets pricing for a specific participant type considering member capabilities and features.
    /// Priority: 1) Matching requirements, 2) Default, 3) First available
    /// </summary>
    public TourPricing? GetPricing(
        ParticipantType participantType, 
        DateTime? date = null,
        IEnumerable<string>? memberCapabilities = null,
        IEnumerable<string>? memberFeatures = null)
    {
        var checkDate = date ?? DateTime.UtcNow;
        var candidatePricings = _pricing
            .Where(p => p.ParticipantType == participantType && p.IsActive && p.IsValidFor(checkDate, 1))
            .ToList();

        if (!candidatePricings.Any())
            return null;

        // Priority 1: Pricing with matching capabilities/features (most specific)
        var matchingWithRequirements = candidatePricings
            .Where(p => p.HasRequirements() && p.MatchesCapabilitiesAndFeatures(memberCapabilities, memberFeatures))
            .OrderByDescending(p => p.Capabilities.Count + p.Features.Count) // More requirements = more specific
            .ThenByDescending(p => p.IsDefault) // Prefer default among equally specific
            .FirstOrDefault();

        if (matchingWithRequirements != null)
            return matchingWithRequirements;

        // Priority 2: Default pricing (no requirements)
        var defaultPricing = candidatePricings
            .FirstOrDefault(p => p.IsDefault && !p.HasRequirements());

        if (defaultPricing != null)
            return defaultPricing;

        // Priority 3: Any pricing without requirements (fallback)
        var fallbackPricing = candidatePricings
            .FirstOrDefault(p => !p.HasRequirements());

        return fallbackPricing;
    }

    /// <summary>
    /// Gets all active pricing rules
    /// </summary>
    public IEnumerable<TourPricing> GetActivePricing()
    {
        return _pricing.Where(p => p.IsActive);
    }

    // Member Capabilities and Features management methods

    /// <summary>
    /// Adds a member capability to the tour
    /// </summary>
    public void AddMemberCapability(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            throw new ArgumentException("Capability ID cannot be empty", nameof(capabilityId));

        // Check if capability already exists
        if (_memberCapabilities.Any(mc => mc.CapabilityId == capabilityId.Trim()))
            return;

        var memberCapability = new TourMemberCapability(Id, capabilityId);
        _memberCapabilities.Add(memberCapability);
    }

    /// <summary>
    /// Removes a member capability from the tour
    /// </summary>
    public void RemoveMemberCapability(string capabilityId)
    {
        if (string.IsNullOrWhiteSpace(capabilityId))
            return;

        var capabilityToRemove = _memberCapabilities.FirstOrDefault(mc => mc.CapabilityId == capabilityId.Trim());
        if (capabilityToRemove != null)
        {
            _memberCapabilities.Remove(capabilityToRemove);
        }
    }

    /// <summary>
    /// Sets multiple member capabilities for the tour
    /// </summary>
    public void SetMemberCapabilities(IEnumerable<string> capabilities)
    {
        if (capabilities == null)
            throw new ArgumentNullException(nameof(capabilities));

        _memberCapabilities.Clear();
        foreach (var capabilityId in capabilities)
        {
            if (!string.IsNullOrWhiteSpace(capabilityId))
            {
                var memberCapability = new TourMemberCapability(Id, capabilityId);
                _memberCapabilities.Add(memberCapability);
            }
        }
    }

    /// <summary>
    /// Adds a required member feature to the tour
    /// </summary>
    public void AddMemberFeature(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            throw new ArgumentException("Feature ID cannot be empty", nameof(featureId));

        // Check if feature already exists
        if (_memberFeatures.Any(mf => mf.FeatureId == featureId.Trim()))
            return;

        var memberFeature = new TourMemberFeature(Id, featureId);
        _memberFeatures.Add(memberFeature);
    }

    /// <summary>
    /// Removes a member feature from the tour
    /// </summary>
    public void RemoveMemberFeature(string featureId)
    {
        if (string.IsNullOrWhiteSpace(featureId))
            return;

        var featureToRemove = _memberFeatures.FirstOrDefault(mf => mf.FeatureId == featureId.Trim());
        if (featureToRemove != null)
        {
            _memberFeatures.Remove(featureToRemove);
        }
    }

    // Tour Features management methods

    /// <summary>
    /// Adds a tour feature to the tour
    /// </summary>
    public void AddTourFeature(TourFeature tourFeature)
    {
        if (tourFeature == null)
            throw new ArgumentNullException(nameof(tourFeature));
        if (tourFeature.TourId != Id)
            throw new ArgumentException("Tour feature does not belong to this tour", nameof(tourFeature));

        // Check if feature already exists
        if (_tourFeatures.Any(tf => tf.FeatureId == tourFeature.FeatureId))
            return;

        _tourFeatures.Add(tourFeature);
    }

    /// <summary>
    /// Removes a tour feature from the tour
    /// </summary>
    public void RemoveTourFeature(Guid featureId)
    {
        var featureToRemove = _tourFeatures.FirstOrDefault(tf => tf.FeatureId == featureId);
        if (featureToRemove != null)
        {
            _tourFeatures.Remove(featureToRemove);
        }
    }

    /// <summary>
    /// Gets all tour features
    /// </summary>
    public IEnumerable<TourFeature> GetTourFeatures()
    {
        return _tourFeatures.ToList();
    }

    // Tour Agency management methods

    /// <summary>
    /// Adds an agency to the tour
    /// </summary>
    public void AddAgency(Guid AgencyId, string agencyCode, string agencyName, 
        DateTime? validFrom = null, DateTime? validTo = null, string? assignedBy = null, 
        string? notes = null, string? accessLevel = null, int? maxReservations = null, 
        int? maxParticipants = null)
    {
        if (AgencyId == Guid.Empty)
            throw new ArgumentException("Representative Office ID cannot be empty", nameof(AgencyId));

        // Check if agency already exists
        if (_tourAgencies.Any(ta => ta.AgencyId == AgencyId))
            return;

        var tourAgency = new TourAgency(
            Id, 
            AgencyId, 
            agencyCode, 
            agencyName, 
            validFrom, 
            validTo, 
            assignedBy, 
            notes, 
            accessLevel, 
            maxReservations, 
            maxParticipants);

        _tourAgencies.Add(tourAgency);
    }

    /// <summary>
    /// Removes an agency from the tour
    /// </summary>
    public void RemoveAgency(Guid AgencyId)
    {
        var agencyToRemove = _tourAgencies.FirstOrDefault(ta => ta.AgencyId == AgencyId);
        if (agencyToRemove != null)
        {
            _tourAgencies.Remove(agencyToRemove);
        }
    }

    /// <summary>
    /// Gets all agency IDs assigned to this tour
    /// </summary>
    public IEnumerable<Guid> GetAssignedAgencyIds()
    {
        return _tourAgencies.Select(ta => ta.AgencyId);
    }

    /// <summary>
    /// Gets all active agencies for this tour
    /// </summary>
    public IEnumerable<TourAgency> GetActiveAgencies()
    {
        return _tourAgencies.Where(ta => ta.IsValid());
    }

    /// <summary>
    /// Checks if a specific agency has access to this tour
    /// </summary>
    public bool HasAgencyAccess(Guid AgencyId)
    {
        return _tourAgencies.Any(ta => ta.AgencyId == AgencyId && ta.IsValid());
    }

    /// <summary>
    /// Gets tour agency assignment for a specific agency
    /// </summary>
    public TourAgency? GetAgencyAssignment(Guid AgencyId)
    {
        return _tourAgencies.FirstOrDefault(ta => ta.AgencyId == AgencyId);
    }






    /// <summary>
    /// Calculates the total price for a participant (basic overload)
    /// </summary>
    public Money? CalculateParticipantPrice(ParticipantType participantType, DateTime? date = null)
    {
        return CalculateParticipantPrice(participantType, date, null, null);
    }

    /// <summary>
    /// Calculates the total price for a participant considering capabilities and features
    /// </summary>
    public Money? CalculateParticipantPrice(
        ParticipantType participantType, 
        DateTime? date = null,
        IEnumerable<string>? memberCapabilities = null,
        IEnumerable<string>? memberFeatures = null)
    {
        var pricing = GetPricing(participantType, date, memberCapabilities, memberFeatures);
        if (pricing != null)
        {
            return pricing.GetEffectivePrice();
        }

        // No pricing found for this participant type
        return null;
    }

    /// <summary>
    /// Calculates the total price for all confirmed participants
    /// </summary>
    public Money? CalculateTotalPrice(DateTime? date = null)
    {
        var checkDate = date ?? DateTime.UtcNow;
        decimal totalAmount = 0L;
        var hasValidPricing = false;

        foreach (var reservation in GetConfirmedReservations())
        {
            foreach (var participant in reservation.Participants)
            {
                var participantPrice = CalculateParticipantPrice(participant.ParticipantType, checkDate);
                if (participantPrice != null)
                {
                    totalAmount += participantPrice.AmountRials;
                    hasValidPricing = true;
                }
            }
        }

        return hasValidPricing ? new Money(totalAmount) : null;
    }

    /// <summary>
    /// Checks if tour has pricing configured for all participant types
    /// </summary>
    public bool HasCompletePricing(DateTime? date = null)
    {
        var checkDate = date ?? DateTime.UtcNow;
        var mainPricing = GetPricing(ParticipantType.Member, checkDate);
        var guestPricing = GetPricing(ParticipantType.Guest, checkDate);

        return mainPricing != null && guestPricing != null;
    }


    /// <summary>
    /// Checks if a participant meets the age requirements
    /// </summary>
    public bool IsParticipantAgeEligible(DateTime? birthDate, DateTime? checkDate = null)
    {
        if (birthDate == null)
            return MinAge == null && MaxAge == null; // If no birth date and no age restrictions, allow

        var referenceDate = checkDate ?? TourStart;
        var age = CalculateAge(birthDate.Value, referenceDate);

        if (MinAge.HasValue && age < MinAge.Value)
            return false;

        if (MaxAge.HasValue && age > MaxAge.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Validates if adding a guest to a reservation would exceed the maximum guests limit
    /// </summary>
    public bool CanAddGuestToReservation(TourReservation reservation)
    {
        if (reservation == null)
            throw new ArgumentNullException(nameof(reservation));

        if (!MaxGuestsPerReservation.HasValue)
            return true; // No limit set

        var currentGuestCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);
        return currentGuestCount < MaxGuestsPerReservation.Value;
    }

    /// <summary>
    /// Validates if the initial guests count for a new reservation is within limits
    /// </summary>
    public bool CanCreateReservationWithGuests(int guestCount)
    {
        if (!MaxGuestsPerReservation.HasValue)
            return true; // No limit set

        return guestCount <= MaxGuestsPerReservation.Value;
    }

    /// <summary>
    /// Gets the maximum number of additional guests that can be added to a reservation
    /// </summary>
    public int GetRemainingGuestSlotsForReservation(TourReservation reservation)
    {
        if (reservation == null)
            throw new ArgumentNullException(nameof(reservation));

        if (!MaxGuestsPerReservation.HasValue)
            return int.MaxValue; // No limit

        var currentGuestCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);
        return Math.Max(0, MaxGuestsPerReservation.Value - currentGuestCount);
    }



    /// <summary>
    /// Activates the tour
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the tour
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    // Status management methods

    /// <summary>
    /// Changes the tour status using the state machine
    /// </summary>
    public void ChangeStatus(TourStatus newStatus, string? reason = null)
    {
        if (!TourStateMachine.IsValidTransition(Status, newStatus))
        {
            var (_, errorMessage) = TourStateMachine.ValidateTransition(Status, newStatus, reason);
            throw new InvalidOperationException(errorMessage);
        }

        Status = newStatus;
    }

    /// <summary>
    /// Publishes the tour (moves from Draft to Published)
    /// Registration status is determined by registration dates.
    /// </summary>
    public void Publish()
    {
        ChangeStatus(TourStatus.Published, "Tour published");
    }

    /// <summary>
    /// Starts the tour (moves from Published to InProgress)
    /// </summary>
    public void StartTour()
    {
        ChangeStatus(TourStatus.InProgress, "Tour started");
    }

    /// <summary>
    /// Marks the tour as completed (moves from InProgress to Completed)
    /// </summary>
    public void CompleteTour()
    {
        ChangeStatus(TourStatus.Completed, "Tour completed");
    }

    /// <summary>
    /// Cancels the tour (can be called from Published or InProgress state)
    /// </summary>
    public void CancelTour(string? reason = null)
    {
        ChangeStatus(TourStatus.Cancelled, reason ?? "Tour cancelled");
    }

    /// <summary>
    /// Gets the overall capacity state based on all active capacities
    /// </summary>
    public CapacityState GetOverallCapacityState()
    {
        var activeCapacities = _capacities.Where(c => c.IsActive).ToList();
        
        if (!activeCapacities.Any())
            return CapacityState.Full;

        // Check capacity states
        var hasSpareCapacity = activeCapacities.Any(c => c.CapacityState == CapacityState.HasSpare);
        var hasTightCapacity = activeCapacities.Any(c => c.CapacityState == CapacityState.Tight);
        var allFullCapacity = activeCapacities.All(c => c.CapacityState == CapacityState.Full);

        // Logic: If all full → Full, else if any spare → HasSpare, else → Tight
        if (allFullCapacity)
            return CapacityState.Full;
        if (hasSpareCapacity)
            return CapacityState.HasSpare;
        if (hasTightCapacity)
            return CapacityState.Tight;

        return CapacityState.Full; // Default fallback
    }

    /// <summary>
    /// Updates capacity states for all active capacities
    /// </summary>
    public void UpdateAllCapacityStates()
    {
        foreach (var capacity in _capacities.Where(c => c.IsActive))
        {
            capacity.UpdateCapacityState();
        }
    }

    /// <summary>
    /// Checks if registration is currently open for any capacity.
    /// Registration status is determined by dates, not by tour status enum.
    /// </summary>
    public bool IsRegistrationOpen(DateTime currentDate)
    {
        return Status == TourStatus.Published && 
               IsActive && 
               _capacities.Any(c => c.IsRegistrationOpen(currentDate));
    }

    /// <summary>
    /// Checks if the tour is fully booked (only considering public capacities, excluding special capacities)
    /// </summary>
    public bool IsFullyBooked()
    {
        var activePublicCapacities = _capacities
            .Where(c => c.IsActive && !c.IsSpecial)
            .ToList();
        
        if (!activePublicCapacities.Any())
            return true;
        
        return activePublicCapacities.Sum(c => c.PublicRemainingParticipants) <= 0;
    }

    /// <summary>
    /// Checks if the tour is nearly full (≥80% utilization) based on public capacities
    /// </summary>
    public bool IsNearlyFull()
    {
        var activePublicCapacities = _capacities
            .Where(c => c.IsActive && !c.IsSpecial)
            .ToList();
        
        if (!activePublicCapacities.Any())
            return false;
        
        var totalMax = activePublicCapacities.Sum(c => c.PublicMaxParticipants);
        if (totalMax <= 0)
            return false;
        
        var totalRemaining = activePublicCapacities.Sum(c => c.PublicRemainingParticipants);
        var totalUsed = totalMax - totalRemaining;
        var utilizationPercentage = (double)totalUsed / totalMax * 100;
        
        return utilizationPercentage >= 80 && !IsFullyBooked();
    }



    // Private validation methods
    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (title.Length > 250)
            throw new ArgumentException("Title cannot exceed 250 characters", nameof(title));
    }

    private static void ValidateTourDates(DateTime tourStart, DateTime tourEnd)
    {
        if (tourStart >= tourEnd)
            throw new ArgumentException("Tour start must be before tour end");
    }


    private static void ValidateAgeRestrictions(int? minAge, int? maxAge)
    {
        if (minAge.HasValue && minAge.Value < 0)
            throw new ArgumentException("Minimum age cannot be negative", nameof(minAge));
        if (maxAge.HasValue && maxAge.Value < 0)
            throw new ArgumentException("Maximum age cannot be negative", nameof(maxAge));
        if (minAge.HasValue && maxAge.HasValue && minAge.Value > maxAge.Value)
            throw new ArgumentException("Minimum age cannot be greater than maximum age");
    }

    private static void ValidateMaxGuestsPerReservation(int? maxGuests)
    {
        if (maxGuests.HasValue && maxGuests.Value < 0)
            throw new ArgumentException("Maximum guests per reservation cannot be negative", nameof(maxGuests));
        if (maxGuests.HasValue && maxGuests.Value > 50) // Reasonable upper limit
            throw new ArgumentException("Maximum guests per reservation cannot exceed 50", nameof(maxGuests));
    }

    // Participant age validation is now done in reservation context

    private static int CalculateAge(DateTime birthDate, DateTime referenceDate)
    {
        var age = referenceDate.Year - birthDate.Year;
        if (referenceDate < birthDate.AddYears(age))
            age--;
        return age;
    }
}