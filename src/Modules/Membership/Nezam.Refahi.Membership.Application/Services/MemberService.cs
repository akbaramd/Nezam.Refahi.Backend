using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.Membership.Application.Dtos;
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
    private readonly IMemberFeatureRepository _memberFeatureRepository;
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
        IMemberFeatureRepository memberFeatureRepository,
        IBasicDefinitionsCacheService cacheService)
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

            // Get existing members from local repository
            var existingMembers = await _memberRepository.GetByNationalCodesAsync(nationalCodeList);
            var existingNationalCodes = existingMembers.Select(m => m.NationalCode.Value).ToHashSet();

            // Add existing members to result
            foreach (var member in existingMembers)
            {
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
                memberDtos.Add(memberDto);
            }

            // Find missing members and fetch from external storage
            var missingNationalCodes = nationalCodeList.Where(nc => !existingNationalCodes.Contains(nc.Value)).ToList();
            
            if (missingNationalCodes.Any())
            {
                _logger.LogDebug("Found {Count} missing members, fetching from external storage", missingNationalCodes.Count);
                
                foreach (var nationalCode in missingNationalCodes)
                {
                    try
                    {
                        var externalMember = await _externalEngineerService.GetMemberByNationalCodeAsync(nationalCode.Value);
                        if (externalMember != null)
                        {
                            _logger.LogDebug("Found member in external storage, saving to local repository: {NationalCode}", nationalCode.Value);
                            
                            // Create new member entity from external data
                            var newMember = new Member(
                                Guid.NewGuid(), // externalUserId - temporary, will be updated when user is created
                                externalMember.MembershipCode ?? string.Empty, // membershipNumber
                                nationalCode, // nationalCode
                                new FullName(externalMember.FirstName, externalMember.LastName), // fullName
                                new Email(externalMember.Email), // email
                                externalMember.PhoneNumber != null ? new PhoneNumber(externalMember.PhoneNumber) : new PhoneNumber(string.Empty), // phoneNumber
                                externalMember.Birthdate // birthDate
                            );

                            // Save member to local repository
                            await _memberRepository.AddAsync(newMember);
                            await _unitOfWork.SaveChangesAsync();

                            // Get and save capabilities from external member data
                            var processedFeatureKeys = new HashSet<string>();
                            
                            foreach (var externalCapability in externalMember.Capabilities)
                            {
                                var memberCapability = new MemberCapability(
                                    newMember.Id,
                                    externalCapability.Capability.Key, // Use capability key directly
                                    externalCapability.Capability.Name ?? externalCapability.Capability.Key, // Use as title
                                    externalCapability.ValidFrom,
                                    externalCapability.ValidTo ?? DateTime.UtcNow.AddYears(1), // Default to 1 year if not specified
                                    externalCapability.AssignedBy,
                                    externalCapability.Notes
                                );
                                await _memberCapabilityRepository.AddAsync(memberCapability);

                                // Save individual features from this capability
                                foreach (var claim in externalCapability.Claims)
                                {
                                    var featureKey = claim.Value;
                                    
                                    // Skip if feature key is null or empty
                                    if (string.IsNullOrWhiteSpace(featureKey))
                                    {
                                        _logger.LogWarning("Skipping feature with empty key for new member {MemberId}", newMember.Id);
                                        continue;
                                    }
                                    
                                    // Skip if we've already processed this feature key
                                    if (processedFeatureKeys.Contains(featureKey))
                                    {
                                        _logger.LogDebug("Skipping duplicate feature {FeatureKey} for new member {MemberId}", featureKey, newMember.Id);
                                        continue;
                                    }
                                    
                                    processedFeatureKeys.Add(featureKey);
                                    
                                    var memberFeature = new MemberFeature(
                                        newMember.Id,
                                        featureKey, // Use claim value as feature key
                                        claim.ClaimTypeTitle ?? featureKey, // Use claim type title as feature title
                                        claim.ValidFrom ?? externalCapability.ValidFrom,
                                        claim.ValidTo ?? externalCapability.ValidTo ?? DateTime.UtcNow.AddYears(1),
                                        claim.AssignedBy ?? externalCapability.AssignedBy,
                                        claim.Notes ?? externalCapability.Notes
                                    );
                                    await _memberFeatureRepository.AddAsync(memberFeature);
                                }
                            }

                            await _unitOfWork.SaveChangesAsync();

                            // Add to result
                            var newMemberDto = new MemberDto
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
                            memberDtos.Add(newMemberDto);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to fetch member {NationalCode} from external storage", nationalCode.Value);
                    }
                }
            }

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
    /// </summary>
    private async Task SyncMemberCapabilitiesAndFeaturesAsync(
        Domain.Entities.Member member, 
        Contracts.Dtos.ExternalMemberResponseDto externalMember)
    {
        try
        {
            _logger.LogDebug("Syncing capabilities and features for member {MemberId}", member.Id);

            // Get existing capabilities and features
            var existingCapabilities = await _memberCapabilityRepository.GetByMemberIdAsync(member.Id);
            var existingFeatures = await _memberFeatureRepository.GetByMemberIdAsync(member.Id);

            // Create sets for comparison
            var existingCapabilityKeys = existingCapabilities.Select(c => c.CapabilityKey).ToHashSet();
            var existingFeatureKeys = existingFeatures.Select(f => f.FeatureKey).ToHashSet();

            // Track features that have been processed in this sync to avoid duplicates
            var processedFeatureKeys = new HashSet<string>();

            // Process external capabilities
            var externalCapabilityKeys = new HashSet<string>();
            foreach (var externalCapability in externalMember.Capabilities)
            {
                var capabilityKey = externalCapability.Capability.Key;
                externalCapabilityKeys.Add(capabilityKey);

                if (!existingCapabilityKeys.Contains(capabilityKey))
                {
                    // Add new capability
                    var memberCapability = new MemberCapability(
                        member.Id,
                        capabilityKey,
                        externalCapability.Capability.Name ?? capabilityKey,
                        externalCapability.ValidFrom,
                        externalCapability.ValidTo ?? DateTime.UtcNow.AddYears(1),
                        externalCapability.AssignedBy,
                        externalCapability.Notes
                    );
                    await _memberCapabilityRepository.AddAsync(memberCapability);
                    _logger.LogDebug("Added new capability {CapabilityKey} for member {MemberId}", capabilityKey, member.Id);
                }
                else
                {
                    // Update existing capability
                    var existingCapability = existingCapabilities.First(c => c.CapabilityKey == capabilityKey);
                    existingCapability.UpdateValidityPeriod(
                        externalCapability.ValidFrom,
                        externalCapability.ValidTo ?? DateTime.UtcNow.AddYears(1)
                    );
                    existingCapability.UpdateAssignedBy(externalCapability.AssignedBy);
                    existingCapability.UpdateNotes(externalCapability.Notes);
                    await _memberCapabilityRepository.UpdateAsync(existingCapability);
                    _logger.LogDebug("Updated existing capability {CapabilityKey} for member {MemberId}", capabilityKey, member.Id);
                }

                // Process features for this capability
                foreach (var claim in externalCapability.Claims)
                {
                    var featureKey = claim.Value;

                    // Skip if feature key is null or empty
                    if (string.IsNullOrWhiteSpace(featureKey))
                    {
                        _logger.LogWarning("Skipping feature with empty key for member {MemberId}", member.Id);
                        continue;
                    }

                    // Skip if we've already processed this feature key in this sync
                    if (processedFeatureKeys.Contains(featureKey))
                    {
                        _logger.LogDebug("Skipping duplicate feature {FeatureKey} for member {MemberId}", featureKey, member.Id);
                        continue;
                    }

                    processedFeatureKeys.Add(featureKey);

                    if (!existingFeatureKeys.Contains(featureKey))
                    {
                        // Add new feature
                        var memberFeature = new MemberFeature(
                            member.Id,
                            featureKey,
                            claim.ClaimTypeTitle ?? featureKey,
                            claim.ValidFrom ?? externalCapability.ValidFrom,
                            claim.ValidTo ?? externalCapability.ValidTo ?? DateTime.UtcNow.AddYears(1),
                            claim.AssignedBy ?? externalCapability.AssignedBy,
                            claim.Notes ?? externalCapability.Notes
                        );
                        await _memberFeatureRepository.AddAsync(memberFeature);
                        _logger.LogDebug("Added new feature {FeatureKey} for member {MemberId}", featureKey, member.Id);
                    }
                    else
                    {
                        // Update existing feature
                        var existingFeature = existingFeatures.First(f => f.FeatureKey == featureKey);
                        existingFeature.UpdateValidityPeriod(
                            claim.ValidFrom ?? externalCapability.ValidFrom,
                            claim.ValidTo ?? externalCapability.ValidTo ?? DateTime.UtcNow.AddYears(1)
                        );
                        existingFeature.UpdateAssignmentMetadata(
                            claim.AssignedBy ?? externalCapability.AssignedBy,
                            claim.Notes ?? externalCapability.Notes
                        );
                        await _memberFeatureRepository.UpdateAsync(existingFeature);
                        _logger.LogDebug("Updated existing feature {FeatureKey} for member {MemberId}", featureKey, member.Id);
                    }
                }
            }

            // Remove features that are no longer in external data
            var allExternalFeatureKeys = externalMember.Capabilities
                .SelectMany(capability => capability.Claims)
                .Select(claim => claim.Value)
                .ToHashSet();

            var featuresToRemove = existingFeatures
                .Where(f => !allExternalFeatureKeys.Contains(f.FeatureKey))
                .ToList();

            foreach (var featureToRemove in featuresToRemove)
            {
                featureToRemove.Deactivate();
                await _memberFeatureRepository.UpdateAsync(featureToRemove);
                _logger.LogDebug("Deactivated feature {FeatureKey} for member {MemberId}", featureToRemove.FeatureKey, member.Id);
            }

            // Remove capabilities that are no longer in external data
            var capabilitiesToRemove = existingCapabilities
                .Where(c => !externalCapabilityKeys.Contains(c.CapabilityKey))
                .ToList();

            foreach (var capabilityToRemove in capabilitiesToRemove)
            {
                capabilityToRemove.Deactivate();
                await _memberCapabilityRepository.UpdateAsync(capabilityToRemove);
                _logger.LogDebug("Deactivated capability {CapabilityKey} for member {MemberId}", capabilityToRemove.CapabilityKey, member.Id);
            }

            _logger.LogDebug("Successfully synced capabilities and features for member {MemberId}", member.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing capabilities and features for member {MemberId}", member.Id);
            throw;
        }
    }
}