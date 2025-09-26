using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.BasicDefinitions.Application.Services;
using Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;

/// <summary>
/// Implementation of Unit of Work pattern for BasicDefinitions bounded context
/// Manages database transactions and coordinates domain event publishing through MediatR
/// </summary>
public class BasicDefinitionsUnitOfWork : BaseUnitOfWork<BasicDefinitionsDbContext>, IBasicDefinitionsUnitOfWork
{
    public BasicDefinitionsUnitOfWork(
        BasicDefinitionsDbContext context, 
        IMediator mediator, 
        ILogger<BasicDefinitionsUnitOfWork> logger)
        : base(context, mediator, logger)
    {
    }
}
