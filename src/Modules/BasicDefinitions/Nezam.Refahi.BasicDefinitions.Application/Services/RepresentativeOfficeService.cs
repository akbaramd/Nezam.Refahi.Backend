using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Implementation of IRepresentativeOfficeService for inter-context communication
/// </summary>
public sealed class RepresentativeOfficeService : IRepresentativeOfficeService
{
    private readonly IRepresentativeOfficeRepository _officeRepository;
    private readonly IBasicDefinitionsUnitOfWork _unitOfWork;
    private readonly ILogger<RepresentativeOfficeService> _logger;

    public RepresentativeOfficeService(
        IRepresentativeOfficeRepository officeRepository,
        IBasicDefinitionsUnitOfWork unitOfWork,
        ILogger<RepresentativeOfficeService> logger)
    {
        _officeRepository = officeRepository ?? throw new ArgumentNullException(nameof(officeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

    public async Task<IEnumerable<RepresentativeOfficeDto>> GetAllOfficesAsync()
    {
        try
        {
            var offices = await _officeRepository.GetActiveOfficesAsync();
            return offices.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all offices");
            return Enumerable.Empty<RepresentativeOfficeDto>();
        }
    }

    public async Task<RepresentativeOfficeDto?> CreateOfficeAsync(RepresentativeOfficeDto officeDto)
    {
        try
        {
            // Validate business rules
            var existingOfficeByCode = await _officeRepository.GetByCodeAsync(officeDto.Code);
            if (existingOfficeByCode != null)
            {
                _logger.LogWarning("Attempted to create office with existing code {Code}", officeDto.Code);
                return null;
            }

            var existingOfficeByExternalCode = await _officeRepository.GetByExternalCodeAsync(officeDto.ExternalCode);
            if (existingOfficeByExternalCode != null)
            {
                _logger.LogWarning("Attempted to create office with existing external code {ExternalCode}", officeDto.ExternalCode);
                return null;
            }

            // Create new office
            var office = new RepresentativeOffice(
                officeDto.Code,
                officeDto.ExternalCode,
                officeDto.Name,
                officeDto.Address,
                officeDto.ManagerName,
                officeDto.ManagerPhone,
                officeDto.EstablishedDate);

            await _officeRepository.AddAsync(office);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(office);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating office with code {Code}", officeDto.Code);
            return null;
        }
    }

    public async Task<RepresentativeOfficeDto?> UpdateOfficeAsync(RepresentativeOfficeDto officeDto)
    {
        try
        {
            var office = await _officeRepository.FindOneAsync(x => x.Id == officeDto.Id);
            if (office == null)
            {
                _logger.LogWarning("Attempted to update non-existent office {OfficeId}", officeDto.Id);
                return null;
            }

            // Check for code conflicts (excluding current office)
            var existingOfficeByCode = await _officeRepository.GetByCodeAsync(officeDto.Code);
            if (existingOfficeByCode != null && existingOfficeByCode.Id != officeDto.Id)
            {
                _logger.LogWarning("Attempted to update office with existing code {Code}", officeDto.Code);
                return null;
            }

            var existingOfficeByExternalCode = await _officeRepository.GetByExternalCodeAsync(officeDto.ExternalCode);
            if (existingOfficeByExternalCode != null && existingOfficeByExternalCode.Id != officeDto.Id)
            {
                _logger.LogWarning("Attempted to update office with existing external code {ExternalCode}", officeDto.ExternalCode);
                return null;
            }

            // Update office properties
            office.UpdateDetails(
                officeDto.Code,
                officeDto.ExternalCode,
                officeDto.Name,
                officeDto.Address,
                officeDto.ManagerName,
                officeDto.ManagerPhone,
                officeDto.EstablishedDate);

            if (officeDto.IsActive != office.IsActive)
            {
                if (officeDto.IsActive)
                    office.Activate();
                else
                    office.Deactivate();
            }

            await _officeRepository.UpdateAsync(office);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(office);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating office {OfficeId}", officeDto.Id);
            return null;
        }
    }

    public async Task<bool> DeleteOfficeAsync(Guid officeId)
    {
        try
        {
            var office = await _officeRepository.FindOneAsync(x => x.Id == officeId);
            if (office == null)
            {
                _logger.LogWarning("Attempted to delete non-existent office {OfficeId}", officeId);
                return false;
            }

            // Soft delete by deactivating
            office.Deactivate();
            await _officeRepository.UpdateAsync(office);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting office {OfficeId}", officeId);
            return false;
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
            CreatedBy = office.CreatedBy,
            UpdatedAt = office.LastModifiedAt,
            UpdatedBy = office.LastModifiedBy
        };
    }
}
