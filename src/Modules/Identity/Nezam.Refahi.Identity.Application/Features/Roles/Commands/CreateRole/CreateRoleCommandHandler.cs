using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Roles.Commands.CreateRole;

/// <summary>
/// Handler for creating a new role
/// </summary>
public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApplicationResult<Guid>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRoleCommandHandler> _logger;

    public CreateRoleCommandHandler(
        IRoleRepository roleRepository,
        IIdentityUnitOfWork unitOfWork,
        ILogger<CreateRoleCommandHandler> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);

        try
        {
            // 1. Check if role name already exists
            var existingRole = await _roleRepository.GetByNameAsync(request.Name, cancellationToken:cancellationToken);
            if (existingRole != null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<Guid>.Failure("A role with this name already exists");
            }

            // 2. Create the role
            var role = new Role(
                name: request.Name,
                description: request.Description,
                isSystemRole: request.IsSystemRole,
                displayOrder: request.DisplayOrder
            );

            await _roleRepository.AddAsync(role, cancellationToken:cancellationToken);

            // 3. Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Successfully created role {RoleName} with ID {RoleId}", 
                request.Name, role.Id);

            return ApplicationResult<Guid>.Success(role.Id, "Role created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to create role {RoleName}", request.Name);
            return ApplicationResult<Guid>.Failure("Failed to create role. Please try again.");
        }
    }
}