using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.Plugin.NezamMohandesi.Constants;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

namespace Nezam.Refahi.Plugin.NezamMohandesi.Seeding;

public class NezamMohandesiBasicDefinitionsSeedContributor : IBasicDefinitionsSeedContributor
{
    private readonly ILogger<NezamMohandesiBasicDefinitionsSeedContributor> _logger;
    private readonly CedoContext _cedoContext;

    public NezamMohandesiBasicDefinitionsSeedContributor(
        ILogger<NezamMohandesiBasicDefinitionsSeedContributor> logger,
        CedoContext cedoContext)
    {
        _logger = logger;
        _cedoContext = cedoContext;
    }

    public Task<List<Features>> SeedFeaturesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding features for {PluginName}", NezamMohandesiConstants.PluginInfo.Name);

        // Get all predefined features from constants
        var features = NezamMohandesiConstants.AllPredefinedFeatures.ToList();

        _logger.LogInformation("Created {Count} features for {PluginName}", features.Count, NezamMohandesiConstants.PluginInfo.Name);
        return Task.FromResult(features);
    }

    public Task<List<Capability>> SeedCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding capabilities for {PluginName}", NezamMohandesiConstants.PluginInfo.Name);

        // Get all predefined capabilities from constants
        var capabilities = NezamMohandesiConstants.AllPredefinedCapabilities.ToList();

        _logger.LogInformation("Created {Count} capabilities for {PluginName}", capabilities.Count, NezamMohandesiConstants.PluginInfo.Name);
        return Task.FromResult(capabilities);
    }

    public async Task<List<Agency>> SeedAgencyiesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding representative offices for {PluginName}", NezamMohandesiConstants.PluginInfo.Name);

        // Get cities from Cedo database
        var cities = await _cedoContext.Cities
            .Include(c => c.Province)
            .Where(c => c.SyncCode.HasValue && !c.IsSatellite) // Only cities with sync codes and not satellite cities
            .OrderBy(c => c.Province.Name)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} cities with sync codes (excluding satellite cities) from Cedo database", cities.Count);

        var offices = new List<Agency>();

        // Create offices for all cities with sync codes (not just major cities)
        foreach (var city in cities)
        {
            var officeCode = $"{city.SyncCode}";
            var officeName = $"دفتر {city.Name} نظام مهندسی";
            var address = $"{city.Name}، {city.Province.Name}";
            var managerName = city.Capital ? "مدیر کل" : "مدیر منطقه";
            var managerPhone = GetCityPhoneCode(city.Name, city.Province.Name);

            var office = new Agency(
                officeCode,
                officeCode, // External code same as office code
                officeName,
                address,
                managerName,
                managerPhone);

            offices.Add(office);

            _logger.LogInformation("Created office: {OfficeName} (Code: {OfficeCode}, SyncCode: {SyncCode}, Capital: {IsCapital})", 
                officeName, officeCode, city.SyncCode, city.Capital);
        }

        _logger.LogInformation("Created {Count} representative offices for {PluginName}", offices.Count, NezamMohandesiConstants.PluginInfo.Name);
        return offices;
    }

    /// <summary>
    /// Gets phone code for a city based on city name and province
    /// </summary>
    private static string GetCityPhoneCode(string cityName, string provinceName)
    {
        // Check city name first
        if (cityName.Contains("تهران"))
            return "021-12345678";
        if (cityName.Contains("ارومیه"))
            return "044-12345678";
        if (cityName.Contains("تبریز"))
            return "041-12345678";
        if (cityName.Contains("اصفهان"))
            return "031-12345678";
        if (cityName.Contains("شیراز"))
            return "071-12345678";
        if (cityName.Contains("مشهد"))
            return "051-12345678";
        if (cityName.Contains("کرج"))
            return "026-12345678";
        if (cityName.Contains("اهواز"))
            return "061-12345678";
        if (cityName.Contains("قم"))
            return "025-12345678";
        if (cityName.Contains("کرمان"))
            return "034-12345678";

        // Check province name as fallback
        return provinceName switch
        {
            var name when name.Contains("تهران") => "021-12345678",
            var name when name.Contains("آذربایجان غربی") => "044-12345678",
            var name when name.Contains("آذربایجان شرقی") => "041-12345678",
            var name when name.Contains("اصفهان") => "031-12345678",
            var name when name.Contains("فارس") => "071-12345678",
            var name when name.Contains("خراسان") => "051-12345678",
            var name when name.Contains("خوزستان") => "061-12345678",
            var name when name.Contains("قم") => "025-12345678",
            var name when name.Contains("کرمان") => "034-12345678",
            _ => "021-12345678" // Default to Tehran
        };
    }
}
