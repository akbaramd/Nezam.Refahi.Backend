# Facility Endpoints Comprehensive Documentation

## Overview
This document provides complete documentation for all Facility API endpoints and their associated Data Transfer Objects (DTOs). The Facility module manages various types of facilities (loans, grants, cards, welfare vouchers) with their cycles, requests, and approval workflows.

## Base URL
All endpoints are prefixed with: `/api/v1/facilities`

## Authentication
All endpoints require authorization via JWT token in the Authorization header.

---

## Endpoints Documentation

### 1. Get Facilities List
**Endpoint:** `GET /api/v1/facilities`

**Description:** Retrieves a paginated list of facilities with optional filtering and search capabilities.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Number of items per page (default: 10)
- `type` (string, optional): Filter by facility type
- `status` (string, optional): Filter by facility status
- `searchTerm` (string, optional): Free-text search in facility names/descriptions
- `onlyActive` (bool, optional): Include only active facilities (default: true)

**Response:** `ApplicationResult<GetFacilitiesResult>`
- **Success (200):** Returns paginated facility list
- **Error (400):** Bad request with error details

**Response DTO:** `GetFacilitiesResult` (inherits from `PaginatedResult<FacilityDto>`)

---

### 2. Get Facility Details
**Endpoint:** `GET /api/v1/facilities/{facilityId:guid}`

**Description:** Retrieves detailed information about a specific facility including cycles, features, and policies.

**Path Parameters:**
- `facilityId` (Guid): Unique facility identifier

**Query Parameters:**
- `includeCycles` (bool, optional): Include facility cycles (default: true)
- `includeFeatures` (bool, optional): Include facility features (default: true)
- `includePolicies` (bool, optional): Include capability policies (default: true)
- `includeUserRequestHistory` (bool, optional): Include user's request history (default: false)

**Response:** `ApplicationResult<FacilityDetailsDto>`
- **Success (200):** Returns detailed facility information
- **Not Found (404):** Facility not found
- **Error (400):** Bad request with error details

---

### 3. Get Facility Cycles
**Endpoint:** `GET /api/v1/facilities/{facilityId:guid}/cycles`

**Description:** Retrieves cycles for a specific facility with user context and eligibility information.

**Path Parameters:**
- `facilityId` (Guid): Unique facility identifier

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Number of items per page (default: 10)
- `status` (string, optional): Filter by cycle status
- `searchTerm` (string, optional): Free-text search in cycle names/descriptions
- `onlyActive` (bool, optional): Include only active cycles (default: true)
- `onlyEligible` (bool, optional): Include only cycles user is eligible for (default: false)
- `onlyWithUserRequests` (bool, optional): Include only cycles where user has requests (default: false)
- `includeUserRequestStatus` (bool, optional): Include user's request status for each cycle (default: true)
- `includeDetailedRequestInfo` (bool, optional): Include detailed request information (default: false)
- `includeStatistics` (bool, optional): Include cycle statistics (default: true)

**Response:** `ApplicationResult<GetFacilityCyclesWithUserQueryResponse>`
- **Success (200):** Returns paginated cycles with user context
- **Not Found (404):** Facility not found
- **Error (400):** Bad request with error details

---

### 4. Get Facility Cycle Details
**Endpoint:** `GET /api/v1/facilities/cycles/{cycleId:guid}`

**Description:** Retrieves detailed information about a specific facility cycle including dependencies, statistics, and user request history.

**Path Parameters:**
- `cycleId` (Guid): Unique cycle identifier

**Query Parameters:**
- `includeFacilityInfo` (bool, optional): Include facility information (default: true)
- `includeUserRequestHistory` (bool, optional): Include user's request history (default: true)
- `includeEligibilityDetails` (bool, optional): Include eligibility details (default: true)
- `includeDependencies` (bool, optional): Include cycle dependencies (default: true)
- `includeStatistics` (bool, optional): Include cycle statistics (default: true)

**Response:** `ApplicationResult<FacilityCycleWithUserDetailDto>`
- **Success (200):** Returns detailed cycle information
- **Not Found (404):** Cycle not found
- **Error (400):** Bad request with error details

---

### 5. Get Facility Requests
**Endpoint:** `GET /api/v1/facilities/requests`

**Description:** Retrieves facility requests for the current user with optional filtering.

**Query Parameters:**
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Number of items per page (default: 10)
- `facilityId` (Guid, optional): Filter by specific facility
- `facilityCycleId` (Guid, optional): Filter by specific cycle
- `status` (string, optional): Filter by request status
- `searchTerm` (string, optional): Free-text search
- `dateFrom` (DateTime, optional): Filter requests from date
- `dateTo` (DateTime, optional): Filter requests to date

**Response:** `ApplicationResult<GetFacilityRequestsByUserQueryResult>`
- **Success (200):** Returns paginated user requests
- **Error (400):** Bad request with error details

---

### 6. Get Facility Request Details
**Endpoint:** `GET /api/v1/facilities/requests/{requestId:guid}`

**Description:** Retrieves detailed information about a specific facility request.

**Path Parameters:**
- `requestId` (Guid): Unique request identifier

**Query Parameters:**
- `includeFacility` (bool, optional): Include facility information (default: true)
- `includeCycle` (bool, optional): Include cycle information (default: true)
- `includePolicySnapshot` (bool, optional): Include policy snapshot (default: true)

**Response:** `ApplicationResult<FacilityRequestDetailsDto>`
- **Success (200):** Returns detailed request information
- **Not Found (404):** Request not found
- **Error (400):** Bad request with error details

---

### 7. Create Facility Request
**Endpoint:** `POST /api/v1/facilities/requests`

**Description:** Creates a new facility request for the current user.

**Request Body:** `CreateFacilityRequestCommand`

**Response:** `ApplicationResult<CreateFacilityRequestResult>`
- **Success (201):** Request created successfully
- **Error (400):** Bad request with error details

---

### 8. Approve Facility Request
**Endpoint:** `POST /api/v1/facilities/requests/{requestId:guid}/approve`

**Description:** Approves a facility request with specified amount and notes.

**Path Parameters:**
- `requestId` (Guid): Unique request identifier

**Request Body:** `ApproveFacilityRequestRequest`

**Response:** `ApplicationResult<ApproveFacilityRequestResult>`
- **Success (200):** Request approved successfully
- **Not Found (404):** Request not found
- **Error (400):** Bad request with error details

---

### 9. Reject Facility Request
**Endpoint:** `POST /api/v1/facilities/requests/{requestId:guid}/reject`

**Description:** Rejects a facility request with specified reason.

**Path Parameters:**
- `requestId` (Guid): Unique request identifier

**Request Body:** `RejectFacilityRequestRequest`

**Response:** `ApplicationResult<RejectFacilityRequestResult>`
- **Success (200):** Request rejected successfully
- **Not Found (404):** Request not found
- **Error (400):** Bad request with error details

---

### 10. Cancel Facility Request
**Endpoint:** `POST /api/v1/facilities/requests/{requestId:guid}/cancel`

**Description:** Cancels a facility request with optional reason.

**Path Parameters:**
- `requestId` (Guid): Unique request identifier

**Request Body:** `CancelFacilityRequestRequest`

**Response:** `ApplicationResult<CancelFacilityRequestResult>`
- **Success (200):** Request cancelled successfully
- **Not Found (404):** Request not found
- **Error (400):** Bad request with error details

---

## Data Transfer Objects (DTOs) Documentation

### Core Facility DTOs

#### FacilityDto
**Purpose:** Enterprise-grade facility data transfer object providing comprehensive facility information.

**Properties:**
- `Id` (Guid): Unique facility identifier
- `Name` (string): Facility display name
- `Code` (string): Unique facility code for system identification
- `Type` (string): Facility type (Loan, Grant, Card, WelfareVoucher, Other)
- `TypeText` (string): Human-readable facility type text
- `Status` (string): Current facility status (Draft, Active, Suspended, Closed, Maintenance)
- `StatusText` (string): Human-readable status text
- `IsActive` (bool): Indicates if facility is currently active and accepting applications
- `Description` (string?): Detailed facility description
- `BankInfo` (BankInfoDto?): Associated bank information
- `CycleStatistics` (FacilityCycleStatisticsDto?): Current cycle statistics
- `Metadata` (Dictionary<string, string>): Facility metadata and additional properties
- `CreatedAt` (DateTime): Facility creation timestamp
- `LastModifiedAt` (DateTime?): Last modification timestamp

#### FacilityDetailsDto
**Purpose:** Detailed facility data transfer object containing full facility information including all details.

**Inherits from:** `FacilityDto`

**Additional Properties:**
- `BankName` (string?): Bank name
- `BankCode` (string?): Bank code
- `BankAccountNumber` (string?): Bank account number
- `Cycles` (List<FacilityCycleWithUserDto>): Facility cycles
- `Features` (List<FacilityFeatureDto>): Facility features
- `CapabilityPolicies` (List<FacilityCapabilityPolicyDto>): Capability policies

#### FacilityInfoDto
**Purpose:** Facility information data transfer object for simplified facility references.

**Properties:**
- `Id` (Guid): Facility ID
- `Name` (string): Facility name
- `Code` (string): Facility code
- `Type` (string): Facility type
- `TypeText` (string): Human-readable facility type text
- `Status` (string): Facility status
- `StatusText` (string): Human-readable status text
- `Description` (string?): Facility description
- `BankInfo` (BankInfoDto?): Bank information
- `IsActive` (bool): Indicates if facility is active

### Cycle DTOs

#### FacilityCycleDto
**Purpose:** Simple facility cycle data transfer object for lists containing only essential fields.

**Properties:**
- `Id` (Guid): Unique cycle identifier
- `Name` (string): Cycle display name
- `StartDate` (DateTime): Cycle start date
- `EndDate` (DateTime): Cycle end date
- `DaysUntilEnd` (int): Days remaining until cycle ends
- `IsActive` (bool): Indicates if cycle is currently active
- `Quota` (int): Total quota for this cycle
- `UsedQuota` (int): Used quota count
- `AvailableQuota` (int): Available quota count
- `Status` (string): Cycle status (Draft, Active, Closed, Completed, Cancelled)
- `StatusText` (string): Human-readable status text
- `MinAmountRials` (decimal): Minimum amount for this cycle
- `MaxAmountRials` (decimal): Maximum amount for this cycle
- `PaymentMonths` (int): Number of payment months
- `CooldownDays` (int): Cooldown period in days
- `CreatedAt` (DateTime): Cycle creation timestamp

#### FacilityCycleWithUserDto
**Purpose:** Facility cycle with user context data transfer object.

**Properties:**
- `Id` (Guid): Cycle ID
- `Name` (string): Cycle name
- `StartDate` (DateTime): Cycle start date
- `EndDate` (DateTime): Cycle end date
- `DaysUntilStart` (int): Days remaining until cycle starts
- `DaysUntilEnd` (int): Days remaining until cycle ends
- `HasStarted` (bool): Indicates if cycle has started
- `HasEnded` (bool): Indicates if cycle has ended
- `IsActive` (bool): Indicates if cycle is currently active
- `IsAcceptingApplications` (bool): Indicates if cycle is accepting applications
- `Quota` (int): Total quota for this cycle
- `UsedQuota` (int): Used quota count
- `AvailableQuota` (int): Available quota count
- `QuotaUtilizationPercentage` (decimal): Quota utilization percentage
- `Status` (string): Cycle status
- `StatusText` (string): Human-readable status description
- `Description` (string?): Cycle description

#### FacilityCycleWithUserDetailDto
**Purpose:** Detailed facility cycle data transfer object containing full cycle information including all details.

**Inherits from:** `FacilityCycleWithUserDto`

**Additional Properties:**
- `Dependencies` (List<FacilityCycleDependencyDto>): Cycle dependencies
- `AdmissionStrategy` (string): Admission strategy (FIFO, Score, Lottery)
- `AdmissionStrategyDescription` (string): Human-readable admission strategy description
- `WaitlistCapacity` (int?): Waitlist capacity
- `Metadata` (Dictionary<string, string>): Cycle metadata
- `LastModifiedAt` (DateTime?): Last modification timestamp
- `Statistics` (CycleStatisticsDto): Cycle statistics
- `UserRequestHistory` (List<UserRequestHistoryDto>): User request history for this cycle

### Request DTOs

#### FacilityRequestDto
**Purpose:** Simple facility request data transfer object for lists containing only essential fields.

**Properties:**
- `Id` (Guid): Unique request identifier
- `Facility` (FacilityInfoDto): Associated facility information
- `Cycle` (FacilityCycleWithUserDto): Associated cycle information
- `Applicant` (ApplicantInfoDto): Applicant information
- `RequestedAmountRials` (decimal): Requested amount in Rials
- `ApprovedAmountRials` (decimal?): Approved amount in Rials
- `Currency` (string): Currency code (default: "IRR")
- `Status` (string): Request status
- `StatusText` (string): Human-readable status text
- `CreatedAt` (DateTime): Request creation timestamp
- `ApprovedAt` (DateTime?): Approval timestamp
- `RejectedAt` (DateTime?): Rejection timestamp
- `RejectionReason` (string?): Rejection reason
- `DaysSinceCreated` (int): Days since request was created
- `IsInProgress` (bool): Indicates if request is in progress
- `IsCompleted` (bool): Indicates if request is completed
- `IsRejected` (bool): Indicates if request is rejected

#### FacilityRequestDetailsDto
**Purpose:** Detailed facility request data transfer object containing full request information.

**Properties:** (Refer to search results for complete definition)

### Command Result DTOs

#### CreateFacilityRequestResult
**Purpose:** Response for facility request creation.

**Properties:**
- `RequestId` (Guid): Created facility request ID
- `RequestNumber` (string): Generated request number
- `Status` (string): Request status
- `RequestedAmountRials` (decimal): Requested amount
- `Currency` (string): Currency
- `CreatedAt` (DateTime): Creation timestamp

#### ApproveFacilityRequestResult
**Purpose:** Response for facility request approval.

**Properties:**
- `RequestId` (Guid): Approved facility request ID
- `RequestNumber` (string): Request number
- `Status` (string): New status after approval
- `ApprovedAmountRials` (decimal): Approved amount
- `Currency` (string): Currency
- `ApprovedAt` (DateTime): Approval timestamp
- `ApproverUserId` (Guid): Approver user ID

#### RejectFacilityRequestResult
**Purpose:** Response for facility request rejection.

**Properties:**
- `RequestId` (Guid): Rejected facility request ID
- `RequestNumber` (string): Request number
- `Status` (string): New status after rejection
- `Reason` (string): Rejection reason
- `RejectedAt` (DateTime): Rejection timestamp
- `RejectorUserId` (Guid): Rejector user ID
- `RejectionId` (Guid): Rejection record ID
- `RejectionType` (string): Rejection type
- `Details` (string?): Additional rejection details
- `Notes` (string?): Additional notes

#### CancelFacilityRequestResult
**Purpose:** Response for facility request cancellation.

**Properties:**
- `RequestId` (Guid): Cancelled facility request ID
- `RequestNumber` (string): Request number
- `Status` (string): New status after cancellation
- `Reason` (string?): Cancellation reason (if provided)
- `CancelledAt` (DateTime): Cancellation timestamp
- `CancelledByUserId` (Guid): User ID who cancelled the request

### Supporting DTOs

#### BankInfoDto
**Purpose:** Bank information data transfer object.

**Properties:**
- `BankName` (string?): Bank name
- `BankCode` (string?): Bank code
- `BankAccountNumber` (string?): Bank account number
- `IsAvailable` (bool): Indicates if bank information is available (computed property)

#### ApplicantInfoDto
**Purpose:** Applicant information data transfer object.

**Properties:**
- `MemberId` (Guid): Member ID
- `FullName` (string?): User full name
- `NationalId` (string?): User national ID
- `IsComplete` (bool): Indicates if applicant information is complete (computed property)

#### CycleStatisticsDto
**Purpose:** Cycle statistics data transfer object.

**Properties:**
- `TotalQuota` (int): Total quota
- `UsedQuota` (int): Used quota
- `AvailableQuota` (int): Available quota
- `UtilizationPercentage` (decimal): Quota utilization percentage
- `PendingRequests` (int): Number of pending requests
- `ApprovedRequests` (int): Number of approved requests
- `RejectedRequests` (int): Number of rejected requests
- `AverageProcessingTimeDays` (decimal?): Average processing time in days
- `CycleDurationDays` (int): Cycle duration in days
- `DaysElapsed` (int): Days elapsed since cycle start
- `DaysRemaining` (int): Days remaining until cycle end
- `CycleProgressPercentage` (decimal): Cycle progress percentage

#### FacilityCycleStatisticsDto
**Purpose:** Facility cycle statistics data transfer object.

**Properties:**
- `ActiveCyclesCount` (int): Number of active cycles
- `TotalCyclesCount` (int): Total number of cycles
- `DraftCyclesCount` (int): Number of draft cycles
- `ClosedCyclesCount` (int): Number of closed cycles
- `CompletedCyclesCount` (int): Number of completed cycles
- `CancelledCyclesCount` (int): Number of cancelled cycles
- `TotalActiveQuota` (int): Total active quota across all active cycles
- `TotalUsedQuota` (int): Total used quota across all active cycles
- `TotalAvailableQuota` (int): Total available quota across all active cycles
- `QuotaUtilizationPercentage` (decimal): Overall quota utilization percentage

#### FacilityFeatureDto
**Purpose:** Facility feature data transfer object.

**Properties:**
- `Id` (Guid): Feature ID
- `FeatureId` (string): Feature ID
- `RequirementType` (string): Requirement type
- `Notes` (string?): Notes
- `AssignedAt` (DateTime): Assigned date

#### FacilityCapabilityPolicyDto
**Purpose:** Facility capability policy data transfer object.

**Properties:**
- `Id` (Guid): Policy ID
- `CapabilityId` (string): Capability ID

#### FacilityCycleDependencyDto
**Purpose:** Facility cycle dependency data transfer object.

**Properties:**
- `Id` (Guid): Dependency ID
- `RequiredFacilityId` (Guid): Required facility ID
- `RequiredFacilityName` (string): Required facility name
- `MustBeCompleted` (bool): Must be completed
- `CreatedAt` (DateTime): Creation date

#### UserRequestHistoryDto
**Purpose:** User request history data transfer object.

**Properties:**
- `RequestId` (Guid): Request ID
- `Status` (string): Request status
- `StatusText` (string): Human-readable status text
- `RequestedAmountRials` (decimal): Requested amount in Rials
- `ApprovedAmountRials` (decimal?): Approved amount in Rials (if different)
- `CreatedAt` (DateTime): Request creation date
- `ApprovedAt` (DateTime?): Request approval date (if approved)
- `RejectedAt` (DateTime?): Request rejection date (if rejected)
- `RejectionReason` (string?): Rejection reason (if rejected)
- `DaysSinceCreated` (int): Days since request creation
- `IsInProgress` (bool): Indicates if request is in progress
- `IsCompleted` (bool): Indicates if request is completed
- `IsRejected` (bool): Indicates if request is rejected
- `IsCancelled` (bool): Indicates if request is cancelled

### Request DTOs (API Input)

#### ApproveFacilityRequestRequest
**Purpose:** Request DTO for approving facility requests.

**Properties:**
- `ApprovedAmountRials` (decimal): Approved amount in Rials
- `Currency` (string): Currency code (default: "IRR")
- `Notes` (string?): Additional notes
- `ApproverUserId` (Guid): Approver user ID

#### RejectFacilityRequestRequest
**Purpose:** Request DTO for rejecting facility requests.

**Properties:**
- `Reason` (string): Rejection reason (required)
- `RejectorUserId` (Guid): Rejector user ID

#### CancelFacilityRequestRequest
**Purpose:** Request DTO for cancelling facility requests.

**Properties:**
- `Reason` (string?): Cancellation reason (optional)
- `CancelledByUserId` (Guid): User ID who is cancelling the request

---

## Response Wrapper

All API responses are wrapped in `ApplicationResult<T>` which provides:
- `IsSuccess` (bool): Indicates if the operation was successful
- `Data` (T): The actual response data
- `Errors` (List<string>): List of error messages if operation failed
- `Message` (string?): Additional message or description

---

## Status Codes and Enums

### Facility Types
- `Loan`: Loan facility
- `Grant`: Grant facility
- `Card`: Card facility
- `WelfareVoucher`: Welfare voucher facility
- `Other`: Other type of facility

### Facility Statuses
- `Draft`: Facility is in draft state
- `Active`: Facility is active and accepting applications
- `Suspended`: Facility is temporarily suspended
- `Closed`: Facility is closed
- `Maintenance`: Facility is under maintenance

### Cycle Statuses
- `Draft`: Cycle is in draft state
- `Active`: Cycle is active and accepting applications
- `Closed`: Cycle is closed
- `Completed`: Cycle has completed
- `Cancelled`: Cycle has been cancelled

### Request Statuses
- `Pending`: Request is pending approval
- `Approved`: Request has been approved
- `Rejected`: Request has been rejected
- `Cancelled`: Request has been cancelled
- `InProgress`: Request is being processed

### Admission Strategies
- `FIFO`: First In, First Out
- `Score`: Based on scoring system
- `Lottery`: Random selection

---

## Error Handling

The API uses standard HTTP status codes:
- `200 OK`: Successful operation
- `201 Created`: Resource created successfully
- `400 Bad Request`: Invalid request data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

All error responses include detailed error messages in the `ApplicationResult.Errors` collection.

---

## Pagination

Paginated endpoints use the `PaginatedResult<T>` wrapper which includes:
- `Items` (List<T>): The actual data items
- `TotalCount` (int): Total number of items
- `Page` (int): Current page number
- `PageSize` (int): Number of items per page
- `TotalPages` (int): Total number of pages
- `HasNextPage` (bool): Indicates if there's a next page
- `HasPreviousPage` (bool): Indicates if there's a previous page

---

## Notes for Client Implementation

1. **Authentication**: All endpoints require valid JWT token in Authorization header
2. **Content-Type**: Use `application/json` for request bodies
3. **Date Formats**: All dates are in ISO 8601 format
4. **Currency**: Default currency is IRR (Iranian Rial)
5. **Pagination**: Use page and pageSize parameters for paginated endpoints
6. **Filtering**: Use query parameters for filtering and searching
7. **Error Handling**: Always check `IsSuccess` property before accessing `Data`
8. **Null Safety**: Many properties are nullable - handle null values appropriately
9. **Computed Properties**: Some DTOs have computed properties (e.g., `IsAvailable`, `IsComplete`)
10. **Metadata**: Use `Metadata` dictionaries for additional custom properties

This documentation covers all endpoints and DTOs used in the Facility module. Each DTO is designed to provide comprehensive information while maintaining clean separation of concerns and following enterprise-grade patterns.
