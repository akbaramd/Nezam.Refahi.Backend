# Enhanced Cycle Queries with User Request Details

## Overview

I have successfully enhanced the Facilities module to show cycles where the current user has requests, along with detailed request information and status. This implementation follows DDD principles and provides comprehensive user context for cycle queries.

## Key Enhancements

### 1. Enhanced GetFacilityCyclesQuery

**New Features Added:**
- `OnlyWithUserRequests`: Filter to show only cycles where user has requests
- `IncludeDetailedRequestInfo`: Include comprehensive request details in response

**Enhanced UserRequestStatusDto:**
- Added detailed request timeline information
- Added formatted amounts (requested/approved)
- Added request processing dates (approval, rejection)
- Added rejection reasons
- Added days since request creation
- Added request status flags (in progress, completed, rejected, cancelled)

### 2. New Specialized Query: GetUserCyclesWithRequestsQuery

**Purpose:** Dedicated query specifically for showing cycles where the user has requests with comprehensive request details.

**Key Features:**
- Focuses exclusively on cycles where user has requests
- Provides detailed request information for each cycle
- Supports filtering by request status and status categories
- Includes comprehensive statistics and summaries
- Supports pagination and facility filtering

## Implementation Details

### Enhanced DTOs

#### UserRequestStatusDto (Enhanced)
```csharp
public record UserRequestStatusDto
{
    // Basic request info
    public bool HasRequested { get; init; }
    public Guid? RequestId { get; init; }
    public string? RequestStatus { get; init; }
    public string? RequestStatusDescription { get; init; }
    
    // Enhanced details
    public DateTime? RequestCreatedAt { get; init; }
    public decimal? RequestedAmountRials { get; init; }
    public decimal? ApprovedAmountRials { get; init; }
    public string? FormattedRequestedAmount { get; init; }
    public string? FormattedApprovedAmount { get; init; }
    
    // Timeline information
    public DateTime? RequestApprovedAt { get; init; }
    public DateTime? RequestRejectedAt { get; init; }
    public string? RequestRejectionReason { get; init; }
    public int? DaysSinceRequestCreated { get; init; }
    
    // Status flags
    public bool IsRequestInProgress { get; init; }
    public bool IsRequestCompleted { get; init; }
    public bool IsRequestRejected { get; init; }
    public bool IsRequestCancelled { get; init; }
    
    // Complete timeline
    public RequestTimelineDto? RequestTimeline { get; init; }
}
```

#### New UserCycleWithRequestDto
```csharp
public record UserCycleWithRequestDto
{
    // Cycle information
    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; }
    public bool IsAcceptingApplications { get; init; }
    
    // Quota information
    public int Quota { get; init; }
    public int UsedQuota { get; init; }
    public int AvailableQuota { get; init; }
    public decimal QuotaUtilizationPercentage { get; init; }
    
    // Associated entities
    public FacilityInfoDto? Facility { get; init; }
    public UserRequestDetailsDto UserRequest { get; init; }
}
```

### Query Usage Examples

#### 1. Get Cycles with User Request Status (Enhanced)
```csharp
var query = new GetFacilityCyclesQuery
{
    FacilityId = facilityId,
    NationalNumber = "1234567890",
    OnlyWithUserRequests = true,  // NEW: Show only cycles with user requests
    IncludeDetailedRequestInfo = true,  // NEW: Include comprehensive request details
    IncludeUserRequestStatus = true,
    Page = 1,
    PageSize = 10
};

var result = await mediator.Send(query);
```

#### 2. Get User Cycles with Requests (New Specialized Query)
```csharp
var query = new GetUserCyclesWithRequestsQuery
{
    NationalNumber = "1234567890",
    FacilityId = facilityId,  // Optional: filter by facility
    RequestStatusCategory = RequestStatusCategory.InProgress,  // Filter by status category
    IncludeDetailedRequestInfo = true,
    IncludeFacilityInfo = true,
    Page = 1,
    PageSize = 10
};

var result = await mediator.Send(query);
```

### Response Structure

#### Enhanced GetFacilityCyclesResponse
```json
{
  "isSuccess": true,
  "data": {
    "facility": { /* facility info */ },
    "userInfo": { /* user info */ },
    "cycles": [
      {
        "id": "cycle-id",
        "name": "Cycle Name",
        "isActive": true,
        "quota": 100,
        "usedQuota": 50,
        "userRequestStatus": {
          "hasRequested": true,
          "requestId": "request-id",
          "requestStatus": "UnderReview",
          "requestStatusDescription": "در حال بررسی",
          "requestedAmountRials": 50000000,
          "approvedAmountRials": null,
          "formattedRequestedAmount": "50,000,000 ریال",
          "daysSinceRequestCreated": 5,
          "isRequestInProgress": true,
          "requestTimeline": {
            "createdAt": "2024-01-01T00:00:00Z",
            "daysSinceCreated": 5,
            "processingTimeDays": null
          }
        }
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 10,
    "totalPages": 1,
    "summary": { /* statistics */ }
  }
}
```

#### New GetUserCyclesWithRequestsResponse
```json
{
  "isSuccess": true,
  "data": {
    "userInfo": { /* user info */ },
    "cycles": [
      {
        "id": "cycle-id",
        "name": "Cycle Name",
        "facility": { /* facility details */ },
        "userRequest": {
          "requestId": "request-id",
          "status": "UnderReview",
          "statusDescription": "در حال بررسی",
          "requestedAmountRials": 50000000,
          "approvedAmountRials": null,
          "formattedRequestedAmount": "50,000,000 ریال",
          "createdAt": "2024-01-01T00:00:00Z",
          "daysSinceCreated": 5,
          "isInProgress": true,
          "timeline": { /* complete timeline */ }
        }
      }
    ],
    "totalCount": 1,
    "summary": {
      "totalCycles": 1,
      "cyclesWithInProgressRequests": 1,
      "totalRequestedAmountRials": 50000000,
      "formattedTotalRequestedAmount": "50,000,000 ریال"
    }
  }
}
```

## Key Benefits

### 1. **Enhanced User Experience**
- Users can see exactly which cycles they have requests for
- Detailed request status and timeline information
- Formatted amounts for better readability
- Clear status descriptions in Persian

### 2. **Comprehensive Request Information**
- Complete request timeline (creation, approval, rejection dates)
- Processing time calculations
- Bank appointment scheduling information
- Rejection reasons when applicable

### 3. **Flexible Filtering**
- Filter by request status (InProgress, Completed, Rejected, etc.)
- Filter by status categories
- Filter by facility
- Support for pagination

### 4. **Rich Statistics**
- Summary of user's request history
- Total amounts requested/approved
- Count of different facilities and cycles
- Timeline information (oldest/most recent requests)

### 5. **Clean Architecture Compliance**
- Follows DDD principles
- Proper separation of concerns
- Comprehensive validation
- Structured error handling
- Persian error messages

## Usage Scenarios

### Scenario 1: User Dashboard
Show user all cycles where they have requests with current status:
```csharp
var query = new GetUserCyclesWithRequestsQuery
{
    NationalNumber = userNationalNumber,
    RequestStatusCategory = RequestStatusCategory.InProgress
};
```

### Scenario 2: Facility-Specific Requests
Show user's requests for a specific facility:
```csharp
var query = new GetUserCyclesWithRequestsQuery
{
    NationalNumber = userNationalNumber,
    FacilityId = specificFacilityId,
    IncludeFacilityInfo = true
};
```

### Scenario 3: Enhanced Cycle Listing
Show cycles with user context (existing query enhanced):
```csharp
var query = new GetFacilityCyclesQuery
{
    FacilityId = facilityId,
    NationalNumber = userNationalNumber,
    OnlyWithUserRequests = true,
    IncludeDetailedRequestInfo = true
};
```

## Technical Implementation

### 1. **Repository Integration**
- Leverages existing `IFacilityRequestRepository`
- Efficient querying with proper filtering
- Minimal database calls with optimized data loading

### 2. **Domain Service Integration**
- Uses existing `EnumTextMappingService` for status descriptions
- Maintains consistency with existing domain logic
- Proper error handling and validation

### 3. **Performance Considerations**
- Efficient pagination implementation
- Minimal data loading (only required fields)
- Optimized filtering at database level
- Proper async/await patterns

### 4. **Validation and Error Handling**
- Comprehensive input validation
- Persian error messages
- Proper exception handling
- Structured logging

This implementation provides a complete solution for showing cycles where users have requests, with detailed request information and status, following clean architecture principles and providing excellent user experience.
