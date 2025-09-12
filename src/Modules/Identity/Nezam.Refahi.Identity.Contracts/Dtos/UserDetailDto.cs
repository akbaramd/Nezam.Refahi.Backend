using System;
using System.Collections.Generic;

namespace Nezam.Refahi.Identity.Contracts.Dtos
{
  public class UserDetailDto : UserDto 
    {
      // References (not flattened)
      public List<UserRoleDto> Roles { get; set; } = new();
      public List<UserClaimDto> Claims { get; set; } = new();
      public List<UserPreferenceDto> Preferences { get; set; } = new();
    }
}
