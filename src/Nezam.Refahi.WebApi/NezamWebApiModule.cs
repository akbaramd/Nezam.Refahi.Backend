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
using Nezam.Refahi.Membership.Presentation;
using Nezam.Refahi.Settings.Presentation;
using System.Reflection;
using Nezam.Refahi.Finance.Presentation;
using Nezam.Refahi.Membership.Application.HostedServices;
using Nezam.Refahi.Recreation.Presentation;
using Nezam.Refahi.Notifications.Presentation;
using Nezam.Refahi.BasicDefinitions.Presentation;
using Nezam.Refahi.Facilities.Presentation;
using Nezam.Refahi.Surveying.Presentation;
using Nezam.Refahi.Shared.Infrastructure.Services;
using FluentValidation;

using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.Parsian;
using System.Globalization;
using Nezam.Refahi.Orchestrator;
using Nezam.Refahi.WebApi.HealthChecks;
using Nezam.Refahi.WebApi.Swagger;
using Swashbuckle.AspNetCore.Filters;


namespace Nezam.Refahi.WebApi;

public class NezamWebApiModule : BonWebModule
{
  public NezamWebApiModule()
  {
    DependOn<NezamRefahiIdentityPresentationModule>();
    DependOn<NezamRefahiSettingsPresentationModule>();
    DependOn<NezamRefahiMembershipPresentationModule>();
    DependOn<NezamRefahiRecreationPresentationModule>();
    DependOn<NezamRefahiFinancePresentationModule>();
    DependOn<NezamRefahiNotificationPresentationModule>();
    DependOn<NezamRefahiBasicDefinitionsPresentationModule>();
    DependOn<NezamRefahiSurveyingPresentationModule>();
    DependOn<NezamRefahiFacilitiesPresentationModule>();
    DependOn<NezamRefahiOrchestratorModule>();
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
      
      // Add example operation filter
      options.OperationFilter<ExampleOperationFilter>();
      
      // Enable examples
      options.ExampleFilters();
      
      // Add XML documentation (if available)
      var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
      if (File.Exists(xmlPath))
      {
        options.IncludeXmlComments(xmlPath);
      }
    });
    
    // Add example filters
    context.Services.AddSwaggerExamplesFromAssemblyOf<NezamWebApiModule>();

  // Configure CORS
    context.Services.AddCors();
    ConfigureJwtAuthentication(context);
  // Configure JSON serialization
    context.Services.AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
      });
    // Add health checks
    context.Services.AddHealthChecks()
      .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready", "live" })
      .AddCheck<OutboxHealthCheck>("outbox", tags: new[] { "ready" });

    context.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    context.Services.Configure<JsonOptions>(options =>
    {
      options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    // Register hosted services


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
      app.UseDeveloperExceptionPage();
      app.UseStaticFiles();
      
      app.UseSwagger();
      app.UseSwaggerUI(options =>
      {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nezam Refahi API v1");
        options.DocumentTitle = "Nezam Refahi API Documentation";
        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelExpandDepth(2);
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
      });

    app.UseHttpsRedirection();

    // Use CORS before authorization
    app.UseCors(c =>
    {
      c.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader();
    });

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

  public override Task OnPostApplicationAsync(BonWebApplicationContext context)
  {
    var app = context.Application;
    
    // Map health check endpoints
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
      ResponseWriter = async (context, report) =>
      {
        context.Response.ContentType = "application/json";
        var response = new
        {
          status = report.Status.ToString(),
          checks = report.Entries.Select(entry => new
          {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description,
            duration = entry.Value.Duration.TotalMilliseconds
          }),
          totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
      }
    });
    
    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
      Predicate = check => check.Tags.Contains("ready")
    });
    
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
      Predicate = check => check.Tags.Contains("live")
    });
  
    context.Application.MapControllers();
    return base.OnPostApplicationAsync(context);
  }
}
