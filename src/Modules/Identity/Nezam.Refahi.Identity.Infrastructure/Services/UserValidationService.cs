using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Models;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using System.Text.RegularExpressions;
using Nezam.Refahi.Identity.Application.Services.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Implementation of user validation service
/// </summary>
public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _userRepository;

    // Persian name validation pattern
    private static readonly Regex PersianNameRegex = new(@"^[\u0600-\u06FF\u0750-\u077F\s]+$", RegexOptions.Compiled);
    
    // Iranian phone number validation pattern
    private static readonly Regex IranianPhoneRegex = new(@"^09\d{9}$", RegexOptions.Compiled);
    
    // National ID validation pattern
    private static readonly Regex NationalIdRegex = new(@"^\d{10}$", RegexOptions.Compiled);

    public UserValidationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserValidationResult> ValidateNationalIdUniquenessAsync(
        string nationalId, 
        Guid? excludeUserId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nationalId))
            {
                return UserValidationResult.Failure("کد ملی نمی‌تواند خالی باشد");
            }

            var nationalIdVo = new NationalId(nationalId);
            var existingUser = await _userRepository.GetByNationalIdAsync(nationalIdVo);

            if (existingUser == null)
            {
                return UserValidationResult.Success("کد ملی منحصر به فرد است");
            }

            if (excludeUserId.HasValue && existingUser.Id == excludeUserId.Value)
            {
                return UserValidationResult.Success("کد ملی متعلق به همین کاربر است");
            }

            return UserValidationResult.Failure("این کد ملی قبلاً توسط کاربر دیگری استفاده شده است");
        }
        catch (Exception ex)
        {
            return UserValidationResult.Failure($"خطا در بررسی یکتایی کد ملی: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<UserValidationResult> ValidatePhoneNumberUniquenessAsync(
        string phoneNumber, 
        Guid? excludeUserId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return UserValidationResult.Failure("شماره موبایل نمی‌تواند خالی باشد");
            }

            var phoneNumberVo = new PhoneNumber(phoneNumber);
            var existingUser = await _userRepository.GetByPhoneNumberValueObjectAsync(phoneNumberVo);

            if (existingUser == null)
            {
                return UserValidationResult.Success("شماره موبایل منحصر به فرد است");
            }

            if (excludeUserId.HasValue && existingUser.Id == excludeUserId.Value)
            {
                return UserValidationResult.Success("شماره موبایل متعلق به همین کاربر است");
            }

            return UserValidationResult.Failure("این شماره موبایل قبلاً توسط کاربر دیگری استفاده شده است");
        }
        catch (Exception ex)
        {
            return UserValidationResult.Failure($"خطا در بررسی یکتایی شماره موبایل: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public UserValidationResult ValidateNationalIdFormat(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId))
        {
            return UserValidationResult.Failure("کد ملی اجباری است");
        }

        if (nationalId.Length != 10)
        {
            return UserValidationResult.Failure("کد ملی باید دقیقاً ۱۰ رقم باشد");
        }

        if (!NationalIdRegex.IsMatch(nationalId))
        {
            return UserValidationResult.Failure("کد ملی باید فقط شامل اعداد باشد");
        }

        // Check if all digits are the same (invalid cases)
        if (nationalId.All(c => c == nationalId[0]))
        {
            return UserValidationResult.Failure("کد ملی نمی‌تواند شامل اعداد یکسان باشد");
        }

        // Validate using Iranian national ID check digit algorithm
        if (!ValidateIranianNationalIdCheckDigit(nationalId))
        {
            return UserValidationResult.Failure("کد ملی وارد شده معتبر نیست");
        }

        return UserValidationResult.Success("فرمت کد ملی صحیح است");
    }

    public UserValidationResult ValidatePhoneNumberFormat(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return UserValidationResult.Failure("شماره موبایل اجباری است");
        }

        if (!IranianPhoneRegex.IsMatch(phoneNumber))
        {
            return UserValidationResult.Failure("شماره موبایل باید به فرم 09xxxxxxxxx باشد");
        }

        return UserValidationResult.Success("فرمت شماره موبایل صحیح است");
    }

    public async Task<UserValidationResult> ValidateUserForCreationAsync(
        string firstName,
        string lastName,
        string phoneNumber,
        string nationalId,
        CancellationToken cancellationToken = default)
    {
        var validationResults = new List<UserValidationResult>();

        // Validate name formats
        validationResults.Add(ValidatePersonName(firstName, "نام"));
        validationResults.Add(ValidatePersonName(lastName, "نام خانوادگی"));

        // Validate phone number format and uniqueness
        var phoneFormatResult = ValidatePhoneNumberFormat(phoneNumber);
        validationResults.Add(phoneFormatResult);
        
        if (phoneFormatResult.IsValid)
        {
            validationResults.Add(await ValidatePhoneNumberUniquenessAsync(phoneNumber, null, cancellationToken));
        }

        // Validate national ID format and uniqueness
        var nationalIdFormatResult = ValidateNationalIdFormat(nationalId);
        validationResults.Add(nationalIdFormatResult);
        
        if (nationalIdFormatResult.IsValid)
        {
            validationResults.Add(await ValidateNationalIdUniquenessAsync(nationalId, null, cancellationToken));
        }

        return UserValidationResult.Combine(validationResults.ToArray());
    }

    public async Task<UserValidationResult> ValidateUserForUpdateAsync(
        Guid userId,
        string firstName,
        string lastName,
        string phoneNumber,
        string nationalId,
        CancellationToken cancellationToken = default)
    {
        var validationResults = new List<UserValidationResult>();

        // Validate user existence
        var user = await _userRepository.FindOneAsync(x=>x.Id==userId,cancellationToken:cancellationToken);
        if (user == null)
        {
            return UserValidationResult.Failure("کاربر یافت نشد");
        }

        // Validate name formats
        validationResults.Add(ValidatePersonName(firstName, "نام"));
        validationResults.Add(ValidatePersonName(lastName, "نام خانوادگی"));

        // Validate phone number format and uniqueness
        var phoneFormatResult = ValidatePhoneNumberFormat(phoneNumber);
        validationResults.Add(phoneFormatResult);
        
        if (phoneFormatResult.IsValid)
        {
            validationResults.Add(await ValidatePhoneNumberUniquenessAsync(phoneNumber, userId, cancellationToken));
        }

        // Validate national ID format and uniqueness
        var nationalIdFormatResult = ValidateNationalIdFormat(nationalId);
        validationResults.Add(nationalIdFormatResult);
        
        if (nationalIdFormatResult.IsValid)
        {
            validationResults.Add(await ValidateNationalIdUniquenessAsync(nationalId, userId, cancellationToken));
        }

        return UserValidationResult.Combine(validationResults.ToArray());
    }

    private static UserValidationResult ValidatePersonName(string name, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UserValidationResult.Failure($"{fieldName} اجباری است");
        }

        if (name.Length < 2)
        {
            return UserValidationResult.Failure($"{fieldName} باید حداقل ۲ کاراکتر باشد");
        }

        if (name.Length > 50)
        {
            return UserValidationResult.Failure($"{fieldName} نمی‌تواند بیشتر از ۵۰ کاراکتر باشد");
        }

        if (!PersianNameRegex.IsMatch(name))
        {
            return UserValidationResult.Failure($"{fieldName} باید فقط شامل حروف فارسی باشد");
        }

        return UserValidationResult.Success($"فرمت {fieldName} صحیح است");
    }

    private static bool ValidateIranianNationalIdCheckDigit(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length != 10)
            return false;

        if (!nationalId.All(char.IsDigit))
            return false;

        // Calculate check digit using Iranian algorithm
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (nationalId[i] - '0') * (10 - i);
        }

        int remainder = sum % 11;
        int checkDigit = remainder < 2 ? remainder : 11 - remainder;

        return checkDigit == (nationalId[9] - '0');
    }
}