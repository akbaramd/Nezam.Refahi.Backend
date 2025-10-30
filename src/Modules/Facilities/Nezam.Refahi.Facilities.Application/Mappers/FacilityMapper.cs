using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityMapper : IMapper<Facility, FacilityDto>
{
    private readonly IMapper<Facility, FacilityCycleStatisticsDto> _facilityCycleStatisticsMapper;

    public FacilityMapper(IMapper<Facility, FacilityCycleStatisticsDto> facilityCycleStatisticsMapper) => _facilityCycleStatisticsMapper = facilityCycleStatisticsMapper;

    public async Task<FacilityDto> MapAsync(Facility source, CancellationToken cancellationToken = new CancellationToken())
    {
        var activeCycles = source.Cycles.Where(c => c.Status == FacilityCycleStatus.Active).ToList();
        var totalActiveQuota = activeCycles.Sum(c => c.Quota);
        var totalUsedQuota = activeCycles.Sum(c => c.UsedQuota);
        var totalAvailableQuota = totalActiveQuota - totalUsedQuota;
        var quotaUtilizationPercentage = totalActiveQuota > 0 ? (decimal)totalUsedQuota / totalActiveQuota * 100 : 0;

        var dto = new FacilityDto
        {
            Id = source.Id,
            Name = source.Name,
            Code = source.Code,
            Type = source.Type.ToString(),
            TypeText = EnumTextMappingService.GetFacilityTypeDescription(source.Type),
            Status = source.Status.ToString(),
            StatusText = EnumTextMappingService.GetFacilityStatusDescription(source.Status),
            IsActive = EnumTextMappingService.IsFacilityActive(source.Status),
            Description = source.Description,
            BankInfo = new BankInfoDto
            {
                BankName = source.BankName,
                BankCode = source.BankCode,
                BankAccountNumber = source.BankAccountNumber
            },
            CycleStatistics = await _facilityCycleStatisticsMapper.MapAsync(source, cancellationToken).ConfigureAwait(false),
            Metadata = source.Metadata,
            CreatedAt = source.CreatedAt,
            LastModifiedAt = source.LastModifiedAt
        };

        return dto;
    }
            
    public async Task MapAsync(Facility source, FacilityDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        destination.Id = source.Id;
        destination.Name = source.Name;
        destination.Code = source.Code;
        destination.Type = source.Type.ToString();
        destination.TypeText = EnumTextMappingService.GetFacilityTypeDescription(source.Type);
        destination.Status = source.Status.ToString();
        destination.StatusText = EnumTextMappingService.GetFacilityStatusDescription(source.Status);
        destination.IsActive = EnumTextMappingService.IsFacilityActive(source.Status);
        destination.Description = source.Description;
        destination.BankInfo = new BankInfoDto
        {
            BankName = source.BankName,
            BankCode = source.BankCode,
            BankAccountNumber = source.BankAccountNumber
        };
        destination.CycleStatistics = await _facilityCycleStatisticsMapper.MapAsync(source, cancellationToken);
        destination.Metadata = source.Metadata;
        destination.CreatedAt = source.CreatedAt;
        destination.LastModifiedAt = source.LastModifiedAt;

        return;
    }
}


