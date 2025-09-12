using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence;

/// <summary>
/// Implementation of Unit of Work pattern for Identity bounded context
/// Manages database transactions and coordinates domain event publishing through MediatR
/// </summary>
public class IdentityUnitOfWork : BaseUnitOfWork<IdentityDbContext>, IIdentityUnitOfWork
{
    public IdentityUnitOfWork(
        IdentityDbContext context, 
        IMediator mediator, 
        ILogger<IdentityUnitOfWork> logger)
        : base(context, mediator, logger)
    {
    }



   
}
