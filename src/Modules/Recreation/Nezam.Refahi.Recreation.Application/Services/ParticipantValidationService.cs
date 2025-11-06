using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Dtos;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Recreation.Contracts.Services;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Services;

/// <summary>
/// Implementation of participant validation service
/// Validates participants and checks membership status if required
/// </summary>
public class ParticipantValidationService : IParticipantValidationService
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
    /// <param name="request">The participant validation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with participant type and member information if applicable</returns>
    public async Task<ParticipantValidationResult> ValidateParticipantAsync(
        ParticipantValidationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return ParticipantValidationResult.Failed("شرکت‌کننده نمی‌تواند خالی باشد");
        }

        // If participant type is Member, validate membership
        if (request.ParticipantType == ParticipantType.Member.ToString())
        {
            return await ValidateMembershipAsync(request, cancellationToken);
        }

        // For guests, no additional validation needed beyond basic validation
        return ParticipantValidationResult.Success(ParticipantType.Guest.ToString());
    }

    /// <summary>
    /// Validates multiple participants
    /// </summary>
    /// <param name="requests">List of participant validation requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of validation results</returns>
    public async Task<List<ParticipantValidationResult>> ValidateParticipantsAsync(
        IEnumerable<ParticipantValidationRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var results = new List<ParticipantValidationResult>();

        foreach (var request in requests)
        {
            var result = await ValidateParticipantAsync(request, cancellationToken);
            results.Add(result);
        }

        return results;
    }

    private async Task<ParticipantValidationResult> ValidateMembershipAsync(
        ParticipantValidationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Validating membership for national number: {NationalNumber}",
                request.NationalNumber);

            var nationalId = new NationalId(request.NationalNumber);
            var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);

            if (memberDetail == null)
            {
                _logger.LogWarning("Member not found for national number: {NationalNumber}",
                    request.NationalNumber);

                return ParticipantValidationResult.Failed(
                    $"شخص با کد ملی {request.NationalNumber} عضو نظام نیست و می‌تواند به عنوان مهمان اضافه شود");
            }

            _logger.LogInformation("Member found: {MemberName} ({MembershipNumber}) with {CapabilityCount} capabilities and {FeatureCount} features",
                memberDetail.FullName, memberDetail.MembershipNumber, memberDetail.Capabilities?.Count ?? 0, memberDetail.Features?.Count ?? 0);

            return ParticipantValidationResult.Success(ParticipantType.Member.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating membership for national number: {NationalNumber}",
                request.NationalNumber);

            return ParticipantValidationResult.Failed(
                "خطا در بررسی عضویت. لطفاً دوباره تلاش کنید یا به عنوان مهمان اضافه کنید");
        }
    }
}
