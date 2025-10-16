using Nezam.Refahi.BasicDefinitions.Domain.Entities;

namespace Nezam.Refahi.BasicDefinitions.Contracts.Services;

/// <summary>
/// Service for seeding basic definitions data
/// </summary>
public interface IBasicDefinitionsSeedContributor
{
    /// <summary>
    /// Seeds features catalog
    /// </summary>
    Task<List<Features>> SeedFeaturesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds capabilities that group features together
    /// </summary>
    Task<List<Capability>> SeedCapabilitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Seeds representative offices
    /// </summary>
    Task<List<Agency>> SeedAgencyiesAsync(CancellationToken cancellationToken = default);
}
