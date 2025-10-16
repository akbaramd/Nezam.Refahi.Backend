using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUserDetail;

public class GetUserDetailQueryResult
{
  public UserDetailDto UserDetail
  {
    get;
    set;
  } = default!;
}
