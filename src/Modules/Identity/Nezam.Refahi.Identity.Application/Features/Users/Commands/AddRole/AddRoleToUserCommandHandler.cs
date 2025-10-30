using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Contracts.IntegrationEvents;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.AddRole;

/// <summary>
/// Handler for adding a role to a user
/// </summary>
public class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand, ApplicationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<AddRoleToUserCommandHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public AddRoleToUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IIdentityUnitOfWork unitOfWork,
        ILogger<AddRoleToUserCommandHandler> logger,
        IPublishEndpoint publishEndpoint)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);

        try
        {
            // 1. Verify user exists
            var user = await _userRepository.FindOneAsync(x=>x.Id == request.UserId, cancellationToken:cancellationToken);
            if (user == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("کاربر مورد نظر یافت نشد");
            }

            // 2. Verify role exists and is active
            var role = await _roleRepository.FindOneAsync(x=>x.Id==request.RoleId);
            if (role == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("نقش مورد نظر یافت نشد");
            }

            if (!role.IsActive)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("امکان اختصاص نقش غیرفعال به کاربر وجود ندارد");
            }

            // 3. Check if user already has this role (active assignment)
            var existingUserRole = user.UserRoles.Any(x=>x.RoleId == request.RoleId);
            if (existingUserRole)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult.Failure("این نقش قبلا به کاربر اختصاص داده شده است");
            }

            // 4. Create new UserRole assignment
            var userRole = new UserRole(
                userId: request.UserId,
                roleId: request.RoleId,
                expiresAt: request.ExpiresAt,
                assignedBy: request.AssignedBy?.ToString(),
                notes: request.Notes
            );

            await _userRoleRepository.AddAsync(userRole, cancellationToken:cancellationToken);

            // 5. Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Publish UserRoleChangedIntegrationEvent
            var userRoleChangedEvent = new UserRoleChangedIntegrationEvent
            {
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber?.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalCode = user.NationalId?.Value,
                AddedRoles = new List<string> { role.Name },
                RemovedRoles = new List<string>(),
                CurrentRoles = user.UserRoles.Select(ur => ur.Role?.Name ?? string.Empty).Where(r => !string.IsNullOrEmpty(r)).ToList(),
                ChangedAt = DateTime.UtcNow,
                ChangedBy = request.AssignedBy?.ToString() ?? "System"
            };

            await _publishEndpoint.Publish(userRoleChangedEvent, cancellationToken);

            _logger.LogInformation("Successfully assigned role {RoleId} to user {UserId}", 
                request.RoleId, request.UserId);

            return ApplicationResult.Success("نقش با موفقیت به کاربر اختصاص داده شد");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to assign role {RoleId} to user {UserId}", 
                request.RoleId, request.UserId);
            return ApplicationResult.Failure("خطا در اختصاص نقش به کاربر. لطفا مجددا تلاش کنید.");
        }
    }
}
