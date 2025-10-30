using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Web.Models;
using Claim = System.Security.Claims.Claim;

namespace Nezam.Refahi.Web.Controllers;

/// <summary>
/// Authentication controller handling OTP-based authentication following DDD principles
/// </summary>
public class AuthController : Controller
{
  private readonly IUserRepository _userRepository;
  private readonly IRoleRepository _roleRepository;
  private readonly IOtpChallengeRepository _otpChallengeRepository;
  private readonly ILogger<AuthController> _logger;

  // Constants for authentication
  private const string DefaultOtpCode = "12345";
  private const string AuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  private const string DefaultRoleName = "User";
  private const string WebClientId = "web-portal";

  public AuthController(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IOtpChallengeRepository otpChallengeRepository,
    ILogger<AuthController> logger)
  {
    _userRepository = userRepository;
    _roleRepository = roleRepository;
    _otpChallengeRepository = otpChallengeRepository;
    _logger = logger;
  }

  /// <summary>
  /// Displays the login form
  /// </summary>
  [HttpGet]
  public IActionResult Login(string? returnUrl = null)
  {
    // If user is already signed in, redirect to dashboard
    if (User.Identity?.IsAuthenticated == true)
    {
      return RedirectToAction("Index", "Dashboard");
    }

    // Check if there's a success message from previous actions
    if (TempData.ContainsKey("SuccessMessage"))
    {
      ViewData["SuccessMessage"] = TempData["SuccessMessage"];
    }

    ViewData["ReturnUrl"] = returnUrl;
    return View(new OtpLoginViewModel());
  }

    /// <summary>
    /// Processes the login request and initiates OTP flow
    /// </summary>
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Login(OtpLoginViewModel model, string? returnUrl = null)
  {
        if (!ModelState.IsValid) 
            return View(model);

        try
        {
            // Validate phone number using domain value object
            var phoneNumber = new PhoneNumber(model.PhoneNumber);
            
            // Check if user exists before creating OTP challenge
            var existingUser = await _userRepository.GetByPhoneNumberValueObjectAsync(phoneNumber);
            if (existingUser == null)
            {
                _logger.LogWarning("Login attempt with non-existent phone number: {PhoneNumber}", phoneNumber.Value);
                ModelState.AddModelError(string.Empty, "کاربری با این شماره موبایل یافت نشد. لطفاً با مدیر سیستم تماس بگیرید.");
                return View(model);
            }
            
            // Generate a secure token for OTP verification
            var token = GenerateSecureToken();
            
            // Create OTP challenge using domain entities and get the challenge ID
            var challengeId = await CreateOtpChallengeAsync(phoneNumber);
            
            // Store phone number, timestamp, and challenge ID in session
            StoreOtpSessionData(token, phoneNumber.Value, challengeId);

            TempData["SuccessMessage"] = $"کد تایید ارسال شد. کد پیش‌فرض: {DefaultOtpCode}";
            return RedirectToAction("VerifyOtp", new { token });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid phone number format: {PhoneNumber}, Error: {Error}", model.PhoneNumber, ex.Message);
            ModelState.AddModelError(nameof(model.PhoneNumber), "شماره موبایل نامعتبر است");
      return View(model);
    }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login process for phone: {PhoneNumber}", model.PhoneNumber);
            ModelState.AddModelError(string.Empty, "خطا در ارسال کد تایید");
            return View(model);
        }
    }

  /// <summary>
  /// Displays the OTP verification form
  /// </summary>
  [HttpGet("/Auth/VerifyOtp/{token}")]
  public IActionResult VerifyOtp(string token)
  {
    // Handle case where user directly navigates to this page
    if (string.IsNullOrEmpty(token))
    {
      return RedirectToAction(nameof(Login));
    }
    
    // Get phone number from token
    var phoneNumber = HttpContext.Session.GetString($"phone_{token}");
    if (string.IsNullOrEmpty(phoneNumber))
    {
      _logger.LogWarning("Invalid or expired token: {Token}", token);
      return RedirectToAction(nameof(Login));
    }

    var model = new VerifyOtpViewModel { PhoneNumber = phoneNumber };

    // Get OTP sent time from session
    var otpSentTime = HttpContext.Session.GetString($"otp_time_{phoneNumber}");
    if (!string.IsNullOrEmpty(otpSentTime))
    {
      ViewData["OtpSentTime"] = otpSentTime;
    }

    return View(model);
  }

  /// <summary>
  /// Processes the OTP verification request
  /// </summary>
  [HttpPost("/Auth/VerifyOtp/{token}")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> VerifyOtp(VerifyOtpViewModel model, string token)
  {
    if (!ModelState.IsValid) return View(model);

    try
    {
      // Get phone number and challenge ID from token
    var phoneNumber = HttpContext.Session.GetString($"phone_{token}");
      var challengeIdString = HttpContext.Session.GetString($"challenge_{token}");

      if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(challengeIdString))
    {
      _logger.LogWarning("Invalid or expired token for OTP verification: {Token}", token);
      ModelState.AddModelError(string.Empty, "توکن نامعتبر یا منقضی شده است.");
      return View(model);
    }

      if (!Guid.TryParse(challengeIdString, out var challengeId))
      {
        _logger.LogWarning("Invalid challenge ID format: {ChallengeId}", challengeIdString);
        ModelState.AddModelError(string.Empty, "شناسه چالش نامعتبر است.");
        return View(model);
      }

      // Verify OTP using specific challenge ID
      var phoneNumberValueObject = new PhoneNumber(phoneNumber);
      var verificationResult = await VerifyOtpCodeAsync(phoneNumberValueObject, model.Otp, challengeId);

      if (!verificationResult.IsValid)
      {
        ModelState.AddModelError(string.Empty, verificationResult.ErrorMessage);
        return View(model);
      }

      // Get existing user
      var user = await GetUserByPhoneAsync(phoneNumberValueObject);
      if (user == null)
      {
        ModelState.AddModelError(string.Empty, "کاربری با این شماره موبایل یافت نشد. لطفاً با مدیر سیستم تماس بگیرید.");
        return View(model);
      }

      // Create authentication claims
      var claims = CreateUserClaims(user);

      // Sign in user
      await SignInUserAsync(claims);

      // Clean up session and consume challenge
      await CleanupOtpSessionAsync(token, phoneNumber, challengeId);

      return RedirectToAction("Index", "Dashboard");
    }
    catch (ArgumentException ex)
    {
      _logger.LogWarning("Invalid OTP format: {Otp}, Error: {Error}", model.Otp, ex.Message);
      ModelState.AddModelError(nameof(model.Otp), "کد تایید نامعتبر است");
      return View(model);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error during OTP verification for phone: {PhoneNumber}", model.PhoneNumber);
      ModelState.AddModelError(string.Empty, "خطا در تایید کد");
      return View(model);
    }
  }

  /// <summary>
  /// Resends OTP code for the given phone number
  /// </summary>
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> ResendOtp(string phoneNumber)
  {
    try
    {
      // Validate phone number using domain value object
      var phoneNumberValueObject = new PhoneNumber(phoneNumber);

      // Generate new token
      var token = GenerateSecureToken();

      // Create new OTP challenge and get the challenge ID
      var challengeId = await CreateOtpChallengeAsync(phoneNumberValueObject);

      // Store phone number and challenge ID in session with new token
      StoreOtpSessionData(token, phoneNumberValueObject.Value, challengeId);

      return Json(new { success = true, message = "کد تایید جدید ارسال شد" });
    }
    catch (ArgumentException ex)
    {
      _logger.LogWarning("Invalid phone number format for resend: {PhoneNumber}, Error: {Error}", phoneNumber,
        ex.Message);
      return Json(new { success = false, message = "شماره موبایل نامعتبر است" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error resending OTP for phone: {PhoneNumber}", phoneNumber);
      return Json(new { success = false, message = "خطا در ارسال کد تایید" });
    }
  }


  /// <summary>
  /// Logs out the current user (GET method for convenience)
  /// </summary>
  [HttpGet]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(AuthScheme);
    return RedirectToAction("Login");
  }

  #region Private Helper Methods

  /// <summary>
  /// Generates a secure token for OTP verification
  /// </summary>
  private static string GenerateSecureToken()
  {
    using var rng = RandomNumberGenerator.Create();
    var bytes = new byte[16];
    rng.GetBytes(bytes);
    return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
  }

  /// <summary>
  /// Stores OTP session data including challenge ID
  /// </summary>
  private void StoreOtpSessionData(string token, string phoneNumber, Guid challengeId)
  {
    HttpContext.Session.SetString($"phone_{token}", phoneNumber);
    HttpContext.Session.SetString($"challenge_{token}", challengeId.ToString());
    HttpContext.Session.SetString($"otp_time_{phoneNumber}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
  }

    /// <summary>
    /// Creates an OTP challenge using domain entities and returns the challenge ID
    /// </summary>
    private async Task<Guid> CreateOtpChallengeAsync(PhoneNumber phoneNumber)
    {
        // Create required value objects
        var clientId = new ClientId(WebClientId);
        var otpPolicy = OtpPolicy.Default;
        var ipAddress = new IpAddress(HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
        
        // Generate OTP hash and nonce
        var otpHash = HashOtpCode(DefaultOtpCode);
        var nonce = GenerateSecureToken();

        // Create device fingerprint from user agent and IP
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var deviceFingerprint = $"{ipAddress.Value}_{userAgent}_{DateTime.UtcNow.Ticks}";
        var deviceFingerprintValueObject = new DeviceFingerprint(deviceFingerprint);

        // Create OTP challenge
        var otpChallenge = new OtpChallenge(
            phoneNumber,
            clientId,
            otpHash,
            nonce,
            otpPolicy,
            deviceFingerprintValueObject,
            ipAddress
        );

        // Mark as sent
        otpChallenge.MarkAsSent();

        // Save to repository
        await _otpChallengeRepository.AddAsync(otpChallenge, true);

        // Return the challenge ID
        return otpChallenge.Id;
    }

  /// <summary>
  /// Verifies OTP code using specific challenge ID
  /// </summary>
  private async Task<OtpVerificationResult> VerifyOtpCodeAsync(PhoneNumber phoneNumber, string otpCode,
    Guid challengeId)
  {
    try
    {
      // Get the specific challenge by ID
      var challenge = await _otpChallengeRepository.GetByIdAsync(challengeId);

      if (challenge == null)
      {
        _logger.LogWarning("OTP challenge not found for ID: {ChallengeId}", challengeId);
        return new OtpVerificationResult { IsValid = false, ErrorMessage = "چالش یافت نشد" };
      }

      // Verify the challenge belongs to the phone number
      if (challenge.PhoneNumber.Value != phoneNumber.Value)
      {
        _logger.LogWarning(
          "OTP challenge phone number mismatch. Challenge: {ChallengePhone}, Requested: {RequestedPhone}",
          challenge.PhoneNumber.Value, phoneNumber.Value);
        return new OtpVerificationResult { IsValid = false, ErrorMessage = "چالش با شماره موبایل مطابقت ندارد" };
      }

      // Check if challenge is still active
      if (challenge.Status != ChallengeStatus.Sent)
      {
        _logger.LogWarning("OTP challenge is not in Sent status. Current status: {Status}, ChallengeId: {ChallengeId}",
          challenge.Status, challengeId);
        return new OtpVerificationResult { IsValid = false, ErrorMessage = "چالش منقضی شده یا قبلاً استفاده شده است" };
      }

      var otpHash = HashOtpCode(otpCode);

      // Attempt verification using domain method
      if (challenge.AttemptVerification(otpHash, Guid.Empty)) // We'll set the actual user ID later
      {
        // Update the challenge in repository
        await _otpChallengeRepository.UpdateAsync(challenge, true);

        _logger.LogInformation("OTP verification successful for challenge: {ChallengeId}", challengeId);
        return new OtpVerificationResult { IsValid = true, ChallengeId = challenge.Id };
      }

      _logger.LogWarning("OTP verification failed for challenge: {ChallengeId}", challengeId);
      return new OtpVerificationResult { IsValid = false, ErrorMessage = "کد تایید نامعتبر است" };
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error verifying OTP for challenge: {ChallengeId}", challengeId);
      return new OtpVerificationResult { IsValid = false, ErrorMessage = "خطا در تایید کد" };
    }
  }

  /// <summary>
  /// Gets an existing user for the given phone number
  /// </summary>
  private async Task<User?> GetUserByPhoneAsync(PhoneNumber phoneNumber)
  {
    var user = await _userRepository.GetByPhoneNumberValueObjectAsync(phoneNumber);

    if (user == null)
    {
      _logger.LogWarning("No user found with phone number: {PhoneNumber}", phoneNumber.Value);
      return null;
    }

        // Record successful authentication for existing user
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
        var deviceFingerprint = $"{ipAddress}_{userAgent}_{DateTime.UtcNow.Ticks}";
        var deviceFingerprintValueObject = new DeviceFingerprint(deviceFingerprint);
        
        user.RecordSuccessfulAuthentication(
            deviceFingerprintValueObject,
            ipAddress,
            userAgent
        );

    await _userRepository.UpdateAsync(user, true);
    return user;
  }

  /// <summary>
  /// Creates authentication claims for the user
  /// </summary>
  private List<Claim> CreateUserClaims(User user)
  {
    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, user.Id.ToString()),
      new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
      new(ClaimTypes.GivenName, user.FirstName),
      new(ClaimTypes.Surname, user.LastName),
      new(ClaimTypes.MobilePhone, user.PhoneNumber.Value),
      new(ClaimTypes.Email, user.Email ?? string.Empty),
      new("FullName", $"{user.FirstName} {user.LastName}".Trim()),
      new("PhoneNumber", user.PhoneNumber.Value),
      new("NationalCode", user.NationalId?.Value ?? string.Empty),
      new("UserId", user.Id.ToString()),
      new("ProfileImagePath", "~/assets/images/users/avatar-1.jpg")
    };

    // Add role claims if user has roles
    var userRoles = user.GetActiveRoles();
    if (userRoles.Any())
    {
      foreach (var userRole in userRoles)
      {
        claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
      }
    }
    else
    {
      // Add default role
      claims.Add(new Claim(ClaimTypes.Role, DefaultRoleName));
    }

    return claims;
  }

  /// <summary>
  /// Signs in the user with the given claims
  /// </summary>
  private async Task SignInUserAsync(List<Claim> claims)
  {
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(AuthScheme, principal);
  }

  /// <summary>
  /// Cleans up OTP session data and consumes the challenge
  /// </summary>
  private async Task CleanupOtpSessionAsync(string token, string phoneNumber, Guid challengeId)
  {
    try
    {
      // Clean up session
      HttpContext.Session.Remove($"phone_{token}");
      HttpContext.Session.Remove($"challenge_{token}");
      HttpContext.Session.Remove($"otp_time_{phoneNumber}");

      // Consume the challenge
      var challenge = await _otpChallengeRepository.GetByIdAsync(challengeId);
      if (challenge != null)
      {
        challenge.Consume();
        await _otpChallengeRepository.UpdateAsync(challenge, true);
        _logger.LogInformation("OTP challenge consumed successfully: {ChallengeId}", challengeId);
      }
      else
      {
        _logger.LogWarning("OTP challenge not found for cleanup: {ChallengeId}", challengeId);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error cleaning up OTP session for challenge: {ChallengeId}", challengeId);
      // Don't throw the exception as this is cleanup code
    }
  }

  /// <summary>
  /// Hashes OTP code for secure storage
  /// </summary>
  private static string HashOtpCode(string otpCode)
  {
    using var sha256 = SHA256.Create();
    var bytes = Encoding.UTF8.GetBytes(otpCode);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
  }

  #endregion
}

/// <summary>
/// Result of OTP verification operation
/// </summary>
public class OtpVerificationResult
{
  public bool IsValid { get; set; }
  public string ErrorMessage { get; set; } = string.Empty;
  public Guid ChallengeId { get; set; }
}
