namespace Nezam.Refahi.Identity.Domain.Enums;

/// <summary>
/// Represents the delivery status of an OTP
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// OTP is pending to be sent
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// OTP has been successfully sent
    /// </summary>
    Sent = 1,
    
    /// <summary>
    /// OTP delivery failed
    /// </summary>
    Failed = 2
}
