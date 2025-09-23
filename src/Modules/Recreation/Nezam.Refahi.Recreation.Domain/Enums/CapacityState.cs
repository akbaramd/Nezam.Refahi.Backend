using System.Text.Json.Serialization;

namespace Nezam.Refahi.Recreation.Domain.Enums;

/// <summary>
/// Represents the capacity state of a tour (separate from tour status)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CapacityState
{
    /// <summary>
    /// Tour has plenty of available spots
    /// </summary>
    HasSpare = 0,

    /// <summary>
    /// Tour is getting close to capacity
    /// </summary>
    Tight = 1,

    /// <summary>
    /// Tour is at full capacity
    /// </summary>
    Full = 2
}
