using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Domain.ValueObjects;

/// <summary>
/// State machine for tour reservation status transitions
/// </summary>
public static class ReservationStateMachine
{
    /// <summary>
    /// Defines valid state transitions
    /// </summary>
    private static readonly Dictionary<ReservationStatus, HashSet<ReservationStatus>> ValidTransitions = new()
    {
        [ReservationStatus.Draft] = new()
        {
            ReservationStatus.Held,
            ReservationStatus.Cancelled
        },
        [ReservationStatus.Held] = new()
        {
            ReservationStatus.Paying,
            ReservationStatus.Confirmed, // Direct confirmation (e.g., free tours)
            ReservationStatus.Cancelled,
            ReservationStatus.Expired,
            ReservationStatus.SystemCancelled
        },
        [ReservationStatus.Paying] = new()
        {
            ReservationStatus.Confirmed,
            ReservationStatus.PaymentFailed,
            ReservationStatus.Cancelled,
            ReservationStatus.SystemCancelled
        },
        [ReservationStatus.Confirmed] = new()
        {
            ReservationStatus.Cancelled,
            ReservationStatus.SystemCancelled,
            ReservationStatus.Refunding
        },
        [ReservationStatus.PaymentFailed] = new()
        {
            ReservationStatus.Paying, // Retry payment
            ReservationStatus.Cancelled,
            ReservationStatus.Expired
        },
        [ReservationStatus.Refunding] = new()
        {
            ReservationStatus.Refunded
        },
        [ReservationStatus.Expired] = new()
        {
            ReservationStatus.Held, // Allow reactivation to Held state
            ReservationStatus.Cancelled // Allow manual cancellation of expired reservations
        },
        // Terminal states (no transitions allowed)
        [ReservationStatus.Cancelled] = new(),
        [ReservationStatus.SystemCancelled] = new(),
        [ReservationStatus.Refunded] = new()
    };

    /// <summary>
    /// Checks if a state transition is valid
    /// </summary>
    public static bool IsValidTransition(ReservationStatus from, ReservationStatus to)
    {
        return ValidTransitions.ContainsKey(from) && ValidTransitions[from].Contains(to);
    }

    /// <summary>
    /// Gets all valid next states for a given current state
    /// </summary>
    public static IEnumerable<ReservationStatus> GetValidNextStates(ReservationStatus currentState)
    {
        return ValidTransitions.ContainsKey(currentState) 
            ? ValidTransitions[currentState] 
            : Enumerable.Empty<ReservationStatus>();
    }

    /// <summary>
    /// Checks if a state is terminal (no further transitions allowed)
    /// </summary>
    public static bool IsTerminalState(ReservationStatus state)
    {
        return !ValidTransitions.ContainsKey(state) || ValidTransitions[state].Count == 0;
    }

    /// <summary>
    /// Checks if a state is active (counts towards capacity)
    /// </summary>
    public static bool IsActiveState(ReservationStatus state)
    {
        return state is ReservationStatus.Held 
                    or ReservationStatus.Paying 
                    or ReservationStatus.Confirmed;
    }

    /// <summary>
    /// Checks if a state allows cancellation
    /// </summary>
    public static bool CanBeCancelled(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.Cancelled);
    }

    /// <summary>
    /// Checks if a state allows payment
    /// </summary>
    public static bool CanInitiatePayment(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.Paying);
    }

    /// <summary>
    /// Checks if a state allows confirmation
    /// </summary>
    public static bool CanBeConfirmed(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.Confirmed);
    }

    /// <summary>
    /// Gets human-readable description of state transition rules
    /// </summary>
    public static string GetTransitionRules(ReservationStatus state)
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
        ReservationStatus from, 
        ReservationStatus to, 
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
