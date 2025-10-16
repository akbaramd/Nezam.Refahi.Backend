using System.Text.Json;
using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Surveying.Domain.ValueObjects;

/// <summary>
/// Value object representing audience filter for survey targeting
/// Uses DSL and Code-based filtering based on stable Definition Codes
/// </summary>
public sealed class AudienceFilter : ValueObject
{
    /// <summary>
    /// DSL filter expression using stable Definition Codes
    /// </summary>
    public string FilterExpression { get; private set; }

    /// <summary>
    /// Version of the filter expression schema
    /// </summary>
    public int FilterVersion { get; private set; } = 1;

    /// <summary>
    /// Creates an audience filter with a DSL expression
    /// </summary>
    public AudienceFilter(string filterExpression, int filterVersion = 1)
    {
        if (string.IsNullOrWhiteSpace(filterExpression))
            throw new ArgumentException("Filter expression cannot be empty", nameof(filterExpression));

        FilterExpression = filterExpression;
        FilterVersion = filterVersion;
    }

    /// <summary>
    /// Creates an audience filter based on Definition Codes
    /// </summary>
    public static AudienceFilter FromDefinitionCodes(
        List<string> requiredFeatureCodes, 
        List<string> requiredCapabilityCodes,
        List<string>? excludedFeatureCodes = null,
        List<string>? excludedCapabilityCodes = null)
    {
        var criteria = new Dictionary<string, object>
        {
            ["requiredFeatures"] = requiredFeatureCodes ?? new List<string>(),
            ["requiredCapabilities"] = requiredCapabilityCodes ?? new List<string>(),
            ["excludedFeatures"] = excludedFeatureCodes ?? new List<string>(),
            ["excludedCapabilities"] = excludedCapabilityCodes ?? new List<string>()
        };

        var filterExpression = JsonSerializer.Serialize(criteria);
        return new AudienceFilter(filterExpression);
    }

    /// <summary>
    /// Creates an audience filter for specific member groups using Definition Codes
    /// </summary>
    public static AudienceFilter ForMemberGroups(List<string> memberGroupCodes)
    {
        var criteria = new Dictionary<string, object>
        {
            ["memberGroups"] = memberGroupCodes ?? new List<string>()
        };

        var filterExpression = JsonSerializer.Serialize(criteria);
        return new AudienceFilter(filterExpression);
    }

    /// <summary>
    /// Gets the filter criteria as a dictionary
    /// </summary>
    public Dictionary<string, object> GetCriteria()
    {
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(FilterExpression) ?? new Dictionary<string, object>();
        }
        catch (JsonException)
        {
            return new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Gets required feature codes from the filter
    /// </summary>
    public List<string> GetRequiredFeatureCodes()
    {
        var criteria = GetCriteria();
        if (criteria.TryGetValue("requiredFeatures", out var features) && features is JsonElement element)
        {
            return element.EnumerateArray().Select(x => x.GetString()).Where(x => !string.IsNullOrEmpty(x)).ToList()!;
        }
        return new List<string>();
    }

    /// <summary>
    /// Gets required capability codes from the filter
    /// </summary>
    public List<string> GetRequiredCapabilityCodes()
    {
        var criteria = GetCriteria();
        if (criteria.TryGetValue("requiredCapabilities", out var capabilities) && capabilities is JsonElement element)
        {
            return element.EnumerateArray().Select(x => x.GetString()).Where(x => !string.IsNullOrEmpty(x)).ToList()!;
        }
        return new List<string>();
    }

    /// <summary>
    /// Checks if the filter is empty (no restrictions)
    /// </summary>
    public bool IsEmpty()
    {
        var criteria = GetCriteria();
        return !criteria.Any() || criteria.Values.All(v => v is JsonElement element && element.ValueKind == JsonValueKind.Array && element.GetArrayLength() == 0);
    }

    private AudienceFilter() 
    {
        FilterExpression = string.Empty;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FilterExpression;
        yield return FilterVersion;
    }
}
