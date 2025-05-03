using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Services;

/// <summary>
/// Domain service for user-related operations that don't naturally belong to the User entity
/// </summary>
public class UserDomainService
{
    private readonly IUserRepository _userRepository;

    public UserDomainService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Checks if a user with the given national ID already exists
    /// </summary>
    /// <param name="nationalId">The national ID to check</param>
    /// <returns>True if a user with the given national ID exists, false otherwise</returns>
    public async Task<bool> IsNationalIdUniqueAsync(string nationalIdValue)
    {
        var nationalId = new NationalId(nationalIdValue);
        var existingUser = await _userRepository.GetByNationalIdAsync(nationalId);
        return existingUser == null;
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="nationalId">User's national ID</param>
    /// <param name="phoneNumber">User's phone number (optional)</param>
    /// <returns>The newly created user</returns>
    /// <exception cref="InvalidOperationException">Thrown if a user with the same national ID already exists</exception>
    public async Task<User> RegisterUserAsync(string firstName, string lastName, string nationalIdValue, string? phoneNumber = null)
    {
        // Check if the national ID is already in use
        if (!await IsNationalIdUniqueAsync(nationalIdValue))
        {
            throw new InvalidOperationException($"A user with national ID {nationalIdValue} already exists");
        }

        // Create the new user
        var user = new User(firstName, lastName, nationalIdValue, phoneNumber);
        
        // Add the user to the repository
        await _userRepository.AddAsync(user);
        
        return user;
    }
}
