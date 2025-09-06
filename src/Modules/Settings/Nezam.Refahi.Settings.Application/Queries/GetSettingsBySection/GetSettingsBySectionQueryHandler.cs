using MediatR;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettingsBySection;

/// <summary>
/// Handler for the GetSettingsBySectionQuery
/// </summary>
public class GetSettingsBySectionQueryHandler : IRequestHandler<GetSettingsBySectionQuery, ApplicationResult<GetSettingsBySectionResponse>>
{
    private readonly ISettingsRepository _settingsRepository;

    public GetSettingsBySectionQueryHandler(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
    }

    public async Task<ApplicationResult<GetSettingsBySectionResponse>> Handle(
        GetSettingsBySectionQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get sections
            IEnumerable<SettingsSection> sections;
            if (!string.IsNullOrWhiteSpace(request.SectionName))
            {
                var section = await _settingsRepository.GetSectionByNameAsync(request.SectionName);
                sections = section != null ? new[] { section } : Enumerable.Empty<SettingsSection>();
            }
            else
            {
                sections = request.OnlyActive 
                    ? await _settingsRepository.GetActiveSectionsAsync()
                    : await _settingsRepository.GetAllSectionsAsync();
            }

            // Build response
            var response = new GetSettingsBySectionResponse
            {
                Sections = new List<SectionWithSettingsDto>(),
                TotalSections = 0,
                TotalCategories = 0,
                TotalSettings = 0
            };

            foreach (var section in sections.OrderBy(s => s.DisplayOrder))
            {
                var sectionDto = await BuildSectionDto(section, request.OnlyActive, request.IncludeEmpty);
                if (sectionDto != null)
                {
                    response.Sections.Add(sectionDto);
                    response.TotalSections++;
                    response.TotalCategories += sectionDto.CategoryCount;
                    response.TotalSettings += sectionDto.TotalSettings;
                }
            }

            return ApplicationResult<GetSettingsBySectionResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<GetSettingsBySectionResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to retrieve settings by section");
        }
    }

    private async Task<SectionWithSettingsDto?> BuildSectionDto(SettingsSection section, bool onlyActive, bool includeEmpty)
    {
        // Get categories for this section
        var categories = onlyActive 
            ? await _settingsRepository.GetActiveCategoriesAsync(section.Id)
            : await _settingsRepository.GetCategoriesBySectionAsync(section.Id);

        var categoryDtos = new List<CategoryWithSettingsDto>();
        var totalSettings = 0;

        foreach (var category in categories.OrderBy(c => c.DisplayOrder))
        {
            var categoryDto = await BuildCategoryDto(category, onlyActive);
            if (categoryDto != null)
            {
                categoryDtos.Add(categoryDto);
                totalSettings += categoryDto.SettingCount;
            }
        }

        // Skip empty sections if not requested
        if (!includeEmpty && totalSettings == 0)
            return null;

        return new SectionWithSettingsDto
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description,
            DisplayOrder = section.DisplayOrder,
            IsActive = section.IsActive,
            Categories = categoryDtos,
            CategoryCount = categoryDtos.Count,
            TotalSettings = totalSettings
        };
    }

    private async Task<CategoryWithSettingsDto?> BuildCategoryDto(SettingsCategory category, bool onlyActive)
    {
        // Get settings for this category
        var settings = onlyActive 
            ? await _settingsRepository.GetActiveSettingsAsync(category.Id)
            : await _settingsRepository.GetSettingsByCategoryAsync(category.Id);

        var settingDtos = settings
            .OrderBy(s => s.DisplayOrder)
            .Select(MapToSimpleSettingDto)
            .ToList();

        return new CategoryWithSettingsDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            Settings = settingDtos,
            SettingCount = settingDtos.Count
        };
    }

    private static SimpleSettingDto MapToSimpleSettingDto(Setting setting)
    {
        return new SimpleSettingDto
        {
            Id = setting.Id,
            Key = setting.Key.Value,
            Value = setting.Value.RawValue,
            Type = setting.Value.Type,
            Description = setting.Description,
            IsReadOnly = setting.IsReadOnly,
            IsActive = setting.IsActive,
            DisplayOrder = setting.DisplayOrder
        };
    }
}
