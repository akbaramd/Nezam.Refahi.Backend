# Enhanced Facility Endpoints - Complete API Coverage

## Overview

I have successfully updated the FacilityEndpoints.cs to support all the enhanced queries we've implemented, including the new optimized methods and comprehensive filtering options.

## Enhanced Endpoints

### 1. **Get Facility Cycles** - `/api/v1/facilities/{facilityId}/cycles`

**Enhanced with new filtering options:**
```http
GET /api/v1/facilities/{facilityId}/cycles
```

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Page size
- `status` (string, optional) - Filter by cycle status
- `onlyActive` (bool, default: true) - Show only active cycles
- `onlyEligible` (bool, default: false) - **NEW**: Show only cycles user is eligible for
- `onlyWithUserRequests` (bool, default: false) - **NEW**: Show only cycles where user has requests
- `includeUserRequestStatus` (bool, default: true) - Include user request status
- `includeDetailedRequestInfo` (bool, default: false) - **NEW**: Include comprehensive request details
- `includeStatistics` (bool, default: true) - Include cycle statistics

**Response:** `ApplicationResult<GetFacilityCyclesResponse>`

### 2. **Get User Cycles With Requests** - `/api/v1/facilities/user/cycles-with-requests`

**NEW ENDPOINT** - Specialized for showing cycles where user has requests:
```http
GET /api/v1/facilities/user/cycles-with-requests
```

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Page size
- `facilityId` (Guid, optional) - Filter by specific facility
- `requestStatus` (string, optional) - Filter by request status
- `requestStatusCategory` (int, optional) - Filter by status category (0=All, 1=InProgress, 2=Completed, 3=Rejected, 4=Cancelled, 5=Terminal)
- `onlyActive` (bool, default: true) - Show only active cycles
- `includeDetailedRequestInfo` (bool, default: true) - Include comprehensive request details
- `includeFacilityInfo` (bool, default: true) - Include facility information
- `includeStatistics` (bool, default: true) - Include summary statistics

**Response:** `ApplicationResult<GetUserCyclesWithRequestsResponse>`

### 3. **Get User Cycle Requests** - `/api/v1/facilities/user/cycle-requests`

**Enhanced existing endpoint:**
```http
GET /api/v1/facilities/user/cycle-requests
```

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Page size
- `facilityId` (Guid, optional) - Filter by facility
- `facilityCycleId` (Guid, optional) - Filter by specific cycle
- `status` (string, optional) - Filter by request status
- `statusCategory` (int, optional) - Filter by status category
- `dateFrom` (DateTime, optional) - Filter from date
- `dateTo` (DateTime, optional) - Filter to date
- `includeFacilityInfo` (bool, default: true) - Include facility details
- `includeCycleInfo` (bool, default: true) - Include cycle details
- `includeTimeline` (bool, default: true) - Include request timeline

**Response:** `ApplicationResult<GetUserCycleRequestsResponse>`

### 4. **Check User Request for Cycle** - `/api/v1/facilities/cycles/{cycleId}/user-request`

**NEW ENDPOINT** - Quick check for user request on specific cycle:
```http
GET /api/v1/facilities/cycles/{cycleId}/user-request
```

**Path Parameters:**
- `cycleId` (Guid) - Cycle ID

**Response:** `ApplicationResult<GetFacilityCycleDetailsResponse>` (optimized for request checking)

### 5. **Get Facility Cycle Details** - `/api/v1/facilities/cycles/{cycleId}`

**Existing endpoint** (already optimized):
```http
GET /api/v1/facilities/cycles/{cycleId}
```

**Query Parameters:**
- `includeFacilityInfo` (bool, default: true)
- `includeUserRequestHistory` (bool, default: true) - **OPTIMIZED**: Uses new repository method
- `includeEligibilityDetails` (bool, default: true)
- `includeDependencies` (bool, default: true)
- `includeStatistics` (bool, default: true)

**Response:** `ApplicationResult<GetFacilityCycleDetailsResponse>`

## API Usage Examples

### 1. Get Cycles Where User Has Requests
```http
GET /api/v1/facilities/{facilityId}/cycles?onlyWithUserRequests=true&includeDetailedRequestInfo=true
```

### 2. Get User's Cycles With Requests (Specialized)
```http
GET /api/v1/facilities/user/cycles-with-requests?requestStatusCategory=1&includeDetailedRequestInfo=true
```

### 3. Get User's In-Progress Requests
```http
GET /api/v1/facilities/user/cycle-requests?statusCategory=1&includeFacilityInfo=true
```

### 4. Check Specific Cycle Request
```http
GET /api/v1/facilities/cycles/{cycleId}/user-request
```

### 5. Get Eligible Cycles Only
```http
GET /api/v1/facilities/{facilityId}/cycles?onlyEligible=true&includeUserRequestStatus=true
```

## Response Structure Examples

### Enhanced GetFacilityCyclesResponse
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
    "summary": {
      "totalCycles": 1,
      "activeCycles": 1,
      "eligibleCycles": 1,
      "requestedCycles": 1
    }
  }
}
```

### New GetUserCyclesWithRequestsResponse
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

### ✅ **Comprehensive API Coverage**
- **All query types** supported through dedicated endpoints
- **Flexible filtering** options for different use cases
- **Optimized performance** with database-level filtering

### ✅ **Enhanced User Experience**
- **Specialized endpoints** for specific scenarios
- **Rich request details** with timeline and status information
- **Persian-formatted** amounts and status descriptions

### ✅ **Performance Optimized**
- **Database-level filtering** instead of application-level
- **Minimal data transfer** with targeted queries
- **Efficient pagination** and statistics

### ✅ **Developer Friendly**
- **Clear endpoint purposes** with descriptive names
- **Comprehensive query parameters** for flexibility
- **Consistent response structure** across all endpoints
- **Proper HTTP status codes** and error handling

### ✅ **Scalable Architecture**
- **Clean separation** of concerns
- **Reusable components** across endpoints
- **Maintainable code** with proper validation
- **Future-extensible** design

## Status Category Enums

### RequestStatusCategory (GetUserCycleRequests)
- `0` - All
- `1` - InProgress
- `2` - Completed  
- `3` - Rejected
- `4` - Cancelled
- `5` - Terminal

### RequestStatusCategory (GetUserCyclesWithRequests)
- `0` - All
- `1` - InProgress
- `2` - Completed
- `3` - Rejected
- `4` - Cancelled
- `5` - Terminal

This comprehensive API coverage ensures that all user scenarios for checking cycles with requests are efficiently supported through optimized, well-structured endpoints.
