using Nezam.Refahi.Membership.Contracts.Dtos;

namespace Nezam.Refahi.Membership.Contracts.Services;

public interface IExternalMemberStorage
{
    Task<ExternalMemberResponseDto?> GetMemberByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default);
    
    Task<ExternalMemberResponseDto?> GetMemberByMembershipCodeAsync(string membershipCode, CancellationToken cancellationToken = default);
    
    Task<bool> ValidateMemberAsync(string nationalCode, string membershipCode, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<ExternalMemberResponseDto>> SearchMembersAsync(ExternalMemberSearchCriteria criteria, CancellationToken cancellationToken = default);
}