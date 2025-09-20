using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MCA.SharedKernel.Application.Contracts;
using MediatR;
using Nezam.Refahi.Identity.Contracts.Dtos;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUserDetail
{
    public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, ApplicationResult<GetUserDetailQueryResult>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserDetailQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApplicationResult<GetUserDetailQueryResult>> Handle(GetUserDetailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindOneAsync(x => x.Id == request.Id);
            if (user is null)
                return ApplicationResult<GetUserDetailQueryResult>.Failure("UserDetail not found.");

            var dto = MapToUserDto(user);

            return ApplicationResult<GetUserDetailQueryResult>.Success(new GetUserDetailQueryResult
            {
                UserDetail = dto
            });
        }

        private static UserDetailDto MapToUserDto(User user)
        {
            var dto = new UserDetailDto
            {
                // Core Identity
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalId = user.NationalId?.Value,
                PhoneNumber = user.PhoneNumber.Value,

                // Auth & Status
                IsPhoneVerified = user.IsPhoneVerified,
                PhoneVerifiedAt = user.PhoneVerifiedAt,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                LastAuthenticatedAt = user.LastAuthenticatedAt,
                FailedAttempts = user.FailedAttempts,
                LockedAt = user.LockedAt,
                LockReason = user.LockReason,
                UnlockAt = user.UnlockAt,

                // Device & Network
                LastIpAddress = user.LastIpAddress,
                LastUserAgent = user.LastUserAgent,
                LastDeviceFingerprint = user.LastDeviceFingerprint?.Value,

                // Audit (نام‌های Utc با DTO هم‌تراز می‌شود)
                CreatedAtUtc = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                UpdatedAtUtc = user.LastModifiedAt,
            };

            // Roles (UserRoleDto سبک، بدون تزریق RoleDto کامل برای جلوگیری از گراف‌های سنگین)
            dto.Roles = user.UserRoles
                .Select(ur => new UserRoleDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role?.Name ?? string.Empty,
                    IsActive = ur.IsActive,
                    AssignedAt = ur.AssignedAt,
                    ExpiresAt = ur.ExpiresAt,
                    AssignedBy = ur.AssignedBy,
                    Notes = ur.Notes
                })
                .ToList();

            // Claims (UserClaimDto → ClaimDto)
            dto.Claims = user.UserClaims
                .Select(uc => new UserClaimDto
                {
                    Id = uc.Id,
                    UserId = uc.UserId,
                    IsActive = uc.IsActive,
                    AssignedAt = uc.AssignedAt,
                    ExpiresAt = uc.ExpiresAt,
                    AssignedBy = uc.AssignedBy,
                    Notes = uc.Notes,
                    Claim = new ClaimDto
                    {
                        Type = uc.Claim.Type,
                        Value = uc.Claim.Value,
                        ValueType = uc.Claim.ValueType
                    }
                })
                .ToList();

            // Preferences (UserPreferenceDto → PreferenceValueDto)
            dto.Preferences = user.Preferences
                .Select(p => new UserPreferenceDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    Key = p.Key.Value,
                    Value = new PreferenceValueDto
                    {
                        RawValue = p.Value.RawValue,
                        Type = (int)p.Value.Type // نام enum
                    },
                    Category = p.Category.ToString(),
                    DisplayOrder = p.DisplayOrder,
                    IsActive = p.IsActive
                })
                .ToList();

    
            return dto;
        }
    }
}
