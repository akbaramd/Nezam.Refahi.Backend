using MCA.SharedKernel.Domain.Contracts.Spesifications;
using Nezam.Refahi.Identity.Domain.Entities;

namespace Nezam.Refahi.Identity.Application.Features.Users.Queries.GetUsersPaginated;

public class UserPagiantedSpec : IPaginatedSpecification<User>
{
  public UserPagiantedSpec(int pageNumber, int pageSize,string? search)
  {
    Skip = (pageNumber - 1) * pageSize;
    Take = pageSize;
    Search = search;
  }

  public string? Search { get; set; }

  public void Handle(ISpecificationContext<User> context)
  {


    if (!string.IsNullOrWhiteSpace(Search))
    {
      context.AddFilter(x => x.FirstName.Contains(Search)).Or(x => x.LastName.Contains(Search))
        .Or(x=>x.PhoneNumber.Value.Contains(Search))
        .Or(x => x.NationalId != null && x.NationalId.Value.Contains(Search));
    }
 

   
  }

  public int Skip { get; }
  public int Take { get; }
}