using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Entities;

namespace Nezam.Refahi.Facilities.Application.Mappers;

/// <summary>
/// Mapper for converting Facility entity to FacilityInfoDto
/// Provides simplified facility information for references and lists
/// </summary>
public sealed class FacilityInfoMapper : IMapper<Facility, FacilityInfoDto>
{
    /// <summary>
    /// Maps Facility entity to FacilityInfoDto
    /// </summary>
    /// <param name="source">Source Facility entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mapped FacilityInfoDto</returns>
    public Task<FacilityInfoDto> MapAsync(Facility source, CancellationToken cancellationToken = new CancellationToken())
    {
        var dto = new FacilityInfoDto
        {
            Id = source.Id,
            Name = source.Name,
            Code = source.Code,
            Type = source.Type.ToString(),
            TypeText = EnumTextMappingService.GetFacilityTypeDescription(source.Type),
            Status = source.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityStatusDescription(source.Status),
            Description = source.Description,
            BankInfo = new BankInfoDto
            {
                BankName = source.BankName,
                BankCode = source.BankCode,
                BankAccountNumber = source.BankAccountNumber
            },
            IsActive = EnumTextMappingService.IsFacilityActive(source.Status)
        };

        return Task.FromResult(dto);
    }

    /// <summary>
    /// Maps Facility entity to existing FacilityInfoDto instance
    /// </summary>
    /// <param name="source">Source Facility entity</param>
    /// <param name="destination">Destination FacilityInfoDto</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the mapping operation</returns>
    public Task MapAsync(Facility source, FacilityInfoDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        // Note: FacilityInfoDto is a record with init-only properties
        // This method is provided for interface compliance but cannot modify the destination
        // In practice, use the MapAsync(source, cancellationToken) overload for record DTOs
        return Task.CompletedTask;
    }
}
