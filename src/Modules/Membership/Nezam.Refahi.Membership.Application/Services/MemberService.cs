using Microsoft.Extensions.Logging;
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
    private readonly IFeatureRepository _iFeatureRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICapabilityRepository _capabilityRepository;
    private readonly IMemberCapabilityRepository _memberCapabilityRepository;
    private readonly IExternalMemberStorage _externalEngineerService;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        IMemberRepository memberRepository,
        IExternalMemberStorage externalEngineerService,
        IMembershipUnitOfWork unitOfWork,
        ILogger<MemberService> logger,
        IMemberRoleRepository memberRoleRepository,
        IRoleRepository roleRepository,
        IFeatureRepository iFeatureRepository,
        ICapabilityRepository capabilityRepository,
        IMemberCapabilityRepository memberCapabilityRepository)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _externalEngineerService = externalEngineerService ?? throw new ArgumentNullException(nameof(externalEngineerService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memberRoleRepository = memberRoleRepository;
        _roleRepository = roleRepository;
        _iFeatureRepository = iFeatureRepository;
        _capabilityRepository = capabilityRepository ?? throw new ArgumentNullException(nameof(capabilityRepository));
        _memberCapabilityRepository = memberCapabilityRepository ?? throw new ArgumentNullException(nameof(memberCapabilityRepository));
    }

    public async Task<MemberDto?> GetMemberByNationalCodeAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
            return null;

        // First, try to find the member in the local database
        var member = await _memberRepository.GetByNationalCodeAsync(nationalCode);
        if (member != null)
        {
            return MapToMemberDto(member);
        }

        // If not found locally, try to get from external engineer API
        try
        {
            var externalEngineer = await _externalEngineerService.GetMemberByNationalCodeAsync(nationalCode);
            if (externalEngineer != null)
            {
                _logger.LogInformation("Found engineer with national code {NationalCode} in external system, creating local member", nationalCode);
                
        
                try
                {
                    // Create new member from external data
                    var newMember = await CreateMemberFromExternalEngineer(externalEngineer);
                    
                
                    
                    return MapToMemberDto(newMember);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching engineer from external system for national code {NationalCode}", nationalCode);
        }

        return null;
    }

    public async Task<IEnumerable<MemberDto>> GetMembersByNationalCodesAsync(IEnumerable<NationalId> nationalCodes)
    {
        if (nationalCodes == null || !nationalCodes.Any())
            return Enumerable.Empty<MemberDto>();

        var members = new List<MemberDto>();
        foreach (var nationalCode in nationalCodes.Where(nc => !string.IsNullOrWhiteSpace(nc)))
        {
            var member = await _memberRepository.GetByNationalCodeAsync(nationalCode);
            if (member != null)
            {
                members.Add(MapToMemberDto(member));
            }
        }

        return members;
    }

    public async Task<MemberDto?> GetMemberByMembershipNumberAsync(string membershipNumber)
    {
        if (string.IsNullOrWhiteSpace(membershipNumber))
            return null;

        var member = await _memberRepository.GetByMembershipNumberAsync(membershipNumber);
        return member != null ? MapToMemberDto(member) : null;
    }

    public async Task<MemberDto?> GetMemberByPhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        try
        {
            var phoneNumberVO = new PhoneNumber(phoneNumber);
            var member = await _memberRepository.GetByPhoneNumberAsync(phoneNumberVO);
            return member != null ? MapToMemberDto(member) : null;
        }
        catch
        {
            // Invalid phone number format
            return null;
        }
    }

    public async Task<MemberDto?> GetMemberByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var member = await _memberRepository.GetByEmailAsync(email);
        return member != null ? MapToMemberDto(member) : null;
    }

    public async Task<MemberDto?> GetMemberByExternalIdAsync(string externalId)
    {
      if (string.IsNullOrWhiteSpace(externalId))
        return null;

      var member = await _memberRepository.FindOneAsync(x=>x.UserId.ToString() == externalId);
      return member != null ? MapToMemberDto(member) : null;
    }

    public async Task<bool> IsMemberExistsByNationalCodeAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
            return false;

        return await _memberRepository.IsNationalCodeExistsAsync(nationalCode);
    }

    public async Task<bool> HasActiveMembershipAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
            return false;

        var member = await _memberRepository.GetByNationalCodeAsync(nationalCode);
        // Note: Member entity doesn't have HasActiveMembership method yet
        return member != null;
    }

    public async Task<BasicMemberInfoDto?> GetBasicMemberInfoAsync(NationalId nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
            return null;

        var member = await _memberRepository.GetByNationalCodeAsync(nationalCode);
        if (member == null)
            return null;

        return new BasicMemberInfoDto
        {
            Id = member.Id,
            FullName = $"{member.FullName.FirstName} {member.FullName.LastName}",
            NationalCode = member.NationalCode?.Value??"",
            MembershipNumber = member.MembershipNumber,
            IsActive = true, // Note: Member entity doesn't have IsActive property
            HasActiveMembership = true // Note: Member entity doesn't have HasActiveMembership method
        };
    }

    private async Task<Member> CreateMemberFromExternalEngineer(ExternalMemberResponseDto externalMember)
    {
        // Begin transaction
        await _unitOfWork.BeginAsync();

        try
        {
            // Create phone number value object if available
            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrEmpty(externalMember.PhoneNumber))
            {
                try
                {
                    phoneNumber = new PhoneNumber(externalMember.PhoneNumber);
                }
                catch
                {
                    _logger.LogWarning("Invalid phone number format for member {NationalCode}: {PhoneNumber}",
                        externalMember.NationalCode, externalMember.PhoneNumber);
                }
            }

            // Create value objects
            var nationalCode = new NationalId(externalMember.NationalCode);
            var fullName = new FullName(externalMember.FirstName, externalMember.LastName);
            var email = !string.IsNullOrEmpty(externalMember.Email) && externalMember.Email.Contains("@")
                ? new Email(externalMember.Email)
                : new Email($"{externalMember.NationalCode}@temp.local"); // Temporary email

            // Create the member
            var member = new Member(
                membershipNumber: externalMember.MembershipCode,
                nationalCode: nationalCode,
                fullName: fullName,
                email: email,
                phoneNumber: phoneNumber ?? new PhoneNumber("0000000000"),
                birthDate:externalMember.Birthdate?.Date
            );

            // Add member to repository first
            await _memberRepository.AddAsync(member);
            await _unitOfWork.SaveChangesAsync(); // Save to get the member ID

            // Process capabilities with proper validation and synchronization
            await ProcessCapabilitiesAsync(member, externalMember.Capabilities);

            // Process roles
            await ProcessRolesAsync(member, externalMember.Roles);

            // Final save and commit
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Successfully created member {NationalCode} with {CapabilityCount} capabilities",
                externalMember.NationalCode, externalMember.Capabilities.Count);

            return member;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating member from external data for {NationalCode}", externalMember.NationalCode);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    private async Task ProcessCapabilitiesAsync(Member member, List<ExternalMemberCapabilityDto> externalCapabilities)
    {
        foreach (var externalCapability in externalCapabilities)
        {
            // Check if capability already exists by key
            var existingCapability = await _capabilityRepository.FindOneAsync(c => c.Id == externalCapability.Capability.Key);

            Capability capability;
            if (existingCapability == null)
            {
                // Create new capability
                capability = new Capability(
                    key: externalCapability.Capability.Key,
                    name: externalCapability.Capability.Name,
                    description: externalCapability.Capability.Description ?? "Imported capability",
                    validFrom: externalCapability.ValidFrom,
                    validTo: externalCapability.ValidTo
                );

                // Process and add claim types for this capability
                await ProcessCapabilityClaimTypesAsync(capability, externalCapability.Claims);

                await _capabilityRepository.AddAsync(capability);
                await _unitOfWork.SaveChangesAsync(); // Save to get capability ID

                _logger.LogInformation("Created new capability: {CapabilityKey} - {CapabilityName}",
                    capability.Id, capability.Name);
            }
            else
            {
                capability = existingCapability;

                // Update existing capability with any new claim types from external data
                await ProcessCapabilityClaimTypesAsync(capability, externalCapability.Claims);

                _logger.LogInformation("Using existing capability: {CapabilityKey} - {CapabilityName}",
                    capability.Id, capability.Name);
            }

            // Check if member already has this capability
            var existingMemberCapability = await _memberCapabilityRepository.GetByMemberAndCapabilityAsync(
                member.Id, capability.Id);

            if (existingMemberCapability == null)
            {
                // Create member capability assignment
                var memberCapability = new MemberCapability(
                    memberId: member.Id,
                    capabilityId: capability.Id,
                    validFrom: externalCapability.ValidFrom,
                    validTo: externalCapability.ValidTo,
                    assignedBy: externalCapability.AssignedBy ?? "external-system",
                    notes: externalCapability.Notes
                );
                await _memberCapabilityRepository.AddAsync(memberCapability);

                // Process member claims for this capability
                await ProcessMemberClaimsAsync(member, externalCapability.Claims);

                _logger.LogInformation("Assigned capability {CapabilityKey} to member {NationalCode}",
                    capability.Id, member.NationalCode?.Value);
            }
            else
            {
                // Update existing assignment if needed
                existingMemberCapability.UpdateValidityPeriod(
                    externalCapability.ValidFrom, externalCapability.ValidTo);
                existingMemberCapability.UpdateNotes(externalCapability.Notes);

                if (!existingMemberCapability.IsActive)
                {
                    existingMemberCapability.Activate();
                }

                _logger.LogInformation("Updated existing capability assignment {CapabilityKey} for member {NationalCode}",
                    capability.Id, member.NationalCode?.Value);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ProcessCapabilityClaimTypesAsync(Capability capability, List<ExternalClaimDto> externalClaims)
    {
        foreach (var externalClaim in externalClaims)
        {
            // Check if claim type already exists
            var existingClaimType = await _iFeatureRepository.FindOneAsync(ct => ct.Id == externalClaim.Value);

            Features claim;
            if (existingClaimType == null)
            {
                // Map external value kind to domain enum
            

                claim = new Features(
                  key: externalClaim.Value,
                    title: externalClaim.ClaimTypeTitle,
                    type: externalClaim.ClaimTypeKey
                );

               

                await _iFeatureRepository.AddAsync(claim);

                _logger.LogInformation("Created new claim type: {ClaimTypeKey} - {ClaimTypeTitle}",
                    claim.Id, claim.Title);
            }
            else
            {
                claim = existingClaimType;

               
            }

            // Add claim type to capability if not already present
            if (!capability.HasFeature(claim.Id))
            {
                capability.AddFeature(claim);
            }
        }
    }

    private async Task ProcessMemberClaimsAsync(Member member, List<ExternalClaimDto> externalClaims)
    {
        // Since Member doesn't have AddClaim method, the claim management is handled at the capability level
        // The claims are already processed when creating/updating capabilities
        // This method can be used for additional member-specific claim processing if needed in the future
        await Task.CompletedTask;
    }

    private async Task ProcessRolesAsync(Member member, List<ExternalRoleDto> externalRoles)
    {
        foreach (var externalRole in externalRoles)
        {
            var existingRole = await _roleRepository.FindOneAsync(r => r.Key == externalRole.Key);

            Role role;
            if (existingRole == null)
            {
                role = new Role(externalRole.Key, externalRole.Name ?? externalRole.Key);
                await _roleRepository.AddAsync(role);
                await _unitOfWork.SaveChangesAsync(); // Save to get role ID

                _logger.LogInformation("Created new role: {RoleKey} - {RoleTitle}", role.Key, role.Title);
            }
            else
            {
                role = existingRole;
            }

            // Assign role to member
            member.AssignRole(role.Id, assignedBy: externalRole.AssignedBy ?? "external-system");

            _logger.LogInformation("Assigned role {RoleKey} to member {NationalCode}",
                role.Key, member.NationalCode?.Value);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetMemberCapabilitiesAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting capabilities for member with national code: {NationalCode}", nationalCode.Value);

            var member = await _memberRepository.FindOneAsync(x=>x.NationalCode.Value == nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found with national code: {NationalCode}", nationalCode.Value);
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
            _logger.LogError(ex, "Error getting capabilities for member with national code: {NationalCode}",
                nationalCode.Value);
            return new List<string>();
        }
    }

    public async Task<IEnumerable<string>> GetMemberFeaturesAsync(NationalId nationalCode)
    {
        try
        {
            _logger.LogDebug("Getting features for member with national code: {NationalCode}", nationalCode.Value);

            var member = await _memberRepository.FindOneAsync(x=>x.NationalCode.Value==nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found with national code: {NationalCode}", nationalCode.Value);
                return new List<string>();
            }

            // Get features through member capabilities
            var validCapabilities = member.GetValidCapabilities();
            var features = new List<string>();

            foreach (var memberCapability in validCapabilities)
            {
                if (memberCapability.Capability?.Features != null)
                {
                    features.AddRange(memberCapability.Capability.Features.Select(f => f.Id));
                }
            }

            // Note: If there are direct member features, add them here
            // This implementation assumes features come through capabilities

            var distinctFeatures = features.Distinct().ToList();

            _logger.LogDebug("Found {Count} features for member {NationalCode}",
                distinctFeatures.Count, nationalCode.Value);

            return distinctFeatures;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting features for member with national code: {NationalCode}",
                nationalCode.Value);
            return new List<string>();
        }
    }

    public async Task<bool> HasCapabilityAsync(NationalId nationalCode, string capabilityId)
    {
        try
        {
            _logger.LogDebug("Checking capability {CapabilityId} for member {NationalCode}",
                capabilityId, nationalCode.Value);

            var member = await _memberRepository.FindOneAsync(x=>x.NationalCode.Value==nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found with national code: {NationalCode}", nationalCode.Value);
                return false;
            }

            var hasCapability = member.HasCapability(capabilityId);

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

            var member = await _memberRepository.FindOneAsync(x=>x.NationalCode.Value==nationalCode.Value);
            if (member == null)
            {
                _logger.LogWarning("Member not found with national code: {NationalCode}", nationalCode.Value);
                return false;
            }

            var hasFeature = member.HasClaimType(featureId);

            _logger.LogDebug("Member {NationalCode} {HasFeature} feature {FeatureId}",
                nationalCode.Value, hasFeature ? "has" : "does not have", featureId);

            return hasFeature;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking feature {FeatureId} for member {NationalCode}",
                featureId, nationalCode.Value);
            return false;
        }
    }

    private static MemberDto MapToMemberDto(Domain.Entities.Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            MembershipNumber = member.MembershipNumber,
            FirstName = member.FullName.FirstName,
            LastName = member.FullName.LastName,
            NationalCode = member.NationalCode?.Value ?? string.Empty,
            PhoneNumber = member.PhoneNumber?.Value,
            Email = member.Email?.Value,
            BirthDate = member.BirthDate,
            IsActive = true, // Note: Member entity doesn't have IsActive property
            CreatedAt = member.CreatedAt,
            CreatedBy = member.CreatedBy,
            ModifiedAt = member.LastModifiedAt,
            ModifiedBy = member.LastModifiedBy
        };
    }
}