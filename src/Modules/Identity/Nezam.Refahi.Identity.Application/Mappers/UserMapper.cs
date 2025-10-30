using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Application.Mappers;

public class UserMapper : IMapper<User,UserDto>
{
  public Task<UserDto> MapAsync(User source, CancellationToken cancellationToken = new CancellationToken())
  {
    var dto = new UserDto
    {
      Id = source.Id,
      FirstName = source.FirstName,
      LastName = source.LastName,
      NationalId = source.NationalId?.Value,
      PhoneNumber = source.PhoneNumber.Value,

      IsPhoneVerified = source.IsPhoneVerified,
      PhoneVerifiedAt = source.PhoneVerifiedAt,
      IsActive = source.IsActive,
      LastLoginAt = source.LastLoginAt,
      LastAuthenticatedAt = source.LastAuthenticatedAt,
      FailedAttempts = source.FailedAttempts,
      LockedAt = source.LockedAt,
      LockReason = source.LockReason,
      UnlockAt = source.UnlockAt,

      LastIpAddress = source.LastIpAddress,
      LastUserAgent = source.LastUserAgent,
      LastDeviceFingerprint = source.LastDeviceFingerprint?.Value,

      CreatedAtUtc = source.CreatedAt,
      CreatedBy = source.CreatedBy,
      UpdatedAtUtc = source.LastModifiedAt,
      UpdatedBy = source.LastModifiedBy
    };

    return Task.FromResult(dto);
  }

  public Task MapAsync(User source, UserDto destination, CancellationToken cancellationToken = new CancellationToken())
  {
    destination.Id = source.Id;
    destination.FirstName = source.FirstName;
    destination.LastName = source.LastName;
    destination.NationalId = source.NationalId?.Value;
    destination.PhoneNumber = source.PhoneNumber.Value;

    destination.IsPhoneVerified = source.IsPhoneVerified;
    destination.PhoneVerifiedAt = source.PhoneVerifiedAt;
    destination.IsActive = source.IsActive;
    destination.LastLoginAt = source.LastLoginAt;
    destination.LastAuthenticatedAt = source.LastAuthenticatedAt;
    destination.FailedAttempts = source.FailedAttempts;
    destination.LockedAt = source.LockedAt;
    destination.LockReason = source.LockReason;
    destination.UnlockAt = source.UnlockAt;

    destination.LastIpAddress = source.LastIpAddress;
    destination.LastUserAgent = source.LastUserAgent;
    destination.LastDeviceFingerprint = source.LastDeviceFingerprint?.Value;

    destination.CreatedAtUtc = source.CreatedAt;
    destination.CreatedBy = source.CreatedBy;
    destination.UpdatedAtUtc = source.LastModifiedAt;
    destination.UpdatedBy = source.LastModifiedBy;

    return Task.CompletedTask;
  }
}
