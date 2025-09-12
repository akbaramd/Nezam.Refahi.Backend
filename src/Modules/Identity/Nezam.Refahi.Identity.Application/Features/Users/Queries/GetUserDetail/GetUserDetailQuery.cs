using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUserDetail;

public class GetUserDetailQuery : IQuery<ApplicationResult<GetUserDetailQueryResult>>
{
  public GetUserDetailQuery(Guid id)
  {
    Id = id;
  }

  public Guid Id { get; set; } 
}