# Special Capacity System Usage Examples

## Overview
The special capacity system allows creating VIP-only tour capacities that are only visible to special members. Regular members cannot see or access these special capacities.

## Key Components

### 1. Member Entity (Membership Context)
- **IsSpecial**: Boolean field indicating if member has VIP status
- **GrantSpecialStatus()**: Method to grant VIP status
- **RevokeSpecialStatus()**: Method to revoke VIP status
- **CanAccessSpecialCapacities()**: Method to check access rights

### 2. TourCapacity Entity (Recreation Context)
- **IsSpecial**: Boolean field indicating if capacity is VIP-only
- **MarkAsSpecial()**: Method to mark capacity as VIP-only
- **RemoveSpecialStatus()**: Method to remove VIP status
- **IsVisibleToMember(bool memberIsSpecial)**: Method to check visibility
- **CanMemberReserve(bool memberIsSpecial)**: Method to check reservation rights

### 3. SpecialCapacityService (Domain Service)
- **FilterCapacitiesForMember()**: Filters capacities based on member status
- **CalculatePublicCapacityStatistics()**: Calculates stats excluding special capacities
- **CalculateSpecialCapacityStatistics()**: Calculates stats for special capacities only

## Usage Examples

### Example 1: Creating Special Capacities

```csharp
// Create a regular capacity
var regularCapacity = new TourCapacity(
    tourId: tourId,
    maxParticipants: 50,
    registrationStart: DateTime.UtcNow.AddDays(1),
    registrationEnd: DateTime.UtcNow.AddDays(30),
    description: "Regular tour capacity",
    isSpecial: false // Regular capacity
);

// Create a special capacity for VIP members
var specialCapacity = new TourCapacity(
    tourId: tourId,
    maxParticipants: 10,
    registrationStart: DateTime.UtcNow.AddDays(1),
    registrationEnd: DateTime.UtcNow.AddDays(30),
    description: "VIP tour capacity",
    isSpecial: true // Special capacity
);
```

### Example 2: Managing Member Special Status

```csharp
// Grant special status to a member
member.GrantSpecialStatus();

// Check if member can access special capacities
if (member.CanAccessSpecialCapacities())
{
    // Member can see and reserve special capacities
}

// Revoke special status
member.RevokeSpecialStatus();
```

### Example 3: Filtering Capacities for Members

```csharp
// Get all capacities for a tour
var allCapacities = await tourCapacityRepository.GetByTourIdAsync(tourId);

// Filter capacities based on member's special status
var specialCapacityService = new SpecialCapacityService();
var visibleCapacities = specialCapacityService.FilterCapacitiesForMember(
    allCapacities, 
    member.IsSpecial
);

// Regular members will only see regular capacities
// Special members will see both regular and special capacities
```

### Example 4: Capacity Statistics

```csharp
var allCapacities = await tourCapacityRepository.GetByTourIdAsync(tourId);

// Calculate public statistics (excluding special capacities)
var publicStats = specialCapacityService.CalculatePublicCapacityStatistics(allCapacities);
Console.WriteLine($"Public Capacity: {publicStats.TotalMaxParticipants} max, {publicStats.TotalRemainingParticipants} remaining");

// Calculate special statistics (only special capacities)
var specialStats = specialCapacityService.CalculateSpecialCapacityStatistics(allCapacities);
Console.WriteLine($"Special Capacity: {specialStats.TotalMaxParticipants} max, {specialStats.TotalRemainingParticipants} remaining");
```

### Example 5: Reservation Logic

```csharp
public async Task<bool> CanReserveCapacity(Guid memberId, Guid capacityId, int participantCount)
{
    var member = await memberRepository.GetByIdAsync(memberId);
    var capacity = await tourCapacityRepository.GetByIdAsync(capacityId);
    
    if (member == null || capacity == null)
        return false;
    
    // Check if member can reserve this capacity
    return capacity.CanMemberReserve(member.IsSpecial) && 
           capacity.CanAccommodateForMember(participantCount, member.IsSpecial);
}
```

### Example 6: API Endpoint for Getting Capacities

```csharp
[HttpGet("tours/{tourId}/capacities")]
public async Task<IActionResult> GetTourCapacities(Guid tourId)
{
    var memberId = GetCurrentMemberId();
    var member = await memberRepository.GetByIdAsync(memberId);
    
    var allCapacities = await tourCapacityRepository.GetByTourIdAsync(tourId);
    
    // Filter capacities based on member's special status
    var specialCapacityService = new SpecialCapacityService();
    var visibleCapacities = specialCapacityService.FilterCapacitiesForMember(
        allCapacities, 
        member.IsSpecial
    );
    
    // Convert to DTOs
    var capacityDtos = visibleCapacities.Select(c => new TourCapacityDto
    {
        Id = c.Id,
        MaxParticipants = c.MaxParticipants,
        RemainingParticipants = c.RemainingParticipants,
        IsSpecial = c.IsSpecial,
        // Only include special capacity details if member is special
        SpecialDetails = member.IsSpecial ? c.Description : null
    });
    
    return Ok(capacityDtos);
}
```

## Key Benefits

1. **Complete Isolation**: Special capacities are completely hidden from regular members
2. **Clean Separation**: Public statistics exclude special capacities
3. **Flexible Access Control**: Easy to grant/revoke special status
4. **Domain-Driven Design**: Business logic encapsulated in domain entities
5. **Performance**: Efficient filtering and calculation methods

## Security Considerations

- Special capacities are filtered at the domain level
- Regular members cannot see special capacity details
- Public statistics exclude special capacities
- Access control is enforced in domain methods
- No way for regular members to discover special capacities exist

This system ensures that special capacities remain completely private and only accessible to VIP members.
