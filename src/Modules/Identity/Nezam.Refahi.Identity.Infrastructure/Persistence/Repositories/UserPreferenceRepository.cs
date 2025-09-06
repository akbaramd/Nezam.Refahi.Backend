using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Identity.Domain.Services;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of user preference repository interface using EF Core
/// </summary>
public class UserPreferenceRepository : EfRepository<IdentityDbContext, UserPreference, Guid>, IUserPreferenceRepository
{
    public UserPreferenceRepository(IdentityDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<UserPreference?> GetByUserAndKeyAsync(Guid userId, PreferenceKey key)
    {
        return await _dbContext.UserPreferences
            .FirstOrDefaultAsync(up => up.UserId == userId && up.Key.Value == key.Value);
    }

    public async Task<UserPreference?> GetByUserAndKeyAsync(Guid userId, string key)
    {
        return await _dbContext.UserPreferences
            .FirstOrDefaultAsync(up => up.UserId == userId && up.Key.Value == key);
    }

    public async Task<IEnumerable<UserPreference>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId)
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPreference>> GetActiveByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && up.IsActive)
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPreference>> GetByUserAndKeysAsync(Guid userId, IEnumerable<string> keys)
    {
        var keyList = keys.ToList();
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && keyList.Contains(up.Key.Value))
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPreference>> GetByUserAndTypeAsync(Guid userId, PreferenceType type)
    {
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && up.Value.Type == type)
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid userId, PreferenceKey key)
    {
        return await _dbContext.UserPreferences
            .AnyAsync(up => up.UserId == userId && up.Key.Value == key.Value);
    }

    public async Task<bool> ExistsAsync(Guid userId, string key)
    {
        return await _dbContext.UserPreferences
            .AnyAsync(up => up.UserId == userId && up.Key.Value == key);
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserPreferences
            .CountAsync(up => up.UserId == userId);
    }

    public async Task<int> GetActiveCountByUserIdAsync(Guid userId)
    {
        return await _dbContext.UserPreferences
            .CountAsync(up => up.UserId == userId && up.IsActive);
    }

    public async Task<IEnumerable<UserPreference>> SearchByDescriptionAsync(Guid userId, string searchTerm)
    {
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && up.Description.Contains(searchTerm))
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }



    public async Task<int> DeleteByUserIdAsync(Guid userId)
    {
        var preferences = await _dbContext.UserPreferences
            .Where(up => up.UserId == userId)
            .ToListAsync();

        _dbContext.UserPreferences.RemoveRange(preferences);
        return preferences.Count;
    }

    public async Task<int> DeactivateByUserIdAsync(Guid userId)
    {
        var preferences = await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && up.IsActive)
            .ToListAsync();

        foreach (var preference in preferences)
        {
            preference.Deactivate();
        }

        return preferences.Count;
    }

    public async Task<IEnumerable<UserPreference>> GetByValueAsync(Guid userId, string key, string value)
    {
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && up.Key.Value == key && up.Value.RawValue == value)
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPreference>> GetByUserAndCategoryAsync(Guid userId, PreferenceCategory category)
    {
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && up.Category == category)
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserPreference>> GetActiveByUserAndCategoryAsync(Guid userId, PreferenceCategory category)
    {
        return await _dbContext.UserPreferences
            .Where(up => up.UserId == userId && up.Category == category && up.IsActive)
            .OrderBy(up => up.DisplayOrder)
            .ToListAsync();
    }

    public async Task<int> GetCountByUserAndCategoryAsync(Guid userId, PreferenceCategory category)
    {
        return await _dbContext.UserPreferences
            .CountAsync(up => up.UserId == userId && up.Category == category);
    }

    public async Task<int> GetActiveCountByUserAndCategoryAsync(Guid userId, PreferenceCategory category)
    {
        return await _dbContext.UserPreferences
            .CountAsync(up => up.UserId == userId && up.Category == category && up.IsActive);
    }
}
