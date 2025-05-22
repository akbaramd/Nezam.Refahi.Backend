using System;
using System.Collections.Generic;

namespace Nezam.Refahi.Application.Common.Models
{
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
        /// Message to return to the client
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// List of errors that occurred during the operation
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
        
        /// <summary>
        /// Exception that occurred during the operation
        /// </summary>
        public Exception? Exception { get; set; }
        
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
                Message = message
            };
        }
        
        /// <summary>
        /// Creates a new failed result
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Optional exception</param>
        /// <returns>A failed result</returns>
        public static ApplicationResult Failure(string message, Exception? exception = null)
        {
            return new ApplicationResult
            {
                IsSuccess = false,
                Message = message,
                Exception = exception
            };
        }
        
        /// <summary>
        /// Creates a new failed result with multiple errors
        /// </summary>
        /// <param name="errors">List of errors</param>
        /// <param name="message">Optional error message</param>
        /// <param name="exception">Optional exception</param>
        /// <returns>A failed result with multiple errors</returns>
        public static ApplicationResult Failure(List<string> errors, string message = "One or more errors occurred", Exception? exception = null)
        {
            return new ApplicationResult
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                Exception = exception
            };
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
        public static ApplicationResult<T> Success(T data, string message = "Operation completed successfully")
        {
            return new ApplicationResult<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }
        
        /// <summary>
        /// Creates a new failed result
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="exception">Optional exception</param>
        /// <returns>A failed result</returns>
        public static new ApplicationResult<T> Failure(string message, Exception? exception = null)
        {
            return new ApplicationResult<T>
            {
                IsSuccess = false,
                Message = message,
                Exception = exception
            };
        }
        
        /// <summary>
        /// Creates a new failed result with multiple errors
        /// </summary>
        /// <param name="errors">List of errors</param>
        /// <param name="message">Optional error message</param>
        /// <param name="exception">Optional exception</param>
        /// <returns>A failed result with multiple errors</returns>
        public static new ApplicationResult<T> Failure(List<string> errors, string message = "One or more errors occurred", Exception? exception = null)
        {
            return new ApplicationResult<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                Exception = exception
            };
        }
    }
}
