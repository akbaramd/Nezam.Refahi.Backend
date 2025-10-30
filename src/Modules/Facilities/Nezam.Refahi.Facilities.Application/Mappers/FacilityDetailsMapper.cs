using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Facilities.Application.Dtos;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Services;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Mappers;

public sealed class FacilityDetailsMapper : IMapper<Facility, FacilityDetailsDto>
{
  private readonly IMapper<FacilityCycle, FacilityCycleWithUserDto> _cycleDetailsMapper;
  private readonly IMapper<FacilityFeature, FacilityFeatureDto> _featureMapper;
  private readonly IMapper<FacilityCapability, FacilityCapabilityPolicyDto> _policyMapper;
  private readonly IMapper<Facility, FacilityCycleStatisticsDto>  _cycleDetailsStatisticsMapper;

  public FacilityDetailsMapper(
    IMapper<FacilityCycle, FacilityCycleWithUserDto> cycleDetailsMapper,
    IMapper<FacilityFeature, FacilityFeatureDto> featureMapper,
    IMapper<FacilityCapability, FacilityCapabilityPolicyDto> policyMapper, IMapper<Facility, FacilityCycleStatisticsDto> cycleDetailsStatisticsMapper)
  {
    _cycleDetailsMapper = cycleDetailsMapper;
    _featureMapper = featureMapper;
    _policyMapper = policyMapper;
    _cycleDetailsStatisticsMapper = cycleDetailsStatisticsMapper;
  }

  public async Task<FacilityDetailsDto> MapAsync(Facility source, CancellationToken cancellationToken = new CancellationToken())
  {
    var cycleTasks = source.Cycles.Select(c => _cycleDetailsMapper.MapAsync(c, cancellationToken)).ToArray();
    var featureTasks = source.Features.Select(f => _featureMapper.MapAsync(f, cancellationToken)).ToArray();
    var policyTasks = source.CapabilityPolicies.Select(p => _policyMapper.MapAsync(p, cancellationToken)).ToArray();

    var cycles = await Task.WhenAll(cycleTasks);
    var features = await Task.WhenAll(featureTasks);
    var policies = await Task.WhenAll(policyTasks);

    return new FacilityDetailsDto
    {
      Id = source.Id,
      Name = source.Name,
      Code = source.Code,
      Type = source.Type.ToString(),
      Status = source.Status.ToString(),
      Description = source.Description,
      BankName = source.BankName,
      BankCode = source.BankCode,
      BankAccountNumber = source.BankAccountNumber,
      Cycles = cycles.ToList(),
      Features = features.ToList(),
      CapabilityPolicies = policies.ToList(),
      IsActive = EnumTextMappingService.IsFacilityActive(source.Status),
      TypeText = EnumTextMappingService.GetFacilityTypeDescription(source.Type),
      StatusText = EnumTextMappingService.GetFacilityStatusDescription(source.Status),
      BankInfo = new BankInfoDto
      {
        BankName = source.BankName,
        BankCode = source.BankCode,
        BankAccountNumber = source.BankAccountNumber
      },
      CycleStatistics = await _cycleDetailsStatisticsMapper.MapAsync(source, cancellationToken).ConfigureAwait(false),
      Metadata = source.Metadata,
      CreatedAt = source.CreatedAt,
      LastModifiedAt = source.LastModifiedAt
    };
  }

    public Task MapAsync(Facility source, FacilityDetailsDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }


}


