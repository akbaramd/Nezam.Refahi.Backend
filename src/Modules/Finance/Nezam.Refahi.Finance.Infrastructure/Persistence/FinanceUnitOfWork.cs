using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence;

/// <summary>
/// Implementation of Unit of Work pattern for Finance bounded context
/// Manages database transactions and coordinates domain event publishing through MediatR
/// </summary>
public class FinanceUnitOfWork : BaseUnitOfWork<FinanceDbContext>, IFinanceUnitOfWork
{
    public FinanceUnitOfWork(
        FinanceDbContext context,
        IMediator mediator,
        ILogger<FinanceUnitOfWork> logger)
        : base(context, mediator, logger)
    {
    }
}