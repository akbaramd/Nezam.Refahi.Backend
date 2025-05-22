using Nezam.Refahi.Infrastructure.IoC;
using Nezam.Refahi.WebApi.Endpoints;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication
builder.Services.AddSwaggerGen(options => 
{
    // Add security definition for JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    // Add security requirement to make the lock icon appear on protected endpoints
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Add API documentation
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nezam Refahi API",
        Version = "v1",
        Description = "API for Nezam Refahi application",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "support@nezamrefahi.com"
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" })
            .WithMethods(builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE" })
            .WithHeaders(builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? new[] { "Content-Type", "Authorization" })
            .AllowCredentials();
    });
});

// Configure JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add application services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS before authorization
app.UseCors("CorsPolicy");

// Add authentication middleware before authorization
app.UseAuthentication();
app.UseAuthorization();

// Map all API endpoints using extension methods
app.MapApiEndpoints();

app.Run();
