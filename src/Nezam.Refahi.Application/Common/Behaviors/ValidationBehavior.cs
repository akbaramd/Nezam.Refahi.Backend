using FluentValidation;
using MediatR;
using Nezam.Refahi.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nezam.Refahi.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior to handle validation of requests and return ApplicationResult
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // If there are no validators, proceed with the next handler
        if (!_validators.Any())
        {
            return await next();
        }

        // Create validation context
        var context = new ValidationContext<TRequest>(request);

        // Execute all validators
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect validation failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // If there are validation failures, return a failure result
        if (failures.Count > 0)
        {
            // Extract error messages
            var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
            
            // Handle different response types
            if (typeof(TResponse).IsAssignableFrom(typeof(ApplicationResult)) || 
                (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(ApplicationResult<>)))
            {
                // Case 1: Response type is ApplicationResult or ApplicationResult<T>
                if (typeof(TResponse).IsGenericType && 
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(ApplicationResult<>))
                {
                    // Create a generic failure result with the appropriate type
                    var resultType = typeof(TResponse);
                    var failureMethod = resultType.GetMethod("Failure", new[] { typeof(List<string>), typeof(string), typeof(Exception) });
                    
                    if (failureMethod != null)
                    {
                        // Call the static Failure method with the error messages
                        object?[] parameters = new object?[3];
                        parameters[0] = errorMessages;
                        parameters[1] = "Validation failed. Please check your input.";
                        parameters[2] = null; // This will be boxed as a null object reference
                        
                        return (TResponse)failureMethod.Invoke(null, parameters)!;
                    }
                }
                
                // Fallback for non-generic ApplicationResult
                if (typeof(TResponse) == typeof(ApplicationResult))
                {
                    return (TResponse)(object)ApplicationResult.Failure(errorMessages, "Validation failed. Please check your input.");
                }
            }
            else
            {
                // Case 2: Response type is not ApplicationResult - try to create a default instance with reflection
                // This is needed for backward compatibility with handlers that don't use ApplicationResult
                try
                {
                    // Try to find a constructor that takes validation errors
                    var constructor = typeof(TResponse).GetConstructor(new[] { typeof(List<string>) });
                    if (constructor != null)
                    {
                        return (TResponse)constructor.Invoke(new object[] { errorMessages });
                    }
                    
                    // Try to find a constructor that takes a message
                    constructor = typeof(TResponse).GetConstructor(new[] { typeof(string) });
                    if (constructor != null)
                    {
                        return (TResponse)constructor.Invoke(new object[] { "Validation failed. Please check your input." });
                    }
                    
                    // If we can't find a suitable constructor, throw an exception
                    throw new InvalidOperationException($"Cannot create validation error response for type {typeof(TResponse).Name}. Please use ApplicationResult<T> as your return type.");
                }
                catch (Exception ex)
                {
                    // If we can't create a response, throw an exception with details
                    throw new InvalidOperationException($"Validation failed but could not create response of type {typeof(TResponse).Name}. Please use ApplicationResult<T> as your return type.", ex);
                }
            }
        }

        // If validation passes, proceed with the next handler
        return await next();
    }
}
