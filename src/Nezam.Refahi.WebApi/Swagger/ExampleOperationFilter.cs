using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nezam.Refahi.WebApi.Swagger;

public class ExampleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add custom headers examples
        if (operation.Parameters != null)
        {
            foreach (var parameter in operation.Parameters)
            {
                switch (parameter.Name.ToLower())
                {
                    case "x-device-id":
                        parameter.Example = new Microsoft.OpenApi.Any.OpenApiString("mobile_app_v1.0");
                        parameter.Description ??= "Device identifier for tracking purposes";
                        break;
                    case "authorization":
                        parameter.Example = new Microsoft.OpenApi.Any.OpenApiString("Bearer your-jwt-token-here");
                        break;
                }
            }
        }

       

        // Add common error responses to all endpoints
        if (operation.Responses != null && !operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal Server Error - خطای داخلی سرور",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["error"] = new OpenApiSchema { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("internal_server_error") },
                                ["message"] = new OpenApiSchema { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("خطای داخلی سرور رخ داده است") },
                                ["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" }
                            }
                        }
                    }
                }
            });
        }
    }
}