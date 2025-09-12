using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Application.Services;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence;

/// <summary>
/// Implementation of Unit of Work pattern for Membership bounded context
/// Manages database transactions and coordinates domain event publishing through MediatR
/// </summary>
public class MembershipUnitOfWork : BaseUnitOfWork<MembershipDbContext>, IMembershipUnitOfWork
{
    public MembershipUnitOfWork(
        MembershipDbContext context, 
        IMediator mediator, 
        ILogger<MembershipUnitOfWork> logger)
        : base(context, mediator, logger)
    {
    }
}