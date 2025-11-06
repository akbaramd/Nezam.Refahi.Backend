using System.ComponentModel;
using System.Runtime.Serialization;

namespace Nezam.Refahi.Shared.Application.Common.Models;

  /// <summary>
  /// Represents the status of an application operation result
  /// </summary>
  public enum ResultStatus
  {
    /// <summary>
    /// Operation completed successfully (200)
    /// </summary>
    [EnumMember(Value = "Success")]
    [Description("موفق")]
    Success = 200,

    /// <summary>
    /// Bad request - validation or input errors (400)
    /// </summary>
    [EnumMember(Value = "BadRequest")]
    [Description("درخواست نامعتبر")]
    BadRequest = 400,

    /// <summary>
    /// Unauthorized - authentication required (401)
    /// </summary>
    [EnumMember(Value = "Unauthorized")]
    [Description("احراز هویت مورد نیاز")]
    Unauthorized = 401,

    /// <summary>
    /// Forbidden - insufficient permissions (403)
    /// </summary>
    [EnumMember(Value = "Forbidden")]
    [Description("دسترسی غیرمجاز")]
    Forbidden = 403,

    /// <summary>
    /// Resource not found (404)
    /// </summary>
    [EnumMember(Value = "NotFound")]
    [Description("یافت نشد")]
    NotFound = 404,

    /// <summary>
    /// Conflict - resource conflict or duplicate (409)
    /// </summary>
    [EnumMember(Value = "Conflict")]
    [Description("تعارض")]
    Conflict = 409,

    /// <summary>
    /// Gone - resource no longer available (410)
    /// </summary>
    [EnumMember(Value = "Gone")]
    [Description("دیگر در دسترس نیست")]
    Gone = 410,

    /// <summary>
    /// Validation failed (422)
    /// </summary>
    [EnumMember(Value = "ValidationFailed")]
    [Description("اعتبارسنجی ناموفق")]
    ValidationFailed = 422,

    /// <summary>
    /// Rate limited - too many requests (429)
    /// </summary>
    [EnumMember(Value = "RateLimited")]
    [Description("محدودیت درخواست")]
    RateLimited = 429,

    /// <summary>
    /// Internal server error (500)
    /// </summary>
    [EnumMember(Value = "InternalError")]
    [Description("خطای سرور")]
    InternalError = 500
  }

  /// <summary>
  /// Extension methods for ResultStatus enum
  /// </summary>
  public static class ResultStatusExtensions
  {
    /// <summary>
    /// Converts ResultStatus to its string representation (using EnumMember value)
    /// </summary>
    public static string ToEnumMemberString(this ResultStatus status)
    {
      var field = status.GetType().GetField(status.ToString());
      if (field != null)
      {
        var attribute = (EnumMemberAttribute?)Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute));
        if (attribute?.Value != null)
          return attribute.Value;
      }
      return status.ToString();
    }

    /// <summary>
    /// Gets the Persian description of the status
    /// </summary>
    public static string GetDescription(this ResultStatus status)
    {
      var field = status.GetType().GetField(status.ToString());
      if (field != null)
      {
        var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        if (attribute?.Description != null)
          return attribute.Description;
      }
      return status.ToString();
    }

    /// <summary>
    /// Parses a string to ResultStatus enum
    /// </summary>
    public static ResultStatus? ParseFromString(string? value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return null;

      // Try exact match first
      if (Enum.TryParse<ResultStatus>(value, true, out var exactMatch))
        return exactMatch;

      // Try by EnumMember value
      foreach (ResultStatus status in Enum.GetValues(typeof(ResultStatus)))
      {
        if (string.Equals(status.ToString(), value, StringComparison.OrdinalIgnoreCase))
          return status;
      }

      return null;
    }
  }

  /// <summary>
  /// Base class for all application results
  /// </summary>
  public class ApplicationResult
  {
      /// <summary>
      /// Indicates whether the operation was successful
      /// </summary>
      public bool IsSuccess { get; set; }
      
      /// <summary>
      /// Status code of the operation result
      /// </summary>
      public ResultStatus Status { get; set; } = ResultStatus.Success;
      
      /// <summary>
      /// Message to return to the client
      /// </summary>
      public string Message { get; set; } = string.Empty;
      
      /// <summary>
      /// List of errors that occurred during the operation
      /// </summary>
      public IEnumerable<string> Errors { get; set; } = new List<string>();
      
      
      /// <summary>
      /// Creates a new successful result
      /// </summary>
      /// <param name="message">Optional success message</param>
      /// <returns>A successful result</returns>
      public static ApplicationResult Success(string message = "Operation completed successfully")
      {
          return new ApplicationResult
          {
              IsSuccess = true,
              Status = ResultStatus.Success,
              Message = message
          };
      }
      
      /// <summary>
      /// Creates a new failed result with BadRequest status
      /// </summary>
      /// <param name="message">Error message</param>
      /// <returns>A failed result</returns>
      public static ApplicationResult Failure(string message)
      {
          return new ApplicationResult
          {
              IsSuccess = false,
              Status = ResultStatus.BadRequest,
              Message = message,
          };
      }

      /// <summary>
      /// Creates a new failed result with specified status
      /// </summary>
      /// <param name="status">Result status</param>
      /// <param name="message">Error message</param>
      /// <returns>A failed result</returns>
      public static ApplicationResult Failure(ResultStatus status, string message)
      {
          return new ApplicationResult
          {
              IsSuccess = false,
              Status = status,
              Message = message,
          };
      }

      /// <summary>
      /// Creates a new Unauthorized result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>An unauthorized result</returns>
      public static ApplicationResult Unauthorized(string message = "احراز هویت مورد نیاز است")
      {
          return Failure(ResultStatus.Unauthorized, message);
      }

      /// <summary>
      /// Creates a new Forbidden result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A forbidden result</returns>
      public static ApplicationResult Forbidden(string message = "شما دسترسی به این عملیات ندارید")
      {
          return Failure(ResultStatus.Forbidden, message);
      }

      /// <summary>
      /// Creates a new NotFound result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A not found result</returns>
      public static ApplicationResult NotFound(string message = "منبع مورد نظر یافت نشد")
      {
          return Failure(ResultStatus.NotFound, message);
      }

      /// <summary>
      /// Creates a new Conflict result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A conflict result</returns>
      public static ApplicationResult Conflict(string message = "تعارض در انجام عملیات")
      {
          return Failure(ResultStatus.Conflict, message);
      }

      /// <summary>
      /// Creates a new ValidationFailed result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A validation failed result</returns>
      public static ApplicationResult ValidationFailed(string message = "اعتبارسنجی ناموفق بود")
      {
          return Failure(ResultStatus.ValidationFailed, message);
      }
      
      /// <summary>
      /// Creates a new failed result with multiple errors (ValidationFailed status)
      /// </summary>
      /// <param name="errors">List of errors</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with multiple errors</returns>
      public static ApplicationResult Failure(IEnumerable<string> errors, string message = "یک یا چند خطا رخ داد")
      {
          return new ApplicationResult
          {
              IsSuccess = false,
              Status = ResultStatus.ValidationFailed,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result with multiple errors and specified status
      /// </summary>
      /// <param name="status">Result status</param>
      /// <param name="errors">List of errors</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with multiple errors</returns>
      public static ApplicationResult Failure(ResultStatus status, IEnumerable<string> errors, string message = "یک یا چند خطا رخ داد")
      {
          return new ApplicationResult
          {
              IsSuccess = false,
              Status = status,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result from an exception with comprehensive error collection (InternalError status)
      /// </summary>
      /// <param name="exception">The exception to extract errors from</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with all exception details</returns>
      public static ApplicationResult Failure(Exception exception, string message = "خطایی رخ داد")
      {
          var errors = ExtractExceptionErrors(exception);
          return new ApplicationResult
          {
              IsSuccess = false,
              Status = ResultStatus.InternalError,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result from an exception with specified status
      /// </summary>
      /// <param name="status">Result status</param>
      /// <param name="exception">The exception to extract errors from</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with all exception details</returns>
      public static ApplicationResult Failure(ResultStatus status, Exception exception, string message = "خطایی رخ داد")
      {
          var errors = ExtractExceptionErrors(exception);
          return new ApplicationResult
          {
              IsSuccess = false,
              Status = status,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result by combining multiple service result errors (InternalError status)
      /// </summary>
      /// <param name="serviceResults">Collection of failed service results</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with all collected errors</returns>
      public static ApplicationResult Failure(IEnumerable<ApplicationResult> serviceResults, string message = "چندین خطا رخ داد")
      {
          var allErrors = new List<string>();
          foreach (var result in serviceResults.Where(r => !r.IsSuccess))
          {
              allErrors.AddRange(result.Errors);
              if (!string.IsNullOrEmpty(result.Message))
              {
                  allErrors.Add(result.Message);
              }
          }
          
          return new ApplicationResult
          {
              IsSuccess = false,
              Status = ResultStatus.InternalError,
              Message = message,
              Errors = allErrors,
          };
      }

      /// <summary>
      /// Extracts all error messages from an exception and its inner exceptions
      /// </summary>
      /// <param name="exception">The exception to extract errors from</param>
      /// <returns>List of all error messages</returns>
      private static List<string> ExtractExceptionErrors(Exception exception)
      {
          var errors = new List<string>();
          var currentException = exception;
          
          while (currentException != null)
          {
              if (!string.IsNullOrEmpty(currentException.Message))
              {
                  errors.Add(currentException.Message);
              }
              
              currentException = currentException.InnerException;
          }
          
          return errors;
      }
  }

  /// <summary>
  /// Generic application result with a return value
  /// </summary>
  /// <typeparam name="T">Type of the result value</typeparam>
  public class ApplicationResult<T> : ApplicationResult
  {
      /// <summary>
      /// The result value
      /// </summary>
      public T? Data { get; set; }
      
      /// <summary>
      /// Creates a new successful result with data
      /// </summary>
      /// <param name="data">The result data</param>
      /// <param name="message">Optional success message</param>
      /// <returns>A successful result with data</returns>
      public static ApplicationResult<T> Success(T data, string message = "عملیات با موفقیت انجام شد")
      {
          return new ApplicationResult<T>
          {
              IsSuccess = true,
              Status = ResultStatus.Success,
              Message = message,
              Data = data
          };
      }
      
      /// <summary>
      /// Creates a new failed result with BadRequest status
      /// </summary>
      /// <param name="message">Error message</param>
      /// <returns>A failed result</returns>
      public static new ApplicationResult<T> Failure(string message)
      {
          return new ApplicationResult<T>
          {
              IsSuccess = false,
              Status = ResultStatus.BadRequest,
              Message = message,
          };
      }

      /// <summary>
      /// Creates a new failed result with specified status
      /// </summary>
      /// <param name="status">Result status</param>
      /// <param name="message">Error message</param>
      /// <returns>A failed result</returns>
      public static new ApplicationResult<T> Failure(ResultStatus status, string message)
      {
          return new ApplicationResult<T>
          {
              IsSuccess = false,
              Status = status,
              Message = message,
          };
      }

      /// <summary>
      /// Creates a new Unauthorized result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>An unauthorized result</returns>
      public static new ApplicationResult<T> Unauthorized(string message = "احراز هویت مورد نیاز است")
      {
          return Failure(ResultStatus.Unauthorized, message);
      }

      /// <summary>
      /// Creates a new Forbidden result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A forbidden result</returns>
      public static new ApplicationResult<T> Forbidden(string message = "شما دسترسی به این عملیات ندارید")
      {
          return Failure(ResultStatus.Forbidden, message);
      }

      /// <summary>
      /// Creates a new NotFound result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A not found result</returns>
      public static new ApplicationResult<T> NotFound(string message = "منبع مورد نظر یافت نشد")
      {
          return Failure(ResultStatus.NotFound, message);
      }

      /// <summary>
      /// Creates a new Conflict result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A conflict result</returns>
      public static new ApplicationResult<T> Conflict(string message = "تعارض در انجام عملیات")
      {
          return Failure(ResultStatus.Conflict, message);
      }

      /// <summary>
      /// Creates a new ValidationFailed result
      /// </summary>
      /// <param name="message">Error message (optional)</param>
      /// <returns>A validation failed result</returns>
      public static new ApplicationResult<T> ValidationFailed(string message = "اعتبارسنجی ناموفق بود")
      {
          return Failure(ResultStatus.ValidationFailed, message);
      }
      
      /// <summary>
      /// Creates a new failed result with multiple errors (ValidationFailed status)
      /// </summary>
      /// <param name="errors">List of errors</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with multiple errors</returns>
      public static new ApplicationResult<T> Failure(IEnumerable<string> errors, string message = "یک یا چند خطا رخ داد")
      {
          return new ApplicationResult<T>
          {
              IsSuccess = false,
              Status = ResultStatus.ValidationFailed,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result with multiple errors and specified status
      /// </summary>
      /// <param name="status">Result status</param>
      /// <param name="errors">List of errors</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with multiple errors</returns>
      public static new ApplicationResult<T> Failure(ResultStatus status, IEnumerable<string> errors, string message = "یک یا چند خطا رخ داد")
      {
          return new ApplicationResult<T>
          {
              IsSuccess = false,
              Status = status,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result from an exception with comprehensive error collection (InternalError status)
      /// </summary>
      /// <param name="exception">The exception to extract errors from</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with all exception details</returns>
      public static new ApplicationResult<T> Failure(Exception exception, string message = "خطایی رخ داد")
      {
          var errors = ExtractExceptionErrors(exception);
          return new ApplicationResult<T>
          {
              IsSuccess = false,
              Status = ResultStatus.InternalError,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result from an exception with specified status
      /// </summary>
      /// <param name="status">Result status</param>
      /// <param name="exception">The exception to extract errors from</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with all exception details</returns>
      public static new ApplicationResult<T> Failure(ResultStatus status, Exception exception, string message = "خطایی رخ داد")
      {
          var errors = ExtractExceptionErrors(exception);
          return new ApplicationResult<T>
          {
              IsSuccess = false,
              Status = status,
              Message = message,
              Errors = errors,
          };
      }

      /// <summary>
      /// Creates a new failed result by combining multiple service result errors (InternalError status)
      /// </summary>
      /// <param name="serviceResults">Collection of failed service results</param>
      /// <param name="message">Optional error message</param>
      /// <returns>A failed result with all collected errors</returns>
      public static new ApplicationResult<T> Failure(IEnumerable<ApplicationResult> serviceResults, string message = "چندین خطا رخ داد")
      {
          var allErrors = new List<string>();
          foreach (var result in serviceResults.Where(r => !r.IsSuccess))
          {
              allErrors.AddRange(result.Errors);
              if (!string.IsNullOrEmpty(result.Message))
              {
                  allErrors.Add(result.Message);
              }
          }
          
          return new ApplicationResult<T>
          {
              IsSuccess = false,
              Status = ResultStatus.InternalError,
              Message = message,
              Errors = allErrors,
          };
      }

      /// <summary>
      /// Extracts all error messages from an exception and its inner exceptions
      /// </summary>
      /// <param name="exception">The exception to extract errors from</param>
      /// <returns>List of all error messages</returns>
      private static List<string> ExtractExceptionErrors(Exception exception)
      {
          var errors = new List<string>();
          var currentException = exception;
          
          while (currentException != null)
          {
              if (!string.IsNullOrEmpty(currentException.Message))
              {
                  errors.Add(currentException.Message);
              }
              
              currentException = currentException.InnerException;
          }
          
          return errors;
      }
  }
