using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Application.ReadModels;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Application.Services;

/// <summary>
/// Implementation of User-Member mapping service
/// Provides read-only access to User-Member relationships
/// </summary>
public class UserMemberMappingService : IUserMemberMappingService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<UserMemberMappingService> _logger;

    public UserMemberMappingService(
        IMemberRepository memberRepository,
        ILogger<UserMemberMappingService> logger)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserMemberReadModel?> GetMemberByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var member = await _memberRepository.GetByExternalUserIdAsync(userId, cancellationToken);
            if (member == null)
            {
                _logger.LogDebug("No member found for UserId: {UserId}", userId);
                return null;
            }

            return new UserMemberReadModel
            {
                UserId = userId,
                MemberId = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName?.FirstName,
                LastName = member.FullName?.LastName,
                NationalId = member.NationalCode?.Value,
                PhoneNumber = member.PhoneNumber?.Value,
                Email = member.Email?.Value,
                IsActive = true, // Member is always active by default
                CreatedAt = member.CreatedAt,
                LastUpdatedAt = member.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting member by UserId: {UserId}", userId);
            throw;
        }
    }

    public async Task<UserMemberReadModel?> GetUserByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        try
        {
            var member = await _memberRepository.GetByIdAsync(memberId, cancellationToken);
            if (member == null)
            {
                _logger.LogDebug("No member found for MemberId: {MemberId}", memberId);
                return null;
            }

            return new UserMemberReadModel
            {
                UserId = member.ExternalUserId,
                MemberId = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName?.FirstName,
                LastName = member.FullName?.LastName,
                NationalId = member.NationalCode?.Value,
                PhoneNumber = member.PhoneNumber?.Value,
                Email = member.Email?.Value,
                IsActive = true, // Member is always active by default
                CreatedAt = member.CreatedAt,
                LastUpdatedAt = member.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by MemberId: {MemberId}", memberId);
            throw;
        }
    }

    public async Task<bool> HasMemberAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var member = await _memberRepository.GetByExternalUserIdAsync(userId, cancellationToken);
            return member != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has member: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<UserMemberReadModel>> GetAllMappingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var members = await _memberRepository.GetAllAsync(m => m);
            
            return members.Select(member => new UserMemberReadModel
            {
                UserId = member.ExternalUserId,
                MemberId = member.Id,
                MembershipNumber = member.MembershipNumber,
                FirstName = member.FullName?.FirstName,
                LastName = member.FullName?.LastName,
                NationalId = member.NationalCode?.Value,
                PhoneNumber = member.PhoneNumber?.Value,
                Email = member.Email?.Value,
                IsActive = true, // Member is always active by default
                CreatedAt = member.CreatedAt,
                LastUpdatedAt = member.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all user-member mappings");
            throw;
        }
    }
}
