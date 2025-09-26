using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Implementation of IRepresentativeOfficeService for inter-context communication
/// </summary>
public class RepresentativeOfficeService : IRepresentativeOfficeService
{
    private readonly IRepresentativeOfficeRepository _officeRepository;
    private readonly ILogger<RepresentativeOfficeService> _logger;

    public RepresentativeOfficeService(
        IRepresentativeOfficeRepository officeRepository,
        ILogger<RepresentativeOfficeService> logger)
    {
        _officeRepository = officeRepository;
        _logger = logger;
    }

    public async Task<RepresentativeOfficeDto?> GetOfficeByIdAsync(Guid officeId)
    {
        try
        {
            var office = await _officeRepository.FindOneAsync(x => x.Id == officeId);
            return office != null ? MapToDto(office) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting office by ID {OfficeId}", officeId);
            return null;
        }
    }

    public async Task<RepresentativeOfficeDto?> GetOfficeByCodeAsync(string officeCode)
    {
        try
        {
            var office = await _officeRepository.GetByCodeAsync(officeCode);
            return office != null ? MapToDto(office) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting office by code {OfficeCode}", officeCode);
            return null;
        }
    }

    public async Task<RepresentativeOfficeDto?> GetOfficeByExternalCodeAsync(string externalCode)
    {
        try
        {
            var office = await _officeRepository.GetByExternalCodeAsync(externalCode);
            return office != null ? MapToDto(office) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting office by external code {ExternalCode}", externalCode);
            return null;
        }
    }

    public async Task<IEnumerable<RepresentativeOfficeDto>> GetActiveOfficesAsync()
    {
        try
        {
            var offices = await _officeRepository.GetActiveOfficesAsync();
            return offices.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active offices");
            return Enumerable.Empty<RepresentativeOfficeDto>();
        }
    }

    private static RepresentativeOfficeDto MapToDto(RepresentativeOffice office)
    {
        return new RepresentativeOfficeDto
        {
            Id = office.Id,
            Code = office.Code,
            ExternalCode = office.ExternalCode,
            Name = office.Name,
            Address = office.Address,
            ManagerName = office.ManagerName,
            ManagerPhone = office.ManagerPhone,
            IsActive = office.IsActive,
            EstablishedDate = office.EstablishedDate,
            CreatedAt = office.CreatedAt,
            CreatedBy = office.CreatedBy
        };
    }
}
