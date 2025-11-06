using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Web.Application.Specifications;
using Nezam.Refahi.Web.Models;
using Nezam.Refahi.Web.Services;

namespace Nezam.Refahi.Web.Controllers;

/// <summary>
/// Controller for managing tours
/// </summary>
public class TourController : Controller
{
    private readonly ITourRepository _tourRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<TourController> _logger;

    public TourController(
        ITourRepository tourRepository,
        IAuthService authService,
        ILogger<TourController> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Displays the list of tours with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(TourFilterViewModel? filter = null, int page = 1, int pageSize = 12)
    {
        try
        {
            // Set default filter if not provided
            filter ??= new TourFilterViewModel
            {
                IsActive = true,
                ShowOnlyAvailable = true
            };

            // Create specification for filtering
            var spec = new TourPaginatedSpec(
                pageNumber: page,
                pageSize: pageSize,
                searchTerm: filter.SearchTerm,
                status: filter.Status,
                isActive: filter.IsActive,
                startDateFrom: filter.StartDateFrom,
                startDateTo: filter.StartDateTo,
                minPriceRials: filter.MinPrice.HasValue ? (long)(filter.MinPrice.Value * 1000) : null,
                maxPriceRials: filter.MaxPrice.HasValue ? (long)(filter.MaxPrice.Value * 1000) : null,
                minAge: filter.MinAge,
                maxAge: filter.MaxAge,
                sortBy: filter.SortBy,
                sortDescending: filter.SortDescending
            );

            // Get tours using repository
            var (tours, totalCount) = await _tourRepository.GetToursPaginatedAsync(
                spec.PageNumber,
                spec.PageSize,
                spec.IsActive ?? true);

            // Convert to view models
            var tourViewModels = tours.Select(MapToTourItemViewModel).ToList();

            // Create view model
            var viewModel = new TourListViewModel
            {
                Tours = tourViewModels,
                Filter = filter,
                Pagination = new TourPaginationViewModel
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                },
                Statistics = await GetTourStatisticsAsync()
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading tours");
            TempData["ErrorMessage"] = "خطا در بارگذاری لیست تورها";
            return View(new TourListViewModel());
        }
    }

    /// <summary>
    /// Displays tour details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "تور مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = MapToTourDetailsViewModel(tour);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading tour details for ID: {TourId}", id);
            TempData["ErrorMessage"] = "خطا در بارگذاری جزئیات تور";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Displays the form for creating a new tour
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new TourFormViewModel();
        return View(viewModel);
    }

    /// <summary>
    /// Creates a new tour
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TourFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userInfo = _authService.GetCurrentUser();
            var createdBy = userInfo?.UserName ?? "System";

            // Create new tour
            var tour = new Tour(
                title: model.Title,
                description: model.Description,
                tourStart: model.TourStart,
                tourEnd: model.TourEnd,
                minAge: model.MinAge,
                maxAge: model.MaxAge,
                maxGuestsPerReservation: model.MaxGuestsPerReservation
            );

            // Set additional properties
            if (!model.IsActive)
                tour.Deactivate();

            if (model.Status != TourStatus.Draft)
                tour.ChangeStatus(model.Status);

            // Save tour
            await _tourRepository.AddAsync(tour);

            TempData["SuccessMessage"] = "تور با موفقیت ایجاد شد";
            return RedirectToAction(nameof(Details), new { id = tour.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating tour");
            ModelState.AddModelError(string.Empty, "خطا در ایجاد تور");
            return View(model);
        }
    }

    /// <summary>
    /// Displays the form for editing a tour
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "تور مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = MapToTourFormViewModel(tour);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading tour for edit, ID: {TourId}", id);
            TempData["ErrorMessage"] = "خطا در بارگذاری تور";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Updates an existing tour
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TourFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var tour = await _tourRepository.GetByIdAsync(model.Id!.Value);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "تور مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            // Update tour details
            tour.UpdateDetails(model.Title, model.Description);
            tour.UpdateTourDates(model.TourStart, model.TourEnd);
            tour.SetAgeRestrictions(model.MinAge, model.MaxAge);
            tour.SetMaxGuestsPerReservation(model.MaxGuestsPerReservation);

            // Update status and active state
            if (tour.Status != model.Status)
                tour.ChangeStatus(model.Status);

            if (tour.IsActive != model.IsActive)
            {
                if (model.IsActive)
                    tour.Activate();
                else
                    tour.Deactivate();
            }

            // Save changes
            await _tourRepository.UpdateAsync(tour);

            TempData["SuccessMessage"] = "تور با موفقیت به‌روزرسانی شد";
            return RedirectToAction(nameof(Details), new { id = tour.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating tour, ID: {TourId}", model.Id);
            ModelState.AddModelError(string.Empty, "خطا در به‌روزرسانی تور");
            return View(model);
        }
    }

    /// <summary>
    /// Deletes a tour
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "تور مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            await _tourRepository.DeleteAsync(tour);

            TempData["SuccessMessage"] = "تور با موفقیت حذف شد";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting tour, ID: {TourId}", id);
            TempData["ErrorMessage"] = "خطا در حذف تور";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Displays tour management interface
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Manage(Guid id)
    {
        try
        {
            var tour = await _tourRepository.GetByIdAsync(id);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "تور مورد نظر یافت نشد";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new TourManagementViewModel
            {
                Tour = MapToTourDetailsViewModel(tour),
                Statistics = GetTourManagementStatistics(tour)
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while loading tour management, ID: {TourId}", id);
            TempData["ErrorMessage"] = "خطا در بارگذاری مدیریت تور";
            return RedirectToAction(nameof(Index));
        }
    }

    #region Private Helper Methods

    private TourItemViewModel MapToTourItemViewModel(Tour tour)
    {
        var duration = (tour.TourEnd - tour.TourStart).TotalDays;
        var remainingDays = (int)(tour.TourStart - DateTime.UtcNow).TotalDays;

        return new TourItemViewModel
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description.Length > 100 
                ? tour.Description.Substring(0, 100) + "..." 
                : tour.Description,
            TourStart = tour.TourStart,
            TourEnd = tour.TourEnd,
            MaxParticipants = tour.MaxParticipants,
            AvailableSpots = tour.GetAvailableSpots(),
            Status = tour.Status,
            IsActive = tour.IsActive,
            MinAge = tour.MinAge,
            MaxAge = tour.MaxAge,
            MaxGuestsPerReservation = tour.MaxGuestsPerReservation,
            StatusDisplayName = GetStatusDisplayName(tour.Status),
            StatusBadgeColor = GetStatusBadgeColor(tour.Status),
            Duration = $"{duration:F1} روز",
            HasPhotos = tour.Photos.Any(),
            PrimaryPhotoUrl = tour.Photos.FirstOrDefault()?.Url ?? "~/assets/images/tours/default-tour.jpg",
            IsRegistrationOpen = tour.IsRegistrationOpen(DateTime.UtcNow),
            RemainingDays = Math.Max(0, remainingDays),
            MinPrice = GetMinPrice(tour),
            MaxPrice = GetMaxPrice(tour),
            PriceRange = GetPriceRange(tour)
        };
    }

    private TourDetailsViewModel MapToTourDetailsViewModel(Tour tour)
    {
        var duration = (tour.TourEnd - tour.TourStart).TotalDays;
        var remainingDays = (int)(tour.TourStart - DateTime.UtcNow).TotalDays;

        return new TourDetailsViewModel
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            TourStart = tour.TourStart,
            TourEnd = tour.TourEnd,
            MaxParticipants = tour.MaxParticipants,
            AvailableSpots = tour.GetAvailableSpots(),
            Status = tour.Status,
            IsActive = tour.IsActive,
            MinAge = tour.MinAge,
            MaxAge = tour.MaxAge,
            MaxGuestsPerReservation = tour.MaxGuestsPerReservation,
            StatusDisplayName = GetStatusDisplayName(tour.Status),
            StatusBadgeColor = GetStatusBadgeColor(tour.Status),
            Duration = $"{duration:F1} روز",
            Photos = tour.Photos.Select(p => new TourPhotoViewModel
            {
                Id = p.Id,
                Url = p.Url,
                DisplayOrder = p.DisplayOrder
            }).ToList(),
            Pricing = tour.Pricing.Select(p => new TourPricingViewModel
            {
                Id = p.Id,
                ParticipantType = p.ParticipantType,
                ParticipantTypeDisplayName = GetParticipantTypeDisplayName(p.ParticipantType),
                Price = p.GetEffectivePrice().AmountRials / 1000m,
                Currency = "ریال",
                ValidFrom = p.ValidFrom,
                ValidTo = p.ValidTo,
                IsActive = p.IsActive
            }).ToList(),
            Capacities = tour.Capacities.Select(c => new TourCapacityViewModel
            {
                Id = c.Id,
                MaxParticipants = c.MaxParticipants,
                RegistrationStart = c.RegistrationStart,
                RegistrationEnd = c.RegistrationEnd,
                Description = c.Description,
                IsActive = c.IsActive,
                IsRegistrationOpen = c.IsRegistrationOpen(DateTime.UtcNow),
                AvailableSpots = c.MaxParticipants - tour.GetConfirmedReservationCount()
            }).ToList(),
            IsRegistrationOpen = tour.IsRegistrationOpen(DateTime.UtcNow),
            RemainingDays = Math.Max(0, remainingDays),
            CreatedAt = tour.CreatedAt,
        };
    }

    private TourFormViewModel MapToTourFormViewModel(Tour tour)
    {
        return new TourFormViewModel
        {
            Id = tour.Id,
            Title = tour.Title,
            Description = tour.Description,
            TourStart = tour.TourStart,
            TourEnd = tour.TourEnd,
            MinAge = tour.MinAge,
            MaxAge = tour.MaxAge,
            MaxGuestsPerReservation = tour.MaxGuestsPerReservation,
            IsActive = tour.IsActive,
            Status = tour.Status
        };
    }

    private async Task<TourStatisticsViewModel> GetTourStatisticsAsync()
    {
        try
        {
            var activeTours = await _tourRepository.GetActiveToursAsync();
            var upcomingTours = await _tourRepository.GetUpcomingToursAsync(DateTime.UtcNow);

            var statistics = new TourStatisticsViewModel
            {
                TotalTours = activeTours.Count(),
                ActiveTours = activeTours.Count(),
                UpcomingTours = upcomingTours.Count(),
                ToursWithAvailableSpots = activeTours.Count(t => t.GetAvailableSpots() > 0),
                ToursWithOpenRegistration = activeTours.Count(t => t.IsRegistrationOpen(DateTime.UtcNow))
            };

            // Group by status
            statistics.ToursByStatus = activeTours
                .GroupBy(t => t.Status)
                .ToDictionary(g => GetStatusDisplayName(g.Key), g => g.Count());

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting tour statistics");
            return new TourStatisticsViewModel();
        }
    }

    private TourManagementStatisticsViewModel GetTourManagementStatistics(Tour tour)
    {
        var confirmedReservations = tour.GetConfirmedReservations();
        var pendingReservations = tour.GetPendingReservations();

        return new TourManagementStatisticsViewModel
        {
            TotalReservations = tour.Reservations.Count,
            ConfirmedReservations = confirmedReservations.Count(),
            PendingReservations = pendingReservations.Count(),
            TotalParticipants = tour.GetConfirmedReservationCount(),
            AvailableSpots = tour.GetAvailableSpots(),
            OccupancyRate = tour.MaxParticipants > 0 
                ? (double)tour.GetConfirmedReservationCount() / tour.MaxParticipants * 100 
                : 0
        };
    }

    private string GetStatusDisplayName(TourStatus status)
    {
        return status switch
        {
            TourStatus.Draft => "پیش‌نویس",
            TourStatus.Published => "برنامه‌ریزی شده",
            TourStatus.InProgress => "در حال اجرا",
            TourStatus.Completed => "تکمیل شده",
            TourStatus.Cancelled => "لغو شده",
            _ => "نامشخص"
        };
    }

    private string GetStatusBadgeColor(TourStatus status)
    {
        return status switch
        {
            TourStatus.Draft => "secondary",
            TourStatus.Published => "info",
            TourStatus.InProgress => "primary",
            TourStatus.Completed => "success",
            TourStatus.Cancelled => "danger",
            _ => "secondary"
        };
    }

    private string GetParticipantTypeDisplayName(ParticipantType participantType)
    {
        return participantType switch
        {
            ParticipantType.Member => "عضو",
            ParticipantType.Guest => "مهمان",
            _ => "نامشخص"
        };
    }

    private decimal? GetMinPrice(Tour tour)
    {
        var activePricing = tour.GetActivePricing();
        return activePricing.Any() 
            ? activePricing.Min(p => p.GetEffectivePrice().AmountRials) / 1000m 
            : null;
    }

    private decimal? GetMaxPrice(Tour tour)
    {
        var activePricing = tour.GetActivePricing();
        return activePricing.Any() 
            ? activePricing.Max(p => p.GetEffectivePrice().AmountRials) / 1000m 
            : null;
    }

    private string GetPriceRange(Tour tour)
    {
        var minPrice = GetMinPrice(tour);
        var maxPrice = GetMaxPrice(tour);

        if (!minPrice.HasValue && !maxPrice.HasValue)
            return "قیمت تعیین نشده";

        if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value == maxPrice.Value)
            return $"{minPrice.Value:N0} ریال";

        if (minPrice.HasValue && maxPrice.HasValue)
            return $"{minPrice.Value:N0} - {maxPrice.Value:N0} ریال";

        if (minPrice.HasValue)
            return $"از {minPrice.Value:N0} ریال";

        if (maxPrice.HasValue)
            return $"تا {maxPrice.Value:N0} ریال";

        return "قیمت تعیین نشده";
    }

    #endregion
}
