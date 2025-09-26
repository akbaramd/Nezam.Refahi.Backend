using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain.Entities;
using Nezam.Refahi.Plugin.NezamMohandesi.Constants;
using Nezam.Refahi.Plugin.NezamMohandesi.Helpers;

namespace Nezam.Refahi.Plugin.NezamMohandesi.Seeding;

public class NezamMohandesiSeedContributor : IMembershipSeedContributor
{
    private readonly ILogger<NezamMohandesiSeedContributor> _logger;

    public NezamMohandesiSeedContributor(ILogger<NezamMohandesiSeedContributor> logger)
    {
        _logger = logger;
    }

    public string Name => NezamMohandesiConstants.PluginInfo.Name;
    public int Priority => NezamMohandesiConstants.PluginInfo.SeedPriority;

    public Task<List<Role>> SeedRolesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding roles for {PluginName}", Name);
        
        var roles = new List<Role>
        {
            new Role(MappingHelper.RoleKeys.Member,
              MappingHelper.RoleDisplayNames.Names[MappingHelper.RoleKeys.Member],
                "Professional Member",
                NezamMohandesiConstants.PluginInfo.DisplayName,
                NezamMohandesiConstants.PluginInfo.Name,
                1),
            
            new Role(MappingHelper.RoleKeys.Employer,
              MappingHelper.RoleDisplayNames.Names[MappingHelper.RoleKeys.Employer],
                "Engineering organization or employer",
                NezamMohandesiConstants.PluginInfo.DisplayName,
                NezamMohandesiConstants.PluginInfo.Name,
                2)
        };
        
        _logger.LogInformation("Created {Count} roles for {PluginName}", roles.Count, Name);
        return Task.FromResult(roles);
    }

    public Task<List<string>> SeedCapabilityKeysAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding capability keys for {PluginName}", Name);

        // Get all predefined capability keys from constants
        var capabilityKeys = NezamMohandesiConstants.AllPredefinedCapabilities
            .Select(c => c.Id)
            .ToList();

        _logger.LogInformation("Created {Count} capability keys for {PluginName}", capabilityKeys.Count, Name);
        return Task.FromResult(capabilityKeys);
    }
}