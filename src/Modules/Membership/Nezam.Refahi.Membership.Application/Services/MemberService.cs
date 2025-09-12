using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Membership.Domain.Enums;
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
    private readonly IMemberClaimRepository _memberClaimRepository;
    private readonly IClaimTypeRepository _claimTypeRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IExternalMemberStorage _externalEngineerService;
    private readonly IMembershipUnitOfWork _unitOfWork;
    private readonly ILogger<MemberService> _logger;

    public MemberService(
        IMemberRepository memberRepository,
        IExternalMemberStorage externalEngineerService,
        IMembershipUnitOfWork unitOfWork,
        ILogger<MemberService> logger, IMemberRoleRepository memberRoleRepository, IMemberClaimRepository memberClaimRepository, IRoleRepository roleRepository, IClaimTypeRepository claimTypeRepository)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _externalEngineerService = externalEngineerService ?? throw new ArgumentNullException(nameof(externalEngineerService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memberRoleRepository = memberRoleRepository;
        _memberClaimRepository = memberClaimRepository;
        _roleRepository = roleRepository;
        _claimTypeRepository = claimTypeRepository;
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
      // Save to database
      
      
      

      
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


        
        
        
        var member = new Member(
          membershipNumber: externalMember.MembershipCode,
          nationalCode: nationalCode,
          fullName: fullName,
          email: email,
          phoneNumber: phoneNumber ?? new PhoneNumber("0000000000") // Provide default if null
        );


        foreach (var claim in externalMember.Claims)
        {
          var claimType = await _claimTypeRepository.FindOneAsync(x=>x.Key == claim.Type);
          if (claimType == null)
          {
             claimType = new ClaimType(claim.Type, claim.Title, ClaimValueKind.String);
            await _claimTypeRepository.AddAsync(claimType);
          }
          
          member.AddClaim(claimType.Id,claim.Value);
        }

        await _unitOfWork.SaveChangesAsync();

    
        foreach (var role in externalMember.Roles)
        {
          var getRile = await _roleRepository.FindOneAsync(x=>x.Key == role.Key);
          if (getRile == null)
          {
            getRile = new Role(role.Key, role.Name);
            await _roleRepository.AddAsync(getRile);
          }
          
          member.AssignRole(getRile.Id);
        }


        await _unitOfWork.SaveChangesAsync();
        // Note: Service fields and types are handled as claims since those entities don't exist
        // Add service information as claims instead
      
        // Note: License information would be added as claims, but ClaimType entities need to exist first
        // This functionality is temporarily disabled until proper claim type management is implemented

        // Note: Member entity doesn't have Activate/Deactivate methods
        // These would need to be added to the domain model if required

        // Note: Member entity doesn't have SetNotes or SetMembershipDates methods
        // These would need to be added to the domain model if required
        await _memberRepository.AddAsync(member);
        await _unitOfWork.SaveAsync();
        await _unitOfWork.CommitAsync();
        return member;
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
            ModifiedAt = member.ModifiedAt,
            ModifiedBy = member.ModifiedBy
        };
    }
}