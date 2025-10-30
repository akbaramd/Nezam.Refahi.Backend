using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Membership.Application.Services;

/// <summary>
/// Implementation of member information service for inter-context communication
/// Provides member info with capabilities and features for other bounded contexts
/// </summary>
public class MemberInfoService : IMemberInfoService
{
    private readonly IMemberService _memberService;
    private readonly ILogger<MemberInfoService> _logger;

    public MemberInfoService(IMemberService memberService, ILogger<MemberInfoService> logger)
    {
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MemberInfoDto?> GetMemberInfoAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting member info for national code: {NationalCode}", nationalCode.Value);

            var member = await _memberService.GetMemberByNationalCodeAsync(nationalCode);
            if (member == null)
            {
                _logger.LogDebug("Member not found for national code: {NationalCode}", nationalCode.Value);
                return null;
            }

            // Get capabilities and features
            var capabilities = await _memberService.GetMemberCapabilitiesAsync(nationalCode);
            var features = await _memberService.GetMemberFeaturesAsync(nationalCode);

            var memberInfo = new MemberInfoDto
            {
                Id = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FirstName,
                LastName = member.LastName,
                NationalCode = member.NationalCode,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                BirthDate = member.BirthDate,
                IsSpecial = member.IsSpecial,
                IsActive = member.IsActive,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.ModifiedAt,
                Capabilities = capabilities.ToList(),
                Features = features.ToList()
            };

            _logger.LogDebug("Successfully retrieved member info for {NationalCode}", nationalCode.Value);
            return memberInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info for national code: {NationalCode}", nationalCode.Value);
            return null;
        }
    }

    public async Task<IEnumerable<MemberInfoDto>> GetMembersInfoAsync(IEnumerable<NationalId> nationalCodes)
    {
        try
        {
            _logger.LogDebug("Getting members info for {Count} national codes", nationalCodes.Count());

            var members = await _memberService.GetMembersByNationalCodesAsync(nationalCodes);
            var memberInfoList = new List<MemberInfoDto>();

            foreach (var member in members)
            {
                var nationalCode = new NationalId(member.NationalCode);
                var capabilities = await _memberService.GetMemberCapabilitiesAsync(nationalCode);
                var features = await _memberService.GetMemberFeaturesAsync(nationalCode);

                var memberInfo = new MemberInfoDto
                {
                    Id = member.Id,
                    MembershipNumber = member.MembershipNumber,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    NationalCode = member.NationalCode,
                    PhoneNumber = member.PhoneNumber,
                    Email = member.Email,
                    BirthDate = member.BirthDate,
                    IsActive = member.IsActive,
                    IsSpecial = member.IsSpecial,
                    CreatedAt = member.CreatedAt,
                    ModifiedAt = member.ModifiedAt,
                    Capabilities = capabilities.ToList(),
                    Features = features.ToList()
                };

                memberInfoList.Add(memberInfo);
            }

            _logger.LogDebug("Successfully retrieved {Count} members info", memberInfoList.Count);
            return memberInfoList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting members info");
            return new List<MemberInfoDto>();
        }
    }

    public async Task<MemberInfoDto?> GetMemberInfoByMembershipNumberAsync(string membershipNumber)
    {
        try
        {
            _logger.LogDebug("Getting member info by membership number: {MembershipNumber}", membershipNumber);

            var member = await _memberService.GetMemberByMembershipNumberAsync(membershipNumber);
            if (member == null)
            {
                _logger.LogDebug("Member not found for membership number: {MembershipNumber}", membershipNumber);
                return null;
            }

            var nationalCode = new NationalId(member.NationalCode);
            var capabilities = await _memberService.GetMemberCapabilitiesAsync(nationalCode);
            var features = await _memberService.GetMemberFeaturesAsync(nationalCode);

            var memberInfo = new MemberInfoDto
            {
                Id = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FirstName,
                LastName = member.LastName,
                NationalCode = member.NationalCode,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                BirthDate = member.BirthDate,
                IsActive = member.IsActive,
                IsSpecial = member.IsSpecial,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.ModifiedAt,
                Capabilities = capabilities.ToList(),
                Features = features.ToList()
            };

            _logger.LogDebug("Successfully retrieved member info for membership number: {MembershipNumber}", membershipNumber);
            return memberInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info by membership number: {MembershipNumber}", membershipNumber);
            return null;
        }
    }

    public async Task<MemberInfoDto?> GetMemberInfoByPhoneNumberAsync(string phoneNumber)
    {
        try
        {
            _logger.LogDebug("Getting member info by phone number: {PhoneNumber}", phoneNumber);

            var member = await _memberService.GetMemberByPhoneNumberAsync(phoneNumber);
            if (member == null)
            {
                _logger.LogDebug("Member not found for phone number: {PhoneNumber}", phoneNumber);
                return null;
            }

            var nationalCode = new NationalId(member.NationalCode);
            var capabilities = await _memberService.GetMemberCapabilitiesAsync(nationalCode);
            var features = await _memberService.GetMemberFeaturesAsync(nationalCode);

            var memberInfo = new MemberInfoDto
            {
                Id = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FirstName,
                LastName = member.LastName,
                NationalCode = member.NationalCode,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                BirthDate = member.BirthDate,
                IsActive = member.IsActive,
                IsSpecial = member.IsSpecial,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.ModifiedAt,
                Capabilities = capabilities.ToList(),
                Features = features.ToList()
            };

            _logger.LogDebug("Successfully retrieved member info for phone number: {PhoneNumber}", phoneNumber);
            return memberInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info by phone number: {PhoneNumber}", phoneNumber);
            return null;
        }
    }

    public async Task<MemberInfoDto?> GetMemberInfoByEmailAsync(string email)
    {
        try
        {
            _logger.LogDebug("Getting member info by email: {Email}", email);

            var member = await _memberService.GetMemberByEmailAsync(email);
            if (member == null)
            {
                _logger.LogDebug("Member not found for email: {Email}", email);
                return null;
            }

            var nationalCode = new NationalId(member.NationalCode);
            var capabilities = await _memberService.GetMemberCapabilitiesAsync(nationalCode);
            var features = await _memberService.GetMemberFeaturesAsync(nationalCode);

            var memberInfo = new MemberInfoDto
            {
                Id = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FirstName,
                LastName = member.LastName,
                NationalCode = member.NationalCode,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                BirthDate = member.BirthDate,
                IsActive = member.IsActive,
                IsSpecial = member.IsSpecial,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.ModifiedAt,
                Capabilities = capabilities.ToList(),
                Features = features.ToList()
            };

            _logger.LogDebug("Successfully retrieved member info for email: {Email}", email);
            return memberInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info by email: {Email}", email);
            return null;
        }
    }

    public async Task<MemberInfoDto?> GetMemberInfoByExternalIdAsync(string externalId)
    {
        try
        {
            _logger.LogDebug("Getting member info by external ID: {ExternalId}", externalId);

            var member = await _memberService.GetMemberByExternalIdAsync(externalId);
            if (member == null)
            {
                _logger.LogDebug("Member not found for external ID: {ExternalId}", externalId);
                return null;
            }

            var nationalCode = new NationalId(member.NationalCode);
            var capabilities = await _memberService.GetMemberCapabilitiesAsync(nationalCode);
            var features = await _memberService.GetMemberFeaturesAsync(nationalCode);

            var memberInfo = new MemberInfoDto
            {
                Id = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FirstName,
                LastName = member.LastName,
                NationalCode = member.NationalCode,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                BirthDate = member.BirthDate,
                IsActive = member.IsActive,
                IsSpecial = member.IsSpecial,
                CreatedAt = member.CreatedAt,
                ModifiedAt = member.ModifiedAt,
                Capabilities = capabilities.ToList(),
                Features = features.ToList()
            };

            _logger.LogDebug("Successfully retrieved member info for external ID: {ExternalId}", externalId);
            return memberInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member info by external ID: {ExternalId}", externalId);
            return null;
        }
    }

    public async Task<bool> HasCapabilityAsync(NationalId nationalCode, string capabilityKey)
    {
        try
        {
            _logger.LogDebug("Checking capability {CapabilityKey} for member {NationalCode}", capabilityKey, nationalCode.Value);
            return await _memberService.HasCapabilityAsync(nationalCode, capabilityKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking capability {CapabilityKey} for member {NationalCode}", capabilityKey, nationalCode.Value);
            return false;
        }
    }

    public async Task<bool> HasFeatureAsync(NationalId nationalCode, string featureKey)
    {
        try
        {
            _logger.LogDebug("Checking feature {FeatureKey} for member {NationalCode}", featureKey, nationalCode.Value);
            return await _memberService.HasFeatureAsync(nationalCode, featureKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking feature {FeatureKey} for member {NationalCode}", featureKey, nationalCode.Value);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetMemberCapabilitiesAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting capabilities for member {NationalCode}", nationalCode.Value);
            return await _memberService.GetMemberCapabilitiesAsync(nationalCode);
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
            return await _memberService.GetMemberFeaturesAsync(nationalCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting features for member {NationalCode}", nationalCode.Value);
            return new List<string>();
        }
    }

    public async Task<bool> IsMemberExistsAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Checking if member exists for national code: {NationalCode}", nationalCode.Value);
            return await _memberService.IsMemberExistsByNationalCodeAsync(nationalCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if member exists for national code: {NationalCode}", nationalCode.Value);
            return false;
        }
    }

    public async Task<bool> HasActiveMembershipAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Checking active membership for member {NationalCode}", nationalCode.Value);
            return await _memberService.HasActiveMembershipAsync(nationalCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking active membership for member {NationalCode}", nationalCode.Value);
            return false;
        }
    }
}
