namespace Nezam.Refahi.Identity.Domain.Dtos;

/// <summary>
/// Types of user lookup operations
/// </summary>
public enum UserLookupType
{
  NationalCode,
  ExternalId,
  PhoneNumber,
  Email
}