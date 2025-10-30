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
            new Feature("مجوز گردشگری", "مجوز گردشگری مخصوص تورها", "fas fa-certificate", null, 6, true, "مجوز رسمی", "required"),
            new Feature("has_licences", "مجوزهای مورد نیاز برای شرکت در تور", "fas fa-id-card", null, 7, true, "مجوزهای تخصصی", "required"),
            new Feature("هتل", "اقامت در هتل مجهز", "fas fa-bed", null, 8, true, "هتل مناسب", "required"),
            new Feature("تور لیدر", "راهنمای تور حرفه‌ای", "fas fa-user-tie", null, 9, true, "راهنمای مجرب", "required"),
            new Feature("شب مانی", "اقامت شبانه در منطقه", "fas fa-moon", null, 10, true, "اقامت شبانه", "required"),
            new Feature("کوهنوردی", "تجهیزات و راهنمای کوهنوردی", "fas fa-mountain", null, 11, true, "کوهنوردی حرفه‌ای", "required"),
            new Feature("اقامتگاه", "اقامتگاه مناسب در طبیعت", "fas fa-campground", null, 12, true, "اقامتگاه طبیعت", "required")
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
        // Kalibar tour dates (September 2025)
        // اواسط شهریور 1404 = September 15, 2025
        // 27 شهریور 1404 = September 27, 2025
        var kalibarRegStart = new DateTime(2025, 9, 15); // اواسط شهریور 1404
        var kalibarRegEnd = new DateTime(2025, 9, 25); // پایان شهریور 1404
        var kalibarTourStart = new DateTime(2025, 9, 27); // 27 شهریور 1404
        var kalibarTourEnd = kalibarTourStart.AddDays(2); // دو روزه

        // Soledoakl waterfall tour dates (August 2025)
        // 15 مرداد 1404 = August 6, 2025
        // اواخر مرداد 1404 = August 25, 2025
        var soledoaklRegStart = new DateTime(2025, 8, 6); // 15 مرداد 1404
        var soledoaklRegEnd = new DateTime(2025, 8, 20); // اواخر مرداد 1404
        var soledoaklTourStart = new DateTime(2025, 8, 25); // اواخر مرداد 1404
        var soledoaklTourEnd = soledoaklTourStart.AddDays(1); // یک روزه

        return new List<TourSeedData>
        {
            new TourSeedData
            {
                Title = "تور 2 روزه کلیبر",
                Description = "تور دو روزه کلیبر شامل بازدید از قلعه تاریخی بابک، جنگل‌های ارسباران، طبیعت بکر و مناظر زیبای منطقه کلیبر در استان آذربایجان شرقی. این تور شامل پیاده‌روی در طبیعت، بازدید از قلعه بابک که بر فراز کوهی قرار دارد و به عنوان نمادی از مقاومت تاریخی شناخته می‌شود، و همچنین لذت بردن از هوای پاک و طبیعت سرسبز منطقه می‌باشد.",
                RegistrationStart = kalibarRegStart,
                RegistrationEnd = kalibarRegEnd,
                TourStart = kalibarTourStart,
                TourEnd = kalibarTourEnd,
                MaxParticipants = 20,
                MinAge = 18,
                MaxAge = 65,
                Photos = new List<PhotoSeedData>
                {
                    new PhotoSeedData { ImageUrl = "/tours/kaleibar1.webp", Caption = "قلعه تاریخی بابک", Order = 1 },
                    new PhotoSeedData { ImageUrl = "/tours/kaleibar2.webp", Caption = "جنگل‌های ارسباران", Order = 2 },
                },
                Pricing = new List<PricingSeedData>
                {
                    new PricingSeedData { ParticipantType = ParticipantType.Member, PriceInRials = 1000000 },
                    new PricingSeedData { ParticipantType = ParticipantType.Guest, PriceInRials = 1700000 }
                },
                MemberCapabilities = new List<string> { "has_licences" },
                MemberFeatures = new List<string> { "has_licences" },
                FeatureIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            },
            new TourSeedData
            {
                Title = "تور 1 روزه آبشار سوله دوکل",
                Description = "تور یک روزه آبشار سوله دوکل در منطقه مرگور، استان آذربایجان غربی. این آبشار با ارتفاع حدود 30 متر، یکی از زیباترین و پرآب‌ترین آبشارهای ایران است که در دامنه کوه دالامپر واقع شده است. در فصل تابستان به دلیل ذوب برف‌ها، جریان آب قوی و پرصدایی دارد. اطراف آبشار با مناظر طبیعی بکر، جنگل‌های سرسبز و رودخانه‌ها احاطه شده است. این تور شامل پیاده‌روی در طبیعت، بازدید از آبشار و لذت بردن از هوای پاک کوهستانی می‌باشد.",
                RegistrationStart = soledoaklRegStart,
                RegistrationEnd = soledoaklRegEnd,
                TourStart = soledoaklTourStart,
                TourEnd = soledoaklTourEnd,
                MaxParticipants = 15,
                MinAge = 16,
                MaxAge = 60,
                Photos = new List<PhotoSeedData>
                {
                    new PhotoSeedData { ImageUrl = "/tours/sole1.jpg", Caption = "آبشار سوله دوکل", Order = 1 },
                    new PhotoSeedData { ImageUrl = "/tours/sole2.webp", Caption = "طبیعت اطراف آبشار", Order = 2 },
                    new PhotoSeedData { ImageUrl = "/tours/sole3.jpg", Caption = "کوه دوکل", Order = 3 }
                },
                Pricing = new List<PricingSeedData>
                {
                    new PricingSeedData { ParticipantType = ParticipantType.Member, PriceInRials = 2000000 },
                    new PricingSeedData { ParticipantType = ParticipantType.Guest, PriceInRials = 2000000 }
                },
                MemberCapabilities = new List<string> { "has_licences" },
                MemberFeatures = new List<string> { "has_licences" },
                FeatureIds = new List<int> { 1, 2, 4, 5, 6, 7, 11, 12 }
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