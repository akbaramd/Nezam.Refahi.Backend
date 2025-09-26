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

      var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
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

          // Step 1: Get capability keys (now just strings)
          var capabilityKeys = await contributor.SeedCapabilityKeysAsync(cancellationToken);
          _logger.LogInformation("Found {Count} capability keys from contributor", capabilityKeys.Count);

          // Step 2: Sync roles
          var roles = await contributor.SeedRolesAsync(cancellationToken);
          await SyncRolesAsync(roles, roleRepository, cancellationToken);
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

  private async Task SyncRolesAsync(
    IEnumerable<Domain.Entities.Role> roles,
    IRoleRepository roleRepository,
    CancellationToken cancellationToken)
  {
    _logger.LogInformation("Syncing roles");

    foreach (var role in roles)
    {
      var existingRole = await roleRepository.FindOneAsync(r => r.Key == role.Key, cancellationToken);

      if (existingRole == null)
      {
        // Create new role
        await roleRepository.AddAsync(role, cancellationToken: cancellationToken);
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