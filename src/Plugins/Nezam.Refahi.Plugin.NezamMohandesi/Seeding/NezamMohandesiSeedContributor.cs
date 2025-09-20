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

  
    public Task<List<Capability>> SeedCapabilitiesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Seeding capabilities for {PluginName}", Name);

        // Get all predefined capabilities from constants (includes all combinations and special capabilities)
        var capabilities = NezamMohandesiConstants.AllPredefinedCapabilities.ToList();

        _logger.LogInformation("Created {Count} capabilities for {PluginName}", capabilities.Count, Name);
        return Task.FromResult(capabilities);
    }
}