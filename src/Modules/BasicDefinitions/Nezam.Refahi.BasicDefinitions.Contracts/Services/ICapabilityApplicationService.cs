using Nezam.Refahi.BasicDefinitions.Contracts.DTOs;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Application service interface for Capability operations
/// </summary>
public interface ICapabilityApplicationService
{
    /// <summary>
    /// Get capability by ID
    /// </summary>
    Task<ApplicationResult<CapabilityDto>> GetCapabilityByIdAsync(string capabilityId);

    /// <summary>
    /// Get capabilities by name
    /// </summary>
    Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetCapabilitiesByNameAsync(string name);

    /// <summary>
    /// Get all active capabilities
    /// </summary>
    Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetActiveCapabilitiesAsync();

    /// <summary>
    /// Get all capabilities
    /// </summary>
    Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetAllCapabilitiesAsync();

    /// <summary>
    /// Get capabilities by multiple keys
    /// </summary>
    Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetCapabilitiesByKeysAsync(IEnumerable<string> keys);

    /// <summary>
    /// Get capabilities that are valid at a specific date
    /// </summary>
    Task<ApplicationResult<IEnumerable<CapabilityDto>>> GetValidCapabilitiesAsync(DateTime date);

    /// <summary>
    /// Create a new capability
    /// </summary>
    Task<ApplicationResult<CapabilityDto>> CreateCapabilityAsync(CapabilityDto capabilityDto);

    /// <summary>
    /// Update an existing capability
    /// </summary>
    Task<ApplicationResult<CapabilityDto>> UpdateCapabilityAsync(CapabilityDto capabilityDto);

    /// <summary>
    /// Delete a capability
    /// </summary>
    Task<ApplicationResult<bool>> DeleteCapabilityAsync(string capabilityId);

    /// <summary>
    /// Add features to a capability
    /// </summary>
    Task<ApplicationResult<CapabilityDto>> AddFeaturesToCapabilityAsync(string capabilityId, IEnumerable<string> featureIds);

    /// <summary>
    /// Remove features from a capability
    /// </summary>
    Task<ApplicationResult<CapabilityDto>> RemoveFeaturesFromCapabilityAsync(string capabilityId, IEnumerable<string> featureIds);
}
