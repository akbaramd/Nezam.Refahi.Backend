 using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.Membership.Application.Mappers;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using MCA.SharedKernel.Application.Contracts;

namespace Nezam.Refahi.Membership.Application.Services;

/// <summary>
/// Implementation of membership service for inter-context communication
/// Provides controlled access to member data following DDD principles
/// </summary>
public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMemberRoleRepository _memberRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMemberCapabilityRepository _memberCapabilityRepository;
    private readonly IMemberFeatureRepository _memberFeatureRepository;
    private readonly IExternalMemberStorage _externalEngineerService;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly IBasicDefinitionsCacheService _cacheService;
    private readonly IMapper<Member, MemberDetailDto> _memberDetailMapper;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        IMemberRepository memberRepository,
        IExternalMemberStorage externalEngineerService,
        IMembershipUnitOfWork unitOfWork,
        ILogger<MemberService> logger,
        IMemberRoleRepository memberRoleRepository,
        IRoleRepository roleRepository,
        IMemberCapabilityRepository memberCapabilityRepository,
        IMemberFeatureRepository memberFeatureRepository,
        IBasicDefinitionsCacheService cacheService,
        IMapper<Member, MemberDetailDto> memberDetailMapper)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _externalEngineerService = externalEngineerService ?? throw new ArgumentNullException(nameof(externalEngineerService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memberRoleRepository = memberRoleRepository;
        _roleRepository = roleRepository;
        _memberCapabilityRepository = memberCapabilityRepository ?? throw new ArgumentNullException(nameof(memberCapabilityRepository));
        _memberFeatureRepository = memberFeatureRepository ?? throw new ArgumentNullException(nameof(memberFeatureRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _memberDetailMapper = memberDetailMapper ?? throw new ArgumentNullException(nameof(memberDetailMapper));
    }

    public async Task<MemberDto?> GetMemberByNationalCodeAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("National code is null or empty");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting member by national code: {NationalCode}", nationalCode);

            // Always get fresh data from external storage first
            var externalMember = await _externalEngineerService.GetMemberByNationalCodeAsync(nationalCode.Value);
            if (externalMember == null)
            {
                _logger.LogDebug("Member not found in external storage: {NationalCode}", nationalCode);
                return null;
            }

            // Check if member exists locally
            var existingMember = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            
            if (existingMember != null)
            {
                _logger.LogDebug("Member exists locally, syncing with external data: {NationalCode}", nationalCode);
                
                // Update existing member data from external storage
                await SyncExistingMemberWithExternalDataAsync(existingMember, externalMember);
                
                // Convert to DTO
                var memberDto = new MemberDto
                {
                    Id = existingMember.Id,
                    NationalCode = existingMember.NationalCode?.Value ?? string.Empty,
                    MembershipNumber = existingMember.MembershipNumber,
                    FirstName = existingMember.FullName.FirstName,
                    LastName = existingMember.FullName.LastName,
                    Email = existingMember.Email,
                    PhoneNumber = existingMember.PhoneNumber?.Value,
                    IsActive = true,
                    IsSpecial = existingMember.IsSpecial,
                    CreatedAt = existingMember.CreatedAt,
                    ModifiedAt = existingMember.LastModifiedAt
                };

                _logger.LogDebug("Successfully synced existing member with external data: {NationalCode}", nationalCode);
                return memberDto;
            }
            else
            {
                _logger.LogDebug("Member not found locally, creating new member from external data: {NationalCode}", nationalCode);
                
                // Create new member entity from external data
                var newMember = new Member(
                    Guid.NewGuid(), // externalUserId - temporary, will be updated when user is created
                    externalMember.MembershipCode ?? string.Empty, // membershipNumber
                    new NationalId(externalMember.NationalCode ?? string.Empty), // nationalCode
                    new FullName(externalMember.FirstName, externalMember.LastName), // fullName
                    string.IsNullOrWhiteSpace(externalMember.Email) ? new Email($"{externalMember.NationalCode}@global.com") : new Email(externalMember.Email), // email
                    externalMember.PhoneNumber != null ? new PhoneNumber(externalMember.PhoneNumber) : new PhoneNumber(string.Empty), // phoneNumber
                    externalMember.Birthdate // birthDate
                );

                // Save member to local repository
                await _memberRepository.AddAsync(newMember);
                await _unitOfWork.SaveChangesAsync();

                // Save capabilities and features from external member data
                await SyncMemberCapabilitiesAndFeaturesAsync(newMember, externalMember);

                await _unitOfWork.SaveChangesAsync();

                // Convert to DTO
                var memberDto = new MemberDto
                {
                    Id = newMember.Id,
                    NationalCode = newMember.NationalCode?.Value ?? string.Empty,
                    MembershipNumber = newMember.MembershipNumber,
                    FirstName = newMember.FullName.FirstName,
                    LastName = newMember.FullName.LastName,
                    Email = newMember.Email,
                    PhoneNumber = newMember.PhoneNumber?.Value,
                    IsActive = true,
                    IsSpecial = newMember.IsSpecial,
                    CreatedAt = newMember.CreatedAt,
                    ModifiedAt = newMember.LastModifiedAt
                };

                _logger.LogDebug("Successfully created new member from external data: {NationalCode}", nationalCode);
                return memberDto;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member by national code: {NationalCode}", nationalCode);
            return null;
        }
    }

    public async Task<MemberDetailDto?> GetMemberDetailByNationalCodeAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("National code is null or empty");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting member detail by national code: {NationalCode}", nationalCode);

            // Get member from repository
            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogDebug("Member not found for national code: {NationalCode}", nationalCode);
                return null;
            }

            // Use mapper to convert to detail DTO
            var memberDetailDto = await _memberDetailMapper.MapAsync(member);
            
            _logger.LogDebug("Successfully retrieved member detail for {NationalCode}", nationalCode);
            return memberDetailDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member detail by national code: {NationalCode}", nationalCode);
            return null;
        }
    }

    public async Task<IEnumerable<MemberDto>> GetMembersByNationalCodesAsync(IEnumerable<NationalId> nationalCodes)
    {
        if (nationalCodes == null || !nationalCodes.Any())
        {
            _logger.LogWarning("National codes list is null or empty");
            return new List<MemberDto>();
        }

        try
        {
            _logger.LogDebug("Getting members by {Count} national codes", nationalCodes.Count());

            var memberDtos = new List<MemberDto>();
            var nationalCodeList = nationalCodes.ToList();

            // Process each national code - sync with external storage first
            foreach (var nationalCode in nationalCodeList)
            {
                try
                {
                    // Use the same sync logic as GetMemberByNationalCodeAsync
                    var memberDto = await GetMemberByNationalCodeAsync(nationalCode);
                    if (memberDto != null)
                    {
                        memberDtos.Add(memberDto);
                    }
                    else
                    {
                        _logger.LogDebug("Member not found for national code: {NationalCode}", nationalCode.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync member {NationalCode} from external storage", nationalCode.Value);
                }
            }

            _logger.LogDebug("Successfully retrieved {Count} members out of {TotalCount} requested", memberDtos.Count, nationalCodeList.Count);
            return memberDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting members by national codes");
            return new List<MemberDto>();
        }
    }

    public async Task<MemberDto?> GetMemberByMembershipNumberAsync(string membershipNumber)
    {
        if (string.IsNullOrWhiteSpace(membershipNumber))
        {
            _logger.LogWarning("Membership number is null or empty");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting member by membership number: {MembershipNumber}", membershipNumber);

            var member = await _memberRepository.GetByMembershipNumberAsync(membershipNumber);
            if (member == null)
            {
                _logger.LogDebug("Member not found for membership number: {MembershipNumber}", membershipNumber);
                return null;
            }

            var memberDto = new MemberDto
            {
                Id = member.Id,
                NationalCode = member.NationalCode?.Value ?? string.Empty,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName.FirstName,
                LastName = member.FullName.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber?.Value,
                IsActive = true,
                IsSpecial = member.IsSpecial,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.LastModifiedAt
            };

            _logger.LogDebug("Successfully retrieved member {MembershipNumber}", membershipNumber);
            return memberDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member by membership number: {MembershipNumber}", membershipNumber);
            return null;
        }
    }

    public async Task<MemberDto?> GetMemberByPhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            _logger.LogWarning("Phone number is null or empty");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting member by phone number: {PhoneNumber}", phoneNumber);

            var member = await _memberRepository.GetByPhoneNumberAsync(new PhoneNumber(phoneNumber));
            if (member == null)
            {
                _logger.LogDebug("Member not found for phone number: {PhoneNumber}", phoneNumber);
                return null;
            }

            var memberDto = new MemberDto
            {
                Id = member.Id,
                NationalCode = member.NationalCode?.Value ?? string.Empty,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName.FirstName,
                LastName = member.FullName.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber?.Value,
                IsActive = true,
                IsSpecial = member.IsSpecial,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.LastModifiedAt
            };

            _logger.LogDebug("Successfully retrieved member {PhoneNumber}", phoneNumber);
            return memberDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member by phone number: {PhoneNumber}", phoneNumber);
            return null;
        }
    }

    public async Task<MemberDto?> GetMemberByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Email is null or empty");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting member by email: {Email}", email);

            var member = await _memberRepository.GetByEmailAsync(email);
            if (member == null)
            {
                _logger.LogDebug("Member not found for email: {Email}", email);
                return null;
            }

            var memberDto = new MemberDto
            {
                Id = member.Id,
                NationalCode = member.NationalCode?.Value ?? string.Empty,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName.FirstName,
                LastName = member.FullName.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber?.Value,
                    IsActive = true,
                    IsSpecial = member.IsSpecial,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.LastModifiedAt
            };

            _logger.LogDebug("Successfully retrieved member {Email}", email);
            return memberDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member by email: {Email}", email);
            return null;
        }
    }

    public async Task<MemberDto?> GetMemberByExternalIdAsync(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
        {
            _logger.LogWarning("External ID is null or empty");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting member by external ID: {ExternalId}", externalId);

            var member = await _memberRepository.FindOneAsync(x => x.ExternalUserId == new Guid(externalId));
            if (member == null)
            {
                _logger.LogDebug("Member not found for external ID: {ExternalId}", externalId);
                return null;
            }

            var memberDto = new MemberDto
            {
                Id = member.Id,
                NationalCode = member.NationalCode?.Value ?? string.Empty,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName.FirstName,
                LastName = member.FullName.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber?.Value,
                IsActive = true,
                IsSpecial = member.IsSpecial,
                    CreatedAt = member.CreatedAt,
                ModifiedAt = member.LastModifiedAt
            };

            _logger.LogDebug("Successfully retrieved member {ExternalId}", externalId);
            return memberDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member by external ID: {ExternalId}", externalId);
            return null;
        }
    }

    public async Task<bool> IsMemberExistsByNationalCodeAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("National code is null or empty");
            return false;
        }

        try
        {
            _logger.LogDebug("Checking if member exists by national code: {NationalCode}", nationalCode);

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            var exists = member != null;

            _logger.LogDebug("Member {Exists} for national code: {NationalCode}", exists ? "exists" : "does not exist", nationalCode);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if member exists by national code: {NationalCode}", nationalCode);
            return false;
        }
    }

    public async Task<bool> HasActiveMembershipAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("National code is null or empty");
            return false;
        }

        try
        {
            _logger.LogDebug("Checking active membership for: {NationalCode}", nationalCode);

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogDebug("Member not found for national code: {NationalCode}", nationalCode);
                return false;
            }

            var hasActiveMembership = member.MembershipNumber != null;

            _logger.LogDebug("Member {NationalCode} {HasActiveMembership} active membership", 
                nationalCode, hasActiveMembership ? "has" : "does not have");
            return hasActiveMembership;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking active membership for: {NationalCode}", nationalCode);
            return false;
        }
    }


    public async Task<IEnumerable<string>> GetMemberCapabilitiesAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting capabilities for member {NationalCode}", nationalCode.Value);

            // First try to find in local repository
            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            
            if (member != null)
            {
                // Get capabilities from local member
                var validCapabilities = member.GetValidCapabilities();
                var capabilityKeys = validCapabilities.Select(mc => mc.CapabilityKey).ToList();

                _logger.LogDebug("Found {Count} valid capabilities for member {NationalCode} from local repository",
                    capabilityKeys.Count, nationalCode.Value);

                return capabilityKeys;
            }

            // If not found locally, try external storage
            _logger.LogDebug("Member not found in local repository, checking external storage for capabilities: {NationalCode}", nationalCode.Value);
            
            var externalMember = await _externalEngineerService.GetMemberByNationalCodeAsync(nationalCode.Value);
            if (externalMember != null)
            {
                // Get capabilities from external member data (use capability keys)
                var externalCapabilities = externalMember.Capabilities.Select(c => c.Capability.Key).ToList();
                
                _logger.LogDebug("Found {Count} capabilities for member {NationalCode} from external storage",
                    externalCapabilities.Count, nationalCode.Value);

                return externalCapabilities;
            }

            _logger.LogWarning("Member not found for national code {NationalCode} in both local and external storage", nationalCode.Value);
            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capabilities for member {NationalCode}", nationalCode.Value);
            return new List<string>();
        }
    }

    public async Task<IEnumerable<string>> GetMemberFeaturesAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting features for member {NationalCode}", nationalCode.Value);

            // First try to find in local repository
            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            
            if (member != null)
            {
                // Get features from local member features (direct assignments)
                var validFeatures = member.GetValidFeatures();
                var directFeatures = validFeatures.Select(mf => mf.FeatureKey).ToList();

                // Get features from local member capabilities (derived from capabilities)
                var validCapabilities = member.GetValidCapabilities();
                var capabilityKeys = validCapabilities.Select(mc => mc.CapabilityKey).ToList();

                // Get features from cache for all capabilities (with capability keys for better management)
                var allFeatures = new HashSet<string>();
                
                // Add direct features
                foreach (var featureKey in directFeatures)
                {
                    allFeatures.Add(featureKey);
                }

                // Add capability-derived features
                foreach (var capabilityKey in capabilityKeys)
                {
                    var features = await _cacheService.GetCapabilityFeaturesAsync(capabilityKey);
                    foreach (var feature in features)
                    {
                        allFeatures.Add($"{capabilityKey}:{feature}"); // Include capability key with feature
                    }
                }

                _logger.LogDebug("Found {Count} unique features for member {NationalCode} from local repository",
                    allFeatures.Count, nationalCode.Value);

                return allFeatures;
            }

            // If not found locally, try external storage
            _logger.LogDebug("Member not found in local repository, checking external storage for features: {NationalCode}", nationalCode.Value);
            
            var externalMember = await _externalEngineerService.GetMemberByNationalCodeAsync(nationalCode.Value);
            if (externalMember != null)
            {
                // Get features from external member data (all claims from all capabilities)
                var externalFeatures = externalMember.Capabilities
                   .SelectMany(capability => capability.Claims.Select(claim => claim.Value))
                   .ToList();
                
                _logger.LogDebug("Found {Count} features for member {NationalCode} from external storage",
                    externalFeatures.Count, nationalCode.Value);

                return externalFeatures;
            }

            _logger.LogWarning("Member not found for national code {NationalCode} in both local and external storage", nationalCode.Value);
            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting features for member {NationalCode}", nationalCode.Value);
            return new List<string>();
        }
    }

    public async Task<bool> HasCapabilityAsync(NationalId nationalCode, string capabilityId)
    {
        try
        {
            _logger.LogDebug("Checking capability {CapabilityId} for member {NationalCode}",
                capabilityId, nationalCode.Value);

            // First try to find in local repository
            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            
            if (member != null)
            {
                var hasCapability = member.HasCapabilityKey(capabilityId);

                _logger.LogDebug("Member {NationalCode} {HasCapability} capability {CapabilityId} from local repository",
                    nationalCode.Value, hasCapability ? "has" : "does not have", capabilityId);

                return hasCapability;
            }

            // If not found locally, try external storage
            _logger.LogDebug("Member not found in local repository, checking external storage for capability: {NationalCode}", nationalCode.Value);
            
            var externalMember = await _externalEngineerService.GetMemberByNationalCodeAsync(nationalCode.Value);
            if (externalMember != null)
            {
                // Check capability in external member data
                var hasExternalCapability = externalMember.Capabilities.Any(c => c.Capability.Key == capabilityId);
                
                _logger.LogDebug("Member {NationalCode} {HasCapability} capability {CapabilityId} from external storage",
                    nationalCode.Value, hasExternalCapability ? "has" : "does not have", capabilityId);

                return hasExternalCapability;
            }

            _logger.LogWarning("Member not found for national code {NationalCode} in both local and external storage", nationalCode.Value);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking capability {CapabilityId} for member {NationalCode}",
                capabilityId, nationalCode.Value);
            return false;
        }
    }

    public async Task<bool> HasFeatureAsync(NationalId nationalCode, string featureId)
    {
        try
        {
            _logger.LogDebug("Checking feature {FeatureId} for member {NationalCode}",
                featureId, nationalCode.Value);

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found for national code {NationalCode}", nationalCode.Value);
                return false;
            }

            // Check direct feature assignments first
            var hasDirectFeature = member.HasFeatureKey(featureId);
            if (hasDirectFeature)
            {
                _logger.LogDebug("Member {NationalCode} has feature {FeatureId} via direct assignment",
                    nationalCode.Value, featureId);
                return true;
            }

            // Check capability-derived features
            var validCapabilities = member.GetValidCapabilities();
            var capabilityKeys = validCapabilities.Select(mc => mc.CapabilityKey).ToList();

            // Check if any capability has this feature
            foreach (var capabilityKey in capabilityKeys)
            {
                var features = await _cacheService.GetCapabilityFeaturesAsync(capabilityKey);
                if (features.Contains(featureId))
                {
                    _logger.LogDebug("Member {NationalCode} has feature {FeatureId} via capability {CapabilityId}",
                        nationalCode.Value, featureId, capabilityKey);
                    return true;
                }
            }

            // If not found locally, try external storage
            _logger.LogDebug("Member not found in local repository, checking external storage for feature: {NationalCode}", nationalCode.Value);
            
            var externalMember = await _externalEngineerService.GetMemberByNationalCodeAsync(nationalCode.Value);
            if (externalMember != null)
            {
                // Check if feature exists in external member data
                var hasExternalFeature = externalMember.Capabilities
                    .SelectMany(capability => capability.Claims)
                    .Any(claim => claim.Value == featureId);
                
                _logger.LogDebug("Member {NationalCode} {HasFeature} feature {FeatureId} from external storage",
                    nationalCode.Value, hasExternalFeature ? "has" : "does not have", featureId);

                return hasExternalFeature;
            }

            _logger.LogDebug("Member {NationalCode} does not have feature {FeatureId}",
                nationalCode.Value, featureId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking feature {FeatureId} for member {NationalCode}",
                featureId, nationalCode.Value);
            return false;
        }
    }

    public async Task<bool> HasAnyAgencyAsync(NationalId nationalCode, IEnumerable<Guid> agencyIds)
    {
        try
        {
            if (agencyIds == null || !agencyIds.Any())
            {
                return true; // No agency requirement
            }

            _logger.LogDebug("Checking agencies for member {NationalCode}", nationalCode.Value);

            var memberDetail = await GetMemberDetailByNationalCodeAsync(nationalCode);
            if (memberDetail == null)
            {
                _logger.LogWarning("Member not found for national code {NationalCode}", nationalCode.Value);
                return false;
            }

            var memberAgencyIds = memberDetail.AgencyIds ?? new List<Guid>();
            var requiredAgencyIds = agencyIds.ToHashSet();

            var hasAnyAgency = memberAgencyIds.Any(agencyId => requiredAgencyIds.Contains(agencyId));

            _logger.LogDebug("Member {NationalCode} {HasAgency} any of the required agencies",
                nationalCode.Value, hasAnyAgency ? "has" : "does not have");

            return hasAnyAgency;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking agencies for member {NationalCode}", nationalCode.Value);
            return false;
        }
    }

    public async Task<MemberEligibilityResult> ValidateMemberEligibilityAsync(
        NationalId nationalCode,
        IEnumerable<string>? requiredCapabilities = null,
        IEnumerable<string>? requiredFeatures = null,
        IEnumerable<Guid>? requiredAgencies = null)
    {
        try
        {
            _logger.LogDebug("Validating eligibility for member {NationalCode}", nationalCode.Value);

            // Get member detail with all required information
            var memberDetail = await GetMemberDetailByNationalCodeAsync(nationalCode);
            if (memberDetail == null)
            {
                return MemberEligibilityResult.MemberNotFound(nationalCode.Value);
            }

            // Check active membership
            var hasActiveMembership = await HasActiveMembershipAsync(nationalCode);
            if (!hasActiveMembership)
            {
                return MemberEligibilityResult.NotEligible(
                    nationalCode.Value,
                    hasActiveMembership: false,
                    errors: new List<string> { "عضویت فعال برای این کد ملی یافت نشد" },
                    memberDetail: memberDetail);
            }

            var missingCapabilities = new List<string>();
            var missingFeatures = new List<string>();
            var missingAgencies = new List<Guid>();
            var errors = new List<string>();

            var requiredCaps = requiredCapabilities?.ToList() ?? new List<string>();
            var requiredFeats = requiredFeatures?.ToList() ?? new List<string>();
            var hasSpecificRequirements = requiredCaps.Any() || requiredFeats.Any();

            // Validate capabilities and features: member must have AT LEAST ONE of the required capabilities OR features
            bool hasAnyRequiredCapability = false;
            bool hasAnyRequiredFeature = false;
            var foundCapabilities = new List<string>();
            var foundFeatures = new List<string>();

            // Check if member has at least one of the required capabilities
            if (requiredCaps.Any())
            {
                var memberCaps = memberDetail.Capabilities ?? new List<string>();
                var memberCapsSet = memberCaps.ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var requiredCap in requiredCaps)
                {
                    // Check if member has capability directly or via service
                    var hasCapability = memberCapsSet.Contains(requiredCap) ||
                                       await HasCapabilityAsync(nationalCode, requiredCap);
                    
                    if (hasCapability)
                    {
                        hasAnyRequiredCapability = true;
                        foundCapabilities.Add(requiredCap);
                    }
                    else
                    {
                        missingCapabilities.Add(requiredCap);
                    }
                }
            }
            else
            {
                // No capabilities required, so this check passes
                hasAnyRequiredCapability = true;
            }

            // Check if member has at least one of the required features
            if (requiredFeats.Any())
            {
                var memberFeats = memberDetail.Features ?? new List<string>();
                var memberFeatsSet = memberFeats.ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var requiredFeat in requiredFeats)
                {
                    // Check if member has feature directly or via service
                    var hasFeature = memberFeatsSet.Contains(requiredFeat) ||
                                    await HasFeatureAsync(nationalCode, requiredFeat);
                    
                    if (hasFeature)
                    {
                        hasAnyRequiredFeature = true;
                        foundFeatures.Add(requiredFeat);
                    }
                    else
                    {
                        missingFeatures.Add(requiredFeat);
                    }
                }
            }
            else
            {
                // No features required, so this check passes
                hasAnyRequiredFeature = true;
            }

            // Member must have at least ONE capability OR ONE feature (OR logic)
            var meetsCapabilityOrFeatureRequirement = hasAnyRequiredCapability || hasAnyRequiredFeature;

            if (!meetsCapabilityOrFeatureRequirement && hasSpecificRequirements)
            {
                // Member doesn't have any of the required capabilities or features
                var capabilityNames = new List<string>();
                foreach (var cap in requiredCaps)
                {
                    var displayName = await GetCapabilityDisplayNameAsync(cap);
                    capabilityNames.Add(displayName);
                }

                var featureNames = new List<string>();
                foreach (var feat in requiredFeats)
                {
                    var displayName = await GetFeatureDisplayNameAsync(feat);
                    featureNames.Add(displayName);
                }

                var allRequiredNames = capabilityNames.Concat(featureNames).ToList();
                var requiredText = string.Join(" یا ", allRequiredNames);
                errors.Add($"عضو باید حداقل یکی از موارد زیر را داشته باشد: {requiredText}");
            }

            // If no specific capabilities or features required, check if user is VIP
            if (!hasSpecificRequirements)
            {
                var isVip = await IsVipAsync(nationalCode);
                if (!isVip)
                {
                    errors.Add("برای شرکت در این تور باید عضو ویژه (VIP) باشید");
                    meetsCapabilityOrFeatureRequirement = false;
                }
            }

            // Validate required agencies separately (member must have at least one of the required agencies)
            bool meetsAgencyRequirement = true;
            var requiredAgenciesList = requiredAgencies?.ToList() ?? new List<Guid>();
            if (requiredAgenciesList.Any())
            {
                var hasAnyAgency = await HasAnyAgencyAsync(nationalCode, requiredAgenciesList);
                if (!hasAnyAgency)
                {
                    meetsAgencyRequirement = false;
                    missingAgencies.AddRange(requiredAgenciesList);
                    
                    // Get agency display names
                    var agencyNames = new List<string>();
                    foreach (var agencyId in requiredAgenciesList)
                    {
                        var agencyName = await GetAgencyDisplayNameAsync(agencyId);
                        agencyNames.Add(agencyName);
                    }
                    
                    var agencyNamesText = string.Join(" یا ", agencyNames);
                    errors.Add($"عضو باید با حداقل یکی از آژانس‌های زیر مرتبط باشد: {agencyNamesText}");
                }
            }

            // Member is eligible if:
            // 1. Has at least one required capability OR feature (OR logic)
            // 2. Meets agency requirement (if agencies are required)
            var isEligible = meetsCapabilityOrFeatureRequirement && meetsAgencyRequirement;

            if (isEligible)
            {
                _logger.LogInformation(
                    "Member {NationalCode} is eligible. Found capabilities: {FoundCaps}, Found features: {FoundFeats}",
                    nationalCode.Value,
                    string.Join(", ", foundCapabilities),
                    string.Join(", ", foundFeatures));
                return MemberEligibilityResult.Eligible(nationalCode.Value, memberDetail);
            }

            _logger.LogWarning(
                "Member {NationalCode} is not eligible. MeetsCapabilityOrFeature: {MeetsCapOrFeat}, MeetsAgency: {MeetsAgency}. Missing: Capabilities={Capabilities}, Features={Features}, Agencies={Agencies}",
                nationalCode.Value,
                meetsCapabilityOrFeatureRequirement,
                meetsAgencyRequirement,
                string.Join(", ", missingCapabilities),
                string.Join(", ", missingFeatures),
                string.Join(", ", missingAgencies));

            return MemberEligibilityResult.NotEligible(
                nationalCode.Value,
                hasActiveMembership: true,
                missingCapabilities: missingCapabilities,
                missingFeatures: missingFeatures,
                missingAgencies: missingAgencies,
                errors: errors,
                memberDetail: memberDetail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating eligibility for member {NationalCode}", nationalCode.Value);
            return MemberEligibilityResult.NotEligible(
                nationalCode.Value,
                hasActiveMembership: false,
                errors: new List<string> { "خطا در بررسی صلاحیت‌های عضویت. لطفاً مجدداً تلاش کنید" });
        }
    }

    public async Task<Dictionary<string, MemberEligibilityResult>> ValidateMembersEligibilityAsync(
        IEnumerable<NationalId> nationalCodes,
        IEnumerable<string>? requiredCapabilities = null,
        IEnumerable<string>? requiredFeatures = null,
        IEnumerable<Guid>? requiredAgencies = null)
    {
        try
        {
            var nationalCodesList = nationalCodes?.ToList() ?? new List<NationalId>();
            if (!nationalCodesList.Any())
            {
                return new Dictionary<string, MemberEligibilityResult>();
            }

            _logger.LogDebug("Validating eligibility for {Count} members", nationalCodesList.Count);

            var results = new Dictionary<string, MemberEligibilityResult>();

            // Validate each member in parallel for better performance
            var validationTasks = nationalCodesList.Select(async nationalCode =>
            {
                var result = await ValidateMemberEligibilityAsync(
                    nationalCode,
                    requiredCapabilities,
                    requiredFeatures,
                    requiredAgencies);
                return (nationalCode.Value, result);
            });

            var validationResults = await Task.WhenAll(validationTasks);

            foreach (var (nationalCode, result) in validationResults)
            {
                results[nationalCode] = result;
            }

            var eligibleCount = results.Values.Count(r => r.IsEligible);
            _logger.LogInformation("Eligibility validation completed. Eligible: {EligibleCount}/{TotalCount}",
                eligibleCount, nationalCodesList.Count);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating eligibility for multiple members");
            throw;
        }
    }

    public async Task<bool> IsVipAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Checking if member {NationalCode} is VIP", nationalCode.Value);

            var memberDetail = await GetMemberDetailByNationalCodeAsync(nationalCode);
            if (memberDetail == null)
            {
                _logger.LogWarning("Member not found for national code {NationalCode}", nationalCode.Value);
                return false;
            }

            var caps = memberDetail.Capabilities ?? new List<string>();
            var feats = memberDetail.Features ?? new List<string>();

            var isVip = caps.Contains("VIP", StringComparer.OrdinalIgnoreCase) ||
                        feats.Contains("VIP", StringComparer.OrdinalIgnoreCase) ||
                        await HasCapabilityAsync(nationalCode, "VIP") ||
                        await HasFeatureAsync(nationalCode, "VIP");

            _logger.LogDebug("Member {NationalCode} is {VipStatus}", nationalCode.Value, isVip ? "VIP" : "not VIP");
            return isVip;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking VIP status for member {NationalCode}", nationalCode.Value);
            return false;
        }
    }

    /// <summary>
    /// Gets display name for capability from cache
    /// </summary>
    private async Task<string> GetCapabilityDisplayNameAsync(string capabilityId)
    {
        try
        {
            var capability = await _cacheService.GetCapabilityAsync(capabilityId);
            return capability?.Name ?? capabilityId;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting capability display name for {CapabilityId}", capabilityId);
            return capabilityId;
        }
    }

    /// <summary>
    /// Gets display name for feature from cache
    /// </summary>
    private async Task<string> GetFeatureDisplayNameAsync(string featureId)
    {
        try
        {
            var feature = await _cacheService.GetFeatureAsync(featureId);
            return feature?.Title ?? featureId;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting feature display name for {FeatureId}", featureId);
            return featureId;
        }
    }

    /// <summary>
    /// Gets display name for agency from cache
    /// </summary>
    private async Task<string> GetAgencyDisplayNameAsync(Guid agencyId)
    {
        try
        {
            var agency = await _cacheService.GetAgencyAsync(agencyId);
            return agency?.Name ?? agencyId.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting agency display name for {AgencyId}", agencyId);
            return agencyId.ToString();
        }
    }

    /// <summary>
    /// Syncs existing member data with external storage data
    /// </summary>
    private async Task SyncExistingMemberWithExternalDataAsync(
        Domain.Entities.Member existingMember, 
        Contracts.Dtos.ExternalMemberResponseDto externalMember)
    {
        try
        {
            _logger.LogDebug("Syncing existing member {MemberId} with external data", existingMember.Id);

            // Update basic member information if needed

            // Check if basic info needs updating
            if (existingMember.FullName.FirstName != externalMember.FirstName ||
                existingMember.FullName.LastName != externalMember.LastName)
            {
                // Note: Since Member entity doesn't have UpdateFullName method, we'll skip this for now
                // In a real implementation, you might need to add these methods to the Member entity
                _logger.LogDebug("Member {MemberId} name differs from external data, but update method not available", existingMember.Id);
            }

            if (existingMember.Email != externalMember.Email)
            {
                // Note: Since Member entity doesn't have UpdateEmail method, we'll skip this for now
                _logger.LogDebug("Member {MemberId} email differs from external data, but update method not available", existingMember.Id);
            }

            if (existingMember.PhoneNumber?.Value != externalMember.PhoneNumber)
            {
                // Note: Since Member entity doesn't have UpdatePhoneNumber method, we'll skip this for now
                _logger.LogDebug("Member {MemberId} phone differs from external data, but update method not available", existingMember.Id);
            }

            // Always sync capabilities and features
            await SyncMemberCapabilitiesAndFeaturesAsync(existingMember, externalMember);

            // Retry pattern for concurrency conflicts
            const int maxRetries = 3;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    await _memberRepository.UpdateAsync(existingMember);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogDebug("Updated basic member information for MemberId: {MemberId}", existingMember.Id);
                    break; // Success, exit retry loop
                }
                catch (DbUpdateConcurrencyException) when (attempt < maxRetries)
                {
                    _logger.LogWarning("Concurrency conflict on attempt {Attempt} for member {MemberId}. Retrying...", 
                        attempt, existingMember.Id);
                    
                    // Reload the entity from database to get latest version
                    var reloadedMember = await _memberRepository.GetByIdAsync(existingMember.Id);
                    if (reloadedMember == null)
                    {
                        _logger.LogError("Member {MemberId} not found during retry", existingMember.Id);
                        throw;
                    }
                    
                    // Update the existing member with latest data
                    existingMember = reloadedMember;
                    
                    // Re-sync with external data
                    await SyncMemberCapabilitiesAndFeaturesAsync(existingMember, externalMember);
                    
                    // Wait a bit before retry
                    await Task.Delay(100 * attempt);
                }
                catch (DbUpdateConcurrencyException ex) when (attempt == maxRetries)
                {
                    _logger.LogError(ex, "Concurrency conflict persisted after {MaxRetries} attempts for member {MemberId}", 
                        maxRetries, existingMember.Id);
                    throw;
                }
            }
           

            _logger.LogDebug("Successfully synced existing member {MemberId} with external data", existingMember.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing existing member {MemberId} with external data", existingMember.Id);
            throw;
        }
    }

    /// <summary>
    /// Syncs member capabilities and features with external storage data
    /// Only syncs capability and feature IDs - adds missing ones, removes ones not in external data
    /// </summary>
    private async Task SyncMemberCapabilitiesAndFeaturesAsync(
        Domain.Entities.Member member, 
        Contracts.Dtos.ExternalMemberResponseDto externalMember)
    {
        try
        {
            _logger.LogDebug("Syncing capabilities and features for member {MemberId}", member.Id);

            // Get existing capabilities and features
            var existingCapabilities = (await _memberCapabilityRepository.GetByMemberIdAsync(member.Id)).ToList();
            var existingFeatures = (await _memberFeatureRepository.GetByMemberIdAsync(member.Id)).ToList();

            // Create sets for quick lookup
            var existingCapabilityKeys = existingCapabilities.Select(c => c.CapabilityKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var existingFeatureKeys = existingFeatures.Select(f => f.FeatureKey).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Collect all external capability and feature keys
            var externalCapabilityKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var allExternalFeatureKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var processedFeatureKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Step 1: Process external capabilities - Add if missing
            foreach (var externalCapability in externalMember.Capabilities)
            {
                var capabilityKey = externalCapability.Capability.Key;
                
                // Skip if capability key is null or empty
                if (string.IsNullOrWhiteSpace(capabilityKey))
                {
                    _logger.LogWarning("Skipping capability with empty key for member {MemberId}", member.Id);
                    continue;
                }

                externalCapabilityKeys.Add(capabilityKey);

                // Only add if it doesn't exist
                if (!existingCapabilityKeys.Contains(capabilityKey))
                {
                    var memberCapability = new MemberCapability(
                        member.Id,
                        capabilityKey
                    );
                    await _memberCapabilityRepository.AddAsync(memberCapability);
                    _logger.LogDebug("Added new capability {CapabilityKey} for member {MemberId}", capabilityKey, member.Id);
                }

                // Step 2: Process features for this capability - Add if missing
                foreach (var claim in externalCapability.Claims)
                {
                    var featureKey = claim.Value;

                    // Skip if feature key is null or empty
                    if (string.IsNullOrWhiteSpace(featureKey))
                    {
                        _logger.LogWarning("Skipping feature with empty key for member {MemberId}", member.Id);
                        continue;
                    }

                    // Track processed features to avoid duplicates
                    if (processedFeatureKeys.Contains(featureKey))
                    {
                        _logger.LogDebug("Skipping duplicate feature {FeatureKey} for member {MemberId}", featureKey, member.Id);
                        continue;
                    }

                    processedFeatureKeys.Add(featureKey);
                    allExternalFeatureKeys.Add(featureKey);

                    // Only add if it doesn't exist
                    if (!existingFeatureKeys.Contains(featureKey))
                    {
                        var memberFeature = new MemberFeature(
                            member.Id,
                            featureKey
                        );
                        await _memberFeatureRepository.AddAsync(memberFeature);
                        _logger.LogDebug("Added new feature {FeatureKey} for member {MemberId}", featureKey, member.Id);
                    }
                }
            }

            // Step 3: Remove features that are no longer in external data
            var featuresToRemove = existingFeatures
                .Where(f => !allExternalFeatureKeys.Contains(f.FeatureKey))
                .ToList();

            foreach (var featureToRemove in featuresToRemove)
            {
                await _memberFeatureRepository.DeleteAsync(featureToRemove);
                _logger.LogDebug("Removed feature {FeatureKey} for member {MemberId} (no longer in external data)", 
                    featureToRemove.FeatureKey, member.Id);
            }

            // Step 4: Remove capabilities that are no longer in external data
            var capabilitiesToRemove = existingCapabilities
                .Where(c => !externalCapabilityKeys.Contains(c.CapabilityKey))
                .ToList();

            foreach (var capabilityToRemove in capabilitiesToRemove)
            {
                await _memberCapabilityRepository.DeleteAsync(capabilityToRemove);
                _logger.LogDebug("Removed capability {CapabilityKey} for member {MemberId} (no longer in external data)", 
                    capabilityToRemove.CapabilityKey, member.Id);
            }

            _logger.LogInformation(
                "Successfully synced capabilities and features for member {MemberId}. " +
                "Capabilities: {CapAdded} added, {CapRemoved} removed. " +
                "Features: {FeatAdded} added, {FeatRemoved} removed.",
                member.Id,
                externalCapabilityKeys.Count - existingCapabilityKeys.Count,
                capabilitiesToRemove.Count,
                allExternalFeatureKeys.Count - existingFeatureKeys.Count,
                featuresToRemove.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing capabilities and features for member {MemberId}", member.Id);
            throw;
        }
    }
}