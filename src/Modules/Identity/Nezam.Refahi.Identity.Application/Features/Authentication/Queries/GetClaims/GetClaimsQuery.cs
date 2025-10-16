using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Identity.Domain.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Identity.Application.Features.Authentication.Queries.GetClaims;

public class GetClaimsQuery : IQuery<ApplicationResult<IEnumerable<ClaimDto>>>
{
  
}
