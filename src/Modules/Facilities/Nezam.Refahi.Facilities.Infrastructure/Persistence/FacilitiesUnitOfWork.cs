using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Infrastructure.Persistence;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for Facilities bounded context
/// </summary>
public class FacilitiesUnitOfWork : BaseUnitOfWork<FacilitiesDbContext>, IFacilitiesUnitOfWork
{
    public FacilitiesUnitOfWork(
        FacilitiesDbContext context,
        IMediator mediator,
        ILogger<FacilitiesUnitOfWork> logger)
        : base(context, mediator, logger)
    {
    }
}
