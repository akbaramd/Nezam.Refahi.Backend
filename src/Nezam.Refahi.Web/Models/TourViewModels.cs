using System.ComponentModel.DataAnnotations;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Web.Models;

/// <summary>
/// ViewModel for tour listing with filtering and pagination
/// </summary>
public class TourListViewModel
{
    public List<TourItemViewModel> Tours { get; set; } = new();
    public TourFilterViewModel Filter { get; set; } = new();
    public TourPaginationViewModel Pagination { get; set; } = new();
    public TourStatisticsViewModel Statistics { get; set; } = new();
}

/// <summary>
/// ViewModel for individual tour item in listing
/// </summary>
public class TourItemViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TourStart { get; set; }
    public DateTime TourEnd { get; set; }
    public int MaxParticipants { get; set; }
    public int AvailableSpots { get; set; }
    public TourStatus Status { get; set; }
    public bool IsActive { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? MaxGuestsPerReservation { get; set; }
    public string StatusDisplayName { get; set; } = string.Empty;
    public string StatusBadgeColor { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public bool HasPhotos { get; set; }
    public string PrimaryPhotoUrl { get; set; } = string.Empty;
    public bool IsRegistrationOpen { get; set; }
    public int RemainingDays { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string PriceRange { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for tour filtering
/// </summary>
public class TourFilterViewModel
{
    public string? SearchTerm { get; set; }
    public TourStatus? Status { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public bool ShowOnlyAvailable { get; set; }
    public bool ShowOnlyWithPhotos { get; set; }
    public bool ShowOnlyRegistrationOpen { get; set; }
}

/// <summary>
/// ViewModel for tour pagination
/// </summary>
public class TourPaginationViewModel
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int StartIndex => (CurrentPage - 1) * PageSize + 1;
    public int EndIndex => Math.Min(CurrentPage * PageSize, TotalCount);
}

/// <summary>
/// ViewModel for tour statistics
/// </summary>
public class TourStatisticsViewModel
{
    public int TotalTours { get; set; }
    public int ActiveTours { get; set; }
    public int PublishedTours { get; set; }
    public int DraftTours { get; set; }
    public int CancelledTours { get; set; }
    public int UpcomingTours { get; set; }
    public int ToursWithAvailableSpots { get; set; }
    public int ToursWithOpenRegistration { get; set; }
    public Dictionary<string, int> ToursByStatus { get; set; } = new();
}

/// <summary>
/// ViewModel for tour details
/// </summary>
public class TourDetailsViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TourStart { get; set; }
    public DateTime TourEnd { get; set; }
    public int MaxParticipants { get; set; }
    public int AvailableSpots { get; set; }
    public TourStatus Status { get; set; }
    public bool IsActive { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? MaxGuestsPerReservation { get; set; }
    public string StatusDisplayName { get; set; } = string.Empty;
    public string StatusBadgeColor { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public List<TourPhotoViewModel> Photos { get; set; } = new();
    public List<TourPricingViewModel> Pricing { get; set; } = new();
    public List<TourCapacityViewModel> Capacities { get; set; } = new();
    public bool IsRegistrationOpen { get; set; }
    public int RemainingDays { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// ViewModel for tour photo
/// </summary>
public class TourPhotoViewModel
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// ViewModel for tour pricing
/// </summary>
public class TourPricingViewModel
{
    public Guid Id { get; set; }
    public ParticipantType ParticipantType { get; set; }
    public string ParticipantTypeDisplayName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "ریال";
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for tour capacity
/// </summary>
public class TourCapacityViewModel
{
    public Guid Id { get; set; }
    public int MaxParticipants { get; set; }
    public DateTime RegistrationStart { get; set; }
    public DateTime RegistrationEnd { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsRegistrationOpen { get; set; }
    public int AvailableSpots { get; set; }
}

/// <summary>
/// ViewModel for creating/editing tours
/// </summary>
public class TourFormViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "عنوان تور الزامی است")]
    [StringLength(250, ErrorMessage = "عنوان نمی‌تواند بیش از 250 کاراکتر باشد")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "توضیحات تور الزامی است")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "تاریخ شروع تور الزامی است")]
    public DateTime TourStart { get; set; } = DateTime.Now.AddDays(7);

    [Required(ErrorMessage = "تاریخ پایان تور الزامی است")]
    public DateTime TourEnd { get; set; } = DateTime.Now.AddDays(8);

    [Range(1, int.MaxValue, ErrorMessage = "حداقل سن باید بزرگتر از صفر باشد")]
    public int? MinAge { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "حداکثر سن باید بزرگتر از صفر باشد")]
    public int? MaxAge { get; set; }

    [Range(1, 50, ErrorMessage = "حداکثر مهمان در هر رزرو باید بین 1 تا 50 باشد")]
    public int? MaxGuestsPerReservation { get; set; }

    public bool IsActive { get; set; } = true;
    public TourStatus Status { get; set; } = TourStatus.Draft;
}

/// <summary>
/// ViewModel for tour management
/// </summary>
public class TourManagementViewModel
{
    public TourDetailsViewModel Tour { get; set; } = new();
    public List<TourReservationViewModel> Reservations { get; set; } = new();
    public TourManagementStatisticsViewModel Statistics { get; set; } = new();
}

/// <summary>
/// ViewModel for tour reservation
/// </summary>
public class TourReservationViewModel
{
    public Guid Id { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public Guid MemberId { get; set; }
    public ReservationStatus Status { get; set; }
    public string StatusDisplayName { get; set; } = string.Empty;
    public string StatusBadgeColor { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int ParticipantCount { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// ViewModel for tour management statistics
/// </summary>
public class TourManagementStatisticsViewModel
{
    public int TotalReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int PendingReservations { get; set; }
    public int CancelledReservations { get; set; }
    public int TotalParticipants { get; set; }
    public decimal TotalRevenue { get; set; }
    public int AvailableSpots { get; set; }
    public double OccupancyRate { get; set; }
}
