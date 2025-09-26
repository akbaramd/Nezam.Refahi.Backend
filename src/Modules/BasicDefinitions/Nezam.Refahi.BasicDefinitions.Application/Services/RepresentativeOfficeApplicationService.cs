using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Application service implementation for RepresentativeOffice operations
/// </summary>
public sealed class RepresentativeOfficeApplicationService : IRepresentativeOfficeApplicationService
{
    private readonly IRepresentativeOfficeRepository _officeRepository;
    private readonly IBasicDefinitionsUnitOfWork _unitOfWork;
    private readonly ILogger<RepresentativeOfficeApplicationService> _logger;

    public RepresentativeOfficeApplicationService(
        IRepresentativeOfficeRepository officeRepository,
        IBasicDefinitionsUnitOfWork unitOfWork,
        ILogger<RepresentativeOfficeApplicationService> logger)
    {
        _officeRepository = officeRepository ?? throw new ArgumentNullException(nameof(officeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<RepresentativeOfficeDto>> GetOfficeByIdAsync(Guid officeId)
    {
        try
        {
            var office = await _officeRepository.FindOneAsync(x => x.Id == officeId);
            if (office == null)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی با شناسه مشخص شده یافت نشد");
            }

            var dto = MapToDto(office);
            return ApplicationResult<RepresentativeOfficeDto>.Success(dto, "اطلاعات دفتر نمایندگی با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting office by ID {OfficeId}", officeId);
            return ApplicationResult<RepresentativeOfficeDto>.Failure(ex, "خطا در دریافت اطلاعات دفتر نمایندگی");
        }
    }

    public async Task<ApplicationResult<RepresentativeOfficeDto>> GetOfficeByCodeAsync(string officeCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(officeCode))
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "کد دفتر نمایندگی نمی‌تواند خالی باشد");
            }

            var office = await _officeRepository.GetByCodeAsync(officeCode);
            if (office == null)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی با کد مشخص شده یافت نشد");
            }

            var dto = MapToDto(office);
            return ApplicationResult<RepresentativeOfficeDto>.Success(dto, "اطلاعات دفتر نمایندگی با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting office by code {OfficeCode}", officeCode);
            return ApplicationResult<RepresentativeOfficeDto>.Failure(ex, "خطا در دریافت اطلاعات دفتر نمایندگی");
        }
    }

    public async Task<ApplicationResult<RepresentativeOfficeDto>> GetOfficeByExternalCodeAsync(string externalCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(externalCode))
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "کد خارجی دفتر نمایندگی نمی‌تواند خالی باشد");
            }

            var office = await _officeRepository.GetByExternalCodeAsync(externalCode);
            if (office == null)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی با کد خارجی مشخص شده یافت نشد");
            }

            var dto = MapToDto(office);
            return ApplicationResult<RepresentativeOfficeDto>.Success(dto, "اطلاعات دفتر نمایندگی با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting office by external code {ExternalCode}", externalCode);
            return ApplicationResult<RepresentativeOfficeDto>.Failure(ex, "خطا در دریافت اطلاعات دفتر نمایندگی");
        }
    }

    public async Task<ApplicationResult<IEnumerable<RepresentativeOfficeDto>>> GetActiveOfficesAsync()
    {
        try
        {
            var offices = await _officeRepository.GetActiveOfficesAsync();
            var dtos = offices.Select(MapToDto);
            return ApplicationResult<IEnumerable<RepresentativeOfficeDto>>.Success(dtos, "لیست دفاتر نمایندگی فعال با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active offices");
            return ApplicationResult<IEnumerable<RepresentativeOfficeDto>>.Failure(ex, "خطا در دریافت لیست دفاتر نمایندگی فعال");
        }
    }

    public async Task<ApplicationResult<IEnumerable<RepresentativeOfficeDto>>> GetAllOfficesAsync()
    {
        try
        {
            var offices = await _officeRepository.GetActiveOfficesAsync();
            var dtos = offices.Select(MapToDto);
            return ApplicationResult<IEnumerable<RepresentativeOfficeDto>>.Success(dtos, "لیست تمام دفاتر نمایندگی با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all offices");
            return ApplicationResult<IEnumerable<RepresentativeOfficeDto>>.Failure(ex, "خطا در دریافت لیست تمام دفاتر نمایندگی");
        }
    }

    public async Task<ApplicationResult<RepresentativeOfficeDto>> CreateOfficeAsync(RepresentativeOfficeDto officeDto)
    {
        try
        {
            // Validate business rules
            var existingOfficeByCode = await _officeRepository.GetByCodeAsync(officeDto.Code);
            if (existingOfficeByCode != null)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی با این کد قبلاً وجود دارد");
            }

            var existingOfficeByExternalCode = await _officeRepository.GetByExternalCodeAsync(officeDto.ExternalCode);
            if (existingOfficeByExternalCode != null)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی با این کد خارجی قبلاً وجود دارد");
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

            var result = MapToDto(office);
            return ApplicationResult<RepresentativeOfficeDto>.Success(result, "دفتر نمایندگی با موفقیت ایجاد شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating office with code {Code}", officeDto.Code);
            return ApplicationResult<RepresentativeOfficeDto>.Failure(ex, "خطا در ایجاد دفتر نمایندگی");
        }
    }

    public async Task<ApplicationResult<RepresentativeOfficeDto>> UpdateOfficeAsync(RepresentativeOfficeDto officeDto)
    {
        try
        {
            var office = await _officeRepository.FindOneAsync(x => x.Id == officeDto.Id);
            if (office == null)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی مورد نظر یافت نشد");
            }

            // Check for code conflicts (excluding current office)
            var existingOfficeByCode = await _officeRepository.GetByCodeAsync(officeDto.Code);
            if (existingOfficeByCode != null && existingOfficeByCode.Id != officeDto.Id)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی با این کد قبلاً وجود دارد");
            }

            var existingOfficeByExternalCode = await _officeRepository.GetByExternalCodeAsync(officeDto.ExternalCode);
            if (existingOfficeByExternalCode != null && existingOfficeByExternalCode.Id != officeDto.Id)
            {
                return ApplicationResult<RepresentativeOfficeDto>.Failure(
                    "دفتر نمایندگی با این کد خارجی قبلاً وجود دارد");
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

            var result = MapToDto(office);
            return ApplicationResult<RepresentativeOfficeDto>.Success(result, "دفتر نمایندگی با موفقیت به‌روزرسانی شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating office {OfficeId}", officeDto.Id);
            return ApplicationResult<RepresentativeOfficeDto>.Failure(ex, "خطا در به‌روزرسانی دفتر نمایندگی");
        }
    }

    public async Task<ApplicationResult<bool>> DeleteOfficeAsync(Guid officeId)
    {
        try
        {
            var office = await _officeRepository.FindOneAsync(x => x.Id == officeId);
            if (office == null)
            {
                return ApplicationResult<bool>.Failure(
                    "دفتر نمایندگی مورد نظر یافت نشد");
            }

            // Soft delete by deactivating
            office.Deactivate();
            await _officeRepository.UpdateAsync(office);
            await _unitOfWork.SaveChangesAsync();

            return ApplicationResult<bool>.Success(true, "دفتر نمایندگی با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting office {OfficeId}", officeId);
            return ApplicationResult<bool>.Failure(ex, "خطا در حذف دفتر نمایندگی");
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
