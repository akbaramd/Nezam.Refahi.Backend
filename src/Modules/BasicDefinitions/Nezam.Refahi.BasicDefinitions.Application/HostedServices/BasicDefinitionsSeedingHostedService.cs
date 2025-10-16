using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Application.Services;
using Nezam.Refahi.BasicDefinitions.Contracts.Services;
using Nezam.Refahi.BasicDefinitions.Domain.Repositories;

namespace Nezam.Refahi.BasicDefinitions.Application.HostedServices;

/// <summary>
/// Hosted service responsible for seeding BasicDefinitions data on application startup
/// Follows the same pattern as MembershipSeedingHostedService for consistency
/// </summary>
public class BasicDefinitionsSeedingHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BasicDefinitionsSeedingHostedService> _logger;

    public BasicDefinitionsSeedingHostedService(
        IServiceProvider serviceProvider,
        ILogger<BasicDefinitionsSeedingHostedService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      // Run seeding in background to not block application startup
        _ = Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation("Starting BasicDefinitions seeding process in background");

                using var scope = _serviceProvider.CreateScope();
                var seedContributors = scope.ServiceProvider.GetServices<IBasicDefinitionsSeedContributor>();

                if (!seedContributors.Any())
                {
                    _logger.LogInformation("No BasicDefinitions seed contributors found");
                    return;
                }

                // Process all seed contributors
                var orderedContributors = seedContributors.ToList();
                _logger.LogInformation("Found {Count} seed contributors", orderedContributors.Count);

                foreach (var contributor in orderedContributors)
                {
                    try
                    {
                        _logger.LogInformation("Processing seed contributor: {ContributorType}", contributor.GetType().Name);

                        // Use Unit of Work for transaction management
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IBasicDefinitionsUnitOfWork>();
                        var featuresRepository = scope.ServiceProvider.GetRequiredService<IFeaturesRepository>();
                        var capabilityRepository = scope.ServiceProvider.GetRequiredService<ICapabilityRepository>();
                        var AgencyRepository = scope.ServiceProvider.GetRequiredService<IAgencyRepository>();
                        
                        await unitOfWork.BeginAsync(cancellationToken);
                        
                        try
                        {
                            // Step 1: Seed Features
                            var features = await contributor.SeedFeaturesAsync(cancellationToken);
                            await SyncFeaturesAsync(features, featuresRepository, cancellationToken);
                            _logger.LogInformation("Processed {Count} features from contributor", features.Count);

                            // Step 2: Seed Capabilities
                            var capabilities = await contributor.SeedCapabilitiesAsync(cancellationToken);
                            await SyncCapabilitiesAsync(capabilities, capabilityRepository, cancellationToken);
                            _logger.LogInformation("Processed {Count} capabilities from contributor", capabilities.Count);

                            // Step 2.5: Sync Capability-Feature relationships
                            await SyncCapabilityFeatureRelationshipsAsync(capabilities, featuresRepository, capabilityRepository, cancellationToken);

                            // Step 3: Seed Representative Offices
                            var Agencyies = await contributor.SeedAgencyiesAsync(cancellationToken);
                            await SyncAgencyiesAsync(Agencyies, AgencyRepository, cancellationToken);
                            _logger.LogInformation("Processed {Count} representative offices from contributor", Agencyies.Count);

                            // Commit all changes for this contributor
                            await unitOfWork.SaveChangesAsync(cancellationToken);
                            await unitOfWork.CommitAsync(cancellationToken);
                            _logger.LogInformation("Successfully processed seed contributor: {ContributorType}", contributor.GetType().Name);
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) 
                        {
                            // Handle primary key constraint violations gracefully
                            _logger.LogWarning("Primary key constraint violation during seeding for contributor {ContributorType}: {Message}. This is expected for duplicate data.", 
                                contributor.GetType().Name, ex.Message);
                            
                            // Try to commit anyway as some data might have been saved
                            try
                            {
                                await unitOfWork.CommitAsync(cancellationToken);
                                _logger.LogInformation("Successfully committed partial data for contributor: {ContributorType}", contributor.GetType().Name);
                            }
                            catch (Exception commitEx)
                            {
                                _logger.LogWarning(commitEx, "Failed to commit partial data for contributor: {ContributorType}", contributor.GetType().Name);
                                await unitOfWork.RollbackAsync(cancellationToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing seed contributor: {ContributorType}, rolling back transaction", contributor.GetType().Name);
                            try
                            {
                                await unitOfWork.RollbackAsync(cancellationToken);
                            }
                            catch (Exception rollbackEx)
                            {
                                _logger.LogError(rollbackEx, "Failed to rollback transaction for contributor: {ContributorType}", contributor.GetType().Name);
                            }
                            // Continue with other contributors rather than failing the entire seeding process
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error running seed contributor: {ContributorType}", contributor.GetType().Name);
                        // Continue with other contributors rather than failing the entire seeding process
                    }
                }

                _logger.LogInformation("Completed BasicDefinitions seeding process");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error during BasicDefinitions seeding process");
                // Don't throw here as it would prevent the application from starting
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    private async Task SyncFeaturesAsync(
        IEnumerable<Domain.Entities.Features> features,
        IFeaturesRepository featuresRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Syncing features");

        foreach (var feature in features)
        {
            var existingFeature = await featuresRepository.FindOneAsync(f => f.Id == feature.Id, cancellationToken);

            if (existingFeature == null)
            {
                // Create new feature without navigation properties to avoid tracking conflicts
                var newFeature = new Domain.Entities.Features(
                    key: feature.Id,
                    title: feature.Title,
                    type: feature.Type);

                await featuresRepository.AddAsync(newFeature, cancellationToken: cancellationToken);
                _logger.LogInformation("Added new feature: {FeatureId} - {FeatureTitle}", feature.Id, feature.Title);
            }
            else
            {
                // Update existing feature
                existingFeature.Update(
                    title: feature.Title,
                    type: feature.Type);

                _logger.LogInformation("Updated existing feature: {FeatureId} - {FeatureTitle}", feature.Id, feature.Title);
            }
        }
    }

    private async Task SyncCapabilitiesAsync(
        IEnumerable<Domain.Entities.Capability> capabilities,
        ICapabilityRepository capabilityRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Syncing capabilities");

        foreach (var capability in capabilities)
        {
            var existingCapability = await capabilityRepository.FindOneAsync(c => c.Id == capability.Id, cancellationToken);

            if (existingCapability == null)
            {
                // Create new capability without navigation properties to avoid tracking conflicts
                var newCapability = new Domain.Entities.Capability(
                    key: capability.Id,
                    name: capability.Name,
                    description: capability.Description,
                    validFrom: capability.ValidFrom,
                    validTo: capability.ValidTo);

                await capabilityRepository.AddAsync(newCapability, cancellationToken: cancellationToken);
                _logger.LogInformation("Added new capability: {CapabilityId} - {CapabilityTitle}", capability.Id, capability.Name);
            }
            else
            {
                // Update existing capability
                existingCapability.Update(
                    name: capability.Name,
                    description: capability.Description,
                    validFrom: capability.ValidFrom,
                    validTo: capability.ValidTo);

                _logger.LogInformation("Updated existing capability: {CapabilityId} - {CapabilityTitle}", capability.Id, capability.Name);
            }
        }
    }

    private async Task SyncAgencyiesAsync(
        IEnumerable<Domain.Entities.Agency> Agencyies,
        IAgencyRepository AgencyRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Syncing representative offices");

        foreach (var office in Agencyies)
        {
            // Check if office has a valid ID (not empty GUID)
            var hasValidId = office.Id != Guid.Empty;
            
            Domain.Entities.Agency? existingOffice = null;
            
            if (hasValidId)
            {
                // Try to find by ID first
                existingOffice = await AgencyRepository.FindOneAsync(o => o.Id == office.Id, cancellationToken);
            }
            
            // If not found by ID or ID is invalid, try to find by Code or ExternalCode
            if (existingOffice == null)
            {
                existingOffice = await AgencyRepository.FindOneAsync(o => o.Code == office.Code, cancellationToken);
            }
            
            if (existingOffice == null)
            {
                existingOffice = await AgencyRepository.FindOneAsync(o => o.ExternalCode == office.ExternalCode, cancellationToken);
            }

            if (existingOffice == null)
            {
                // Create new representative office with proper ID generation
                var newOffice = new Domain.Entities.Agency(
                    code: office.Code,
                    externalCode: office.ExternalCode,
                    name: office.Name,
                    address: office.Address,
                    managerName: office.ManagerName,
                    managerPhone: office.ManagerPhone,
                    establishedDate: office.EstablishedDate);

                // Set active status
                if (!office.IsActive)
                {
                    newOffice.Deactivate();
                }

                await AgencyRepository.AddAsync(newOffice, cancellationToken: cancellationToken);
                _logger.LogInformation("Added new representative office: {OfficeId} - {OfficeName} (Code: {Code})", newOffice.Id, newOffice.Name, newOffice.Code);
            }
            else
            {
                // Update existing representative office
                existingOffice.UpdateInfo(
                    name: office.Name,
                    address: office.Address,
                    managerName: office.ManagerName,
                    managerPhone: office.ManagerPhone);

                if (office.IsActive && !existingOffice.IsActive)
                {
                    existingOffice.Activate();
                }
                else if (!office.IsActive && existingOffice.IsActive)
                {
                    existingOffice.Deactivate();
                }

                _logger.LogInformation("Updated existing representative office: {OfficeId} - {OfficeName} (Code: {Code})", existingOffice.Id, existingOffice.Name, existingOffice.Code);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

     private async Task SyncCapabilityFeatureRelationshipsAsync(
        IEnumerable<Domain.Entities.Capability> capabilities,
        IFeaturesRepository featuresRepository,
        ICapabilityRepository capabilityRepository,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Syncing capability-feature relationships");

        foreach (var capability in capabilities)
        {
            // Skip if capability has no features to avoid unnecessary database calls
            if (!capability.Features.Any())
            {
                continue;
            }

            // Get the capability from the database to ensure it's properly tracked
            var trackedCapability = await capabilityRepository.FindOneAsync(c => c.Id == capability.Id, cancellationToken);
            if (trackedCapability == null)
            {
                _logger.LogWarning("Capability {CapabilityId} not found in database, skipping relationship sync", capability.Id);
                continue;
            }

            // Process each feature in the capability's navigation property
            foreach (var feature in capability.Features)
            {
                // Get the feature from the database to ensure it's properly tracked
                var trackedFeature = await featuresRepository.FindOneAsync(f => f.Id == feature.Id, cancellationToken);
                if (trackedFeature == null)
                {
                    _logger.LogWarning("Feature {FeatureId} not found in database, skipping relationship", feature.Id);
                    continue;
                }

                // Check if the relationship already exists before adding
                var relationshipExists = trackedCapability.Features.Any(f => f.Id == feature.Id);
                
                if (!relationshipExists)
                {
                    try
                    {
                        trackedCapability.AddFeature(trackedFeature);
                        _logger.LogInformation("Added feature {FeatureId} to capability {CapabilityId}", feature.Id, capability.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to add feature {FeatureId} to capability {CapabilityId}", feature.Id, capability.Id);
                    }
                }
                else
                {
                    _logger.LogDebug("Feature {FeatureId} already exists in capability {CapabilityId}, skipping", feature.Id, capability.Id);
                }
            }
        }
    }


}
