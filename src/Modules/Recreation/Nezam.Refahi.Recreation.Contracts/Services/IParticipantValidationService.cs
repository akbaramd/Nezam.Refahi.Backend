
namespace Nezam.Refahi.Recreation.Contracts.Services;

/// <summary>
/// Service interface for validating participants and their membership status
/// This follows DDD principles by providing a clean service contract for cross-context communication
/// Uses domain-agnostic request DTOs to avoid coupling with other module DTOs
/// </summary>
public interface IParticipantValidationService
{
    /// <summary>
    /// Validates a participant and checks membership status if required
    /// </summary>
    /// <param name="request">The participant validation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with participant type and member information if applicable</returns>
    Task<ParticipantValidationResult> ValidateParticipantAsync(
        ParticipantValidationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates multiple participants
    /// </summary>
    /// <param name="requests">List of participant validation requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of validation results</returns>
    Task<List<ParticipantValidationResult>> ValidateParticipantsAsync(
        IEnumerable<ParticipantValidationRequest> requests,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of participant validation
/// ParticipantType is stored as string to avoid dependency on Domain enum in Contracts
/// </summary>
public class ParticipantValidationResult
{
    public bool IsValid { get; private set; }
    public string ParticipantType { get; private set; } = string.Empty;
    public string? ErrorMessage { get; private set; }

    private ParticipantValidationResult(bool isValid, string participantType, string? errorMessage = null)
    {
        IsValid = isValid;
        ParticipantType = participantType;
        ErrorMessage = errorMessage;
    }

    public static ParticipantValidationResult Success(string participantType)
    {
        return new ParticipantValidationResult(true, participantType, null);
    }

    public static ParticipantValidationResult Failed(string errorMessage)
    {
        return new ParticipantValidationResult(false, "Guest", errorMessage);
    }
}

