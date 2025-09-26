using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Application service implementation for Capability operations
/// </summary>
public sealed class CapabilityApplicationService : ICapabilityApplicationService
{
    private readonly ICapabilityRepository _capabilityRepository;
    private readonly IFeaturesRepository _featuresRepository;
    private readonly IBasicDefinitionsUnitOfWork _unitOfWork;
    private readonly ILogger<CapabilityApplicationService> _logger;

    public CapabilityApplicationService(
        ICapabilityRepository capabilityRepository,
        IFeaturesRepository featuresRepository,
        IBasicDefinitionsUnitOfWork unitOfWork,
        ILogger<CapabilityApplicationService> logger)
    {
        _capabilityRepository = capabilityRepository ?? throw new ArgumentNullException(nameof(capabilityRepository));
        _featuresRepository = featuresRepository ?? throw new ArgumentNullException(nameof(featuresRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<CapabilityDto>> GetCapabilityByIdAsync(string capabilityId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(capabilityId))
            {
                return ApplicationResult<CapabilityDto>.Failure(
                    "شناسه قابلیت نمی‌تواند خالی باشد");
            }

            var capability = await _capabilityRepository.FindOneAsync(x => x.Id == capabilityId);
            if (capability == null)
            {
                return ApplicationResult<CapabilityDto>.Failure(
                    "قابلیت با شناسه مشخص شده یافت نشد");
            }

            var dto = await MapToDtoAsync(capability);
            return ApplicationResult<CapabilityDto>.Success(dto, "اطلاعات قابلیت با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capability by ID {CapabilityId}", capabilityId);
            return ApplicationResult<CapabilityDto>.Failure(ex, "خطا در دریافت اطلاعات قابلیت");
        }
    }

    public async Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetCapabilitiesByNameAsync(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ApplicationResult<IEnumerable<CapabilityDto>>.Failure(
                    "نام قابلیت نمی‌تواند خالی باشد");
            }

            var capabilities = await _capabilityRepository.GetByNameAsync(name);
            var dtos = new List<CapabilityDto>();
            
            foreach (var capability in capabilities)
            {
                dtos.Add(await MapToDtoAsync(capability));
            }

            return ApplicationResult<IEnumerable<CapabilityDto>>.Success(dtos, "لیست قابلیت‌های مشخص شده با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capabilities by name {Name}", name);
            return ApplicationResult<IEnumerable<CapabilityDto>>.Failure(ex, "خطا در دریافت لیست قابلیت‌ها بر اساس نام");
        }
    }

    public async Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetActiveCapabilitiesAsync()
    {
        try
        {
            var capabilities = await _capabilityRepository.GetActiveCapabilitiesAsync();
            var dtos = new List<CapabilityDto>();
            
            foreach (var capability in capabilities)
            {
                dtos.Add(await MapToDtoAsync(capability));
            }

            return ApplicationResult<IEnumerable<CapabilityDto>>.Success(dtos, "لیست قابلیت‌های فعال با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active capabilities");
            return ApplicationResult<IEnumerable<CapabilityDto>>.Failure(ex, "خطا در دریافت لیست قابلیت‌های فعال");
        }
    }

    public async Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetAllCapabilitiesAsync()
    {
        try
        {
            var capabilities = await _capabilityRepository.GetActiveCapabilitiesAsync();
            var dtos = new List<CapabilityDto>();
            
            foreach (var capability in capabilities)
            {
                dtos.Add(await MapToDtoAsync(capability));
            }

            return ApplicationResult<IEnumerable<CapabilityDto>>.Success(dtos, "لیست تمام قابلیت‌ها با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all capabilities");
            return ApplicationResult<IEnumerable<CapabilityDto>>.Failure(ex, "خطا در دریافت لیست تمام قابلیت‌ها");
        }
    }

    public async Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetCapabilitiesByKeysAsync(IEnumerable<string> keys)
    {
        try
        {
            if (keys == null || !keys.Any())
            {
                return ApplicationResult<IEnumerable<CapabilityDto>>.Failure(
                    "لیست کلیدها نمی‌تواند خالی باشد");
            }

            var capabilities = await _capabilityRepository.GetByKeysAsync(keys);
            var dtos = new List<CapabilityDto>();
            
            foreach (var capability in capabilities)
            {
                dtos.Add(await MapToDtoAsync(capability));
            }

            return ApplicationResult<IEnumerable<CapabilityDto>>.Success(dtos, "لیست قابلیت‌های مشخص شده با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting capabilities by keys");
            return ApplicationResult<IEnumerable<CapabilityDto>>.Failure(ex, "خطا در دریافت قابلیت‌ها بر اساس کلیدها");
        }
    }

    public async Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetValidCapabilitiesAsync(DateTime date)
    {
        try
        {
            var capabilities = await _capabilityRepository.GetValidCapabilitiesAsync(date);
            var dtos = new List<CapabilityDto>();
            
            foreach (var capability in capabilities)
            {
                dtos.Add(await MapToDtoAsync(capability));
            }

            return ApplicationResult<IEnumerable<CapabilityDto>>.Success(dtos, "لیست قابلیت‌های معتبر با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting valid capabilities for date {Date}", date);
            return ApplicationResult<IEnumerable<CapabilityDto>>.Failure(ex, "خطا در دریافت قابلیت‌های معتبر");
        }
    }

    public async Task<ApplicationResult<CapabilityDto>> CreateCapabilityAsync(CapabilityDto capabilityDto)
    {
        try
        {
            // Check if capability already exists
            var existingCapability = await _capabilityRepository.ExistsAsync(capabilityDto.Id);
            if (existingCapability)
            {
                return ApplicationResult<CapabilityDto>.Failure(
                    "قابلیت با این شناسه قبلاً وجود دارد");
            }

            // Create new capability
            var capability = new Capability(
                capabilityDto.Id,
                capabilityDto.Name,
                capabilityDto.Description,
                capabilityDto.ValidFrom,
                capabilityDto.ValidTo);

            // Add features if provided
            if (capabilityDto.Features.Any())
            {
                var features = await _featuresRepository.GetByKeysAsync(capabilityDto.Features.Select(f => f.Id));
                foreach (var feature in features)
                {
                    capability.AddFeature(feature);
                }
            }

            await _capabilityRepository.AddAsync(capability);
            await _unitOfWork.SaveChangesAsync();

            var result = await MapToDtoAsync(capability);
            return ApplicationResult<CapabilityDto>.Success(result, "قابلیت با موفقیت ایجاد شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating capability with ID {CapabilityId}", capabilityDto.Id);
            return ApplicationResult<CapabilityDto>.Failure(ex, "خطا در ایجاد قابلیت");
        }
    }

    public async Task<ApplicationResult<CapabilityDto>> UpdateCapabilityAsync(CapabilityDto capabilityDto)
    {
        try
        {
            var capability = await _capabilityRepository.FindOneAsync(x => x.Id == capabilityDto.Id);
            if (capability == null)
            {
                return ApplicationResult<CapabilityDto>.Failure(
                    "قابلیت مورد نظر یافت نشد");
            }

            // Update capability properties
            capability.Update(
                capabilityDto.Name,
                capabilityDto.Description,
                capabilityDto.ValidFrom,
                capabilityDto.ValidTo);

            // Update active status
            if (capabilityDto.IsActive != capability.IsActive)
            {
                if (capabilityDto.IsActive)
                    capability.Activate();
                else
                    capability.Deactivate();
            }

            await _capabilityRepository.UpdateAsync(capability);
            await _unitOfWork.SaveChangesAsync();

            var result = await MapToDtoAsync(capability);
            return ApplicationResult<CapabilityDto>.Success(result, "قابلیت با موفقیت به‌روزرسانی شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating capability {CapabilityId}", capabilityDto.Id);
            return ApplicationResult<CapabilityDto>.Failure(ex, "خطا در به‌روزرسانی قابلیت");
        }
    }

    public async Task<ApplicationResult<bool>> DeleteCapabilityAsync(string capabilityId)
    {
        try
        {
            var capability = await _capabilityRepository.FindOneAsync(x => x.Id == capabilityId);
            if (capability == null)
            {
                return ApplicationResult<bool>.Failure(
                    "قابلیت مورد نظر یافت نشد");
            }

            await _capabilityRepository.DeleteAsync(capability);
            await _unitOfWork.SaveChangesAsync();

            return ApplicationResult<bool>.Success(true, "قابلیت با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting capability {CapabilityId}", capabilityId);
            return ApplicationResult<bool>.Failure(ex, "خطا در حذف قابلیت");
        }
    }

    public async Task<ApplicationResult<CapabilityDto>> AddFeaturesToCapabilityAsync(string capabilityId, IEnumerable<string> featureIds)
    {
        try
        {
            var capability = await _capabilityRepository.FindOneAsync(x => x.Id == capabilityId);
            if (capability == null)
            {
                return ApplicationResult<CapabilityDto>.Failure(
                    "قابلیت مورد نظر یافت نشد");
            }

            var features = await _featuresRepository.GetByKeysAsync(featureIds);
            foreach (var feature in features)
            {
                capability.AddFeature(feature);
            }

            await _capabilityRepository.UpdateAsync(capability);
            await _unitOfWork.SaveChangesAsync();

            var result = await MapToDtoAsync(capability);
            return ApplicationResult<CapabilityDto>.Success(result, "ویژگی‌ها با موفقیت به قابلیت اضافه شدند");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding features to capability {CapabilityId}", capabilityId);
            return ApplicationResult<CapabilityDto>.Failure(ex, "خطا در اضافه کردن ویژگی‌ها به قابلیت");
        }
    }

    public async Task<ApplicationResult<CapabilityDto>> RemoveFeaturesFromCapabilityAsync(string capabilityId, IEnumerable<string> featureIds)
    {
        try
        {
            var capability = await _capabilityRepository.FindOneAsync(x => x.Id == capabilityId);
            if (capability == null)
            {
                return ApplicationResult<CapabilityDto>.Failure(
                    "قابلیت مورد نظر یافت نشد");
            }

            foreach (var featureId in featureIds)
            {
                capability.RemoveFeature(featureId);
            }

            await _capabilityRepository.UpdateAsync(capability);
            await _unitOfWork.SaveChangesAsync();

            var result = await MapToDtoAsync(capability);
            return ApplicationResult<CapabilityDto>.Success(result, "ویژگی‌ها با موفقیت از قابلیت حذف شدند");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing features from capability {CapabilityId}", capabilityId);
            return ApplicationResult<CapabilityDto>.Failure(ex, "خطا در حذف ویژگی‌ها از قابلیت");
        }
    }

    private async Task<CapabilityDto> MapToDtoAsync(Capability capability)
    {
        var features = new List<FeaturesDto>();
        
        if (capability.Features.Any())
        {
            var featureDtos = capability.Features.Select(f => new FeaturesDto
            {
                Id = f.Id,
                Title = f.Title,
                Type = f.Type,
                CreatedAt = f.CreatedAt,
                CreatedBy = f.CreatedBy,
                UpdatedAt = f.LastModifiedAt,
                UpdatedBy = f.LastModifiedBy
            });
            features.AddRange(featureDtos);
        }

        return new CapabilityDto
        {
            Id = capability.Id,
            Name = capability.Name,
            Description = capability.Description,
            IsActive = capability.IsActive,
            ValidFrom = capability.ValidFrom,
            ValidTo = capability.ValidTo,
            Features = features,
            CreatedAt = capability.CreatedAt,
            CreatedBy = capability.CreatedBy,
            UpdatedAt = capability.LastModifiedAt,
            UpdatedBy = capability.LastModifiedBy
        };
    }
}
