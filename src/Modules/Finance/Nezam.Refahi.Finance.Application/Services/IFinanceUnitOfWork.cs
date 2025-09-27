using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Finance.Application.Services;

/// <summary>
/// Unit of Work interface for Finance module
/// Manages database transactions and coordinates domain event publishing
/// </summary>
public interface IFinanceUnitOfWork : IUnitOfWork
{
}
