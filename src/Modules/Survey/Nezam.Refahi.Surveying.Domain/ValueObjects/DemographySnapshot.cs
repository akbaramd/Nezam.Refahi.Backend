using MCA.SharedKernel.Domain;

namespace Nezam.Refahi.Surveying.Domain.ValueObjects;

/// <summary>
/// Value object representing demographic snapshot for survey responses
/// Uses controlled key registry for BI reporting and indexing
/// </summary>
public sealed class DemographySnapshot : ValueObject
{
    /// <summary>
    /// Version of the demography schema for tracking changes
    /// </summary>
    public int SchemaVersion { get; private set; } = 1;

    /// <summary>
    /// Controlled demographic data using whitelisted keys
    /// </summary>
    public Dictionary<string, string> Data { get; private set; }

    /// <summary>
    /// Allowed demographic keys for controlled data entry
    /// </summary>
    public static readonly HashSet<string> AllowedKeys = new()
    {
        "DisciplineCode",      // رشته تحصیلی
        "ProvinceCode",        // استان
        "LicenseGradeCode",    // درجه پروانه
        "SeniorityBand",       // سابقه کاری
        "EducationLevel",      // سطح تحصیلات
        "AgeGroup",           // گروه سنی
        "Gender",             // جنسیت
        "OrganizationType",   // نوع سازمان
        "PositionLevel"       // سطح پست
    };

    /// <summary>
    /// Creates a demographic snapshot with controlled keys
    /// </summary>
    public DemographySnapshot(Dictionary<string, string> data, int schemaVersion = 1)
    {
        SchemaVersion = schemaVersion;
        Data = ValidateAndCleanData(data ?? throw new ArgumentNullException(nameof(data)));
    }

    /// <summary>
    /// Creates an empty demographic snapshot
    /// </summary>
    public static DemographySnapshot Empty()
    {
        return new DemographySnapshot(new Dictionary<string, string>());
    }

    /// <summary>
    /// Adds or updates a demographic field with key validation
    /// </summary>
    public DemographySnapshot WithField(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        if (!AllowedKeys.Contains(key))
            throw new ArgumentException($"Key '{key}' is not in the allowed demographic keys", nameof(key));

        var newData = new Dictionary<string, string>(Data)
        {
            [key] = value
        };

        return new DemographySnapshot(newData, SchemaVersion);
    }

    /// <summary>
    /// Gets a demographic field value
    /// </summary>
    public string? GetField(string key)
    {
        return Data.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Checks if a demographic field exists
    /// </summary>
    public bool HasField(string key)
    {
        return Data.ContainsKey(key);
    }

    /// <summary>
    /// Gets all demographic fields
    /// </summary>
    public IReadOnlyDictionary<string, string> GetAllFields()
    {
        return Data.AsReadOnly();
    }

    /// <summary>
    /// Validates and cleans demographic data against allowed keys
    /// </summary>
    private static Dictionary<string, string> ValidateAndCleanData(Dictionary<string, string> data)
    {
        var cleanedData = new Dictionary<string, string>();
        
        foreach (var kvp in data)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key))
                continue;

            if (!AllowedKeys.Contains(kvp.Key))
                throw new ArgumentException($"Key '{kvp.Key}' is not in the allowed demographic keys");

            cleanedData[kvp.Key] = kvp.Value ?? string.Empty;
        }

        return cleanedData;
    }

    private DemographySnapshot()
    {
        Data = new Dictionary<string, string>();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SchemaVersion;
        foreach (var kvp in Data.OrderBy(x => x.Key))
        {
            yield return kvp.Key;
            yield return kvp.Value;
        }
    }
}
