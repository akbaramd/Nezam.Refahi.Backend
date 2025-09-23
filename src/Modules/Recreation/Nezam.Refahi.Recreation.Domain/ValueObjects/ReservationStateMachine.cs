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
            ReservationStatus.Cancelled,
            ReservationStatus.Waitlisted,
            ReservationStatus.Rejected
        },
        [ReservationStatus.Held] = new()
        {
            ReservationStatus.Paying,
            ReservationStatus.Confirmed, // Direct confirmation (e.g., free tours)
            ReservationStatus.Cancelled,
            ReservationStatus.Expired,
            ReservationStatus.SystemCancelled,
            ReservationStatus.Waitlisted
        },
        [ReservationStatus.Paying] = new()
        {
            ReservationStatus.Confirmed,
            ReservationStatus.PaymentFailed,
            ReservationStatus.Cancelled,
            ReservationStatus.SystemCancelled,
            ReservationStatus.CancelRequested
        },
        [ReservationStatus.Confirmed] = new()
        {
            ReservationStatus.Cancelled,
            ReservationStatus.SystemCancelled,
            ReservationStatus.Refunding,
            ReservationStatus.CancelRequested,
            ReservationStatus.AmendRequested,
            ReservationStatus.NoShow
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
        [ReservationStatus.Waitlisted] = new()
        {
            ReservationStatus.Held, // Promote to held when capacity becomes available
            ReservationStatus.Cancelled,
            ReservationStatus.Expired,
            ReservationStatus.SystemCancelled
        },
        [ReservationStatus.CancelRequested] = new()
        {
            ReservationStatus.Cancelled, // Complete cancellation after PSP arbitration
            ReservationStatus.Confirmed, // Revert if cancellation is denied
            ReservationStatus.Paying // Revert if cancellation is denied
        },
        [ReservationStatus.AmendRequested] = new()
        {
            ReservationStatus.Confirmed, // Approve amendment
            ReservationStatus.Cancelled, // Reject amendment
            ReservationStatus.SystemCancelled
        },
        [ReservationStatus.NoShow] = new()
        {
            ReservationStatus.Cancelled, // Convert to cancelled after no-show
            ReservationStatus.SystemCancelled
        },
        // Terminal states (no transitions allowed)
        [ReservationStatus.Cancelled] = new(),
        [ReservationStatus.SystemCancelled] = new(),
        [ReservationStatus.Refunded] = new(),
        [ReservationStatus.Rejected] = new()
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
                    or ReservationStatus.Confirmed
                    or ReservationStatus.Waitlisted;
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
    /// Checks if a state allows waitlisting
    /// </summary>
    public static bool CanBeWaitlisted(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.Waitlisted);
    }

    /// <summary>
    /// Checks if a state allows cancellation request
    /// </summary>
    public static bool CanRequestCancellation(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.CancelRequested);
    }

    /// <summary>
    /// Checks if a state allows amendment request
    /// </summary>
    public static bool CanRequestAmendment(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.AmendRequested);
    }

    /// <summary>
    /// Checks if a state can be marked as no-show
    /// </summary>
    public static bool CanBeMarkedAsNoShow(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.NoShow);
    }

    /// <summary>
    /// Checks if a state can be rejected
    /// </summary>
    public static bool CanBeRejected(ReservationStatus state)
    {
        return GetValidNextStates(state).Contains(ReservationStatus.Rejected);
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
