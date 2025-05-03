using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Gets a user by their national ID
    /// </summary>
    /// <param name="nationalId">The user's national ID</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByNationalIdAsync(NationalId nationalId);
    
    /// <summary>
    /// Adds a new user to the repository
    /// </summary>
    /// <param name="user">The user to add</param>
    Task AddAsync(User user);
    
    /// <summary>
    /// Updates an existing user in the repository
    /// </summary>
    /// <param name="user">The user to update</param>
    Task UpdateAsync(User user);
    
    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>A collection of all users</returns>
    Task<IEnumerable<User>> GetAllAsync();
}
