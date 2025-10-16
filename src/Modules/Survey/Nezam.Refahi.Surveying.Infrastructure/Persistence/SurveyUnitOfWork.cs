using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence;

/// <summary>
/// Implementation of Unit of Work pattern for Survey bounded context
/// Manages database transactions and coordinates domain event publishing through MediatR
/// </summary>
public class SurveyUnitOfWork : BaseUnitOfWork<SurveyDbContext>, ISurveyUnitOfWork
{
    public SurveyUnitOfWork(
        SurveyDbContext context,
        IMediator mediator,
        ILogger<SurveyUnitOfWork> logger)
        : base(context, mediator, logger)
    {
    }
}
