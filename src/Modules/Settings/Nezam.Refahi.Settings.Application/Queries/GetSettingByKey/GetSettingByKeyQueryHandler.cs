using MediatR;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettingByKey;

/// <summary>
/// Handler for the GetSettingByKeyQuery
/// </summary>
public class GetSettingByKeyQueryHandler : IRequestHandler<GetSettingByKeyQuery, ApplicationResult<GetSettingByKeyResponse>>
{
    private readonly ISettingsRepository _settingsRepository;

    public GetSettingByKeyQueryHandler(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
    }

    public async Task<ApplicationResult<GetSettingByKeyResponse>> Handle(
        GetSettingByKeyQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get setting by key
            var setting = await _settingsRepository.GetSettingByKeyAsync(request.SettingKey);

            if (setting == null)
            {
                return ApplicationResult<GetSettingByKeyResponse>.Success(new GetSettingByKeyResponse
                {
                    Setting = null,
                    Found = false
                });
            }

            // Apply active filter if requested
            if (request.OnlyActive && !setting.IsActive)
            {
                return ApplicationResult<GetSettingByKeyResponse>.Success(new GetSettingByKeyResponse
                {
                    Setting = null,
                    Found = false
                });
            }

            // Map to response DTO
            var settingDto = MapToSettingDetailDto(setting);

            var response = new GetSettingByKeyResponse
            {
                Setting = settingDto,
                Found = true
            };

            return ApplicationResult<GetSettingByKeyResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<GetSettingByKeyResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to retrieve setting");
        }
    }

    private static SettingDetailDto MapToSettingDetailDto(Setting setting)
    {
        return new SettingDetailDto
        {
            Id = setting.Id,
            Key = setting.Key.Value,
            Value = setting.Value.RawValue,
            Type = setting.Value.Type,
            Description = setting.Description,
            IsReadOnly = setting.IsReadOnly,
            IsActive = setting.IsActive,
            DisplayOrder = setting.DisplayOrder,
            Category = new CategoryDetailDto
            {
                Id = setting.Category?.Id ?? Guid.Empty,
                Name = setting.Category?.Name ?? string.Empty,
                Description = setting.Category?.Description ?? string.Empty,
                DisplayOrder = setting.Category?.DisplayOrder ?? 0,
                IsActive = setting.Category?.IsActive ?? false
            },
            Section = new SectionDetailDto
            {
                Id = setting.Category?.Section?.Id ?? Guid.Empty,
                Name = setting.Category?.Section?.Name ?? string.Empty,
                Description = setting.Category?.Section?.Description ?? string.Empty,
                DisplayOrder = setting.Category?.Section?.DisplayOrder ?? 0,
                IsActive = setting.Category?.Section?.IsActive ?? false
            },
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            TypedValue = GetTypedValue(setting)
        };
    }

    private static object? GetTypedValue(Setting setting)
    {
        try
        {
            return setting.GetTypedValue<object>();
        }
        catch
        {
            return null;
        }
    }
}
