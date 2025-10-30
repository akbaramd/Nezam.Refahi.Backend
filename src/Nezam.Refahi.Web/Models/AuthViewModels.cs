using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.Web.Models;

/// <summary>
/// View model for OTP login form
/// </summary>
public class OtpLoginViewModel
{
    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره موبایل باید 11 رقم باشد")]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل نامعتبر است")]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

/// <summary>
/// View model for OTP verification form
/// </summary>
public class VerifyOtpViewModel
{
    [Required(ErrorMessage = "کد تایید الزامی است")]
    [StringLength(5, MinimumLength = 5, ErrorMessage = "کد تایید باید 5 رقم باشد")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "کد تایید باید شامل اعداد باشد")]
    public string Otp { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}
