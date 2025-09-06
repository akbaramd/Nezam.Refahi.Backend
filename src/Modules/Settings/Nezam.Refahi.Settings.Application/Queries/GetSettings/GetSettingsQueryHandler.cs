using MediatR;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Settings.Application.Queries.GetSettings;

/// <summary>
/// Handler for the GetSettingsQuery
/// </summary>
public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, ApplicationResult<GetSettingsResponse>>
{
    private readonly ISettingsRepository _settingsRepository;

    public GetSettingsQueryHandler(ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
    }

    public async Task<ApplicationResult<GetSettingsResponse>> Handle(
        GetSettingsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get settings based on filters
            IEnumerable<Setting> settings;
            
            if (!string.IsNullOrWhiteSpace(request.SectionName) && !string.IsNullOrWhiteSpace(request.CategoryName))
            {
                settings = await _settingsRepository.GetSettingsBySectionAndCategoryAsync(request.SectionName, request.CategoryName);
            }
            else if (!string.IsNullOrWhiteSpace(request.SectionName))
            {
                settings = await _settingsRepository.GetSettingsBySectionAsync(request.SectionName);
            }
            else if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                // Note: This would require a method to get settings by category name
                // For now, we'll get all settings and filter by category
                settings = await _settingsRepository.GetAllSettingsAsync();
                settings = settings.Where(s => s.Category?.Name == request.CategoryName);
            }
            else if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                settings = await _settingsRepository.SearchSettingsAsync(request.SearchTerm);
            }
            else
            {
                settings = await _settingsRepository.GetAllSettingsAsync();
            }

            // Apply type filter
            if (request.Type.HasValue)
            {
                settings = settings.Where(s => s.Value.Type == request.Type.Value);
            }

            // Apply active filter
            if (request.OnlyActive)
            {
                settings = settings.Where(s => s.IsActive);
            }

            // Apply sorting
            var sortedSettings = ApplySorting(settings, request.SortBy, request.SortDescending);

            // Apply pagination
            var totalCount = sortedSettings.Count();
            var paginatedSettings = sortedSettings
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Build response
            var response = new GetSettingsResponse
            {
                Settings = paginatedSettings.Select(MapToSettingDto).ToList(),
                TotalCount = totalCount,
                Pagination = BuildPaginationInfo(request.PageNumber, request.PageSize, totalCount),
                Filters = BuildAppliedFilters(request)
            };

            return ApplicationResult<GetSettingsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<GetSettingsResponse>.Failure(
                new List<string> { ex.Message },
                "Failed to retrieve settings");
        }
    }

    private static IEnumerable<Setting> ApplySorting(IEnumerable<Setting> settings, string sortBy, bool sortDescending)
    {
        return sortBy.ToLower() switch
        {
            "key" => sortDescending ? settings.OrderByDescending(s => s.Key.Value) : settings.OrderBy(s => s.Key.Value),
            "value" => sortDescending ? settings.OrderByDescending(s => s.Value.RawValue) : settings.OrderBy(s => s.Value.RawValue),
            "description" => sortDescending ? settings.OrderByDescending(s => s.Description) : settings.OrderBy(s => s.Description),
            "modifiedat" => sortDescending ? settings.OrderByDescending(s => s.ModifiedAt) : settings.OrderBy(s => s.ModifiedAt),
            _ => sortDescending ? settings.OrderByDescending(s => s.DisplayOrder) : settings.OrderBy(s => s.DisplayOrder)
        };
    }

    private static PaginationInfo BuildPaginationInfo(int pageNumber, int pageSize, int totalCount)
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        return new PaginationInfo
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        };
    }

    private static AppliedFilters BuildAppliedFilters(GetSettingsQuery query)
    {
        return new AppliedFilters
        {
            SectionName = query.SectionName,
            CategoryName = query.CategoryName,
            SearchTerm = query.SearchTerm,
            Type = query.Type,
            OnlyActive = query.OnlyActive,
            SortBy = query.SortBy,
            SortDescending = query.SortDescending
        };
    }

    private static SettingDto MapToSettingDto(Setting setting)
    {
        return new SettingDto
        {
            Id = setting.Id,
            Key = setting.Key.Value,
            Value = setting.Value.RawValue,
            Type = setting.Value.Type,
            Description = setting.Description,
            IsReadOnly = setting.IsReadOnly,
            IsActive = setting.IsActive,
            DisplayOrder = setting.DisplayOrder,
            Category = new CategoryInfo
            {
                Id = setting.Category?.Id ?? Guid.Empty,
                Name = setting.Category?.Name ?? string.Empty,
                Description = setting.Category?.Description ?? string.Empty
            },
            Section = new SectionInfo
            {
                Id = setting.Category?.Section?.Id ?? Guid.Empty,
                Name = setting.Category?.Section?.Name ?? string.Empty,
                Description = setting.Category?.Section?.Description ?? string.Empty
            },
            ModifiedAt = DateTime.UtcNow
        };
    }
}
