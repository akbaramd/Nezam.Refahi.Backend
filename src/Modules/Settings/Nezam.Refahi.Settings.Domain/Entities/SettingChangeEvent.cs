using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Domain.Entities;

/// <summary>
/// Represents a change event for a setting, providing full audit trail
/// </summary>
public class SettingChangeEvent : DeletableAggregateRoot<Guid>
{
    public Guid SettingId { get; private set; }
    public SettingKey SettingKey { get; private set; } = null!;
    public SettingValue OldValue { get; private set; } = null!;
    public SettingValue NewValue { get; private set; } = null!;
    public Guid ChangedByUserId { get; private set; }
    public string? ChangeReason { get; private set; }
    
    // Navigation properties
    public virtual Setting Setting { get; private set; } = null!;
    
    // Private constructor for EF Core
    public SettingChangeEvent() : base() { }
    
    public SettingChangeEvent(Guid settingId, SettingKey settingKey, SettingValue oldValue, SettingValue newValue, Guid changedByUserId, string? changeReason = null)
        : base(Guid.NewGuid())
    {
        if (settingId == Guid.Empty)
            throw new ArgumentException("Setting ID cannot be empty", nameof(settingId));
            
        if (settingKey == null)
            throw new ArgumentNullException(nameof(settingKey));
            
        if (oldValue == null)
            throw new ArgumentNullException(nameof(oldValue));
            
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));
            
        if (changedByUserId == Guid.Empty)
            throw new ArgumentException("Changed by user ID cannot be empty", nameof(changedByUserId));
            
        if (changeReason != null && changeReason.Length > 500)
            throw new ArgumentException("Change reason cannot exceed 500 characters", nameof(changeReason));
            
        SettingId = settingId;
        SettingKey = settingKey;
        OldValue = oldValue;
        NewValue = newValue;
        ChangedByUserId = changedByUserId;
        ChangeReason = changeReason;
    }
    
    /// <summary>
    /// Gets a human-readable description of the change
    /// </summary>
    public string GetChangeDescription()
    {
        var changeType = OldValue.RawValue != NewValue.RawValue ? "Value Changed" : "Metadata Changed";
        var reason = !string.IsNullOrWhiteSpace(ChangeReason) ? $" - Reason: {ChangeReason}" : "";
        
        return $"{changeType}: {SettingKey.Value} from '{OldValue.RawValue}' to '{NewValue.RawValue}'{reason}";
    }
    
    /// <summary>
    /// Checks if this is a value change (not just metadata)
    /// </summary>
    public bool IsValueChange()
    {
        return OldValue.RawValue != NewValue.RawValue;
    }
    
    /// <summary>
    /// Gets the change summary for display purposes
    /// </summary>
    public string GetChangeSummary()
    {
        if (IsValueChange())
        {
            return $"Changed {SettingKey.Value}: {OldValue.RawValue} → {NewValue.RawValue}";
        }
        
        return $"Updated {SettingKey.Value} metadata";
    }
}
