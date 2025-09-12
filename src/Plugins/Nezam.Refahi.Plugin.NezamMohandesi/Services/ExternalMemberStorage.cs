using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo;
using Nezam.Refahi.Plugin.NezamMohandesi.Constants;

namespace Nezam.Refahi.Plugin.NezamMohandesi.Services;

public class ExternalMemberStorage : IExternalMemberStorage
{
  private readonly ILogger<ExternalMemberStorage> _logger;
  private readonly CedoContext _cedoContext;

  public ExternalMemberStorage( ILogger<ExternalMemberStorage> logger, CedoContext cedoContext)
  {
    _logger = logger;
    _cedoContext = cedoContext;
  }


 

  public async Task<ExternalMemberResponseDto?> GetMemberByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Searching for member with national code: {NationalCode}", nationalCode);
      
      var member = await _cedoContext
        .Members
        .Include(x => x.ActivityLicenses.Where(v => v.ExpireDate >= DateTime.UtcNow))
        .ThenInclude(x => x.MemberServices)
        .ThenInclude(x => x.ServiceField)
        .Include(x => x.ActivityLicenses.Where(v => v.ExpireDate >= DateTime.UtcNow))
        .ThenInclude(x => x.MemberServices)
        .ThenInclude(x => x.ServiceType)
        .Include(x => x.User)
        .ThenInclude(x => x.UserProfile)
        .FirstOrDefaultAsync(x => x.User.UserProfile != null 
                                  && x.User.UserProfile.NationalCode != null 
                                  && x.User.UserProfile.NationalCode == nationalCode, cancellationToken);

      if (member == null)
      {
        _logger.LogInformation("Member not found for national code: {NationalCode}", nationalCode);
        return null;
      }

      return MapToExternalMemberResponse(member);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving member by national code: {NationalCode}", nationalCode);
      return null;
    }
  }

  public async Task<ExternalMemberResponseDto?> GetMemberByMembershipCodeAsync(string membershipCode, CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Searching for member with membership code: {MembershipCode}", membershipCode);
      
      var member = await _cedoContext
        .Members
        .Include(x => x.ActivityLicenses.Where(v => v.ExpireDate >= DateTime.UtcNow))
        .ThenInclude(x => x.MemberServices)
        .Include(x => x.User)
        .ThenInclude(x => x.UserProfile)
        .FirstOrDefaultAsync(x => x.MembershipCode == membershipCode, cancellationToken);

      if (member == null)
      {
        _logger.LogInformation("Member not found for membership code: {MembershipCode}", membershipCode);
        return null;
      }

      return MapToExternalMemberResponse(member);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error retrieving member by membership code: {MembershipCode}", membershipCode);
      return null;
    }
  }

  public async Task<bool> ValidateMemberAsync(string nationalCode, string membershipCode, CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Validating member with national code: {NationalCode} and membership code: {MembershipCode}", 
        nationalCode, membershipCode);
      
      var exists = await _cedoContext
        .Members
        .AnyAsync(x => x.MembershipCode == membershipCode 
                      && x.User.UserProfile != null 
                      && x.User.UserProfile.NationalCode == nationalCode
                      && x.IsActive, cancellationToken);

      _logger.LogInformation("Member validation result: {IsValid}", exists);
      return exists;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error validating member with national code: {NationalCode} and membership code: {MembershipCode}", 
        nationalCode, membershipCode);
      return false;
    }
  }

  public async Task<IReadOnlyList<ExternalMemberResponseDto>> SearchMembersAsync(ExternalMemberSearchCriteria criteria, CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Searching members with criteria");
      
      var query = _cedoContext.Members
        .Include(x => x.ActivityLicenses.Where(v => v.ExpireDate >= DateTime.UtcNow))
        .ThenInclude(x => x.MemberServices)
        .Include(x => x.User)
        .ThenInclude(x => x.UserProfile)
        .AsQueryable();

      if (!string.IsNullOrWhiteSpace(criteria.FirstName))
      {
        query = query.Where(x => x.User.UserProfile != null && 
                                x.User.UserProfile.FirstName!.Contains(criteria.FirstName));
      }

      if (!string.IsNullOrWhiteSpace(criteria.LastName))
      {
        query = query.Where(x => x.User.UserProfile != null && 
                                x.User.UserProfile.LastName!.Contains(criteria.LastName));
      }

      if (!string.IsNullOrWhiteSpace(criteria.NationalCode))
      {
        query = query.Where(x => x.User.UserProfile != null && 
                                x.User.UserProfile.NationalCode == criteria.NationalCode);
      }

      if (!string.IsNullOrWhiteSpace(criteria.MembershipCode))
      {
        query = query.Where(x => x.MembershipCode == criteria.MembershipCode);
      }

      if (!string.IsNullOrWhiteSpace(criteria.PhoneNumber))
      {
        query = query.Where(x => x.User.PhoneNumber != null && 
                                x.User.PhoneNumber.Contains(criteria.PhoneNumber));
      }

      if (!string.IsNullOrWhiteSpace(criteria.Email))
      {
        query = query.Where(x => x.User.Email != null && 
                                x.User.Email.Contains(criteria.Email));
      }

      if (criteria.IsActive.HasValue)
      {
        query = query.Where(x => x.IsActive == criteria.IsActive.Value);
      }

      var totalCount = await query.CountAsync(cancellationToken);
      var skip = (criteria.PageNumber - 1) * criteria.PageSize;

      var members = await query
        .Skip(skip)
        .Take(criteria.PageSize)
        .ToListAsync(cancellationToken);

      _logger.LogInformation("Found {Count} members out of {Total} total", members.Count, totalCount);

      return members.Select(MapToExternalMemberResponse).ToList();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error searching members");
      return new List<ExternalMemberResponseDto>();
    }
  }

  private ExternalMemberResponseDto MapToExternalMemberResponse(Cedo.Models.Member member)
  {
    // Get only active licenses (ExpireDate >= DateTime.UtcNow is already filtered in the query)
    var activeLicenses = member.ActivityLicenses
      .Where(al => al.ExpireDate >= DateTime.UtcNow)
      .ToList();

    // Build member claims
    var memberClaims = BuildMemberClaims(member, activeLicenses);
    
    // Build member roles
    var memberRoles = BuildMemberRoles(member, activeLicenses);

    // Map active licenses to DTOs with claims
    var activeLicenseDtos = activeLicenses.Select(al => new ExternalLicenseDto
    {
      LicenseNumber = al.LicenseSerial ?? "",
      IssueDate = al.IssueDate,
      ExtensionDate = al.ExtensionDate ?? DateTime.MinValue,
      ExpireDate = al.ExpireDate,
      IsActive = al.ExpireDate >= DateTime.UtcNow,
      Claims = BuildLicenseClaims(al)
    }).ToList();

    return new ExternalMemberResponseDto
    {
      FirstName = member.User?.UserProfile?.FirstName ?? "",
      LastName = member.User?.UserProfile?.LastName ?? "",
      NationalCode = member.User?.UserProfile?.NationalCode ?? "",
      MembershipCode = member.MembershipCode,
      PhoneNumber = member.User?.PhoneNumber ?? "",
      Email = member.User?.Email ?? "",
      IsActive = member.IsActive,
      Claims = memberClaims,
      Roles = memberRoles,
      ActiveLicenses = activeLicenseDtos
    };
  }

  private List<ExternalClaimDto> BuildMemberClaims(Cedo.Models.Member member, List<Cedo.Models.ActivityLicense> activeLicenses)
  {
    var claims = new List<ExternalClaimDto>();

    // Service Types - MultiSelect claim (required)
    var serviceCapabilities = activeLicenses
      .SelectMany(al => al.MemberServices)
      .Select(ms => MapServiceTypeToCapability(ms.ServiceType?.Title))
      .Where(capability => !string.IsNullOrEmpty(capability))
      .Cast<string>()
      .Distinct()
      .ToList();

    foreach (var capability in serviceCapabilities)
    {
      claims.Add(new ExternalClaimDto
      {
        Type = NezamMohandesiConstants.ClaimTypes.ServiceTypes,
        Title = NezamMohandesiConstants.ServiceTypes.DisplayNames[capability],
        Value = capability, // MultiSelect values as comma-separated
        Category = NezamMohandesiConstants.ClaimCategories.Capabilities
      });
    }
    
      

    // Service Fields (Engineering Fields) - MultiSelect claim (required)
    var engineeringFields = activeLicenses
      .SelectMany(al => al.MemberServices)
      .Select(ms => MapServiceFieldToEngineering(ms.ServiceField?.Title))
      .Where(field => !string.IsNullOrEmpty(field))
      .Cast<string>()
      .Distinct()
      .ToList();

    foreach (var field in engineeringFields)
    {claims.Add(new ExternalClaimDto
      {
        Type = NezamMohandesiConstants.ClaimTypes.ServiceFields,
        Title = NezamMohandesiConstants.ServiceFields.DisplayNames[field],
        Value = field, // MultiSelect values as comma-separated
        Category = NezamMohandesiConstants.ClaimCategories.Professional
      });
      
    }
      
    

    // License Status - Select claim (required)
    var licenseStatus = activeLicenses.Any() ? NezamMohandesiConstants.LicenseStatus.HasLicense : NezamMohandesiConstants.LicenseStatus.NoLicense;
    claims.Add(new ExternalClaimDto
    {
      Type = NezamMohandesiConstants.ClaimTypes.LicenseStatus,
      Value = licenseStatus,
      Category = NezamMohandesiConstants.ClaimCategories.License
    });

    return claims;
  }

  private List<ExternalRoleDto> BuildMemberRoles(Cedo.Models.Member member, List<Cedo.Models.ActivityLicense> activeLicenses)
  {
    var roles = new List<ExternalRoleDto>();

    // Engineer role - for members with active licenses
    if (activeLicenses.Any())
    {
      var engineerRole = new ExternalRoleDto
      {
        Key = NezamMohandesiConstants.RoleKeys.Engineer,
        Name = NezamMohandesiConstants.RoleDisplayNames.Names[NezamMohandesiConstants.RoleKeys.Engineer],
        Description = "Professional engineer",
        Claims = new List<ExternalClaimDto>
        {
          new()
          {
            Type = NezamMohandesiConstants.ClaimTypes.LicenseStatus,
            Value = NezamMohandesiConstants.LicenseStatus.HasLicense,
            Category = NezamMohandesiConstants.ClaimCategories.License
          }
        }
      };
      roles.Add(engineerRole);
    }
    else
    {
      // Employer role - for members without active licenses
      var employerRole = new ExternalRoleDto
      {
        Key = NezamMohandesiConstants.RoleKeys.Employer,
        Name = NezamMohandesiConstants.RoleDisplayNames.Names[NezamMohandesiConstants.RoleKeys.Employer],
        Description = "Engineering organization or employer",
        Claims = new List<ExternalClaimDto>
        {
          new()
          {
            Type = NezamMohandesiConstants.ClaimTypes.LicenseStatus,
            Value = NezamMohandesiConstants.LicenseStatus.NoLicense,
            Category = NezamMohandesiConstants.ClaimCategories.License
          }
        }
      };
      roles.Add(employerRole);
    }

    return roles;
  }

  private List<ExternalClaimDto> BuildLicenseClaims(Cedo.Models.ActivityLicense license)
  {
    var claims = new List<ExternalClaimDto>();

    // License Status - simplified to has_license since license is active
    claims.Add(new ExternalClaimDto
    {
      Type = NezamMohandesiConstants.ClaimTypes.LicenseStatus,
      Value = NezamMohandesiConstants.LicenseStatus.HasLicense,
      Title = NezamMohandesiConstants.LicenseStatus.DisplayNames[NezamMohandesiConstants.LicenseStatus.HasLicense],
      Category = NezamMohandesiConstants.ClaimCategories.License,
      Options = new Dictionary<string, string>
      {
        { "license_number", license.LicenseSerial ?? "" },
        { "issue_date", license.IssueDate.ToString("yyyy-MM-dd") },
        { "expire_date", license.ExpireDate.ToString("yyyy-MM-dd") },
        { "extension_date", license.ExtensionDate?.ToString("yyyy-MM-dd") ?? "" }
      }
    });

    // Service Types - MultiSelect claim
    var serviceCapabilities = license.MemberServices
      .Select(ms => MapServiceTypeToCapability(ms.ServiceType?.Title))
      .Where(capability => !string.IsNullOrEmpty(capability))
      .Distinct()
      .ToList();

    if (serviceCapabilities.Any())
    {
      claims.Add(new ExternalClaimDto
      {
        Type = NezamMohandesiConstants.ClaimTypes.ServiceTypes,
        Value = string.Join(",", serviceCapabilities), // MultiSelect values as comma-separated
        Category = NezamMohandesiConstants.ClaimCategories.Capabilities
      });
    }

    // Service Fields - MultiSelect claim
    var engineeringFields = license.MemberServices
      .Select(ms => MapServiceFieldToEngineering(ms.ServiceField?.Title))
      .Where(field => !string.IsNullOrEmpty(field))
      .Distinct()
      .ToList();

    if (engineeringFields.Any())
    {
      claims.Add(new ExternalClaimDto
      {
        Type = NezamMohandesiConstants.ClaimTypes.ServiceFields,
        Value = string.Join(",", engineeringFields), // MultiSelect values as comma-separated
        Category = NezamMohandesiConstants.ClaimCategories.Professional
      });
    }

    return claims;
  }

  private string? MapServiceFieldToEngineering(string? serviceFieldTitle)
  {
    if (string.IsNullOrEmpty(serviceFieldTitle)) return null;

    return NezamMohandesiConstants.ExternalMappings.ServiceFieldMappings.TryGetValue(serviceFieldTitle, out var mapped) ? mapped : null;
  }

  private string? MapServiceTypeToCapability(string? serviceTypeTitle)
  {
    if (string.IsNullOrEmpty(serviceTypeTitle)) return null;

    return NezamMohandesiConstants.ExternalMappings.ServiceTypeMappings.TryGetValue(serviceTypeTitle, out var mapped) ? mapped : null;
  }
}
