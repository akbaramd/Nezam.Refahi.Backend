using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Domain.ValueObjects;

/// <summary>
/// State machine for tour status transitions
/// </summary>
public static class TourStateMachine
{
    /// <summary>
    /// Defines valid state transitions
    /// Simplified: Draft -> Published -> InProgress -> Completed
    /// Tours can also be cancelled from Published or InProgress states.
    /// Registration status is determined by dates, not by enum state.
    /// </summary>
    private static readonly Dictionary<TourStatus, HashSet<TourStatus>> ValidTransitions = new()
    {
        [TourStatus.Draft] = new()
        {
            TourStatus.Published
        },
        [TourStatus.Published] = new()
        {
            TourStatus.InProgress,
            TourStatus.Cancelled
        },
        [TourStatus.InProgress] = new()
        {
            TourStatus.Completed,
            TourStatus.Cancelled
        },
        // Terminal states (no transitions allowed)
        [TourStatus.Completed] = new(),
        [TourStatus.Cancelled] = new()
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
    /// Checks if a state is active (tour is operational)
    /// Registration status is determined by dates, not by enum state.
    /// </summary>
    public static bool IsActiveState(TourStatus state)
    {
        return state is TourStatus.Published or TourStatus.InProgress;
    }

    /// <summary>
    /// Checks if a state is terminal (no further transitions allowed)
    /// </summary>
    public static bool IsTerminalStateExplicit(TourStatus state)
    {
        return state is TourStatus.Completed or TourStatus.Cancelled;
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
