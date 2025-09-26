using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.BasicDefinitions.Application.Services;

/// <summary>
/// Unit of Work pattern for BasicDefinitions bounded context
/// Manages transactions and coordinates saving changes across repositories
/// </summary>
public interface IBasicDefinitionsUnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Begins a new database transaction
    /// </summary>
   
}
