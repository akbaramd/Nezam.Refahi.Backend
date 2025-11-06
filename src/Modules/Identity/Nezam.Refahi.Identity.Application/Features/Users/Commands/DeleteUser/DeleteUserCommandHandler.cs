using MCA.SharedKernel.Application.Contracts;
using MediatR;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Contracts.IntegrationEvents;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApplicationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IBus _publishEndpoint;

    public DeleteUserCommandHandler(
        IUserRepository userRepository, 
        IIdentityUnitOfWork unitOfWork,
        IBus publishEndpoint)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Find the user to delete
            var user = await _userRepository.FindOneAsync(x=>x.Id==request.Id, cancellationToken:cancellationToken);
            if (user == null)
            {
                return ApplicationResult.Failure("کاربر یافت نشد");
            }

            // Store user info for integration event
            var userInfo = new
            {
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber?.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalCode = user.NationalId?.Value
            };

            // Check if user has active tokens or active sessions that might prevent deletion
            var activeTokens = user.GetActiveTokensForUserAsync(request.Id);
            if (activeTokens.Any())
            {
                // Revoke all active tokens before deletion
                user.RevokeAllUserRefreshTokens(isSoftDelete: true);
            }

            if (request.SoftDelete)
            {
                // Soft delete: Deactivate the user
                var lockReason = string.IsNullOrEmpty(request.DeleteReason) 
                    ? "کاربر توسط مدیر سیستم غیرفعال شده است" 
                    : $"غیرفعال شده به دلیل: {request.DeleteReason}";
                    
                user.Lock(lockReason, lockDurationMinutes: 0); // 0 means indefinite lock
                
                await _userRepository.UpdateAsync(user, cancellationToken:cancellationToken);
            }
            else
            {
                // Hard delete: Completely remove the user
                await _userRepository.DeleteAsync(user, cancellationToken:cancellationToken);
            }
            
            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Publish UserDeletedIntegrationEvent
            var userDeletedEvent = new UserDeletedIntegrationEvent
            {
                UserId = userInfo.UserId,
                PhoneNumber = userInfo.PhoneNumber,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                NationalCode = userInfo.NationalCode,
                DeletedAt = DateTime.UtcNow,
                DeletedBy = "System", // TODO: Get from current user context
                Reason = request.DeleteReason ?? (request.SoftDelete ? "Soft delete by system" : "Hard delete by system")
            };

            await _publishEndpoint.Publish(userDeletedEvent, cancellationToken);

            var successMessage = request.SoftDelete 
                ? "کاربر با موفقیت غیرفعال شد" 
                : "کاربر با موفقیت حذف شد";
                
            return ApplicationResult.Success(successMessage);
        }
        catch (InvalidOperationException ex)
        {
            return ApplicationResult.Failure(ex, ex.Message);
        }
        catch (Exception ex)
        {
            var errorMessage = request.SoftDelete 
                ? $"خطا در غیرفعال‌سازی کاربر: {ex.Message}"
                : $"خطا در حذف کاربر: {ex.Message}";
                
            return ApplicationResult.Failure(errorMessage);
        }
    }
}