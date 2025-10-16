using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Contracts.IntegrationEvents;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Shared.Infrastructure.Outbox;
using Nezam.Refahi.Shared.Domain.Services;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApplicationResult<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IOutboxPublisher _outboxPublisher;
    private readonly IIdempotencyService _idempotencyService;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository, 
        IRoleRepository roleRepository, 
        IIdentityUnitOfWork unitOfWork,
        IOutboxPublisher outboxPublisher,
        IIdempotencyService idempotencyService,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _outboxPublisher = outboxPublisher ?? throw new ArgumentNullException(nameof(outboxPublisher));
        _idempotencyService = idempotencyService ?? throw new ArgumentNullException(nameof(idempotencyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var correlationId = request.CorrelationId ?? Guid.NewGuid().ToString();
        var idempotencyKey = request.IdempotencyKey ?? GenerateIdempotencyKey(request);
        
        _logger.LogInformation("Processing CreateUserCommand for {FirstName} {LastName} with correlation {CorrelationId} and idempotency {IdempotencyKey}", 
            request.FirstName, request.LastName, correlationId, idempotencyKey);

        try
        {
            // Check idempotency first
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                var isProcessed = await _idempotencyService.IsEventProcessedAsync(idempotencyKey, cancellationToken);
                if (isProcessed)
                {
                    _logger.LogInformation("User creation already processed for idempotency key {IdempotencyKey}", idempotencyKey);
                    return ApplicationResult<Guid>.Failure("User creation already processed");
                }
            }

            // Check for existing user by external ID if provided
            if (request.ExternalUserId.HasValue)
            {
                var existingUserByExternalId = await _userRepository.GetByExternalIdAsync(request.ExternalUserId.Value, cancellationToken);
                if (existingUserByExternalId != null)
                {
                    _logger.LogInformation("User with external ID {ExternalUserId} already exists", request.ExternalUserId.Value);
                    return ApplicationResult<Guid>.Success(existingUserByExternalId.Id, "User already exists");
                }
            }

            // Check uniqueness unless explicitly skipped (for seeding operations)
            if (!request.SkipDuplicateCheck)
            {
                var existingUserByPhone = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);
                if (existingUserByPhone != null)
                {
                    return ApplicationResult<Guid>.Failure("این شماره موبایل قبلاً ثبت شده است");
                }

                var nationalIdVo = new NationalId(request.NationalId);
                var existingUserByNationalId = await _userRepository.GetByNationalIdAsync(nationalIdVo);
                if (existingUserByNationalId != null)
                {
                    return ApplicationResult<Guid>.Failure("این کد ملی قبلاً ثبت شده است");
                }

                // Check email uniqueness if provided
                if (!string.IsNullOrEmpty(request.Email))
                {
                    var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
                    if (existingUserByEmail != null)
                    {
                        return ApplicationResult<Guid>.Failure("این ایمیل قبلاً ثبت شده است");
                    }
                }
            }

            // Create new user with enhanced fields
            var user = new User(
                firstName: request.FirstName,
                lastName: request.LastName,
                nationalId: request.NationalId,
                phoneNumber: request.PhoneNumber,
                email: request.Email,
                username: request.Username,
                externalUserId: request.ExternalUserId
            );

            // Add source tracking metadata
            if (!string.IsNullOrEmpty(request.SourceSystem))
            {
                user.SetSourceMetadata(request.SourceSystem, request.SourceVersion, request.SourceChecksum);
            }

            // Add profile snapshot for audit
            if (!string.IsNullOrEmpty(request.ProfileSnapshot))
            {
                user.SetProfileSnapshot(request.ProfileSnapshot);
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // Add user to repository
                await _userRepository.AddAsync(user, cancellationToken: cancellationToken);

                // Assign roles
                await AssignRolesToUser(user, request.Roles, cancellationToken);

                // Add claims
                foreach (var claim in request.Claims)
                {
                    // Skip empty or invalid claims
                    if (!string.IsNullOrWhiteSpace(claim.Key) && !string.IsNullOrWhiteSpace(claim.Value))
                    {
                        user.AddClaim(claim.Key, claim.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Skipping invalid claim with key '{Key}' and value '{Value}' for user {UserId}", 
                            claim.Key ?? "null", claim.Value ?? "null", user.Id);
                    }
                }

                // Save changes within transaction
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                // Rollback transaction on any error
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }

            // Mark idempotency as processed
            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                await _idempotencyService.MarkEventAsProcessedAsync(idempotencyKey, user.Id, cancellationToken);
            }

            // Publish UserCreatedIntegrationEvent with enhanced metadata
            var userCreatedEvent = new UserCreatedIntegrationEvent
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalId = user.NationalId?.Value,
                PhoneNumber = user.PhoneNumber?.Value ?? string.Empty,
                Email = user.Email,
                Username = user.Username,
                ExternalUserId = user.ExternalUserId,
                SourceSystem = request.SourceSystem,
                SourceVersion = request.SourceVersion,
                CreatedAt = user.CreatedAt,
                CorrelationId = correlationId,
                IdempotencyKey = idempotencyKey,
                IsSeedingOperation = request.IsSeedingOperation,
                Claims = request.Claims
            };

            // Publish to outbox with idempotency and correlation
            await _outboxPublisher.PublishAsync(userCreatedEvent, user.Id, correlationId, idempotencyKey, cancellationToken);

            _logger.LogInformation("Successfully created user {UserId} with correlation {CorrelationId}", user.Id, correlationId);

            return ApplicationResult<Guid>.Success(
                data: user.Id,
                message: "کاربر با موفقیت ایجاد شد"
            );
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Validation error creating user with correlation {CorrelationId}", correlationId);
            return ApplicationResult<Guid>.Failure(ex, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with correlation {CorrelationId}", correlationId);
            return ApplicationResult<Guid>.Failure(ex, "خطا در ایجاد کاربر");
        }
    }

    private async Task AssignRolesToUser(User user, List<string> roleNames, CancellationToken cancellationToken)
    {
        foreach (var roleName in roleNames)
        {
            var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken: cancellationToken);
            if (role != null)
            {
                user.AssignRole(role);
            }
            else
            {
                _logger.LogWarning("Role {RoleName} not found, skipping assignment", roleName);
            }
        }

        // Always assign default Member role if no roles specified
        if (!roleNames.Any())
        {
            var memberRole = await _roleRepository.GetByNameAsync("Member", cancellationToken: cancellationToken);
            if (memberRole != null)
            {
                user.AssignRole(memberRole);
            }
        }
    }

    private static string GenerateIdempotencyKey(CreateUserCommand request)
    {
        var keyComponents = new List<string>();
        
        if (request.ExternalUserId.HasValue)
        {
            keyComponents.Add($"ext:{request.ExternalUserId.Value}");
        }
        
        if (!string.IsNullOrEmpty(request.SourceSystem))
        {
            keyComponents.Add($"src:{request.SourceSystem}");
        }
        
        keyComponents.Add($"phone:{request.PhoneNumber}");
        keyComponents.Add($"national:{request.NationalId}");
        
        return string.Join("|", keyComponents);
    }
}