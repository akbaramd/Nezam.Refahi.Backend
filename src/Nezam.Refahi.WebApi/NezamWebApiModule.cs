using System.Text;
using System.Text.Json.Serialization;
using Bonyan.AspNetCore;
using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nezam.Refahi.Identity.Presentation;
using Nezam.Refahi.Settings.Presentation;
using Nezam.Refahi.WebApi.Endpoints;
using Nezam.Refahi.WebApi.Swagger;
using Swashbuckle.AspNetCore.Filters;

namespace Nezam.Refahi.WebApi;

public class NezamWebApiModule : BonWebModule
{
  public NezamWebApiModule()
  {
    DependOn<NezamRefahiIdentityPresentationModule>();
    DependOn<NezamRefahiSettingsPresentationModule>();
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    var configuration = context.GetRequireService<IConfiguration>();
    // Add services to the container
    context.Services.AddEndpointsApiExplorer();
  
// Configure Swagger with JWT authentication
    context.Services.AddSwaggerGen(options =>
    {
      // Add security definition for JWT Bearer authentication
      options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
          Description =
            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer"
        });

      // Add API documentation
      options.SwaggerDoc("v1",
        new OpenApiInfo
        {
          Title = "Nezam Refahi API",
          Version = "v1",
          Description = "API for Nezam Refahi application",
          Contact = new OpenApiContact { Name = "Development Team", Email = "support@nezamrefahi.com" }
        });
      
      // Add custom operation filter for selective authorization display
      options.OperationFilter<AuthorizationOperationFilter>();
    });

  // Configure CORS
    context.Services.AddCors(options =>
    {
      options.AddPolicy("CorsPolicy", policy =>
      {
        policy
          .WithOrigins(configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" })
          .WithMethods(configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ??
                       new[] { "GET", "POST", "PUT", "DELETE" })
          .WithHeaders(configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ??
                       new[] { "Content-Type", "Authorization" })
          .AllowCredentials();
      });
    });
    ConfigureJwtAuthentication(context);
  // Configure JSON serialization
    context.Services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      });

    context.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    context.Services.Configure<JsonOptions>(options =>
    {
      options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    return base.OnConfigureAsync(context);
  }

  public override Task OnInitializeAsync(BonInitializedContext context)
  {
    Console.WriteLine("Module initialized");
    return base.OnInitializeAsync(context);
  }

  public override Task OnApplicationAsync(BonWebApplicationContext context)
  {
    var app = context.Application;
    
    
      app.UseSwagger();
      app.UseSwaggerUI();

    app.UseHttpsRedirection();

    // Use CORS before authorization
    app.UseCors("CorsPolicy");

    // Add authentication middleware before authorization
    app.UseAuthentication();
    app.UseAuthorization();

    return base.OnApplicationAsync(context);
  }

  private void ConfigureJwtAuthentication(BonConfigurationContext context)
  {
    var configuration = context.GetRequireService<IConfiguration>();

    context.Services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options =>
      {
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured"))),
          ValidateIssuer = true,
          ValidIssuer = configuration["Jwt:Issuer"],
          ValidateAudience = true,
          ValidAudience = configuration["Jwt:Audience"],
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero
        };
      });
  }
}
