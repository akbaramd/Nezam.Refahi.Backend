using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Application.Mappers;

/// <summary>
/// Mapper for converting Member entity to MemberDetailDto
/// Handles mapping of member data including capabilities, features, roles, and agencies
/// </summary>
public class MemberDetailMapper : IMapper<Member, MemberDetailDto>
{
    public Task<MemberDetailDto> MapAsync(Member source, CancellationToken cancellationToken = default)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var destination = new MemberDetailDto();
        MapAsync(source, destination, cancellationToken);
        return Task.FromResult(destination);
    }

    public Task MapAsync(Member source, MemberDetailDto destination, CancellationToken cancellationToken = default)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        // Map basic member properties
        destination.Id = source.Id;
        destination.MembershipNumber = source.MembershipNumber;
        destination.FirstName = source.FullName?.FirstName ?? string.Empty;
        destination.LastName = source.FullName?.LastName ?? string.Empty;
        destination.NationalCode = source.NationalCode?.Value ?? string.Empty;
        destination.IsSpecial = source.IsSpecial;
        destination.PhoneNumber = source.PhoneNumber?.Value;
        destination.Email = source.Email;
        destination.BirthDate = source.BirthDate;
        destination.IsActive = true; // Member is considered active if it exists
        destination.CreatedAt = source.CreatedAt;
        destination.ModifiedAt = source.LastModifiedAt;
        
        // Map additional detail properties
        destination.ExternalUserId = source.ExternalUserId;
        
        // Map capabilities (get valid capabilities only)
        var validCapabilities = source.GetValidCapabilities();
        destination.Capabilities = validCapabilities
            .Select(mc => mc.CapabilityKey)
            .ToList();
        
        // Map features (get valid features only)
        var validFeatures = source.GetValidFeatures();
        destination.Features = validFeatures
            .Select(mf => mf.FeatureKey)
            .ToList();
        
        // Map roles (get valid roles only)
        var validRoles = source.GetValidRoles();
        destination.RoleIds = validRoles
            .Select(mr => mr.RoleId)
            .ToList();
        
        // Map agencies (get valid agencies only)
        var validAgencies = source.GetValidOfficeAccesses();
        destination.AgencyIds = validAgencies
            .Select(ma => ma.AgencyId)
            .ToList();
        
        return Task.CompletedTask;
    }
}
