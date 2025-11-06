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
  private readonly IMapper<Facility, FacilityCycleStatisticsDto>  _cycleDetailsStatisticsMapper;

  public FacilityDetailsMapper(
    IMapper<FacilityCycle, FacilityCycleWithUserDto> cycleDetailsMapper,
    IMapper<Facility, FacilityCycleStatisticsDto> cycleDetailsStatisticsMapper)
  {
    _cycleDetailsMapper = cycleDetailsMapper;
    _cycleDetailsStatisticsMapper = cycleDetailsStatisticsMapper;
  }

  public async Task<FacilityDetailsDto> MapAsync(Facility source, CancellationToken cancellationToken = new CancellationToken())
  {
    var cycleTasks = source.Cycles.Select(c => _cycleDetailsMapper.MapAsync(c, cancellationToken)).ToArray();
    var cycles = await Task.WhenAll(cycleTasks);

    return new FacilityDetailsDto
    {
      Id = source.Id,
      Name = source.Name,
      Code = source.Code,
      Description = source.Description,
      BankName = source.BankName,
      BankCode = source.BankCode,
      BankAccountNumber = source.BankAccountNumber,
      Cycles = cycles.ToList(),
      BankInfo = new BankInfoDto
      {
        BankName = source.BankName,
        BankCode = source.BankCode,
        BankAccountNumber = source.BankAccountNumber
      },
      CycleStatistics = await _cycleDetailsStatisticsMapper.MapAsync(source, cancellationToken).ConfigureAwait(false),
      CreatedAt = source.CreatedAt,
      LastModifiedAt = source.LastModifiedAt
    };
  }

    public Task MapAsync(Facility source, FacilityDetailsDto destination, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }


}


