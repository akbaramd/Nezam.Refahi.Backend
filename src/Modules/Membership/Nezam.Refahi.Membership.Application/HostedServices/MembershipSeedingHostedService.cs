using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Membership.Domain.Repositories;

namespace Nezam.Refahi.Membership.Application.HostedServices;

public class MembershipSeedingHostedService : IHostedService
{
  private readonly IServiceProvider _serviceProvider;
  private readonly ILogger<MembershipSeedingHostedService> _logger;

  public MembershipSeedingHostedService(
    IServiceProvider serviceProvider,
    ILogger<MembershipSeedingHostedService> logger)
  {
    _serviceProvider = serviceProvider;
    _logger = logger;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    try
    {
      _logger.LogInformation("Starting membership seeding process");

      using var scope = _serviceProvider.CreateScope();
      var seedContributors = scope.ServiceProvider.GetServices<IMembershipSeedContributor>();

      var claimRepository = scope.ServiceProvider.GetRequiredService<IFeatureRepository>();
      var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
      var capabilityRepository = scope.ServiceProvider.GetRequiredService<ICapabilityRepository>();
      var uow = scope.ServiceProvider.GetRequiredService<IMembershipUnitOfWork>();

      if (!seedContributors.Any())
      {
        _logger.LogInformation("No membership seed contributors found");
        return;
      }

      // Sort by priority (lower number = higher priority)
      var orderedContributors = seedContributors
        .ToList();

      _logger.LogInformation("Found {Count} seed contributors", orderedContributors.Count);

      foreach (var contributor in orderedContributors)
      {
        await uow.BeginAsync(cancellationToken);
        try
        {
          _logger.LogInformation("Processing seed contributor: {ContributorType}", contributor.GetType().Name);

          // Step 1: Get all capabilities first to collect claim types
          var capabilities = await contributor.SeedCapabilitiesAsync(cancellationToken);

          // Step 2: Sync all claim types first (dependencies for capabilities)
          await SyncClaimTypesAsync(capabilities, claimRepository, cancellationToken:cancellationToken);
          await uow.SaveChangesAsync(cancellationToken);

          // Step 3: Sync capabilities (now that all claim types exist)
          await SyncCapabilitiesAsync(capabilities, capabilityRepository, claimRepository, cancellationToken:cancellationToken);
          await uow.SaveChangesAsync(cancellationToken);

          // Step 4: Sync roles
          var roles = await contributor.SeedRolesAsync(cancellationToken);
          await SyncRolesAsync(roles, roleRepository, cancellationToken:cancellationToken);
          await uow.SaveChangesAsync(cancellationToken);

          await uow.CommitAsync(cancellationToken);

          _logger.LogInformation("Successfully processed seed contributor: {ContributorType}", contributor.GetType().Name);
        }
        catch (Exception ex)
        {
          await uow.RollbackAsync(cancellationToken);
          _logger.LogError(ex, "Error running seed contributor: {ContributorType}", contributor.GetType().Name);
          // Continue with other contributors rather than failing the entire seeding process
        }
      }

      _logger.LogInformation("Completed membership seeding process");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Fatal error during membership seeding process");
      // Don't throw here as it would prevent the application from starting
    }
  }

  private async Task SyncClaimTypesAsync(
    IEnumerable<Domain.Entities.Capability> capabilities,
    IFeatureRepository claimRepository,
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Syncing claim types");

    // Collect all unique claim types from all capabilities
    var allClaimTypes = capabilities
      .SelectMany(c => c.Features)
      .GroupBy(ct => ct.Id)
      .Select(g => g.First()) // Take first occurrence of each unique key
      .ToList();

    _logger.LogInformation("Found {Count} unique claim types to sync", allClaimTypes.Count);

    foreach (var claimType in allClaimTypes)
    {
      var existingClaimType = await claimRepository.FindOneAsync(ct => ct.Id == claimType.Id, cancellationToken:cancellationToken);

      if (existingClaimType == null)
      {
        // Create new claim type
        await claimRepository.AddAsync(claimType, cancellationToken:cancellationToken);
        _logger.LogInformation("Added new claim type: {ClaimTypeKey} - {ClaimTypeTitle}",
          claimType.Id, claimType.Title);
      }
      else
      {
        // Update existing claim type
        existingClaimType.Update(
          title: claimType.Title,
          type: claimType.Type);


        _logger.LogInformation("Updated existing claim type: {ClaimTypeKey} - {ClaimTypeTitle}",
          claimType.Id, claimType.Title);
      }
    }
  }

  private async Task SyncCapabilitiesAsync(
    IEnumerable<Domain.Entities.Capability> capabilities,
    ICapabilityRepository capabilityRepository,
    IFeatureRepository claimRepository,
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Syncing capabilities");

    foreach (var capability in capabilities)
    {
      var existingCapability = await capabilityRepository.FindOneAsync(c => c.Id == capability.Id, cancellationToken:cancellationToken);

      if (existingCapability == null)
      {
        // Create new capability
        var newCapability = new Domain.Entities.Capability(
          key: capability.Id,
          name: capability.Name,
          description: capability.Description,
          validFrom: capability.ValidFrom,
          validTo: capability.ValidTo);

        // Add claim types to the new capability
        foreach (var claimType in capability.Features)
        {
          var dbClaimType = await claimRepository.FindOneAsync(ct => ct.Id == claimType.Id, cancellationToken:cancellationToken);
          if (dbClaimType != null)
          {
            newCapability.AddFeature(dbClaimType);
          }
        }

        await capabilityRepository.AddAsync(newCapability, cancellationToken:cancellationToken);
        _logger.LogInformation("Added new capability: {CapabilityKey} - {CapabilityName} with {ClaimTypeCount} claim types",
          capability.Id, capability.Name, capability.Features.Count);
      }
      else
      {
        // Update existing capability
        existingCapability.Update(
          name: capability.Name,
          description: capability.Description,
          validFrom: capability.ValidFrom,
          validTo: capability.ValidTo);

        // Sync claim types for the capability
        foreach (var claimType in capability.Features)
        {
          var dbClaimType = await claimRepository.FindOneAsync(ct => ct.Id == claimType.Id, cancellationToken:cancellationToken);
          if (dbClaimType != null && !existingCapability.HasFeature(dbClaimType.Id))
          {
            existingCapability.AddFeature(dbClaimType);
          }
        }

        if (!existingCapability.IsActive)
        {
          existingCapability.Activate();
        }

        _logger.LogInformation("Updated existing capability: {CapabilityKey} - {CapabilityName} with {ClaimTypeCount} claim types",
          capability.Id, capability.Name, capability.Features.Count);
      }
    }
  }

  private async Task SyncRolesAsync(
    IEnumerable<Domain.Entities.Role> roles,
    IRoleRepository roleRepository,
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Syncing roles");

    foreach (var role in roles)
    {
      var existingRole = await roleRepository.FindOneAsync(r => r.Key == role.Key, cancellationToken:cancellationToken);

      if (existingRole == null)
      {
        // Create new role
        await roleRepository.AddAsync(role, cancellationToken:cancellationToken);
        _logger.LogInformation("Added new role: {RoleKey} - {RoleTitle}", role.Id, role.Title);
      }
      else
      {
        // Update existing role
        existingRole.Update(
          englishTitle: role.Title,
          persianTitle: role.Title, // Assuming same title for now
          description: role.Description,
          employerName: role.EmployerName,
          employerCode: role.EmployerCode,
          sortOrder: role.SortOrder);

        if (!existingRole.IsActive)
        {
          existingRole.Activate();
        }

        _logger.LogInformation("Updated existing role: {RoleKey} - {RoleTitle}", role.Id, role.Title);
      }
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
