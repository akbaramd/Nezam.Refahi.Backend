using MediatR;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Contracts.Commands;

/// <summary>
/// Command to start answering a survey (C1)
/// </summary>
public class StartSurveyResponseCommand : IRequest<ApplicationResult<StartSurveyResponseResponse>>
{
    public Guid SurveyId { get; set; }
    public string? NationalNumber { get; set; } // National number for member identification
    public string? ParticipantHash { get; set; } // For anonymous surveys
    public Dictionary<string, string>? DemographyData { get; set; } // Optional demographic data
    public bool ResumeActiveIfAny { get; set; } = true; // Resume active attempt if exists
    public bool ForceNewAttempt { get; set; } = false; // Force new attempt even if active exists
    public string? IdempotencyKey { get; set; } // For idempotent operations
}

public class StartSurveyResponseResponse
{
    public Guid ResponseId { get; set; }
    public Guid SurveyId { get; set; }
    public int AttemptNumber { get; set; }
    public string AttemptStatus { get; set; } = string.Empty; // Active, Submitted, Canceled, Expired
    public Guid? CurrentQuestionId { get; set; } // First unanswered question
    public int CurrentRepeatIndex { get; set; } = 1; // Current repeat index for repeatable questions
    public bool AllowsBackNavigation { get; set; }
    public CooldownInfo? Cooldown { get; set; } // Cooldown information for client
    public bool IsResumed { get; set; } // Whether existing attempt was resumed
    public bool CanAnswer { get; set; }
    public string? Message { get; set; }
    public List<string> EligibilityReasons { get; set; } = new(); // Why user can/cannot participate
}

public class CooldownInfo
{
    public bool Enabled { get; set; }
    public int Seconds { get; set; }
    public string? Message { get; set; }
}
