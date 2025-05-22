using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;

namespace Nezam.Refahi.Application.Common.Interfaces;

/// <summary>
/// Service for generating and validating authentication tokens
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for a user
    /// </summary>
    /// <param name="user">The user to generate a token for</param>
    /// <returns>The generated JWT token</returns>
    string GenerateToken(User user);
    
    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>True if the token is valid, false otherwise</returns>
    bool ValidateToken(string token);
}
