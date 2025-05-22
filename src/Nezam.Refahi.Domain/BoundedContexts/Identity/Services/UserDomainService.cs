using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Repositories;
using Nezam.Refahi.Domain.BoundedContexts.Identity.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nezam.Refahi.Domain.BoundedContexts.Identity.Services;

/// <summary>
/// Domain service for user-related operations that don't naturally belong to the User entity
/// </summary>
public class UserDomainService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenDomainService _tokenDomainService;

    public UserDomainService(IUserRepository userRepository, TokenDomainService tokenDomainService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenDomainService = tokenDomainService ?? throw new ArgumentNullException(nameof(tokenDomainService));
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

        // Create the new user with default User role
        var user = new User(firstName, lastName, nationalIdValue, phoneNumber);
        
        // Add the user to the repository
        await _userRepository.AddAsync(user);
        
        return user;
    }
    
    /// <summary>
    /// Registers a new user with specific roles in the system
    /// </summary>
    /// <param name="firstName">User's first name</param>
    /// <param name="lastName">User's last name</param>
    /// <param name="nationalIdValue">User's national ID</param>
    /// <param name="role">Initial roles to assign to the user</param>
    /// <param name="phoneNumber">User's phone number (optional)</param>
    /// <returns>The newly created user</returns>
    /// <exception cref="InvalidOperationException">Thrown if a user with the same national ID already exists</exception>
    public async Task<User> RegisterUserWithRoleAsync(string firstName, string lastName, string nationalIdValue, Role role, string? phoneNumber = null)
    {
        // Check if the national ID is already in use
        if (!await IsNationalIdUniqueAsync(nationalIdValue))
        {
            throw new InvalidOperationException($"A user with national ID {nationalIdValue} already exists");
        }

        // Create the new user with specified role
        var user = new User(firstName, lastName, nationalIdValue, phoneNumber, role);
        
        // Add the user to the repository
        await _userRepository.AddAsync(user);
        
        return user;
    }
    
    /// <summary>
    /// Creates a new user with just a phone number (for OTP authentication)
    /// </summary>
    /// <param name="phoneNumber">The phone number to associate with the user</param>
    /// <returns>The newly created user</returns>
    /// <exception cref="ArgumentException">Thrown if the phone number is invalid</exception>
    public User CreateUser(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
            
        // Create a new user with default User role
        return new User(phoneNumber);
    }
    
    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="role">The role to assign</param>
    /// <returns>The updated user</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<User> AssignRoleAsync(Guid userId, Role role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        user.AssignRole(role);
        await _userRepository.UpdateAsync(user);
        
        return user;
    }
    
    /// <summary>
    /// Adds a role to a user's existing roles
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="role">The role to add</param>
    /// <returns>The updated user</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<User> AddRoleAsync(Guid userId, Role role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        user.AddRole(role);
        await _userRepository.UpdateAsync(user);
        
        return user;
    }
    
    /// <summary>
    /// Removes a role from a user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="role">The role to remove</param>
    /// <returns>The updated user</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<User> RemoveRoleAsync(Guid userId, Role role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        user.RemoveRole(role);
        await _userRepository.UpdateAsync(user);
        
        return user;
    }
    
    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="role">The role to check</param>
    /// <returns>True if the user has the role, false otherwise</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<bool> HasRoleAsync(Guid userId, Role role)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        return user.HasRole(role);
    }
    
    /// <summary>
    /// Checks if a user has access to perform an operation based on their role
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="requiredRole">The role required for the operation</param>
    /// <returns>True if the user has access, false otherwise</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<bool> HasAccessAsync(Guid userId, Role requiredRole)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        // Administrator role has access to everything
        if (user.HasRole(Role.Administrator))
        {
            return true;
        }
        
        return user.HasRole(requiredRole);
    }
    
    #region Token Management
    
    /// <summary>
    /// Generates an OTP code for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="expiresInMinutes">Minutes until OTP expiration (default: 5)</param>
    /// <param name="deviceId">Optional device identifier</param>
    /// <param name="ipAddress">Optional IP address</param>
    /// <returns>The generated OTP code</returns>
    public async Task<string> GenerateOtpForUserAsync(Guid userId, int expiresInMinutes = 5, string? deviceId = null, string? ipAddress = null)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        // Delegate to token domain service
        return await _tokenDomainService.GenerateOtpAsync(userId, expiresInMinutes, deviceId, ipAddress);
    }
    
    /// <summary>
    /// Generates an OTP code for a user identified by phone number
    /// </summary>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <param name="expiresInMinutes">Minutes until OTP expiration (default: 5)</param>
    /// <param name="deviceId">Optional device identifier</param>
    /// <param name="ipAddress">Optional IP address</param>
    /// <returns>The generated OTP code and user ID</returns>
    /// <exception cref="InvalidOperationException">Thrown if the user doesn't exist</exception>
    public async Task<(string otpCode, Guid userId)> GenerateOtpForPhoneNumberAsync(string phoneNumber, int expiresInMinutes = 5, string? deviceId = null, string? ipAddress = null)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);
        if (user == null)
        {
            throw new InvalidOperationException($"User with phone number {phoneNumber} not found");
        }
        
        var otpCode = await _tokenDomainService.GenerateOtpAsync(user.Id, expiresInMinutes, deviceId, ipAddress);
        return (otpCode, user.Id);
    }
    
    /// <summary>
    /// Validates an OTP code for a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="otpCode">The OTP code to validate</param>
    /// <returns>True if the OTP is valid, false otherwise</returns>
    public async Task<bool> ValidateOtpAsync(Guid userId, string otpCode)
    {
        return await _tokenDomainService.ValidateOtpAsync(userId, otpCode);
    }
    

    /// <summary>
    /// Generates authentication tokens for a user (both JWT reference and refresh token)
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="jwtId">The JWT ID (jti claim)</param>
    /// <param name="jwtExpiresInMinutes">Minutes until JWT expiration</param>
    /// <param name="refreshTokenExpiresInMinutes">Minutes until refresh token expiration (default: 10080 = 7 days)</param>
    /// <param name="deviceId">Optional device identifier</param>
    /// <param name="ipAddress">Optional IP address</param>
    /// <returns>The refresh token</returns>
    public async Task<string> GenerateAuthTokensAsync(Guid userId, string jwtId, int jwtExpiresInMinutes, int refreshTokenExpiresInMinutes = 10080, string? deviceId = null, string? ipAddress = null)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        // Store JWT reference
        await _tokenDomainService.StoreJwtReferenceAsync(userId, jwtId, jwtExpiresInMinutes, deviceId, ipAddress);
        
        // Generate refresh token
        return await _tokenDomainService.GenerateRefreshTokenAsync(userId, refreshTokenExpiresInMinutes, deviceId, ipAddress);
    }
    
    /// <summary>
    /// Validates a refresh token and returns the associated user ID if valid
    /// </summary>
    /// <param name="refreshToken">The refresh token to validate</param>
    /// <returns>The user ID if the token is valid, null otherwise</returns>
    public async Task<Guid?> ValidateRefreshTokenAsync(string refreshToken)
    {
        return await _tokenDomainService.ValidateRefreshTokenAsync(refreshToken);
    }
    
    /// <summary>
    /// Validates if a JWT ID is valid (not revoked)
    /// </summary>
    /// <param name="jwtId">The JWT ID to validate</param>
    /// <returns>True if the JWT is valid, false otherwise</returns>
    public async Task<bool> ValidateJwtAsync(string jwtId)
    {
        return await _tokenDomainService.ValidateJwtAsync(jwtId);
    }
    
    /// <summary>
    /// Logs out a user by revoking their JWT and refresh tokens
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="jwtId">The JWT ID to revoke (optional)</param>
    /// <param name="refreshToken">The refresh token to revoke (optional)</param>
    /// <returns>True if the logout was successful</returns>
    public async Task<bool> LogoutUserAsync(Guid userId, string? jwtId = null, string? refreshToken = null)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        bool success = true;
        
        // Revoke specific JWT if provided
        if (!string.IsNullOrEmpty(jwtId))
        {
            success = await _tokenDomainService.RevokeJwtAsync(jwtId) && success;
        }
        
        // Revoke specific refresh token if provided
        if (!string.IsNullOrEmpty(refreshToken))
        {
            success = await _tokenDomainService.RevokeRefreshTokenAsync(refreshToken) && success;
        }
        
        return success;
    }
    
    /// <summary>
    /// Logs out a user from all devices by revoking all their refresh tokens
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>Number of tokens revoked</returns>
    public async Task<int> LogoutUserFromAllDevicesAsync(Guid userId)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found");
        }
        
        return await _tokenDomainService.RevokeAllRefreshTokensAsync(userId);
    }
    
    #endregion
    
    /// <summary>
    /// Gets all users with a specific role
    /// </summary>
    /// <param name="role">The role to filter by</param>
    /// <returns>A list of users with the specified role</returns>
    public async Task<IEnumerable<User>> GetUsersByRoleAsync(Role role)
    {
        var allUsers = await _userRepository.GetAllAsync();
        var usersWithRole = new List<User>();
        
        foreach (var user in allUsers)
        {
            if (user.HasRole(role))
            {
                usersWithRole.Add(user);
            }
        }
        
        return usersWithRole;
    }
}
