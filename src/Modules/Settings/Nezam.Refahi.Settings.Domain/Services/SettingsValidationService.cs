using Nezam.Refahi.Settings.Domain.ValueObjects;

namespace Nezam.Refahi.Settings.Domain.Services;

/// <summary>
/// Validation service for settings business rules
/// This service is stateless and focuses on validation logic
/// </summary>
public class SettingsValidationService
{
    /// <summary>
    /// Validates a setting key format and rules
    /// </summary>
    public ValidationResult ValidateSettingKey(string key)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(key))
            return result.AddError("Setting key cannot be empty");

        if (key.Length > 100)
            return result.AddError("Setting key cannot exceed 100 characters");

        if (!IsValidKeyFormat(key))
            return result.AddError("Setting key must contain only letters, numbers, and underscores");

        return result;
    }

    /// <summary>
    /// Validates a setting value based on its type
    /// </summary>
    public ValidationResult ValidateSettingValue(string value, SettingType type)
    {
        var result = new ValidationResult();

        if (value == null)
            return result.AddError("Value cannot be null");

        if (string.IsNullOrWhiteSpace(value) && type != SettingType.String)
            return result.AddError("Value cannot be empty for non-string types");

        // Type-specific validation
        switch (type)
        {
            case SettingType.Integer:
                if (!int.TryParse(value, out _))
                    result.AddError($"Value '{value}' is not a valid integer");
                break;

            case SettingType.Boolean:
                if (!bool.TryParse(value, out _))
                    result.AddError($"Value '{value}' is not a valid boolean");
                break;

            case SettingType.Decimal:
                if (!decimal.TryParse(value, out _))
                    result.AddError($"Value '{value}' is not a valid decimal");
                break;

            case SettingType.DateTime:
                if (!DateTime.TryParse(value, out _))
                    result.AddError($"Value '{value}' is not a valid datetime");
                break;

            case SettingType.Json:
                if (!IsValidJson(value))
                    result.AddError($"Value '{value}' is not valid JSON");
                break;

            case SettingType.Email:
                if (!IsValidEmail(value))
                    result.AddError($"Value '{value}' is not a valid email address");
                break;

            case SettingType.Url:
                if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                    result.AddError($"Value '{value}' is not a valid URL");
                break;

            case SettingType.PhoneNumber:
                if (!IsValidPhoneNumber(value))
                    result.AddError($"Value '{value}' is not a valid phone number");
                break;
        }

        return result;
    }

    /// <summary>
    /// Validates a section name format and rules
    /// </summary>
    public ValidationResult ValidateSectionName(string name)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(name))
            return result.AddError("Section name cannot be empty");

        if (name.Length > 100)
            return result.AddError("Section name cannot exceed 100 characters");

        if (name.Length < 2)
            return result.AddWarning("Section name is very short");

        return result;
    }

    /// <summary>
    /// Validates a category name format and rules
    /// </summary>
    public ValidationResult ValidateCategoryName(string name)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(name))
            return result.AddError("Category name cannot be empty");

        if (name.Length > 100)
            return result.AddError("Category name cannot exceed 100 characters");

        if (name.Length < 2)
            return result.AddWarning("Category name is very short");

        return result;
    }

    /// <summary>
    /// Validates a description format and rules
    /// </summary>
    public ValidationResult ValidateDescription(string description)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(description))
            return result.AddError("Description cannot be empty");

        if (description.Length > 500)
            return result.AddError("Description cannot exceed 500 characters");

        if (description.Length < 10)
            return result.AddWarning("Description is very short");

        return result;
    }

    /// <summary>
    /// Validates display order value
    /// </summary>
    public ValidationResult ValidateDisplayOrder(int displayOrder)
    {
        var result = new ValidationResult();

        if (displayOrder < 0)
            return result.AddError("Display order cannot be negative");

        if (displayOrder > 10000)
            return result.AddWarning("Display order is very high");

        return result;
    }

    /// <summary>
    /// Validates if a setting value change is allowed
    /// </summary>
    public ValidationResult ValidateSettingValueChange(string oldValue, string newValue, SettingType type)
    {
        var result = new ValidationResult();

        if (oldValue == newValue)
            return result.AddWarning("Setting value is not changing");

        // Validate the new value
        var newValueValidation = ValidateSettingValue(newValue, type);
        result.AddErrors(newValueValidation.Errors);
        result.AddWarnings(newValueValidation.Warnings);

        return result;
    }

    /// <summary>
    /// Validates business rules for setting creation
    /// </summary>
    public ValidationResult ValidateSettingCreation(SettingKey key, SettingValue value, string description, Guid categoryId)
    {
        var result = new ValidationResult();

        // Validate key
        var keyValidation = ValidateSettingKey(key.Value);
        result.AddErrors(keyValidation.Errors);
        result.AddWarnings(keyValidation.Warnings);

        // Validate value
        var valueValidation = ValidateSettingValue(value.RawValue, value.Type);
        result.AddErrors(valueValidation.Errors);
        result.AddWarnings(valueValidation.Warnings);

        // Validate description
        var descriptionValidation = ValidateDescription(description);
        result.AddErrors(descriptionValidation.Errors);
        result.AddWarnings(descriptionValidation.Warnings);

        // Validate category ID
        if (categoryId == Guid.Empty)
            result.AddError("Category ID cannot be empty");

        return result;
    }

    /// <summary>
    /// Validates business rules for section creation
    /// </summary>
    public ValidationResult ValidateSectionCreation(string name, string description, int displayOrder)
    {
        var result = new ValidationResult();

        // Validate name
        var nameValidation = ValidateSectionName(name);
        result.AddErrors(nameValidation.Errors);
        result.AddWarnings(nameValidation.Warnings);

        // Validate description
        var descriptionValidation = ValidateDescription(description);
        result.AddErrors(descriptionValidation.Errors);
        result.AddWarnings(descriptionValidation.Warnings);

        // Validate display order
        var displayOrderValidation = ValidateDisplayOrder(displayOrder);
        result.AddErrors(displayOrderValidation.Errors);
        result.AddWarnings(displayOrderValidation.Warnings);

        return result;
    }

    /// <summary>
    /// Validates business rules for category creation
    /// </summary>
    public ValidationResult ValidateCategoryCreation(string name, string description, Guid sectionId, int displayOrder)
    {
        var result = new ValidationResult();

        // Validate name
        var nameValidation = ValidateCategoryName(name);
        result.AddErrors(nameValidation.Errors);
        result.AddWarnings(nameValidation.Warnings);

        // Validate description
        var descriptionValidation = ValidateDescription(description);
        result.AddErrors(descriptionValidation.Errors);
        result.AddWarnings(descriptionValidation.Warnings);

        // Validate section ID
        if (sectionId == Guid.Empty)
            result.AddError("Section ID cannot be empty");

        // Validate display order
        var displayOrderValidation = ValidateDisplayOrder(displayOrder);
        result.AddErrors(displayOrderValidation.Errors);
        result.AddWarnings(displayOrderValidation.Warnings);

        return result;
    }

    #region Private Helper Methods

    private static bool IsValidKeyFormat(string key)
    {
        return key.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    private static bool IsValidJson(string value)
    {
        try
        {
            System.Text.Json.JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhoneNumber(string phone)
    {
        // Basic phone number validation - can be enhanced based on requirements
        return phone.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ' || c == '(' || c == ')');
    }

    #endregion
}

/// <summary>
/// Result of a validation operation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; }
    public List<string> Errors { get; }
    public List<string> Warnings { get; }

    public ValidationResult(bool isValid = true)
    {
        IsValid = isValid;
        Errors = new List<string>();
        Warnings = new List<string>();
    }

    public ValidationResult AddError(string error)
    {
        Errors.Add(error);
        return this;
    }

    public ValidationResult AddWarning(string warning)
    {
        Warnings.Add(warning);
        return this;
    }

    public ValidationResult AddErrors(IEnumerable<string> errors)
    {
        Errors.AddRange(errors);
        return this;
    }

    public ValidationResult AddWarnings(IEnumerable<string> warnings)
    {
        Warnings.AddRange(warnings);
        return this;
    }

    public static ValidationResult Success() => new ValidationResult(true);
    public static ValidationResult Failure(params string[] errors) => new ValidationResult(false).AddErrors(errors);
}
