using MCA.SharedKernel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Settings.Domain.Entities;
using Nezam.Refahi.Settings.Domain.Repositories;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for SettingChangeEvent (event sourcing)
/// </summary>
public class SettingChangeEventRepository : EfRepository<SettingsDbContext, SettingChangeEvent, Guid>, ISettingChangeEventRepository
{
    public SettingChangeEventRepository(SettingsDbContext dbContext) : base(dbContext)
    {
    }

    /// <summary>
    /// Adds a new change event
    /// </summary>
    public async Task<SettingChangeEvent> AddAsync(SettingChangeEvent changeEvent)
    {
        var entity = await _dbContext.SettingChangeEvents.AddAsync(changeEvent);
        return entity.Entity;
    }

    /// <summary>
    /// Gets a change event by ID
    /// </summary>
    public async Task<SettingChangeEvent?> GetByIdAsync(Guid id)
    {
        return await _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Gets all change events for a specific setting
    /// </summary>
    public async Task<IEnumerable<SettingChangeEvent>> GetBySettingIdAsync(Guid settingId)
    {
        return await _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(e => e.SettingId == settingId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all change events for a specific user
    /// </summary>
    public async Task<IEnumerable<SettingChangeEvent>> GetByUserIdAsync(Guid userId)
    {
                return await _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(e => e.ChangedByUserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all change events within a date range
    /// </summary>
    public async Task<IEnumerable<SettingChangeEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all change events for a specific setting key
    /// </summary>
    public async Task<IEnumerable<SettingChangeEvent>> GetBySettingKeyAsync(string settingKey)
    {
                return await _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(e => e.Setting.Key == settingKey)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the latest change event for a specific setting
    /// </summary>
    public async Task<SettingChangeEvent?> GetLatestBySettingIdAsync(Guid settingId)
    {
        return await _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .Where(e => e.SettingId == settingId)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets change events with pagination
    /// </summary>
    public async Task<(IEnumerable<SettingChangeEvent> Events, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Guid? settingId = null, 
        Guid? userId = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null)
    {
        var query = _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .AsQueryable();

        // Apply filters
        if (settingId.HasValue)
            query = query.Where(e => e.SettingId == settingId.Value);

        if (userId.HasValue)
            query = query.Where(e => e.ChangedByUserId == userId.Value);

        if (startDate.HasValue)
            query = query.Where(e => e.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.CreatedAt <= endDate.Value);

        var totalCount = await query.CountAsync();

        var events = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (events, totalCount);
    }

    /// <summary>
    /// Gets change events for audit purposes
    /// </summary>
    public async Task<IEnumerable<SettingChangeEvent>> GetAuditTrailAsync(
        Guid? settingId = null, 
        Guid? userId = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null)
    {
        var query = _dbContext.SettingChangeEvents
            .Include(e => e.Setting)
            .ThenInclude(s => s.Category)
            .ThenInclude(c => c.Section)
            .AsQueryable();

        // Apply filters
        if (settingId.HasValue)
            query = query.Where(e => e.SettingId == settingId.Value);

        if (userId.HasValue)
            query = query.Where(e => e.ChangedByUserId == userId.Value);

        if (startDate.HasValue)
            query = query.Where(e => e.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(e => e.CreatedAt <= endDate.Value);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Gets change events summary for reporting
    /// </summary>
    public async Task<IEnumerable<object>> GetChangeSummaryAsync(DateTime startDate, DateTime endDate)
    {
        var summary = await _dbContext.SettingChangeEvents
            .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
            .GroupBy(e => new {  Date = e.CreatedAt.Date })
            .Select(g => new
            {
                Date = g.Key.Date,
                Count = g.Count(),
                Users = g.Select(e => e.ChangedByUserId).Distinct().Count()
            })
            .OrderBy(s => s.Date)
            .ToListAsync();

        return summary.Cast<object>();
    }

    /// <summary>
    /// Gets the count of change events for a specific setting
    /// </summary>
    public async Task<int> GetChangeCountBySettingIdAsync(Guid settingId)
    {
        return await _dbContext.SettingChangeEvents
            .CountAsync(e => e.SettingId == settingId);
    }

    /// <summary>
    /// Gets the count of change events for a specific user
    /// </summary>
    public async Task<int> GetChangeCountByUserIdAsync(Guid userId)
    {
        return await _dbContext.SettingChangeEvents
            .CountAsync(e => e.ChangedByUserId == userId);
    }

    /// <summary>
    /// Checks if a change event exists
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.SettingChangeEvents.AnyAsync(e => e.Id == id);
    }
}
