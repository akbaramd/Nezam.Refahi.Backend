using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.BasicDefinitions.Domain.Entities;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Application service implementation for Features operations
/// </summary>
public sealed class FeaturesApplicationService : IFeaturesApplicationService
{
    private readonly IFeaturesRepository _featuresRepository;
    private readonly IBasicDefinitionsUnitOfWork _unitOfWork;
    private readonly ILogger<FeaturesApplicationService> _logger;

    public FeaturesApplicationService(
        IFeaturesRepository featuresRepository,
        IBasicDefinitionsUnitOfWork unitOfWork,
        ILogger<FeaturesApplicationService> logger)
    {
        _featuresRepository = featuresRepository ?? throw new ArgumentNullException(nameof(featuresRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<FeaturesDto>> GetFeatureByIdAsync(string featureId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(featureId))
            {
                return ApplicationResult<FeaturesDto>.Failure(
                    "شناسه ویژگی نمی‌تواند خالی باشد");
            }

            var feature = await _featuresRepository.FindOneAsync(x => x.Id == featureId);
            if (feature == null)
            {
                return ApplicationResult<FeaturesDto>.Failure(
                    "ویژگی با شناسه مشخص شده یافت نشد");
            }

            var dto = MapToDto(feature);
            return ApplicationResult<FeaturesDto>.Success(dto, "اطلاعات ویژگی با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting feature by ID {FeatureId}", featureId);
            return ApplicationResult<FeaturesDto>.Failure(ex, "خطا در دریافت اطلاعات ویژگی");
        }
    }

    public async Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetFeaturesByTypeAsync(string type)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return ApplicationResult<IEnumerable<FeaturesDto>>.Failure(
                    "نوع ویژگی نمی‌تواند خالی باشد");
            }

            var features = await _featuresRepository.GetByTypeAsync(type);
            var dtos = features.Select(MapToDto);
            return ApplicationResult<IEnumerable<FeaturesDto>>.Success(dtos, "لیست ویژگی‌های نوع مشخص شده با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting features by type {Type}", type);
            return ApplicationResult<IEnumerable<FeaturesDto>>.Failure(ex, "خطا در دریافت لیست ویژگی‌ها بر اساس نوع");
        }
    }

    public async Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetActiveFeaturesAsync()
    {
        try
        {
            var features = await _featuresRepository.GetActiveFeaturesAsync();
            var dtos = features.Select(MapToDto);
            return ApplicationResult<IEnumerable<FeaturesDto>>.Success(dtos, "لیست ویژگی‌های فعال با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active features");
            return ApplicationResult<IEnumerable<FeaturesDto>>.Failure(ex, "خطا در دریافت لیست ویژگی‌های فعال");
        }
    }

    public async Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetAllFeaturesAsync()
    {
        try
        {
            var features = await _featuresRepository.GetActiveFeaturesAsync();
            var dtos = features.Select(MapToDto);
            return ApplicationResult<IEnumerable<FeaturesDto>>.Success(dtos, "لیست تمام ویژگی‌ها با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all features");
            return ApplicationResult<IEnumerable<FeaturesDto>>.Failure(ex, "خطا در دریافت لیست تمام ویژگی‌ها");
        }
    }

    public async Task<ApplicationResult<IEnumerable<FeaturesDto>>> GetFeaturesByKeysAsync(IEnumerable<string> keys)
    {
        try
        {
            if (keys == null || !keys.Any())
            {
                return ApplicationResult<IEnumerable<FeaturesDto>>.Failure(
                    "لیست کلیدها نمی‌تواند خالی باشد");
            }

            var features = await _featuresRepository.GetByKeysAsync(keys);
            var dtos = features.Select(MapToDto);
            return ApplicationResult<IEnumerable<FeaturesDto>>.Success(dtos, "لیست ویژگی‌های مشخص شده با موفقیت دریافت شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting features by keys");
            return ApplicationResult<IEnumerable<FeaturesDto>>.Failure(ex, "خطا در دریافت ویژگی‌ها بر اساس کلیدها");
        }
    }

    public async Task<ApplicationResult<FeaturesDto>> CreateFeatureAsync(FeaturesDto featureDto)
    {
        try
        {
            // Check if feature already exists
            var existingFeature = await _featuresRepository.ExistsAsync(featureDto.Id);
            if (existingFeature)
            {
                return ApplicationResult<FeaturesDto>.Failure(
                    "ویژگی با این شناسه قبلاً وجود دارد");
            }

            // Create new feature
            var feature = new Features(featureDto.Id, featureDto.Title, featureDto.Type);

            await _featuresRepository.AddAsync(feature);
            await _unitOfWork.SaveChangesAsync();

            var result = MapToDto(feature);
            return ApplicationResult<FeaturesDto>.Success(result, "ویژگی با موفقیت ایجاد شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating feature with ID {FeatureId}", featureDto.Id);
            return ApplicationResult<FeaturesDto>.Failure(ex, "خطا در ایجاد ویژگی");
        }
    }

    public async Task<ApplicationResult<FeaturesDto>> UpdateFeatureAsync(FeaturesDto featureDto)
    {
        try
        {
            var feature = await _featuresRepository.FindOneAsync(x => x.Id == featureDto.Id);
            if (feature == null)
            {
                return ApplicationResult<FeaturesDto>.Failure(
                    "ویژگی مورد نظر یافت نشد");
            }

            // Update feature properties
            feature.Update(featureDto.Title, featureDto.Type);

            await _featuresRepository.UpdateAsync(feature);
            await _unitOfWork.SaveChangesAsync();

            var result = MapToDto(feature);
            return ApplicationResult<FeaturesDto>.Success(result, "ویژگی با موفقیت به‌روزرسانی شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating feature {FeatureId}", featureDto.Id);
            return ApplicationResult<FeaturesDto>.Failure(ex, "خطا در به‌روزرسانی ویژگی");
        }
    }

    public async Task<ApplicationResult<bool>> DeleteFeatureAsync(string featureId)
    {
        try
        {
            var feature = await _featuresRepository.FindOneAsync(x => x.Id == featureId);
            if (feature == null)
            {
                return ApplicationResult<bool>.Failure(
                    "ویژگی مورد نظر یافت نشد");
            }

            await _featuresRepository.DeleteAsync(feature);
            await _unitOfWork.SaveChangesAsync();

            return ApplicationResult<bool>.Success(true, "ویژگی با موفقیت حذف شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting feature {FeatureId}", featureId);
            return ApplicationResult<bool>.Failure(ex, "خطا در حذف ویژگی");
        }
    }

    private static FeaturesDto MapToDto(Features feature)
    {
        return new FeaturesDto
        {
            Id = feature.Id,
            Title = feature.Title,
            Type = feature.Type,
            CreatedAt = feature.CreatedAt,
            CreatedBy = feature.CreatedBy,
            UpdatedAt = feature.LastModifiedAt,
            UpdatedBy = feature.LastModifiedBy
        };
    }
}
