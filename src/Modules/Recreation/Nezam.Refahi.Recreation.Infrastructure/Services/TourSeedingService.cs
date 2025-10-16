using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Infrastructure.Persistence;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Infrastructure.Services;

/// <summary>
/// High-performance service for seeding tour data with efficient duplicate checking
/// </summary>
public class TourSeedingService : ITourSeedingService
{
    private readonly ITourRepository _tourRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly IRecreationUnitOfWork _uow;
    private readonly ILogger<TourSeedingService> _logger;

    public TourSeedingService(
        ITourRepository tourRepository,
        IFeatureRepository featureRepository,
        ILogger<TourSeedingService> logger, IRecreationUnitOfWork uow)
    {
        _tourRepository = tourRepository;
        _featureRepository = featureRepository;
        _logger = logger;
        _uow = uow;
    }

    public async Task SeedToursAsync()
    {
        _logger.LogInformation("Starting efficient tour seeding process...");

        try
        {
            await _uow.BeginAsync();

            // Seed features first
            await SeedFeaturesAsync();

            // Seed predefined tours efficiently
            await SeedPredefinedToursAsync();

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();
            _logger.LogInformation("Tour seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during tour seeding");
            await _uow.RollbackAsync();
            throw;
        }
    }

    private async Task SeedFeaturesAsync()
    {
        _logger.LogInformation("Seeding features...");

        var features = new List<Feature>
        {
            new Feature("حمل و نقل", "سرویس حمل و نقل شامل بسته", "fas fa-bus", null, 1, true, "اتوبوس VIP", "required"),
            new Feature("وعده های غذایی", "تمام وعده های غذایی شامل بسته", "fas fa-utensils", null, 2, true, "صبحانه، نهار و شام", "required"),
            new Feature("اقامت", "اقامت در هتل", "fas fa-hotel", null, 3, true, "هتل 4 ستاره", "required"),
            new Feature("راهنمای تور", "راهنمای حرفه‌ای تور", "fas fa-user-tie", null, 4, false, "راهنمای مجرب", "optional"),
            new Feature("بیمه مسافرتی", "بیمه مسافرتی شامل بسته", "fas fa-shield-alt", null, 5, true, "پوشش کامل", "required"),
            new Feature("مجوز گردشگری", "مجوز گردشگری مخصوص تورها", "fas fa-certificate", null, 6, true, "مجوز رسمی", "required")
        };

        foreach (var feature in features)
        {
            var existingFeature = await _featureRepository.FindOneAsync(x=>x.Id == feature.Id);
            if (existingFeature == null)
            {
                await _featureRepository.AddAsync(feature);
                _logger.LogDebug("Added feature: {FeatureName}", feature.Name);
            }
        }

        await _featureRepository.SaveAsync();
        _logger.LogInformation("Features seeding completed");
    }

    private async Task SeedPredefinedToursAsync()
    {
        _logger.LogInformation("Seeding predefined tours with efficient duplicate checking...");

        var toursToSeed = GetPredefinedTours();
        var seededCount = 0;

        foreach (var tourData in toursToSeed)
        {
            // Efficient check: Search by exact title instead of loading all tours
            var existingTours = await _tourRepository.SearchToursByTitleAsync(tourData.Title);
            if (existingTours.Any(t => t.Title.Equals(tourData.Title, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogDebug("Tour '{Title}' already exists, skipping", tourData.Title);
                continue;
            }

            await CreateTourAsync(tourData);
            seededCount++;
            _logger.LogInformation("Created tour: {Title}", tourData.Title);
        }

        _logger.LogInformation("Tour seeding completed. {Count} new tours created", seededCount);
    }

    private async Task CreateTourAsync(TourSeedData tourData)
    {
        var tour = new Tour(
            tourData.Title,
            tourData.Description,
            tourData.TourStart,
            tourData.TourEnd,
            tourData.MinAge,
            tourData.MaxAge
        );

        // Add capacity with registration period
        tour.AddCapacity(
            tourData.MaxParticipants,
            tourData.RegistrationStart,
            tourData.RegistrationEnd,
            "ظرفیت اولیه تور"
        );

        // Add photos
        foreach (var photo in tourData.Photos)
        {
            tour.AddPhoto(new TourPhoto(tour.Id, photo.ImageUrl, photo.Caption, photo.Order));
        }

        // Add pricing
        foreach (var pricing in tourData.Pricing)
        {
            var tourPricing = new TourPricing(
                tour.Id,
                pricing.ParticipantType,
                new Money(pricing.PriceInRials),
                null,
                tourData.RegistrationStart,
                tourData.RegistrationEnd
            );
            tour.AddPricing(tourPricing);
        }

        // Add member capabilities
        foreach (var capability in tourData.MemberCapabilities)
        {
            tour.AddMemberCapability(capability);
        }

        // Add member features
        foreach (var feature in tourData.MemberFeatures)
        {
            tour.AddMemberFeature(feature);
        }

        // Add tour features efficiently (get features once, reuse)
        var availableFeatures = await _featureRepository.FindAsync(x => true);
        var featuresList = availableFeatures.ToList(); // Materialize once

        foreach (var featureIndex in tourData.FeatureIds)
        {
            if (featureIndex > 0 && featureIndex <= featuresList.Count)
            {
                var feature = featuresList[featureIndex - 1];
                var tourFeature = new TourFeature(tour.Id, feature.Id, "Included in package", true);
                tour.AddTourFeature(tourFeature);
            }
        }

        await _tourRepository.AddAsync(tour);
    }

    private static List<TourSeedData> GetPredefinedTours()
    {
        var baseDate = DateTime.UtcNow;
        var regStart1 = baseDate.AddDays(1);
        var tourStart1 = baseDate.AddMonths(2);
        var regEnd1 = tourStart1.AddDays(-10);
        var tourEnd1 = tourStart1.AddDays(7);

        var regStart2 = baseDate.AddDays(5);
        var tourStart2 = baseDate.AddMonths(3);
        var regEnd2 = tourStart2.AddDays(-15);
        var tourEnd2 = tourStart2.AddDays(5);

        var regStart3 = baseDate.AddDays(3);
        var tourStart3 = baseDate.AddMonths(4);
        var regEnd3 = tourStart3.AddDays(-12);
        var tourEnd3 = tourStart3.AddDays(6);

        return new List<TourSeedData>
        {
            new TourSeedData
            {
                Title = "تور 7 روزه شمال ایران",
                Description = "تور فوق‌العاده 7 روزه به مناطق دیدنی شمال ایران شامل گیلان و مازندران با بازدید از جنگل‌های هیرکانی، دریاچه انزلی، بندر انزلی و سایر مناطق توریستی. این تور شامل اقامت در هتل‌های 4 ستاره، وعده‌های غذایی کامل، حمل و نقل مجهز و راهنمای حرفه‌ای می‌باشد.",
                RegistrationStart = regStart1,
                RegistrationEnd = regEnd1,
                TourStart = tourStart1,
                TourEnd = tourEnd1,
                MaxParticipants = 25,
                MinAge = 18,
                MaxAge = 65,
                Photos = new List<PhotoSeedData>
                {
                    new PhotoSeedData { ImageUrl = "/uploads/tours/northern-iran-1.jpg", Caption = "جنگل‌های سبز شمال", Order = 1 },
                    new PhotoSeedData { ImageUrl = "/uploads/tours/northern-iran-2.jpg", Caption = "دریاچه انزلی", Order = 2 },
                    new PhotoSeedData { ImageUrl = "/uploads/tours/northern-iran-3.jpg", Caption = "بندر زیبای انزلی", Order = 3 }
                },
                Pricing = new List<PricingSeedData>
                {
                    new PricingSeedData { ParticipantType = ParticipantType.Member, PriceInRials = 15000000 },
                    new PricingSeedData { ParticipantType = ParticipantType.Guest, PriceInRials = 12000000 }
                },
                MemberCapabilities = new List<string> {  },
                MemberFeatures = new List<string> { "execute", "grade3" ,"grade1"},
                FeatureIds = new List<int> { 1, 2, 3 }
            },
            new TourSeedData
            {
                Title = "تور 5 روزه اصفهان",
                Description = "تور 5 روزه اصفهان شامل بازدید از میدان نقش جهان، مسجد شاه، مسجد شیخ لطف‌الله، کاخ عالی‌قاپو، سی و سه پل و سایر جاذبه‌های تاریخی و فرهنگی اصفهان.",
                RegistrationStart = regStart2,
                RegistrationEnd = regEnd2,
                TourStart = tourStart2,
                TourEnd = tourEnd2,
                MaxParticipants = 20,
                MinAge = 16,
                MaxAge = 70,
                Photos = new List<PhotoSeedData>
                {
                    new PhotoSeedData { ImageUrl = "/uploads/tours/isfahan-1.jpg", Caption = "میدان نقش جهان", Order = 1 },
                    new PhotoSeedData { ImageUrl = "/uploads/tours/isfahan-2.jpg", Caption = "مسجد شاه", Order = 2 }
                },
                Pricing = new List<PricingSeedData>
                {
                    new PricingSeedData { ParticipantType = ParticipantType.Member, PriceInRials = 10000000 },
                    new PricingSeedData { ParticipantType = ParticipantType.Guest, PriceInRials = 8500000 }
                },
                MemberCapabilities = new List<string> { "structure_supervisor_grade1" },
                MemberFeatures = new List<string> { "structure", "grade1" },
                FeatureIds = new List<int> { 1, 2, 4 }
            },
            new TourSeedData
            {
                Title = "تور 6 روزه شیراز",
                Description = "تور 6 روزه شیراز شامل بازدید از تخت جمشید، نقش رستم، حافظیه، سعدیه، مسجد نصیرالملک، باغ ارم و سایر مناطق تاریخی و فرهنگی شیراز.",
                RegistrationStart = regStart3,
                RegistrationEnd = regEnd3,
                TourStart = tourStart3,
                TourEnd = tourEnd3,
                MaxParticipants = 22,
                MinAge = 18,
                MaxAge = 68,
                Photos = new List<PhotoSeedData>
                {
                    new PhotoSeedData { ImageUrl = "/uploads/tours/shiraz-1.jpg", Caption = "تخت جمشید", Order = 1 },
                    new PhotoSeedData { ImageUrl = "/uploads/tours/shiraz-2.jpg", Caption = "حافظیه", Order = 2 },
                    new PhotoSeedData { ImageUrl = "/uploads/tours/shiraz-3.jpg", Caption = "مسجد نصیرالملک", Order = 3 }
                },
                Pricing = new List<PricingSeedData>
                {
                    new PricingSeedData { ParticipantType = ParticipantType.Member, PriceInRials = 13000000 },
                    new PricingSeedData { ParticipantType = ParticipantType.Guest, PriceInRials = 11000000 }
                },
                MemberCapabilities = new List<string> { "architecture_execute_grade2"  },
                MemberFeatures = new List<string> { "architecture", "execute", "grade2" },
                FeatureIds = new List<int> { 1, 2, 3, 5 }
            }
        };
    }

    #region Seed Data Models

    private record TourSeedData
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime RegistrationStart { get; init; }
        public DateTime RegistrationEnd { get; init; }
        public DateTime TourStart { get; init; }
        public DateTime TourEnd { get; init; }
        public int MaxParticipants { get; init; }
        public int MinAge { get; init; }
        public int MaxAge { get; init; }
        public List<PhotoSeedData> Photos { get; init; } = new();
        public List<PricingSeedData> Pricing { get; init; } = new();
        public List<string> MemberCapabilities { get; init; } = new();
        public List<string> MemberFeatures { get; init; } = new();
        public List<int> FeatureIds { get; init; } = new();
    }

    private record PhotoSeedData
    {
        public string ImageUrl { get; init; } = string.Empty;
        public string Caption { get; init; } = string.Empty;
        public int Order { get; init; }
    }

    private record PricingSeedData
    {
        public ParticipantType ParticipantType { get; init; }
        public decimal PriceInRials { get; init; }
    }

    #endregion
}