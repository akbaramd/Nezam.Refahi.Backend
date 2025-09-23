using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Domain.ValueObjects;

/// <summary>
/// State machine for tour status transitions
/// </summary>
public static class TourStateMachine
{
    /// <summary>
    /// Defines valid state transitions
    /// </summary>
    private static readonly Dictionary<TourStatus, HashSet<TourStatus>> ValidTransitions = new()
    {
        [TourStatus.Draft] = new()
        {
            TourStatus.Scheduled,
            TourStatus.Cancelled
        },
        [TourStatus.Scheduled] = new()
        {
            TourStatus.RegistrationOpen,
            TourStatus.Cancelled,
            TourStatus.Postponed,
            TourStatus.Suspended
        },
        [TourStatus.RegistrationOpen] = new()
        {
            TourStatus.RegistrationClosed,
            TourStatus.Cancelled,
            TourStatus.Postponed,
            TourStatus.Suspended
        },
        [TourStatus.RegistrationClosed] = new()
        {
            TourStatus.InProgress,
            TourStatus.Cancelled,
            TourStatus.Postponed,
            TourStatus.Suspended
        },
        [TourStatus.InProgress] = new()
        {
            TourStatus.Completed,
            TourStatus.Cancelled,
            TourStatus.Postponed
        },
        [TourStatus.Completed] = new()
        {
            TourStatus.Archived
        },
        [TourStatus.Postponed] = new()
        {
            TourStatus.Scheduled,
            TourStatus.RegistrationOpen,
            TourStatus.Cancelled
        },
        [TourStatus.Suspended] = new()
        {
            TourStatus.Scheduled,
            TourStatus.RegistrationOpen,
            TourStatus.Cancelled
        },
        // Terminal states (no transitions allowed)
        [TourStatus.Cancelled] = new(),
        [TourStatus.Archived] = new()
    };

    /// <summary>
    /// Checks if a state transition is valid
    /// </summary>
    public static bool IsValidTransition(TourStatus from, TourStatus to)
    {
        return ValidTransitions.ContainsKey(from) && ValidTransitions[from].Contains(to);
    }

    /// <summary>
    /// Gets all valid next states for a given current state
    /// </summary>
    public static IEnumerable<TourStatus> GetValidNextStates(TourStatus currentState)
    {
        return ValidTransitions.ContainsKey(currentState) 
            ? ValidTransitions[currentState] 
            : Enumerable.Empty<TourStatus>();
    }

    /// <summary>
    /// Checks if a state is terminal (no further transitions allowed)
    /// </summary>
    public static bool IsTerminalState(TourStatus state)
    {
        return !ValidTransitions.ContainsKey(state) || ValidTransitions[state].Count == 0;
    }

    /// <summary>
    /// Checks if a state allows registration
    /// </summary>
    public static bool AllowsRegistration(TourStatus state)
    {
        return state == TourStatus.RegistrationOpen;
    }

    /// <summary>
    /// Checks if a state is active (tour is operational)
    /// </summary>
    public static bool IsActiveState(TourStatus state)
    {
        return state is TourStatus.Scheduled 
                    or TourStatus.RegistrationOpen 
                    or TourStatus.RegistrationClosed 
                    or TourStatus.InProgress;
    }

    /// <summary>
    /// Checks if a state allows postponement
    /// </summary>
    public static bool CanBePostponed(TourStatus state)
    {
        return GetValidNextStates(state).Contains(TourStatus.Postponed);
    }

    /// <summary>
    /// Checks if a state allows suspension
    /// </summary>
    public static bool CanBeSuspended(TourStatus state)
    {
        return GetValidNextStates(state).Contains(TourStatus.Suspended);
    }

    /// <summary>
    /// Checks if a state allows cancellation
    /// </summary>
    public static bool CanBeCancelled(TourStatus state)
    {
        return GetValidNextStates(state).Contains(TourStatus.Cancelled);
    }

    /// <summary>
    /// Checks if a state can be archived
    /// </summary>
    public static bool CanBeArchived(TourStatus state)
    {
        return GetValidNextStates(state).Contains(TourStatus.Archived);
    }

    /// <summary>
    /// Checks if a state allows rescheduling from postponed
    /// </summary>
    public static bool CanBeRescheduled(TourStatus state)
    {
        return state == TourStatus.Postponed;
    }

    /// <summary>
    /// Gets human-readable description of state transition rules
    /// </summary>
    public static string GetTransitionRules(TourStatus state)
    {
        var nextStates = GetValidNextStates(state);
        if (!nextStates.Any())
        {
            return $"{state} is a terminal state - no transitions allowed";
        }

        var stateNames = string.Join(", ", nextStates.Select(s => s.ToString()));
        return $"From {state}, can transition to: {stateNames}";
    }

    /// <summary>
    /// Validates state transition with detailed error message
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) ValidateTransition(
        TourStatus from, 
        TourStatus to, 
        string? context = null)
    {
        if (IsValidTransition(from, to))
        {
            return (true, null);
        }

        var contextMsg = string.IsNullOrEmpty(context) ? "" : $" ({context})";
        var availableTransitions = string.Join(", ", GetValidNextStates(from));
        
        return (false, $"Invalid state transition{contextMsg}: {from} → {to}. " +
                      $"Valid transitions from {from}: {availableTransitions}");
    }

}
