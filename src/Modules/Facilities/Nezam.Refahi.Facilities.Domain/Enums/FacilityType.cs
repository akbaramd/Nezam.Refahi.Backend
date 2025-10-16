using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Domain.Enums;

/// <summary>
/// Type of facility
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FacilityType
{
    /// <summary>
    /// Loan facility
    /// </summary>
    Loan = 1,

    /// <summary>
    /// Grant facility
    /// </summary>
    Grant = 2,

    /// <summary>
    /// Card facility
    /// </summary>
    Card = 3,

    /// <summary>
    /// Welfare voucher facility
    /// </summary>
    WelfareVoucher = 4,

    /// <summary>
    /// Other facility types
    /// </summary>
    Other = 5
}
