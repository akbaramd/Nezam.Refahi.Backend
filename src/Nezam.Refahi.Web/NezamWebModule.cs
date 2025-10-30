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
using Nezam.Refahi.Recreation.Presentation;
using Nezam.Refahi.Notifications.Presentation;
using Nezam.Refahi.BasicDefinitions.Presentation;
using Nezam.Refahi.Facilities.Presentation;
using Nezam.Refahi.Surveying.Presentation;
using Nezam.Refahi.Shared.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nezam.New.EES;
using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.Parsian;
using System.Globalization;


namespace Nezam.Refahi.WebApi;

public class NezamWebModule : BonWebModule
{
  public NezamWebModule()
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
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    var configuration = context.GetRequireService<IConfiguration>();
    
    // Get connection strings
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    // Configure Cookie Authentication as the default scheme
    context.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AccessDenied";
            options.SlidingExpiration = true;
            options.Cookie.Name = "NezamRefahi.Auth";

            // append returnUrl on access-denied
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToAccessDenied = ctx =>
                {
                    string ret = ctx.Request.Path + ctx.Request.QueryString;
                    ctx.Response.Redirect($"{options.AccessDeniedPath}?returnUrl={Uri.EscapeDataString(ret)}");
                    return Task.CompletedTask;
                }
            };
        });

    // Add JWT as secondary authentication scheme
    var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
    if (jwtSettings != null && !string.IsNullOrEmpty(jwtSettings.Secret))
    {
        var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
        
        context.Services.AddAuthentication()
            .AddJwtBearer("JwtBearer", options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers["Token-Expired"] = "true";
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }

    // Configure JWT Settings (used by JwtService)
    context.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

    context.Services.AddAuthorization(options =>
    {
     
    });

    /*─────────────────────────────────────────────────
      APPLICATION & INFRASTRUCTURE SERVICES
    ─────────────────────────────────────────────────*/
    context.Services.AddHttpContextAccessor();
    context.Services.AddMemoryCache();
    context.Services.AddSession();
    
    // Register authentication service
    context.Services.AddScoped<Nezam.Refahi.Web.Services.IAuthService, Nezam.Refahi.Web.Services.AuthService>();
    
    // Register Tour repository (using existing Recreation module repository)
    context.Services.AddScoped<Nezam.Refahi.Recreation.Domain.Repositories.ITourRepository, Nezam.Refahi.Recreation.Infrastructure.Persistence.Repositories.TourRepository>();

    /*─────────────────────────────────────────────────
      HANGFIRE  (jobs + dashboard)
    ─────────────────────────────────────────────────*/
    context.Services.AddHangfire(cfg => cfg
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout       = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout   = TimeSpan.FromMinutes(5),
            QueuePollInterval            = TimeSpan.FromSeconds(15),
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks           = true,
            PrepareSchemaIfNecessary     = true
        }));

    context.Services.AddHangfireServer(opt =>
    {
        opt.ServerName  = "Nezam.EES.Hangfire";
        opt.Queues      = new[] { "default" };
        opt.WorkerCount = Environment.ProcessorCount * 5;
    });

    /*─────────────────────────────────────────────────
      PARBAD (payment gateways)


    /*─────────────────────────────────────────────────
      SWAGGER / OPENAPI
    ─────────────────────────────────────────────────*/
    context.Services.AddEndpointsApiExplorer();
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
      
      
      // Add XML documentation (if available)
      var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
      if (File.Exists(xmlPath))
      {
        options.IncludeXmlComments(xmlPath);
      }

      // JWT support
      options.AddSecurityDefinition("JwtBearer", new OpenApiSecurityScheme
      {
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Name        = "Authorization",
          In          = ParameterLocation.Header,
          Type        = SecuritySchemeType.Http,
          Scheme      = "bearer",
          BearerFormat = "JWT"
      });
      
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
          {
              new OpenApiSecurityScheme
              {
                  Reference = new OpenApiReference
                  {
                      Type = ReferenceType.SecurityScheme,
                      Id = "JwtBearer"
                  }
              },
              new string[] {}
          }
      });

      // Handle conflicting actions by using the first one
      options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

      // Only include API controllers (those with "api" in the route)
      options.DocInclusionPredicate((docName, apiDesc) =>
      {
          // Check if the controller route contains "api"
          var controllerRoute = apiDesc.RelativePath?.ToLowerInvariant() ?? "";
          return controllerRoute.Contains("api");
      });
    });
    

    /*─────────────────────────────────────────────────
      CORS
    ─────────────────────────────────────────────────*/
    context.Services.AddCors();

    /*─────────────────────────────────────────────────
      MVC
    ─────────────────────────────────────────────────*/
    context.Services.AddControllersWithViews();

    // Add services to the container
    context.Services.AddEndpointsApiExplorer();
  
    // Configure CORS
    context.Services.AddCors();
    
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

  public override Task OnApplicationAsync(BonWebApplicationContext context)
  {
    var app = context.Application;
    
    // Handle database migrations and seeding
    var serviceProvider = app.Services;
    var logger = serviceProvider.GetRequiredService<ILogger<NezamWebModule>>();
    
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nezam Engineering API V1"));
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseStaticFiles();
    app.UseParbadVirtualGateway();
    app.UseSession();

    // CORS must be before routing and authentication
    app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    /* Hangfire dashboard */
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization                = new[] { new HangfireAuthorizationFilter() },
        DashboardTitle               = "Nezam EES Jobs",
        AppPath                      = "/",            // logo link
        StatsPollingInterval         = 5000,
        DisplayStorageConnectionString = false
    });

    /*─────────────────────────────────────────────────
      ROUTING – Areas first, then default
    ─────────────────────────────────────────────────*/

    // API Controllers
    app.MapControllers();

    app.MapControllerRoute("default", pattern: "{controller=Dashboard}/{action=Index}/{id?}");

    return base.OnApplicationAsync(context);
  }



  public override Task OnPostApplicationAsync(BonWebApplicationContext context)
  {
    var app = context.Application;
    
    context.Application.MapControllers();
    return base.OnPostApplicationAsync(context);
  }
}

/*─────────────────────────────────────────────────
  MEDIATR VALIDATION BEHAVIOUR
─────────────────────────────────────────────────*/
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var context  = new ValidationContext<TRequest>(request);
        var failures = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, ct)));
        var errors   = failures.SelectMany(r => r.Errors).Where(f => f is not null).ToList();
        if (errors.Count != 0) throw new ValidationException(errors);
        return await next();
    }
}

/*─────────────────────────────────────────────────
  JWT SETTINGS CLASS
─────────────────────────────────────────────────*/
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationHours { get; set; } = 24;
}

/*─────────────────────────────────────────────────
  HANGFIRE DASHBOARD AUTH FILTER
─────────────────────────────────────────────────*/
public sealed class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var http = context.GetHttpContext();
        return http.User.Identity?.IsAuthenticated == true && http.User.IsInRole("Admin");
  }
}
