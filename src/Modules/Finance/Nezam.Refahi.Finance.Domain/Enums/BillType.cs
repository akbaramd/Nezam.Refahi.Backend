namespace Nezam.Refahi.Finance.Domain.Enums;

/// <summary>
/// Type of bill
/// </summary>
public enum BillType
{
    /// <summary>
    /// Tour reservation bill
    /// </summary>
    TourReservation = 1,

    /// <summary>
    /// Membership fee bill
    /// </summary>
    MembershipFee = 2,

    /// <summary>
    /// Event registration bill
    /// </summary>
    EventRegistration = 3,

    /// <summary>
    /// Service fee bill
    /// </summary>
    ServiceFee = 4,

    /// <summary>
    /// Penalty or fine bill
    /// </summary>
    Penalty = 5,

    /// <summary>
    /// Miscellaneous charges
    /// </summary>
    Miscellaneous = 6
}