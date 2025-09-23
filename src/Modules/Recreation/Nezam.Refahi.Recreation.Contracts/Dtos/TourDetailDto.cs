using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Complete tour details DTO with all related information
/// </summary>
public class TourDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LongDescription { get; set; }
    public string? Summary { get; set; }
    
    // Tour dates and timing
    public DateTime TourStart { get; set; }
    public DateTime TourEnd { get; set; }
    public TimeSpan Duration { get; set; }
    public int DurationDays { get; set; }
    public int DurationHours { get; set; }

    // Enhanced capacity information
    public int MaxCapacity { get; set; }
    public int RemainingCapacity { get; set; }
    public int ReservedCapacity { get; set; }
    public double CapacityUtilizationPercentage { get; set; }
    public bool IsFullyBooked { get; set; }
    public bool IsNearlyFull { get; set; }
    public string CapacityStatus { get; set; } = string.Empty;
    public string CapacityMessage { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPopular { get; set; }
    
    // Location information
    public string? Location { get; set; }
    public string? MeetingPoint { get; set; }
    public string? Coordinates { get; set; }
    public string? Region { get; set; }
    public string? City { get; set; }

    // Age and requirements
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? AgeRestrictionNote { get; set; }

    // Guest limitations per reservation
    public int? MaxGuestsPerReservation { get; set; }
    public string? PhysicalRequirements { get; set; }
    public string? HealthRequirements { get; set; }

    // Registration period
    public DateTime? RegistrationStart { get; set; }
    public DateTime? RegistrationEnd { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public string RegistrationStatus { get; set; } = string.Empty;
    public TimeSpan? TimeUntilRegistrationEnd { get; set; }

    // Detailed capacity information
    public List<TourCapacityDetailDto> Capacities { get; set; } = new();

    // Member requirements
    public List<string> RequiredCapabilities { get; set; } = new();
    public List<string> RequiredFeatures { get; set; } = new();
    public List<string> RecommendedFeatures { get; set; } = new();

    // Tour features with full details
    public List<TourFeatureDetailDto> Features { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<string> Inclusions { get; set; } = new();
    public List<string> Exclusions { get; set; } = new();

    // Restricted tours
    public List<RestrictedTourDetailDto> RestrictedTours { get; set; } = new();

    // Pricing information with details
    public List<TourPricingDetailDto> Pricing { get; set; } = new();
    public decimal? LowestPrice { get; set; }
    public decimal? HighestPrice { get; set; }
    public bool HasDiscount { get; set; }
    public decimal? MaxDiscountPercentage { get; set; }

    // Media
    public List<TourPhotoDetailDto> Photos { get; set; } = new();
    public string? MainPhotoUrl { get; set; }
    public string? VideoUrl { get; set; }
    public List<string> GalleryUrls { get; set; } = new();

    // Statistics
    public int TotalParticipants { get; set; }
    public int MainParticipants { get; set; }
    public int GuestParticipants { get; set; }
    public int TotalReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int PendingReservations { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }

    // User-specific information
    public ReservationStatus? UserReservationStatus { get; set; }
    public Guid? UserReservationId { get; set; }
    public string? UserReservationTrackingCode { get; set; }
    public DateTime? UserReservationDate { get; set; }
    public DateTime? UserReservationExpiryDate { get; set; }
    public bool CanUserReserve { get; set; }
    public List<string> ReservationBlockReasons { get; set; } = new();

    // Logistics
    public string? TransportationType { get; set; }
    public string? AccommodationType { get; set; }
    public string? MealPlan { get; set; }
    public List<string> Equipment { get; set; } = new();
    public List<string> WhatToBring { get; set; } = new();

    // Policies
    public string? CancellationPolicy { get; set; }
    public string? RefundPolicy { get; set; }
    public string? TermsAndConditions { get; set; }
    public DateTime? CancellationDeadline { get; set; }
    public DateTime? ChangeDeadline { get; set; }

    // Weather and season
    public string? Season { get; set; }
    public string? WeatherConditions { get; set; }
    public string? BestTimeToVisit { get; set; }

    // Contact and support
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? GuideInfo { get; set; }
    public string? EmergencyContact { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public int ViewCount { get; set; }
    public DateTime? LastViewedAt { get; set; }
    
    // SEO and marketing
    public List<string> Tags { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? ShareUrl { get; set; }
}

/// <summary>
/// Detailed tour feature information
/// </summary>
public class TourFeatureDetailDto
{
    public Guid Id { get; set; }
    public Guid TourId { get; set; }
    public Guid FeatureId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Value { get; set; }
    public string? IconClass { get; set; }
    public string? Color { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsHighlight { get; set; }
    public string? Category { get; set; }
    public string? GroupName { get; set; }
}

/// <summary>
/// Restricted tour details
/// </summary>
public class RestrictedTourDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime TourStart { get; set; }
    public DateTime TourEnd { get; set; }
    public bool IsActive { get; set; }
    public string? Reason { get; set; }
    public string? ConflictType { get; set; }
}

/// <summary>
/// Detailed pricing information
/// </summary>
public class TourPricingDetailDto
{
    public Guid Id { get; set; }
    public ParticipantType ParticipantType { get; set; }
    public string ParticipantTypeName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal EffectivePrice { get; set; }
    public decimal? DiscountAmount { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public string? Currency { get; set; } = "ریال";
    public bool IsEarlyBird { get; set; }
    public bool IsLastMinute { get; set; }
    public string? PriceNote { get; set; }
}

/// <summary>
/// Detailed photo information
/// </summary>
public class TourPhotoDetailDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsMain { get; set; }
    public bool IsPublic { get; set; }
    public string? Category { get; set; }
    public DateTime? TakenAt { get; set; }
    public string? Photographer { get; set; }
    public string? Location { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
}
