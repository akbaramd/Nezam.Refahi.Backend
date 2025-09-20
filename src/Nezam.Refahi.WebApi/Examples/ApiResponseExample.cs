using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi.Examples;

// Example for generic API response format
public class ApiResponseExample : IExamplesProvider<object>
{
    public object GetExamples()
    {
        return new
        {
            success = true,
            message = "عملیات با موفقیت انجام شد",
            data = new
            {
                id = Guid.NewGuid(),
                name = "نمونه داده",
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            },
            timestamp = DateTime.UtcNow
        };
    }
}

// Example for error response format

// Example for pagination response

// Example for Iranian national ID validation

// Example for Iranian phone number