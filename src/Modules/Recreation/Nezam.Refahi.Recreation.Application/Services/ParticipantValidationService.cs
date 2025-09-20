using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Service for validating participants and their membership status
/// </summary>
public class ParticipantValidationService
{
    private readonly IMemberService _memberService;
    private readonly ILogger<ParticipantValidationService> _logger;

    public ParticipantValidationService(
        IMemberService memberService,
        ILogger<ParticipantValidationService> logger)
    {
        _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates a participant and checks membership status if required
    /// </summary>
    /// <param name="participant">The participant to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with member information if applicable</returns>
    public async Task<ParticipantValidationResult> ValidateParticipantAsync(
        GuestParticipantDto participant, 
        CancellationToken cancellationToken = default)
    {
        if (participant == null)
        {
            return ParticipantValidationResult.Failed("شرکت‌کننده نمی‌تواند خالی باشد");
        }

        // If participant type is Member, validate membership
        if (participant.ParticipantType == ParticipantType.Member)
        {
            return await ValidateMembershipAsync(participant, cancellationToken);
        }

        // For guests, no additional validation needed beyond basic DTO validation
        return ParticipantValidationResult.Success(participant.ParticipantType);
    }

    /// <summary>
    /// Validates multiple participants
    /// </summary>
    /// <param name="participants">List of participants to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of validation results</returns>
    public async Task<List<ParticipantValidationResult>> ValidateParticipantsAsync(
        IEnumerable<GuestParticipantDto> participants,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ParticipantValidationResult>();
        
        foreach (var participant in participants)
        {
            var result = await ValidateParticipantAsync(participant, cancellationToken);
            results.Add(result);
        }

        return results;
    }

    private async Task<ParticipantValidationResult> ValidateMembershipAsync(
        GuestParticipantDto participant,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Validating membership for national number: {NationalNumber}", 
                participant.NationalNumber);

            var nationalId = new NationalId(participant.NationalNumber);
            var member = await _memberService.GetMemberByNationalCodeAsync(nationalId);

            if (member == null)
            {
                _logger.LogWarning("Member not found for national number: {NationalNumber}", 
                    participant.NationalNumber);
                
                return ParticipantValidationResult.Failed(
                    $"شخص با کد ملی {participant.NationalNumber} عضو نظام نیست و می‌تواند به عنوان مهمان اضافه شود");
            }

            _logger.LogInformation("Member found: {MemberName} ({MembershipNumber})", 
                member.FullName, member.MembershipNumber);

            return ParticipantValidationResult.Success(ParticipantType.Member, member);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating membership for national number: {NationalNumber}", 
                participant.NationalNumber);
            
            return ParticipantValidationResult.Failed(
                "خطا در بررسی عضویت. لطفاً دوباره تلاش کنید یا به عنوان مهمان اضافه کنید");
        }
    }
}

/// <summary>
/// Result of participant validation
/// </summary>
public class ParticipantValidationResult
{
    public bool IsValid { get; private set; }
    public ParticipantType ParticipantType { get; private set; }
    public string? ErrorMessage { get; private set; }
    public object? MemberInfo { get; private set; }

    private ParticipantValidationResult(bool isValid, ParticipantType participantType, string? errorMessage = null, object? memberInfo = null)
    {
        IsValid = isValid;
        ParticipantType = participantType;
        ErrorMessage = errorMessage;
        MemberInfo = memberInfo;
    }

    public static ParticipantValidationResult Success(ParticipantType participantType, object? memberInfo = null)
    {
        return new ParticipantValidationResult(true, participantType, null, memberInfo);
    }

    public static ParticipantValidationResult Failed(string errorMessage)
    {
        return new ParticipantValidationResult(false, ParticipantType.Guest, errorMessage);
    }
}
