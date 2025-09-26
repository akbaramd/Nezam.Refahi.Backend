using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Repositories;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

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
    private readonly IExternalMemberStorage _externalEngineerService;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly IBasicDefinitionsCacheService _cacheService;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        IMemberRepository memberRepository,
        IExternalMemberStorage externalEngineerService,
        IMembershipUnitOfWork unitOfWork,
        ILogger<MemberService> logger,
        IMemberRoleRepository memberRoleRepository,
        IRoleRepository roleRepository,
        IMemberCapabilityRepository memberCapabilityRepository,
        IBasicDefinitionsCacheService cacheService)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _externalEngineerService = externalEngineerService ?? throw new ArgumentNullException(nameof(externalEngineerService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memberRoleRepository = memberRoleRepository;
        _roleRepository = roleRepository;
        _memberCapabilityRepository = memberCapabilityRepository ?? throw new ArgumentNullException(nameof(memberCapabilityRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
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

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogDebug("Member not found for national code: {NationalCode}", nationalCode);
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
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.LastModifiedAt
            };

            _logger.LogDebug("Successfully retrieved member {NationalCode}", nationalCode);
            return memberDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member by national code: {NationalCode}", nationalCode);
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

            var members = await _memberRepository.GetByNationalCodesAsync(nationalCodes);
            var memberDtos = members.Select(member => new MemberDto
            {
                Id = member.Id,
                NationalCode = member.NationalCode?.Value ?? string.Empty,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName.FirstName,
                LastName = member.FullName.LastName,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber?.Value,
                IsActive = true,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.LastModifiedAt
            }).ToList();

            _logger.LogDebug("Successfully retrieved {Count} members", memberDtos.Count);
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

            var member = await _memberRepository.FindOneAsync(x => x.UserId == new Guid(externalId));
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

    public async Task<BasicMemberInfoDto?> GetBasicMemberInfoAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            _logger.LogWarning("National code is null or empty");
            return null;
        }

        try
        {
            _logger.LogDebug("Getting basic member info for: {NationalCode}", nationalCode);

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogDebug("Member not found for national code: {NationalCode}", nationalCode);
                return null;
            }

            var basicInfo = new BasicMemberInfoDto
            {
                Id = member.Id,
                NationalCode = member.NationalCode?.Value ?? string.Empty,
                MembershipNumber = member.MembershipNumber,
                FullName = member.FullName.ToString(),
                IsActive = true
            };

            _logger.LogDebug("Successfully retrieved basic info for {NationalCode}", nationalCode);
            return basicInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting basic member info for: {NationalCode}", nationalCode);
            return null;
        }
    }

    public async Task<IEnumerable<string>> GetMemberCapabilitiesAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting capabilities for member {NationalCode}", nationalCode.Value);

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found for national code {NationalCode}", nationalCode.Value);
                return new List<string>();
            }

            var validCapabilities = member.GetValidCapabilities();
            var capabilityIds = validCapabilities.Select(mc => mc.CapabilityId).ToList();

            _logger.LogDebug("Found {Count} valid capabilities for member {NationalCode}",
                capabilityIds.Count, nationalCode.Value);

            return capabilityIds;
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

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found for national code {NationalCode}", nationalCode.Value);
                return new List<string>();
            }

            var validCapabilities = member.GetValidCapabilities();
            var capabilityIds = validCapabilities.Select(mc => mc.CapabilityId).ToList();

            // Get features from cache for all capabilities
            var allFeatures = new HashSet<string>();
            foreach (var capabilityId in capabilityIds)
            {
                var features = await _cacheService.GetCapabilityFeaturesAsync(capabilityId);
                foreach (var feature in features)
                {
                    allFeatures.Add(feature);
                }
            }

            _logger.LogDebug("Found {Count} unique features for member {NationalCode}",
                allFeatures.Count, nationalCode.Value);

            return allFeatures;
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

            var member = await _memberRepository.FindOneAsync(x => x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found for national code {NationalCode}", nationalCode.Value);
                return false;
            }

            var hasCapability = member.HasCapabilityKey(capabilityId);

            _logger.LogDebug("Member {NationalCode} {HasCapability} capability {CapabilityId}",
                nationalCode.Value, hasCapability ? "has" : "does not have", capabilityId);

            return hasCapability;
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

            var validCapabilities = member.GetValidCapabilities();
            var capabilityIds = validCapabilities.Select(mc => mc.CapabilityId).ToList();

            // Check if any capability has this feature
            foreach (var capabilityId in capabilityIds)
            {
                var features = await _cacheService.GetCapabilityFeaturesAsync(capabilityId);
                if (features.Contains(featureId))
                {
                    _logger.LogDebug("Member {NationalCode} has feature {FeatureId} via capability {CapabilityId}",
                        nationalCode.Value, featureId, capabilityId);
                    return true;
                }
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
}