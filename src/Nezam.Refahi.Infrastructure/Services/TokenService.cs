using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Identity.Enums;

namespace Nezam.Refahi.Infrastructure.Services;

/// <summary>
/// Service for generating and validating JWT tokens
/// </summary>
public class TokenService : ITokenService
{
  private readonly IConfiguration _configuration;

  public TokenService(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public string GenerateToken(User user)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ??
                                     throw new InvalidOperationException("JWT key is not configured"));

    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()), new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
    };

    // Add phone number if available
    if (!string.IsNullOrEmpty(user.PhoneNumber))
    {
      claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
    }

    // Add role claims
    if (user.HasRole(Role.User))
    {
      claims.Add(new Claim(ClaimTypes.Role, "User"));
    }

    if (user.HasRole(Role.Engineer))
    {
      claims.Add(new Claim(ClaimTypes.Role, "User"));
      claims.Add(new Claim(ClaimTypes.Role, "Engineer"));
    }

    if (user.HasRole(Role.Editor))
    {
      claims.Add(new Claim(ClaimTypes.Role, "User"));
      claims.Add(new Claim(ClaimTypes.Role, "Engineer"));
      claims.Add(new Claim(ClaimTypes.Role, "Editor"));
    }

    if (user.HasRole(Role.Manager))
    {
      claims.Add(new Claim(ClaimTypes.Role, "User"));
      claims.Add(new Claim(ClaimTypes.Role, "Engineer"));
      claims.Add(new Claim(ClaimTypes.Role, "Editor"));
      claims.Add(new Claim(ClaimTypes.Role, "Manager"));
    }

    if (user.HasRole(Role.Administrator))
    {
      claims.Add(new Claim(ClaimTypes.Role, "User"));
      claims.Add(new Claim(ClaimTypes.Role, "Engineer"));
      claims.Add(new Claim(ClaimTypes.Role, "Editor"));
      claims.Add(new Claim(ClaimTypes.Role, "Manager"));
      claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
    }

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddDays(7),
      Issuer = _configuration["Jwt:Issuer"],
      Audience = _configuration["Jwt:Audience"],
      SigningCredentials =
        new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  public bool ValidateToken(string token)
  {
    if (string.IsNullOrEmpty(token))
      return false;

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ??
                                      throw new InvalidOperationException("JWT key is not configured"));

    try
    {
      tokenHandler.ValidateToken(token,
        new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidIssuer = _configuration["Jwt:Issuer"],
          ValidateAudience = true,
          ValidAudience = _configuration["Jwt:Audience"],
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        }, out _);

      return true;
    }
    catch
    {
      return false;
    }
  }
}
