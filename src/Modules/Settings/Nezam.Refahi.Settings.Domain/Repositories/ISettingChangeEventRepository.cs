using Nezam.Refahi.Settings.Domain.Entities;

namespace Nezam.Refahi.Settings.Domain.Repositories;

/// <summary>
/// Repository interface for managing setting change events (event sourcing)
/// </summary>
public interface ISettingChangeEventRepository
{
    /// <summary>
    /// Adds a new change event
    /// </summary>
    Task<SettingChangeEvent> AddAsync(SettingChangeEvent changeEvent);
    
    /// <summary>
    /// Gets a change event by ID
    /// </summary>
    Task<SettingChangeEvent?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Gets all change events for a specific setting
    /// </summary>
    Task<IEnumerable<SettingChangeEvent>> GetBySettingIdAsync(Guid settingId);
    
    /// <summary>
    /// Gets all change events for a specific user
    /// </summary>
    Task<IEnumerable<SettingChangeEvent>> GetByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets all change events within a date range
    /// </summary>
    Task<IEnumerable<SettingChangeEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Gets all change events for a specific setting key
    /// </summary>
    Task<IEnumerable<SettingChangeEvent>> GetBySettingKeyAsync(string settingKey);
    
    /// <summary>
    /// Gets the latest change event for a specific setting
    /// </summary>
    Task<SettingChangeEvent?> GetLatestBySettingIdAsync(Guid settingId);
    
    /// <summary>
    /// Gets change events with pagination
    /// </summary>
    Task<(IEnumerable<SettingChangeEvent> Events, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Guid? settingId = null, 
        Guid? userId = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null);
    
    /// <summary>
    /// Gets change events for audit purposes
    /// </summary>
    Task<IEnumerable<SettingChangeEvent>> GetAuditTrailAsync(
        Guid? settingId = null, 
        Guid? userId = null, 
        DateTime? startDate = null, 
        DateTime? endDate = null);
    
    /// <summary>
    /// Gets change events summary for reporting
    /// </summary>
    Task<IEnumerable<object>> GetChangeSummaryAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Gets the count of change events for a specific setting
    /// </summary>
    Task<int> GetChangeCountBySettingIdAsync(Guid settingId);
    
    /// <summary>
    /// Gets the count of change events for a specific user
    /// </summary>
    Task<int> GetChangeCountByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Checks if a change event exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}
