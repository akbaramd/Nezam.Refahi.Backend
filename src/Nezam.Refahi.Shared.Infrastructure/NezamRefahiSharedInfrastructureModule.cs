using System.Reflection;
using Bonyan.Modularity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Nezam.Refahi.Shared.Application;
using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Shared.Application.Common.Behaviors;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Infrastructure.Services;

namespace Nezam.Refahi.Shared.Infrastructure;

public class NezamRefahiSharedInfrastructureModule<TDbContext> : BonWebModule where TDbContext : DbContext
{
  public NezamRefahiSharedInfrastructureModule()
  {
    DependOn<NezamRefahiSharedApplicationModule>();
  }

  public override Task OnConfigureAsync(BonConfigurationContext context)
  {
    
    context.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    
    // Add memory cache for shared services
    context.Services.AddMemoryCache();
    
    // Add HTTP context accessor for shared services
    context.Services.AddHttpContextAccessor();
    
    context.Services.AddMediatR(cfg => 
    {
      cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
      // Add pipeline behaviors
      cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    });
        
    // Add FluentValidation
    context.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    // Configure JWT authentication (shared across modules)

    return base.OnConfigureAsync(context);
  }

 

  
}
