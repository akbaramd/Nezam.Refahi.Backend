using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Membership.Contracts.Services;

/// <summary>
/// Interface for seeding membership-related data from plugins
/// </summary>
public interface IMembershipSeedContributor
{
    /// <summary>
    /// Plugin name for identification
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Seeding priority (lower numbers execute first)
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Seeds roles for the plugin
    /// </summary>
    Task<List<Role>> SeedRolesAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Seeds capabilities that group claim types together
    /// </summary>
    Task<List<Capability>> SeedCapabilitiesAsync(CancellationToken cancellationToken = default);
}