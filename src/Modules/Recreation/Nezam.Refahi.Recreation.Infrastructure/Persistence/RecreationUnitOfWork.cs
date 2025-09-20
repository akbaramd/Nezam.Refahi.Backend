using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence;

/// <summary>
/// Unit of Work implementation for Recreation module
/// </summary>
public class RecreationUnitOfWork : BaseUnitOfWork<RecreationDbContext>, IRecreationUnitOfWork
{
    public RecreationUnitOfWork(RecreationDbContext context,IMediator mediator,ILogger<RecreationUnitOfWork> logger) : base(context,mediator,logger)
    {
    }

}