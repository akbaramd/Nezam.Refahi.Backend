using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Gets a user by their national ID
    /// </summary>
    /// <param name="nationalId">The user's national ID</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByNationalIdAsync(NationalId nationalId);
    
    /// <summary>
    /// Gets a user by their phone number
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
}
