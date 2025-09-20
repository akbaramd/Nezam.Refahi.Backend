using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Plugin.NezamMohandesi.Cedo;
using Nezam.Refahi.Plugin.NezamMohandesi.Constants;
using Nezam.Refahi.Plugin.NezamMohandesi.Helpers;

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
        .ThenInclude(x => x.Grade)
        .Include(x => x.ActivityLicenses.Where(v => v.ExpireDate >= DateTime.UtcNow))
        .ThenInclude(x => x.MemberServices)
        .ThenInclude(x => x.ServiceType)
        .Include(x => x.User)
        .ThenInclude(x => x.UserProfile)
        .FirstOrDefaultAsync(x => x.User.UserProfile != null 
                                  && x.User.UserProfile.NationalCode != null 
                                  && x.User.UserProfile.NationalCode == nationalCode, cancellationToken:cancellationToken);

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
        .FirstOrDefaultAsync(x => x.MembershipCode == membershipCode, cancellationToken:cancellationToken);

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
                      && x.IsActive, cancellationToken:cancellationToken);

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

    var grades = activeLicenses.SelectMany(x => x.MemberServices).Select(x => x.Grade);

    // Build member capabilities (including individual license capabilities)
    var memberCapabilities = BuildMemberCapabilities(member, activeLicenses);

    // Build member roles
    var memberRoles = BuildMemberRoles(member, activeLicenses);

    // Map active licenses to DTOs with individual capabilities per service combination
    var activeLicenseDtos = activeLicenses.Select(al => new ExternalLicenseDto
    {
      LicenseNumber = al.LicenseSerial ?? "",
      IssueDate = al.IssueDate,
      ExtensionDate = al.ExtensionDate ?? DateTime.MinValue,
      ExpireDate = al.ExpireDate,
      IsActive = al.ExpireDate >= DateTime.UtcNow,
      Claims = BuildLicenseCapabilitiesAsClaims(al)
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
      Capabilities = memberCapabilities,
      Birthdate = member.User?.UserProfile?.Birthdate,
      Roles = memberRoles,
      ActiveLicenses = activeLicenseDtos
    };
  }

  private List<ExternalMemberCapabilityDto> BuildMemberCapabilities(Cedo.Models.Member member, List<Cedo.Models.ActivityLicense> activeLicenses)
  {
    var capabilities = new List<ExternalMemberCapabilityDto>();

    // Create individual capabilities for each unique service combination across all licenses
    var uniqueServiceCombinations = new HashSet<string>();

    foreach (var license in activeLicenses)
    {
      foreach (var memberService in license.MemberServices)
      {
        var serviceField = MapServiceFieldToEngineering(memberService.ServiceField?.Title);
        var serviceType = MapServiceTypeToCapability(memberService.ServiceType?.Title);
        var grade = MapGradeToConstant(memberService.Grade?.Title);

        if (!string.IsNullOrEmpty(serviceField) && !string.IsNullOrEmpty(serviceType) && !string.IsNullOrEmpty(grade))
        {
          var capabilityKey = MappingHelper.CapabilityKeys.GenerateKey(serviceField, serviceType, grade);

          // Only add unique capabilities (avoid duplicates across licenses)
          if (uniqueServiceCombinations.Add(capabilityKey))
          {
            var capabilityDto = new ExternalMemberCapabilityDto
            {
              Id = Guid.NewGuid(),
              CapabilityId = Guid.NewGuid(),
              Capability = new ExternalCapabilityDto
              {
                Id = Guid.NewGuid(),
                Key = capabilityKey,
                Name = MappingHelper.CapabilityDisplayNames.GenerateDisplayName(serviceField, serviceType, grade),
                Description = $"Professional capability for {serviceField} {serviceType} at {grade} level from license {license.LicenseSerial}",
                IsActive = true
              },
              IsActive = true,
              ValidFrom = license.IssueDate,
              ValidTo = license.ExpireDate,
              AssignedAt = DateTime.UtcNow,
              AssignedBy = "external-system",
              Notes = $"Derived from license {license.LicenseSerial} - {memberService.ServiceField?.Title} {memberService.ServiceType?.Title} {memberService.Grade?.Title}",
              Claims = BuildIndividualCapabilityClaims(serviceField, serviceType, grade, license)
            };

            capabilities.Add(capabilityDto);
          }
        }
      }
    }

    // If no individual capabilities were created, create a general capability
    if (!capabilities.Any())
    {
      var generalCapability = new ExternalMemberCapabilityDto
      {
        Id = Guid.NewGuid(),
        CapabilityId = Guid.NewGuid(),
        Capability = new ExternalCapabilityDto
        {
          Id = Guid.NewGuid(),
          Key = MappingHelper.CapabilityKeys.HasLicense,
          Name = "General Professional License / پروانه عمومی",
          Description = "General professional license holder",
          IsActive = true
        },
        IsActive = true,
        AssignedAt = DateTime.UtcNow,
        AssignedBy = "external-system",
        Claims = BuildGeneralCapabilityClaims(activeLicenses)
      };

      capabilities.Add(generalCapability);
    }

    return capabilities;
  }

  private List<ExternalClaimDto> BuildCapabilityClaims(List<Cedo.Models.ActivityLicense> activeLicenses, CapabilityInfo capability)
  {
    var claims = new List<ExternalClaimDto>();

    // Service Types - MultiSelect claim (required)
    var serviceCapabilities = activeLicenses
      .SelectMany(al => al.MemberServices)
      .Select(ms => MapServiceTypeToCapability(ms.ServiceType?.Title))
      .Where(cap => !string.IsNullOrEmpty(cap))
      .Cast<string>()
      .Distinct()
      .ToList();

    if (serviceCapabilities.Any())
    {
      claims.Add(new ExternalClaimDto
      {
        ClaimTypeKey = MappingHelper.ClaimTypes.ServiceTypes,
        ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceTypes,
        Value = string.Join(",", serviceCapabilities),
        ValueKind = "MultiSelect",
        IsActive = true,
        AssignedAt = DateTime.UtcNow
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

    if (engineeringFields.Any())
    {
      claims.Add(new ExternalClaimDto
      {
        ClaimTypeKey = MappingHelper.ClaimTypes.ServiceFields,
        ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceFields,
        Value = string.Join(",", engineeringFields),
        ValueKind = "MultiSelect",
        IsActive = true,
        AssignedAt = DateTime.UtcNow
      });
    }

    // License Status - Select claim (required)
    var licenseStatus = activeLicenses.Any() ? MappingHelper.LicenseStatus.HasLicense : MappingHelper.LicenseStatus.NoLicense;
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.LicenseStatus,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.LicenseStatus,
      Value = licenseStatus,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow
    });

    // Grade claim
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.LicenseGrade,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.LicenseGrade,
      Value = capability.Grade,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow
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
        Id = Guid.NewGuid(),
        Key = MappingHelper.RoleKeys.Member,
        Name = MappingHelper.RoleDisplayNames.Names[MappingHelper.RoleKeys.Member],
        Description = "Professional engineer",
        IsActive = true,
        AssignedAt = DateTime.UtcNow
      };
      roles.Add(engineerRole);
    }
    else
    {
      // Employer role - for members without active licenses
      var employerRole = new ExternalRoleDto
      {
        Id = Guid.NewGuid(),
        Key = MappingHelper.RoleKeys.Employer,
        Name = MappingHelper.RoleDisplayNames.Names[MappingHelper.RoleKeys.Employer],
        Description = "Engineering organization or employer",
        IsActive = true,
        AssignedAt = DateTime.UtcNow
      };
      roles.Add(employerRole);
    }

    return roles;
  }

  private List<ExternalClaimDto> BuildLicenseCapabilitiesAsClaims(Cedo.Models.ActivityLicense license)
  {
    var claims = new List<ExternalClaimDto>();

    // Create individual capability for each service combination in the license
    foreach (var memberService in license.MemberServices)
    {
      var serviceField = MapServiceFieldToEngineering(memberService.ServiceField?.Title);
      var serviceType = MapServiceTypeToCapability(memberService.ServiceType?.Title);
      var grade = MapGradeToConstant(memberService.Grade?.Title);

      if (!string.IsNullOrEmpty(serviceField) && !string.IsNullOrEmpty(serviceType) && !string.IsNullOrEmpty(grade))
      {
        // Create a capability claim for this specific service combination
        var capabilityKey = MappingHelper.CapabilityKeys.GenerateKey(serviceField, serviceType, grade);
        var capabilityName = MappingHelper.CapabilityDisplayNames.GenerateDisplayName(serviceField, serviceType, grade);

        claims.Add(new ExternalClaimDto
        {
          ClaimTypeKey = "individual_capability",
          ClaimTypeTitle = "Individual License Capability / قابلیت پروانه فردی",
          Value = capabilityKey,
          ValueKind = "String",
          IsActive = true,
          AssignedAt = DateTime.UtcNow,
          Notes = $"License: {license.LicenseSerial}, Capability: {capabilityName}, Issue: {license.IssueDate:yyyy-MM-dd}, Expire: {license.ExpireDate:yyyy-MM-dd}"
        });

        // Add individual claims for this service combination
        claims.Add(new ExternalClaimDto
        {
          ClaimTypeKey = MappingHelper.ClaimTypes.ServiceFields,
          ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceFields,
          Value = serviceField,
          ValueKind = "Select",
          IsActive = true,
          AssignedAt = DateTime.UtcNow,
          Notes = $"License: {license.LicenseSerial} - Service: {memberService.ServiceField?.Title}"
        });

        claims.Add(new ExternalClaimDto
        {
          ClaimTypeKey = MappingHelper.ClaimTypes.ServiceTypes,
          ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceTypes,
          Value = serviceType,
          ValueKind = "Select",
          IsActive = true,
          AssignedAt = DateTime.UtcNow,
          Notes = $"License: {license.LicenseSerial} - Service: {memberService.ServiceType?.Title}"
        });

        claims.Add(new ExternalClaimDto
        {
          ClaimTypeKey = MappingHelper.ClaimTypes.LicenseGrade,
          ClaimTypeTitle = MappingHelper.ClaimTypeTitles.LicenseGrade,
          Value = grade,
          ValueKind = "Select",
          IsActive = true,
          AssignedAt = DateTime.UtcNow,
          Notes = $"License: {license.LicenseSerial} - Grade: {memberService.Grade?.Title}"
        });
      }
    }

    // Add general license status claim
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.LicenseStatus,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.LicenseStatus,
      Value = MappingHelper.LicenseStatus.HasLicense,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow,
      Notes = $"License: {license.LicenseSerial}, Issue: {license.IssueDate:yyyy-MM-dd}, Expire: {license.ExpireDate:yyyy-MM-dd}"
    });

    
    return claims;
  }

  private List<ExternalClaimDto> BuildIndividualCapabilityClaims(string serviceField, string serviceType, string grade, Cedo.Models.ActivityLicense license)
  {
    var claims = new List<ExternalClaimDto>();

    // Service Field claim
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.ServiceFields,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceFields,
      Value = serviceField,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow,
      Notes = $"License: {license.LicenseSerial}"
    });

    // Service Type claim
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.ServiceTypes,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceTypes,
      Value = serviceType,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow,
      Notes = $"License: {license.LicenseSerial}"
    });

    // Grade claim
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.LicenseGrade,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.LicenseGrade,
      Value = grade,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow,
      Notes = $"License: {license.LicenseSerial}"
    });

    // License Status claim
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.LicenseStatus,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.LicenseStatus,
      Value = MappingHelper.LicenseStatus.HasLicense,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow,
      Notes = $"License: {license.LicenseSerial}, Issue: {license.IssueDate:yyyy-MM-dd}, Expire: {license.ExpireDate:yyyy-MM-dd}"
    });

   

    return claims;
  }

  private List<ExternalClaimDto> BuildGeneralCapabilityClaims(List<Cedo.Models.ActivityLicense> activeLicenses)
  {
    var claims = new List<ExternalClaimDto>();

    // Collect all service fields across all licenses
    var allServiceFields = activeLicenses
      .SelectMany(al => al.MemberServices)
      .Select(ms => MapServiceFieldToEngineering(ms.ServiceField?.Title))
      .Where(field => !string.IsNullOrEmpty(field))
      .Cast<string>()
      .Distinct()
      .ToList();

    if (allServiceFields.Any())
    {
      claims.Add(new ExternalClaimDto
      {
        ClaimTypeKey = MappingHelper.ClaimTypes.ServiceFields,
        ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceFields,
        Value = string.Join(",", allServiceFields),
        ValueKind = "MultiSelect",
        IsActive = true,
        AssignedAt = DateTime.UtcNow
      });
    }

    // Collect all service types across all licenses
    var allServiceTypes = activeLicenses
      .SelectMany(al => al.MemberServices)
      .Select(ms => MapServiceTypeToCapability(ms.ServiceType?.Title))
      .Where(type => !string.IsNullOrEmpty(type))
      .Cast<string>()
      .Distinct()
      .ToList();

    if (allServiceTypes.Any())
    {
      claims.Add(new ExternalClaimDto
      {
        ClaimTypeKey = MappingHelper.ClaimTypes.ServiceTypes,
        ClaimTypeTitle = MappingHelper.ClaimTypeTitles.ServiceTypes,
        Value = string.Join(",", allServiceTypes),
        ValueKind = "MultiSelect",
        IsActive = true,
        AssignedAt = DateTime.UtcNow
      });
    }

    // License Status claim
    claims.Add(new ExternalClaimDto
    {
      ClaimTypeKey = MappingHelper.ClaimTypes.LicenseStatus,
      ClaimTypeTitle = MappingHelper.ClaimTypeTitles.LicenseStatus,
      Value = MappingHelper.LicenseStatus.HasLicense,
      ValueKind = "Select",
      IsActive = true,
      AssignedAt = DateTime.UtcNow
    });

    return claims;
  }

  private string? MapServiceFieldToEngineering(string? serviceFieldTitle)
  {
    if (string.IsNullOrEmpty(serviceFieldTitle)) return null;

    return MappingHelper.ExternalMappings.ServiceFieldMappings.TryGetValue(serviceFieldTitle, out var mapped) ? mapped : null;
  }

  private string? MapServiceTypeToCapability(string? serviceTypeTitle)
  {
    if (string.IsNullOrEmpty(serviceTypeTitle)) return null;

    return MappingHelper.ExternalMappings.ServiceTypeMappings.TryGetValue(serviceTypeTitle, out var mapped) ? mapped : null;
  }

  private CapabilityInfo? GetBestCapability(List<Cedo.Models.ActivityLicense> activeLicenses)
  {
    if (!activeLicenses.Any()) return null;

    var capabilities = new List<CapabilityInfo>();

    // Extract all possible capabilities from active licenses
    foreach (var license in activeLicenses)
    {
      foreach (var memberService in license.MemberServices)
      {
        var field = MapServiceFieldToEngineering(memberService.ServiceField?.Title);
        var serviceType = MapServiceTypeToCapability(memberService.ServiceType?.Title);
        var grade = MapGradeToConstant(memberService.Grade?.Title);

        if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(serviceType) && !string.IsNullOrEmpty(grade))
        {
          var capabilityKey = MappingHelper.CapabilityKeys.GenerateKey(field, serviceType, grade);
          capabilities.Add(new CapabilityInfo
          {
            Key = capabilityKey,
            Field = field,
            ServiceType = serviceType,
            Grade = grade,
            GradeHierarchy = MappingHelper.Grades.Hierarchy.GetValueOrDefault(grade, 0)
          });
        }
      }
    }

    if (!capabilities.Any()) return null;

    // Return the capability with the highest grade hierarchy
    // If multiple capabilities have the same highest grade, return the first one
    return capabilities
      .OrderByDescending(c => c.GradeHierarchy)
      .ThenBy(c => c.Field)
      .ThenBy(c => c.ServiceType)
      .First();
  }

  private string? MapGradeToConstant(string? gradeTitle)
  {
    if (string.IsNullOrEmpty(gradeTitle)) return null;

    return MappingHelper.ExternalMappings.GradeMappings.TryGetValue(gradeTitle, out var mapped) ? mapped : null;
  }

  private class CapabilityInfo
  {
    public string Key { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public int GradeHierarchy { get; set; }
  }
}
