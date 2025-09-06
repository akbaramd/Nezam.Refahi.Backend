using System.Text.Json.Serialization;

namespace Nezam.Refahi.Shared.Application.Ports
{
   
  
    public interface IEngineerHttpClient
    {
        /// <summary>
        /// Calls the external API to fetch an engineer by national code.
        /// </summary>
        Task<EngineerDto?> GetByNationalCodeAsync(string nationalCode);
    }

    /// <summary>
    /// Data transfer object representing an engineer.
    /// </summary>
    public class EngineerDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }

        [JsonPropertyName("user")]
        public UserDto User { get; set; } = null!;

        [JsonPropertyName("membershipNumber")]
        public long MembershipNumber { get; set; }

        [JsonPropertyName("membershipMark")]
        public string MembershipMark { get; set; } = string.Empty;

        [JsonPropertyName("membershipEndDate")]
        public string MembershipEndDate { get; set; } = string.Empty;

        [JsonPropertyName("licenseNumber")]
        public long LicenseNumber { get; set; }

        [JsonPropertyName("licenseStatus")]
        public int LicenseStatus { get; set; }

        [JsonPropertyName("engineerSourceId")]
        public int EngineerSourceId { get; set; }

        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; } = string.Empty;

        [JsonPropertyName("fieldCode")]
        public int FieldCode { get; set; }

        [JsonPropertyName("jobAddress")]
        public string JobAddress { get; set; } = string.Empty;

        [JsonPropertyName("jobPhone")]
        public string JobPhone { get; set; } = string.Empty;

        [JsonPropertyName("provinceCode")]
        public int ProvinceCode { get; set; }

        [JsonPropertyName("licenseDate")]
        public string LicenseDate { get; set; } = string.Empty;

        [JsonPropertyName("renewalDate")]
        public string RenewalDate { get; set; } = string.Empty;

        [JsonPropertyName("isDesigner")]
        public bool IsDesigner { get; set; }

        [JsonPropertyName("isSupervisor")]
        public bool IsSupervisor { get; set; }

        [JsonPropertyName("isImplementer")]
        public bool IsImplementer { get; set; }

        [JsonPropertyName("isConsultant")]
        public bool IsConsultant { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("dateModified")]
        public DateTime DateModified { get; set; }
    }

    /// <summary>
    /// Data transfer object representing a user in the system.
    /// </summary>
    public class UserDto
    {
        [JsonPropertyName("nationalNumber")]
        public string NationalNumber { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("lastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("profilePictureUrl")]
        public string? ProfilePictureUrl { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }

        [JsonPropertyName("jobTitle")]
        public string? JobTitle { get; set; }

        [JsonPropertyName("dateModified")]
        public DateTime DateModified { get; set; }

        [JsonPropertyName("certificatePassword")]
        public string? CertificatePassword { get; set; }

        [JsonPropertyName("userAgencies")]
        public List<UserAgencyDto> UserAgencies { get; set; } = new();

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("normalizedUserName")]
        public string NormalizedUserName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("normalizedEmail")]
        public string NormalizedEmail { get; set; } = string.Empty;

        [JsonPropertyName("emailConfirmed")]
        public bool EmailConfirmed { get; set; }


        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumberConfirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [JsonPropertyName("twoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [JsonPropertyName("lockoutEnd")]
        public DateTime? LockoutEnd { get; set; }

        [JsonPropertyName("lockoutEnabled")]
        public bool LockoutEnabled { get; set; }

        [JsonPropertyName("accessFailedCount")]
        public int AccessFailedCount { get; set; }
    }

    /// <summary>
    /// Placeholder for user agency details; extend with actual properties when known.
    /// </summary>
    public class UserAgencyDto
    {
        // Define properties for user agency as needed
    }
}
