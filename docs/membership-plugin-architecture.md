# Membership Plugin Architecture Implementation

This document describes the complete implementation of the port-based plugin architecture for the Membership module.

## Architecture Overview

The implementation follows the principles described in the architectural blueprint:
1. **Ports in Membership.Contracts** - Pure abstractions for external dependencies
2. **Plugin Constants** - Domain-specific keys and values owned by plugins
3. **Seeding via Application Services** - Safe bootstrapping without bypassing domain rules
4. **External Storage Integration** - Clean anti-corruption boundary for external systems

## Implementation Components

### 1. Port Definitions (Membership.Contracts)

#### IExternalMemberStorage
```csharp
public interface IExternalMemberStorage
{
    Task<ExternalMemberResponseDto?> GetMemberByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default);
    Task<ExternalMemberResponseDto?> GetMemberByMembershipCodeAsync(string membershipCode, CancellationToken cancellationToken = default);
    Task<bool> ValidateMemberAsync(string nationalCode, string membershipCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExternalMemberResponseDto>> SearchMembersAsync(ExternalMemberSearchCriteria criteria, CancellationToken cancellationToken = default);
}
```

#### IMembershipSeedContributor
```csharp
public interface IMembershipSeedContributor
{
    Task SeedAsync(CancellationToken cancellationToken = default);
    int Priority { get; }
    string Name { get; }
}
```

#### Supporting DTOs
- `ExternalMemberResponseDto` - Member data from external systems with claims and roles
- `ExternalLicenseDto` - License information with claims
- `ExternalClaimDto` - Claim information with type, value, category, and options
- `ExternalRoleDto` - Role information with associated claims
- `ExternalMemberSearchCriteria` - Search parameters

### 2. Plugin Structure (Plugin.NezamMohandesi)

#### Folder Layout
```
src/Plugins/Nezam.Refahi.Plugin.NezamMohandesi/
├── Constants/
│   └── NezamMohandesiConstants.cs          # Domain-specific keys
├── Seeding/
│   └── NezamMohandesiSeedContributor.cs    # Seeding implementation
├── Services/
│   └── ExternalMemberStorage.cs            # Port implementation
├── Cedo/                                   # External system models
└── NezamRefahiNezamMohandesiPlugin.cs      # Module registration
```

#### Domain Constants
```csharp
public static class NezamMohandesiConstants
{
    public static class ClaimTypes
    {
        public const string MembershipCode = "membership_code";
        public const string LicenseNumber = "license_number";
        public const string MemberStatus = "member_status";
        public const string OrganizationType = "organization_type";
        public const string EngineeringField = "engineering_field";
        public const string ServiceCapability = "service_capability";
        public const string LicenseStatus = "license_status";
        // ... more claim types
    }
    
    public static class ClaimValues
    {
        public static class EngineeringFields
        {
            public const string Civil = "civil";
            public const string Mechanical = "mechanical";
            public const string Electrical = "electrical";
            // ... more fields
        }
        
        public static class ServiceCapabilities
        {
            public const string Design = "design";
            public const string Supervision = "supervision";
            public const string Consultation = "consultation";
            // ... more capabilities
        }
    }
    
    public static class RoleKeys
    {
        public const string EngineeringMember = "engineering_member";
        public const string LicensedEngineer = "licensed_engineer";
        public const string ConsultingMember = "consulting_member";
        // ... more roles
    }
}
```

#### External Member Storage Implementation
The `ExternalMemberStorage` class implements `IExternalMemberStorage` and provides:
- National code and membership code lookups with claims-based responses
- Member validation
- Search functionality with pagination
- Claims and roles mapping from external data to domain concepts
- Persian/local language mapping to standard engineering fields and capabilities
- Full EF Core integration with the CEDO database
- Comprehensive logging and error handling

#### Seeding Implementation
The `NezamMohandesiSeedContributor` provides:
- Priority-based execution
- Placeholder for future claim type seeding
- Placeholder for future role seeding
- Proper error handling and logging

### 3. DI Registration and Startup Integration

#### Plugin Registration
```csharp
public class NezamRefahiNezamMohandesiPlugin : BonModule
{
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        // Register external database context
        context.Services.AddDbContext<CedoContext>(opt => 
            opt.UseSqlServer(configuration.GetConnectionString("CedoConnection")));
        
        // Register port implementation
        context.Services.AddScoped<IExternalMemberStorage, ExternalMemberStorage>();
        
        // Register seed contributor
        context.Services.AddScoped<IMembershipSeedContributor, NezamMohandesiSeedContributor>();
        
        return base.OnConfigureAsync(context);
    }
}
```

#### Startup Integration
```csharp
// In WebAPI module
context.Services.AddHostedService<MembershipSeedingHostedService>();
```

The `MembershipSeedingHostedService`:
- Discovers all registered `IMembershipSeedContributor` instances
- Runs them in priority order on application startup
- Handles errors gracefully without failing application startup
- Provides comprehensive logging

## Usage Examples

### Using External Member Storage in Application Layer
```csharp
public class MemberApplicationService
{
    private readonly IExternalMemberStorage _externalStorage;

    public async Task<MemberDto?> GetMemberByNationalCodeAsync(string nationalCode)
    {
        var externalMember = await _externalStorage.GetMemberByNationalCodeAsync(nationalCode);
        if (externalMember == null) return null;
        
        // Convert to internal DTO and return
        return MapToMemberDto(externalMember);
    }
}
```

### Adding a New Plugin
1. Create plugin project referencing `Membership.Contracts`
2. Implement `IExternalMemberStorage` for your external system
3. Implement `IMembershipSeedContributor` for seeding
4. Create constants class for domain-specific keys
5. Register services in plugin module
6. Plugin is automatically discovered and loaded

### Configuration
```json
{
  "ConnectionStrings": {
    "CedoConnection": "Server=...;Database=CEDO;..."
  }
}
```

## Key Architectural Benefits

1. **Clean Separation**: Membership domain knows nothing about external systems
2. **Replaceable Adapters**: Can swap out external systems without changing core code
3. **Testable**: Easy to mock ports for unit testing
4. **Safe Seeding**: All seeding goes through application services
5. **Plugin Isolation**: Each plugin owns its own constants and configuration
6. **Startup Integration**: Automatic discovery and execution of seed contributors

## Extension Points

To extend this architecture:

1. **Add New Ports**: Define new interfaces in `Membership.Contracts.Services`
2. **Add New DTOs**: Create new data transfer objects in `Membership.Contracts.Dtos`
3. **Add New Constants**: Plugin-specific values go in plugin constants classes
4. **Add New Seeding**: Implement additional `IMembershipSeedContributor` instances

## Future Enhancements

1. **Application Services**: Once CQRS command/query handlers are available, the seeding implementations can be completed
2. **Validation**: Add port validation during startup
3. **Caching**: Add caching layer for external member storage
4. **Metrics**: Add performance metrics for external calls
5. **Circuit Breaker**: Add resilience patterns for external system calls

This implementation provides a solid foundation for the port-based plugin architecture while maintaining clean architectural boundaries and providing flexibility for future enhancements.