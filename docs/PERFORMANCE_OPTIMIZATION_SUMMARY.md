# Performance Optimization: User Request Checking for Cycles

## Overview

I have implemented optimized repository methods to efficiently check if the current user has sent requests for specific cycles, rather than loading all user requests. This provides significant performance improvements, especially when dealing with large datasets.

## Key Performance Optimizations

### 1. **New Optimized Repository Methods**

#### `HasUserRequestForCycleAsync`
```csharp
Task<bool> HasUserRequestForCycleAsync(Guid userId, Guid cycleId, CancellationToken cancellationToken = default);
```
- **Purpose**: Quickly check if user has any request for a specific cycle
- **Performance**: Uses `AnyAsync()` for minimal data transfer
- **Use Case**: Boolean checks without needing request details

#### `GetUserRequestForCycleAsync`
```csharp
Task<FacilityRequest?> GetUserRequestForCycleAsync(Guid userId, Guid cycleId, CancellationToken cancellationToken = default);
```
- **Purpose**: Get user's request details for a specific cycle
- **Performance**: Single targeted query with full entity details
- **Use Case**: When you need the actual request data for one cycle

#### `GetUserRequestsForCyclesAsync`
```csharp
Task<Dictionary<Guid, FacilityRequest>> GetUserRequestsForCyclesAsync(Guid userId, IEnumerable<Guid> cycleIds, CancellationToken cancellationToken = default);
```
- **Purpose**: Get user requests for multiple cycles in one query
- **Performance**: Single query for multiple cycles, returns dictionary for O(1) lookup
- **Use Case**: Batch processing multiple cycles efficiently

#### `GetCyclesWithUserRequestsAsync`
```csharp
Task<HashSet<Guid>> GetCyclesWithUserRequestsAsync(Guid userId, IEnumerable<Guid> cycleIds, CancellationToken cancellationToken = default);
```
- **Purpose**: Get only cycle IDs where user has requests
- **Performance**: Minimal data transfer, only cycle IDs
- **Use Case**: Filtering cycles without loading request details

### 2. **Optimized Query Handler Updates**

#### GetFacilityCyclesQueryHandler
**Before (Inefficient):**
```csharp
// Load ALL user requests
var userRequestsList = await _requestRepository.GetByUserIdAsync(memberInfo.Id, cancellationToken);
var userRequestCycleIds = userRequestsList
    .Where(r => r.FacilityId == request.FacilityId)
    .Select(r => r.FacilityCycleId)
    .ToHashSet();
```

**After (Optimized):**
```csharp
// Load only cycle IDs for specific cycles
var cycleIds = cyclesList.Select(c => c.Id).ToList();
var userRequestCycleIds = await _requestRepository.GetCyclesWithUserRequestsAsync(memberInfo.Id, cycleIds, cancellationToken);
```

**Performance Benefits:**
- ✅ **Reduced Data Transfer**: Only loads cycle IDs, not full request entities
- ✅ **Targeted Query**: Only queries for specific cycles, not all user requests
- ✅ **Database Optimization**: Uses `SELECT DISTINCT` for minimal data
- ✅ **Memory Efficiency**: Returns `HashSet<Guid>` instead of full entities

#### GetUserCyclesWithRequestsQueryHandler
**Before (Inefficient):**
```csharp
// Load ALL user requests then filter in memory
var allRequests = await _requestRepository.GetByUserIdAsync(memberInfo.Id, cancellationToken);
var filteredRequests = allRequests.AsEnumerable();
// Apply multiple filters in memory...
```

**After (Optimized):**
```csharp
// Use optimized query parameters for database-level filtering
var queryParameters = new FacilityRequestQueryParameters
{
    MemberId = memberInfo.Id,
    FacilityId = request.FacilityId,
    Status = request.RequestStatus,
    Page = request.Page,
    PageSize = request.PageSize
};
var paginatedRequests = await _requestRepository.GetFacilityRequestsAsync(queryParameters, cancellationToken);
```

**Performance Benefits:**
- ✅ **Database-Level Filtering**: Filters applied at database level, not in memory
- ✅ **Pagination at Source**: Database handles pagination, not application
- ✅ **Reduced Memory Usage**: Only loads required records
- ✅ **Better Query Performance**: Database can use indexes effectively

#### GetFacilityCycleDetailsQueryHandler
**Before (Inefficient):**
```csharp
// Load ALL user requests then filter
var userRequests = await _requestRepository.GetByUserIdAsync(memberInfo.Id, cancellationToken);
var cycleRequests = userRequests.Where(r => r.FacilityCycleId == request.CycleId);
```

**After (Optimized):**
```csharp
// Direct query for specific cycle
var userRequest = await _requestRepository.GetUserRequestForCycleAsync(memberInfo.Id, request.CycleId, cancellationToken);
```

**Performance Benefits:**
- ✅ **Single Targeted Query**: Direct query for specific cycle
- ✅ **No Memory Filtering**: Database handles the filtering
- ✅ **Optimal Performance**: Uses indexes on `MemberId` and `FacilityCycleId`

## Performance Impact Analysis

### Database Query Optimization

#### Before Optimization
```sql
-- Load ALL user requests (potentially thousands of records)
SELECT * FROM FacilityRequests 
WHERE MemberId = @userId
ORDER BY CreatedAt DESC;

-- Then filter in application memory
-- Multiple WHERE clauses applied in C# code
```

#### After Optimization
```sql
-- Check existence only (minimal data transfer)
SELECT COUNT(1) FROM FacilityRequests 
WHERE MemberId = @userId AND FacilityCycleId = @cycleId;

-- Or get specific request for cycle
SELECT * FROM FacilityRequests 
WHERE MemberId = @userId AND FacilityCycleId = @cycleId
ORDER BY CreatedAt DESC
LIMIT 1;

-- Or batch check multiple cycles
SELECT DISTINCT FacilityCycleId FROM FacilityRequests 
WHERE MemberId = @userId AND FacilityCycleId IN (@cycleIds);
```

### Memory Usage Reduction

| Scenario | Before | After | Improvement |
|----------|--------|-------|-------------|
| User with 100 requests, checking 5 cycles | Load 100 full entities | Load 5 cycle IDs | **95% reduction** |
| User with 1000 requests, checking 10 cycles | Load 1000 full entities | Load 10 cycle IDs | **99% reduction** |
| Single cycle check | Load all user requests | Load 1 specific request | **99%+ reduction** |

### Query Performance Benefits

1. **Index Utilization**: Queries now use composite indexes on `(MemberId, FacilityCycleId)`
2. **Reduced Network Traffic**: Minimal data transfer between database and application
3. **Memory Efficiency**: No unnecessary object instantiation
4. **CPU Optimization**: No in-memory filtering operations
5. **Scalability**: Performance remains consistent regardless of total user requests

## Implementation Details

### Repository Implementation
```csharp
public async Task<HashSet<Guid>> GetCyclesWithUserRequestsAsync(Guid userId, IEnumerable<Guid> cycleIds, CancellationToken cancellationToken = default)
{
    var cycleIdsList = cycleIds.ToList();
    if (!cycleIdsList.Any())
        return new HashSet<Guid>();

    // Single optimized query
    var cycleIdsWithRequests = await _dbSet
        .Where(r => r.MemberId == userId && cycleIdsList.Contains(r.FacilityCycleId))
        .Select(r => r.FacilityCycleId)  // Only select what we need
        .Distinct()
        .ToListAsync(cancellationToken);

    return cycleIdsWithRequests.ToHashSet();
}
```

### Query Handler Usage
```csharp
// Efficient batch checking
var cycleIds = cyclesList.Select(c => c.Id).ToList();
var userRequestCycleIds = await _requestRepository.GetCyclesWithUserRequestsAsync(memberInfo.Id, cycleIds, cancellationToken);

// Efficient request details loading
var cycleIds = paginatedCycles.Select(c => c.Id).ToList();
var userRequests = await _requestRepository.GetUserRequestsForCyclesAsync(memberInfo.Id, cycleIds, cancellationToken);
```

## Usage Examples

### 1. Check if User Has Request for Specific Cycle
```csharp
var hasRequest = await _requestRepository.HasUserRequestForCycleAsync(userId, cycleId, cancellationToken);
if (hasRequest)
{
    var request = await _requestRepository.GetUserRequestForCycleAsync(userId, cycleId, cancellationToken);
    // Process request details
}
```

### 2. Batch Check Multiple Cycles
```csharp
var cycleIds = cycles.Select(c => c.Id).ToList();
var cyclesWithRequests = await _requestRepository.GetCyclesWithUserRequestsAsync(userId, cycleIds, cancellationToken);

// Filter cycles that have user requests
var filteredCycles = cycles.Where(c => cyclesWithRequests.Contains(c.Id));
```

### 3. Get Request Details for Multiple Cycles
```csharp
var cycleIds = cycles.Select(c => c.Id).ToList();
var userRequests = await _requestRepository.GetUserRequestsForCyclesAsync(userId, cycleIds, cancellationToken);

// Process each cycle with its request
foreach (var cycle in cycles)
{
    if (userRequests.TryGetValue(cycle.Id, out var request))
    {
        // Process cycle with request details
    }
}
```

## Benefits Summary

### ✅ **Performance Improvements**
- **95-99% reduction** in data transfer for most scenarios
- **Database-level filtering** instead of application-level filtering
- **Optimized queries** using proper indexes
- **Minimal memory usage** with targeted data loading

### ✅ **Scalability Benefits**
- Performance remains consistent regardless of total user requests
- Efficient handling of users with hundreds or thousands of requests
- Better resource utilization under high load

### ✅ **Code Quality**
- **Clean separation of concerns** with specialized repository methods
- **Reusable methods** for different use cases
- **Maintainable code** with clear method purposes
- **Type safety** with proper return types

### ✅ **User Experience**
- **Faster response times** for cycle queries
- **Better performance** on mobile devices with limited bandwidth
- **Consistent performance** regardless of user's request history

This optimization ensures that checking if a user has requests for specific cycles is performed efficiently at the database level, providing significant performance improvements while maintaining clean, maintainable code architecture.