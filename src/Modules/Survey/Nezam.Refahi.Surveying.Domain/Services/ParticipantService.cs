using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Domain.Services;

/// <summary>
/// Domain service for participant hash generation and demography snapshot creation
/// Single responsibility: handling participant identification and demographic data
/// </summary>
public class ParticipantService
{
    /// <summary>
    /// تولید هش شرکت‌کننده برای نظرسنجی‌های ناشناس
    /// </summary>
    public string GenerateParticipantHash(string opaqueToken, Guid surveyId, string salt)
    {
        if (string.IsNullOrWhiteSpace(opaqueToken))
            throw new ArgumentException("Opaque token cannot be empty", nameof(opaqueToken));

        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt cannot be empty", nameof(salt));

        var input = $"{opaqueToken}:{surveyId}:{salt}";
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// ایجاد تصویر دموگرافیک از داده‌های عضو
    /// </summary>
    public DemographySnapshot CreateDemographySnapshot(Dictionary<string, object> memberData)
    {
        var snapshotData = new Dictionary<string, string>();

        // Map standard demographic fields to allowed keys
        var fieldMapping = new Dictionary<string, string>
        {
            ["Discipline"] = "DisciplineCode",
            ["Province"] = "ProvinceCode", 
            ["LicenseGrade"] = "LicenseGradeCode",
            ["AgeGroup"] = "AgeGroup",
            ["Gender"] = "Gender"
        };

        foreach (var (sourceField, targetField) in fieldMapping)
        {
            if (memberData.TryGetValue(sourceField, out var value))
            {
                snapshotData[targetField] = value?.ToString() ?? string.Empty;
            }
        }

        return new DemographySnapshot(snapshotData);
    }
}
