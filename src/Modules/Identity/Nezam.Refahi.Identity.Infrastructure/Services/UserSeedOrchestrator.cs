using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Features.Users.Commands.CreateUser;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Service for orchestrating user seeding operations
/// Provides high-level coordination of seeding processes with proper error handling and metrics
/// </summary>
public class UserSeedOrchestrator : IUserSeedOrchestrator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<UserSeedOrchestrator> _logger;

    public UserSeedOrchestrator(
        IServiceProvider serviceProvider,
        IMediator mediator,
        ILogger<UserSeedOrchestrator> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserSeedResult> RunOnceAsync(
        int batchSize = 1000,
        int maxParallel = 4,
        IUserSeedSource? source = null,
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting user seeding operation with batch size {BatchSize}, max parallel {MaxParallel}, dry run {DryRun}", 
            batchSize, maxParallel, dryRun);

        var result = new UserSeedResult
        {
            StartedAt = startTime,
            IsDryRun = dryRun
        };

        try
        {
            // Get all available seed sources if none specified
            var sources = source != null ? new[] { source } : GetAllSeedSources();
            
            foreach (var seedSource in sources)
            {
                _logger.LogInformation("Processing seed source: {SourceName}", seedSource.SourceName);
                result.SourceSystem = seedSource.SourceName;

                // Check if source is available
                if (!await seedSource.IsAvailableAsync(cancellationToken))
                {
                    _logger.LogWarning("Seed source {SourceName} is not available, skipping", seedSource.SourceName);
                    continue;
                }

                // Process batches from this source
                var sourceResult = await ProcessSourceAsync(seedSource, batchSize, maxParallel, dryRun, cancellationToken);
                
                // Aggregate results
                result.TotalProcessed += sourceResult.TotalProcessed;
                result.SuccessfullyCreated += sourceResult.SuccessfullyCreated;
                result.Skipped += sourceResult.Skipped;
                result.Failed += sourceResult.Failed;
                result.ValidationErrors += sourceResult.ValidationErrors;
                result.Errors.AddRange(sourceResult.Errors);
                result.Warnings.AddRange(sourceResult.Warnings);
                result.LastWatermark = sourceResult.LastWatermark;
            }

            result.CompletedAt = DateTime.UtcNow;
            result.Duration = result.CompletedAt - result.StartedAt;

            _logger.LogInformation("User seeding operation completed: {TotalProcessed} processed, {SuccessfullyCreated} created, {Skipped} skipped, {Failed} failed in {Duration}",
                result.TotalProcessed, result.SuccessfullyCreated, result.Skipped, result.Failed, result.Duration);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user seeding operation");
            result.CompletedAt = DateTime.UtcNow;
            result.Duration = result.CompletedAt - result.StartedAt;
            result.Errors.Add(new UserSeedError
            {
                ErrorCode = "SEEDING_ERROR",
                Message = ex.Message,
                OccurredAt = DateTime.UtcNow
            });
            return result;
        }
    }

    public async Task<UserSeedResult> RunIncrementalAsync(
        UserSeedWatermark watermark,
        int batchSize = 1000,
        int maxParallel = 4,
        IUserSeedSource? source = null,
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting incremental user seeding from watermark {Watermark}", watermark);

        var result = new UserSeedResult
        {
            StartedAt = startTime,
            IsDryRun = dryRun,
            LastWatermark = watermark
        };

        try
        {
            var sources = source != null ? new[] { source } : GetAllSeedSources();
            
            foreach (var seedSource in sources)
            {
                _logger.LogInformation("Processing incremental seed source: {SourceName}", seedSource.SourceName);
                result.SourceSystem = seedSource.SourceName;

                if (!await seedSource.IsAvailableAsync(cancellationToken))
                {
                    _logger.LogWarning("Seed source {SourceName} is not available, skipping", seedSource.SourceName);
                    continue;
                }

                var sourceResult = await ProcessSourceIncrementalAsync(seedSource, watermark, batchSize, maxParallel, dryRun, cancellationToken);
                
                result.TotalProcessed += sourceResult.TotalProcessed;
                result.SuccessfullyCreated += sourceResult.SuccessfullyCreated;
                result.Skipped += sourceResult.Skipped;
                result.Failed += sourceResult.Failed;
                result.ValidationErrors += sourceResult.ValidationErrors;
                result.Errors.AddRange(sourceResult.Errors);
                result.Warnings.AddRange(sourceResult.Warnings);
                result.LastWatermark = sourceResult.LastWatermark;
            }

            result.CompletedAt = DateTime.UtcNow;
            result.Duration = result.CompletedAt - result.StartedAt;

            _logger.LogInformation("Incremental user seeding completed: {TotalProcessed} processed, {SuccessfullyCreated} created, {Skipped} skipped, {Failed} failed in {Duration}",
                result.TotalProcessed, result.SuccessfullyCreated, result.Skipped, result.Failed, result.Duration);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during incremental user seeding operation");
            result.CompletedAt = DateTime.UtcNow;
            result.Duration = result.CompletedAt - result.StartedAt;
            result.Errors.Add(new UserSeedError
            {
                ErrorCode = "INCREMENTAL_SEEDING_ERROR",
                Message = ex.Message,
                OccurredAt = DateTime.UtcNow
            });
            return result;
        }
    }

    public Task<UserSeedStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        // This would typically read from a statistics store
        // For now, return basic statistics
        return Task.FromResult(new UserSeedStatistics
        {
            GeneratedAt = DateTime.UtcNow
        });
    }

    public async Task<UserSeedValidationResult> ValidateConfigurationAsync(CancellationToken cancellationToken = default)
    {
        var result = new UserSeedValidationResult { IsValid = true };

        try
        {
            var sources = GetAllSeedSources();
            
            foreach (var source in sources)
            {
                var isAvailable = await source.IsAvailableAsync(cancellationToken);
                if (!isAvailable)
                {
                    result.Warnings.Add($"Source {source.SourceName} is not available");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Configuration validation error: {ex.Message}");
            return result;
        }
    }

    private IEnumerable<IUserSeedSource> GetAllSeedSources()
    {
        return _serviceProvider.GetServices<IUserSeedSource>();
    }

    private async Task<UserSeedResult> ProcessSourceAsync(
        IUserSeedSource source, 
        int batchSize, 
        int maxParallel, 
        bool dryRun, 
        CancellationToken cancellationToken)
    {
        var result = new UserSeedResult
        {
            SourceSystem = source.SourceName,
            IsDryRun = dryRun
        };

        int offset = 0;
        bool hasMore = true;

        while (hasMore && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                var batch = await source.GetUserBatchAsync(batchSize, offset, cancellationToken);
                hasMore = batch.HasMore;
                offset += batchSize;

                var batchResult = await ProcessBatchAsync(batch, source, dryRun, cancellationToken);
                
                result.TotalProcessed += batchResult.TotalProcessed;
                result.SuccessfullyCreated += batchResult.SuccessfullyCreated;
                result.Skipped += batchResult.Skipped;
                result.Failed += batchResult.Failed;
                result.ValidationErrors += batchResult.ValidationErrors;
                result.Errors.AddRange(batchResult.Errors);
                result.Warnings.AddRange(batchResult.Warnings);
                result.LastWatermark = batch.NextWatermark;

                _logger.LogDebug("Processed batch from {SourceName}: {Processed} processed, {Created} created, {Skipped} skipped, {Failed} failed",
                    source.SourceName, batchResult.TotalProcessed, batchResult.SuccessfullyCreated, batchResult.Skipped, batchResult.Failed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch from source {SourceName}", source.SourceName);
                result.Errors.Add(new UserSeedError
                {
                    ErrorCode = "BATCH_PROCESSING_ERROR",
                    Message = ex.Message,
                    SourceSystem = source.SourceName,
                    OccurredAt = DateTime.UtcNow
                });
                break;
            }
        }

        return result;
    }

    private async Task<UserSeedResult> ProcessSourceIncrementalAsync(
        IUserSeedSource source,
        UserSeedWatermark watermark,
        int batchSize,
        int maxParallel,
        bool dryRun,
        CancellationToken cancellationToken)
    {
        var result = new UserSeedResult
        {
            SourceSystem = source.SourceName,
            IsDryRun = dryRun,
            LastWatermark = watermark
        };

        bool hasMore = true;
        var currentWatermark = watermark;

        while (hasMore && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                var batch = await source.GetUserBatchByWatermarkAsync(currentWatermark, batchSize, cancellationToken);
                hasMore = batch.HasMore;

                var batchResult = await ProcessBatchAsync(batch, source, dryRun, cancellationToken);
                
                result.TotalProcessed += batchResult.TotalProcessed;
                result.SuccessfullyCreated += batchResult.SuccessfullyCreated;
                result.Skipped += batchResult.Skipped;
                result.Failed += batchResult.Failed;
                result.ValidationErrors += batchResult.ValidationErrors;
                result.Errors.AddRange(batchResult.Errors);
                result.Warnings.AddRange(batchResult.Warnings);
                result.LastWatermark = batch.NextWatermark;
                currentWatermark = batch.NextWatermark ?? currentWatermark;

                _logger.LogDebug("Processed incremental batch from {SourceName}: {Processed} processed, {Created} created, {Skipped} skipped, {Failed} failed",
                    source.SourceName, batchResult.TotalProcessed, batchResult.SuccessfullyCreated, batchResult.Skipped, batchResult.Failed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing incremental batch from source {SourceName}", source.SourceName);
                result.Errors.Add(new UserSeedError
                {
                    ErrorCode = "INCREMENTAL_BATCH_PROCESSING_ERROR",
                    Message = ex.Message,
                    SourceSystem = source.SourceName,
                    OccurredAt = DateTime.UtcNow
                });
                break;
            }
        }

        return result;
    }

    private async Task<UserSeedResult> ProcessBatchAsync(
        UserSeedBatch batch, 
        IUserSeedSource source, 
        bool dryRun, 
        CancellationToken cancellationToken)
    {
        var result = new UserSeedResult
        {
            SourceSystem = source.SourceName,
            IsDryRun = dryRun
        };

        _logger.LogInformation("Processing batch of {Count} records from {SourceName}", batch.Records.Count, source.SourceName);

        // Log batch details for debugging
        if (batch.Records.Count == 0)
        {
            _logger.LogWarning("Empty batch received from {SourceName} - this might indicate pagination issues", source.SourceName);
            return result;
        }

        // Track national IDs in this batch to prevent duplicates within the same batch
        var batchNationalIds = new HashSet<string>();

        foreach (var record in batch.Records)
        {
            try
            {
                result.TotalProcessed++;

                // CRITICAL: Skip users without valid national numbers
                if (string.IsNullOrWhiteSpace(record.NationalId))
                {
                    result.ValidationErrors++;
                    result.Errors.Add(new UserSeedError
                    {
                        ErrorCode = "MISSING_NATIONAL_ID",
                        Message = "National ID is required for seeding",
                        ExternalUserId = record.ExternalUserId.ToString(),
                        SourceSystem = source.SourceName,
                        OccurredAt = DateTime.UtcNow
                    });
                    _logger.LogWarning("Skipping user {ExternalUserId} from {SourceName} - missing national ID", 
                        record.ExternalUserId, source.SourceName);
                    continue;
                }

                // Check for duplicate national ID within this batch
                if (batchNationalIds.Contains(record.NationalId))
                {
                    result.ValidationErrors++;
                    result.Errors.Add(new UserSeedError
                    {
                        ErrorCode = "DUPLICATE_NATIONAL_ID_IN_BATCH",
                        Message = $"Duplicate National ID {record.NationalId} found in same batch",
                        ExternalUserId = record.ExternalUserId.ToString(),
                        SourceSystem = source.SourceName,
                        OccurredAt = DateTime.UtcNow
                    });
                    _logger.LogWarning("Skipping user {ExternalUserId} from {SourceName} - duplicate national ID {NationalId} in batch", 
                        record.ExternalUserId, source.SourceName, record.NationalId);
                    continue;
                }

                // Validate record
                if (!record.IsValid)
                {
                    result.ValidationErrors++;
                    result.Errors.Add(new UserSeedError
                    {
                        ErrorCode = "VALIDATION_ERROR",
                        Message = string.Join(", ", record.ValidationErrors),
                        ExternalUserId = record.ExternalUserId.ToString(),
                        SourceSystem = source.SourceName,
                        OccurredAt = DateTime.UtcNow
                    });
                    _logger.LogWarning("Skipping user {ExternalUserId} from {SourceName} - validation failed: {Errors}", 
                        record.ExternalUserId, source.SourceName, string.Join(", ", record.ValidationErrors));
                    continue;
                }

                // Add national ID to batch tracking
                batchNationalIds.Add(record.NationalId);

                if (dryRun)
                {
                    result.SuccessfullyCreated++;
                    _logger.LogDebug("Dry run: Would create user {ExternalUserId} with National ID {NationalId} from {SourceName}", 
                        record.ExternalUserId, record.NationalId, source.SourceName);
                    continue;
                }

                // Create user using CreateUserCommand
                var command = new CreateUserCommand
                {
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    PhoneNumber = record.PhoneNumber,
                    NationalId = record.NationalId,
                    Email = record.Email,
                    Username = record.Username,
                    ExternalUserId = record.ExternalUserId,
                    SourceSystem = record.SourceSystem,
                    SourceVersion = record.SourceVersion,
                    SourceChecksum = record.SourceChecksum,
                    Claims = record.Claims,
                    Roles = record.Roles,
                    ProfileSnapshot = record.ProfileSnapshot,
                    IdempotencyKey = GenerateIdempotencyKey(record),
                    CorrelationId = Guid.NewGuid().ToString(),
                    SkipDuplicateCheck = false, // Enable duplicate check for national ID validation
                    IsSeedingOperation = true
                };

                var commandResult = await _mediator.Send(command, cancellationToken);
                
                if (commandResult.IsSuccess)
                {
                    result.SuccessfullyCreated++;
                    _logger.LogDebug("Successfully created user {UserId} from external user {ExternalUserId} with National ID {NationalId} in source {SourceName}", 
                        commandResult.Data, record.ExternalUserId, record.NationalId, source.SourceName);
                }
                else if (commandResult.Message == "User creation already processed")
                {
                    // This is expected behavior for idempotency - treat as success
                    result.SuccessfullyCreated++;
                    _logger.LogDebug("User {ExternalUserId} with National ID {NationalId} from {SourceName} already processed (idempotency)", 
                        record.ExternalUserId, record.NationalId, source.SourceName);
                }
                else
                {
                    result.Failed++;
                    result.Errors.Add(new UserSeedError
                    {
                        ErrorCode = "USER_CREATION_FAILED",
                        Message = commandResult.Message,
                        ExternalUserId = record.ExternalUserId.ToString(),
                        SourceSystem = source.SourceName,
                        OccurredAt = DateTime.UtcNow
                    });
                    _logger.LogWarning("Failed to create user {ExternalUserId} with National ID {NationalId} from {SourceName}: {Error}", 
                        record.ExternalUserId, record.NationalId, source.SourceName, commandResult.Message);
                }
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.Errors.Add(new UserSeedError
                {
                    ErrorCode = "PROCESSING_ERROR",
                    Message = ex.Message,
                    ExternalUserId = record.ExternalUserId.ToString(),
                    SourceSystem = source.SourceName,
                    OccurredAt = DateTime.UtcNow
                });
                _logger.LogError(ex, "Error processing user record {ExternalUserId} with National ID {NationalId} from source {SourceName}", 
                    record.ExternalUserId, record.NationalId ?? "N/A", source.SourceName);
                
                // Continue processing other users - don't break the entire batch
            }
        }

        // Log batch processing summary
        _logger.LogInformation("Batch processing completed for {SourceName}: " +
            "Total={TotalProcessed}, Created={SuccessfullyCreated}, Skipped={Skipped}, Failed={Failed}, ValidationErrors={ValidationErrors}",
            source.SourceName, result.TotalProcessed, result.SuccessfullyCreated, result.Skipped, result.Failed, result.ValidationErrors);

        return result;
    }

    private static string GenerateIdempotencyKey(UserSeedRecord record)
    {
        var keyComponents = new List<string>
        {
            $"src:{record.SourceSystem}",
            $"national:{record.NationalId}", // Primary key for uniqueness
            $"ext:{record.ExternalUserId}",
            $"phone:{record.PhoneNumber}"
        };
        
        return string.Join("|", keyComponents);
    }
}
