# Facilities Module - Client DTOs Documentation

This document provides a comprehensive guide to all Data Transfer Objects (DTOs) used in the Facilities module for client applications.

## 📋 Table of Contents

1. [DTO Pattern Overview](#dto-pattern-overview)
2. [Core Entity DTOs](#core-entity-dtos)
3. [Supporting DTOs](#supporting-dtos)
4. [API Endpoint Mapping](#api-endpoint-mapping)
5. [Client Implementation Guide](#client-implementation-guide)

---

## 🏗️ DTO Pattern Overview

The Facilities module follows a **Simple vs Details** DTO pattern:

- **Simple DTOs**: Used for lists, pagination, and summaries (minimal fields)
- **Details DTOs**: Used for single entity views (comprehensive information)
- **Details DTOs inherit from Simple DTOs** to avoid duplication

### Pattern Benefits:
- ✅ **Performance**: Lists load faster with minimal data
- ✅ **Consistency**: Same base structure across simple/details
- ✅ **Maintainability**: Changes to base fields automatically propagate
- ✅ **Type Safety**: Client gets strongly typed interfaces

---

## 🎯 Core Entity DTOs

### 1. Facility DTOs

#### `FacilityDto` (Simple)
**Used for**: Facility lists, summaries, pagination
**Endpoints**: `GET /api/v1/facilities`

```typescript
interface FacilityDto {
  id: string;                    // Unique facility identifier (UUID)
  name: string;                  // Facility display name
  code: string;                  // Unique facility code (e.g., "TEJARAT-001")
  type: FacilityType;            // Facility type enum
  typeText: string;              // Human-readable type text (e.g., "تسهیلات تجارت")
  status: FacilityStatus;        // Current status enum
  statusText: string;            // Human-readable status text (e.g., "فعال")
  isActive: boolean;             // Is facility currently active (computed from status)
  description?: string;          // Detailed description
  bankInfo?: BankInfoDto;        // Associated bank information
  cycleStatistics?: FacilityCycleStatisticsDto; // Current cycle stats
  metadata: Record<string, string>; // Additional properties
  createdAt: string;            // Creation timestamp (ISO 8601)
  lastModifiedAt?: string;      // Last modification timestamp (ISO 8601)
  hasActiveCycles: boolean;      // Computed: has any active cycles
  isAcceptingApplications: boolean; // Computed: is accepting new apps
}
```

#### `FacilityDetailsDto` (Details)
**Used for**: Single facility view with full details
**Endpoints**: `GET /api/v1/facilities/{facilityId}`
**Inherits from**: `FacilityDto`

```typescript
interface FacilityDetailsDto extends FacilityDto {
  bankName?: string;             // Bank name
  bankCode?: string;             // Bank code
  bankAccountNumber?: string;     // Bank account number
  cycles: FacilityCycleDetailsDto[]; // All facility cycles
  features: FacilityFeatureDto[];    // Facility features
  capabilityPolicies: FacilityCapabilityPolicyDto[]; // Capability policies
}
```

### 2. Facility Cycle DTOs

#### `FacilityCycleDto` (Simple)
**Used for**: Cycle lists, summaries
**Endpoints**: `GET /api/v1/facilities/{facilityId}/cycles`

```typescript
interface FacilityCycleDto {
  id: string;                    // Unique cycle identifier (UUID)
  name: string;                  // Cycle display name
  startDate: string;             // Cycle start date (ISO 8601)
  endDate: string;               // Cycle end date (ISO 8601)
  daysUntilEnd: number;          // Days remaining until cycle ends (computed)
  isActive: boolean;             // Is cycle currently active (computed from status)
  quota: number;                 // Total quota for this cycle
  usedQuota: number;             // Used quota count
  availableQuota: number;        // Available quota count (computed: quota - usedQuota)
  status: CycleStatus;           // Cycle status enum
  statusText: string;            // Human-readable status text (e.g., "فعال")
  minAmountRials: number;        // Minimum amount in Rials
  maxAmountRials: number;        // Maximum amount in Rials
  paymentMonths: number;         // Number of payment months
  cooldownDays: number;          // Cooldown period in days
  createdAt: string;            // Creation timestamp (ISO 8601)
}
```

#### `FacilityCycleDetailsDto` (Details)
**Used for**: Single cycle view with full details
**Endpoints**: `GET /api/v1/facilities/cycles/{cycleId}`
**Inherits from**: `FacilityCycleDto`

```typescript
interface FacilityCycleDetailsDto extends FacilityCycleDto {
  daysUntilStart: number;        // Days until cycle starts
  hasStarted: boolean;           // Has cycle started
  hasEnded: boolean;             // Has cycle ended
  isAcceptingApplications: boolean; // Is accepting applications
  quotaUtilizationPercentage: number; // Quota utilization %
  description?: string;          // Cycle description
  financialTerms: DetailedFinancialTermsDto; // Financial terms
  rules: DetailedCycleRulesDto;  // Cycle rules and policies
  dependencies: FacilityCycleDependencyDto[]; // Cycle dependencies
  admissionStrategy: string;     // Admission strategy
  admissionStrategyDescription: string; // Human-readable strategy
  waitlistCapacity?: number;     // Waitlist capacity
  metadata: Record<string, string>; // Additional properties
  lastModifiedAt?: string;       // Last modification timestamp
  statistics: CycleStatisticsDto; // Cycle statistics
  userEligibility?: DetailedEligibilityDto; // User eligibility (if user context)
  userRequestHistory: UserRequestHistoryDto[]; // User request history
}
```

#### `FacilityCycleWithUserContextDto` (Specialized)
**Used for**: Cycles with user-specific context
**Endpoints**: `GET /api/v1/facilities/{facilityId}/cycles` (with user context)

```typescript
interface FacilityCycleWithUserContextDto {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  daysUntilStart: number;
  daysUntilEnd: number;
  hasStarted: boolean;
  hasEnded: boolean;
  isActive: boolean;
  isAcceptingApplications: boolean;
  quota: number;
  usedQuota: number;
  availableQuota: number;
  quotaUtilizationPercentage: number;
  status: string;
  statusDescription: string;
  description?: string;
  financialTerms: FinancialTermsDto;
  rules: CycleRulesDto;
  userEligibility?: UserEligibilityDto; // User-specific eligibility
  userRequestHistory: UserRequestHistoryDto[]; // User's request history
  lastRequest?: UserRequestHistoryDto; // Most recent request
  createdAt: string;
}
```

### 3. Facility Request DTOs

#### `FacilityRequestDto` (Simple)
**Used for**: Request lists, summaries
**Endpoints**: `GET /api/v1/facilities/requests`, `GET /api/v1/facilities/user/cycle-requests`

```typescript
interface FacilityRequestDto {
  id: string;                    // Unique request identifier
  facility: FacilityInfoDto;     // Associated facility information
  cycle: FacilityCycleDto;       // Associated cycle information
  applicant: ApplicantInfoDto;   // Applicant information
  requestedAmountRials: number;  // Requested amount in Rials
  approvedAmountRials?: number;  // Approved amount in Rials
  currency: string;              // Currency code (default: "IRR")
  status: string;                // Request status
  statusText: string;            // Human-readable status text
  createdAt: string;            // Request creation timestamp
  daysSinceCreated: number;      // Days since request creation
  isInProgress: boolean;        // Is request in progress
  isCompleted: boolean;          // Is request completed
  isRejected: boolean;           // Is request rejected
  isCancelled: boolean;         // Is request cancelled
}
```

#### `FacilityRequestDetailsDto` (Details)
**Used for**: Single request view with full details
**Endpoints**: `GET /api/v1/facilities/requests/{requestId}`
**Inherits from**: `FacilityRequestDto`

```typescript
interface FacilityRequestDetailsDto extends FacilityRequestDto {
  requestNumber: string;          // Request number
  description?: string;          // Request description
  rejectionReason?: string;      // Rejection reason (if rejected)
  financialInfo: RequestFinancialInfoDto; // Financial information
  statusDetails: RequestStatusDto; // Request status details
  timeline: RequestTimelineDto;  // Request timeline
  metadata: Record<string, string>; // Request metadata
  isTerminal: boolean;           // Is status terminal
  canBeCancelled: boolean;       // Can request be cancelled
  requiresApplicantAction: boolean; // Requires applicant action
  requiresBankAction: boolean;   // Requires bank action
  daysUntilBankAppointment?: number; // Days until bank appointment
  isBankAppointmentOverdue: boolean; // Is bank appointment overdue
  lastModifiedAt?: string;       // Last modification timestamp
  facilityDetails?: FacilityDetailsDto; // Facility details (if requested)
  cycleDetails?: FacilityCycleDetailsDto; // Cycle details (if requested)
}
```

---

## 🔧 Supporting DTOs

### User Context DTOs

#### `UserRequestHistoryDto`
**Used for**: User's request history for cycles

```typescript
interface UserRequestHistoryDto {
  requestId: string;             // Request ID
  status: string;               // Request status
  statusText: string;            // Human-readable status text
  requestedAmountRials: number;   // Requested amount
  approvedAmountRials?: number;  // Approved amount
  createdAt: string;            // Creation date
  approvedAt?: string;          // Approval date
  rejectedAt?: string;          // Rejection date
  rejectionReason?: string;     // Rejection reason
  daysSinceCreated: number;      // Days since creation
  isInProgress: boolean;        // Is in progress
  isCompleted: boolean;         // Is completed
  isRejected: boolean;          // Is rejected
  isCancelled: boolean;         // Is cancelled
}
```

#### `UserEligibilityDto`
**Used for**: User eligibility information

```typescript
interface UserEligibilityDto {
  isEligible: boolean;           // Is user eligible
  eligibilityScore: number;      // Eligibility score
  eligibilityReasons: string[];  // Eligibility reasons
  ineligibilityReasons: string[]; // Ineligibility reasons
  requiredDocuments: string[];   // Required documents
  missingDocuments: string[];    // Missing documents
  lastEligibilityCheck: string;  // Last eligibility check date
}
```

### Information DTOs

#### `FacilityInfoDto`
**Used for**: Basic facility information in requests

```typescript
interface FacilityInfoDto {
  id: string;                    // Facility ID
  name: string;                  // Facility name
  code: string;                  // Facility code
  type: string;                  // Facility type
  typeText: string;              // Human-readable type text
  status: string;                // Facility status
  statusText: string;            // Human-readable status text
  description?: string;          // Facility description
  bankInfo?: BankInfoDto;        // Bank information
  isActive: boolean;             // Is facility active
}
```

#### `BankInfoDto`
**Used for**: Bank information

```typescript
interface BankInfoDto {
  bankName?: string;             // Bank name
  bankCode?: string;             // Bank code
  bankAccountNumber?: string;     // Bank account number
  isAvailable: boolean;          // Is bank information available
}
```

#### `ApplicantInfoDto`
**Used for**: Applicant information

```typescript
interface ApplicantInfoDto {
  memberId: string;              // Member ID
  fullName: string;              // Full name
  nationalId: string;            // National ID
  isComplete: boolean;           // Is profile complete
}
```

### Statistics DTOs

#### `FacilityCycleStatisticsDto`
**Used for**: Facility cycle statistics

```typescript
interface FacilityCycleStatisticsDto {
  totalCycles: number;           // Total cycles
  activeCycles: number;          // Active cycles
  completedCycles: number;       // Completed cycles
  totalQuota: number;            // Total quota
  usedQuota: number;             // Used quota
  availableQuota: number;        // Available quota
  quotaUtilizationPercentage: number; // Quota utilization %
}
```

#### `CycleStatisticsDto`
**Used for**: Individual cycle statistics

```typescript
interface CycleStatisticsDto {
  totalRequests: number;         // Total requests
  pendingRequests: number;       // Pending requests
  approvedRequests: number;      // Approved requests
  rejectedRequests: number;     // Rejected requests
  cancelledRequests: number;    // Cancelled requests
  averageProcessingTime: number; // Average processing time (days)
  quotaUtilizationPercentage: number; // Quota utilization %
}
```

### Financial DTOs

#### `FinancialTermsDto` (Simple)
**Used for**: Basic financial terms

```typescript
interface FinancialTermsDto {
  minAmountRials: number;        // Minimum amount
  maxAmountRials: number;        // Maximum amount
  defaultAmountRials?: number;   // Default amount
  currency: string;              // Currency code
  paymentMonths: number;         // Payment months
  interestRate?: number;         // Interest rate
  processingFee?: number;        // Processing fee
}
```

#### `DetailedFinancialTermsDto` (Details)
**Used for**: Detailed financial terms
**Inherits from**: `FinancialTermsDto`

```typescript
interface DetailedFinancialTermsDto extends FinancialTermsDto {
  gracePeriodMonths?: number;    // Grace period
  earlyPaymentDiscount?: number; // Early payment discount
  latePaymentPenalty?: number;   // Late payment penalty
  collateralRequirements?: string; // Collateral requirements
  insuranceRequirements?: string; // Insurance requirements
  additionalFees: Record<string, number>; // Additional fees
}
```

### Rules DTOs

#### `CycleRulesDto` (Simple)
**Used for**: Basic cycle rules

```typescript
interface CycleRulesDto {
  cooldownDays: number;          // Cooldown period
  maxRequestsPerUser: number;    // Max requests per user
  allowMultipleRequests: boolean; // Allow multiple requests
  requiresDocumentation: boolean; // Requires documentation
  requiresApproval: boolean;     // Requires approval
}
```

#### `DetailedCycleRulesDto` (Details)
**Used for**: Detailed cycle rules
**Inherits from**: `CycleRulesDto`

```typescript
interface DetailedCycleRulesDto extends CycleRulesDto {
  eligibilityCriteria: string[];  // Eligibility criteria
  requiredDocuments: string[];   // Required documents
  approvalWorkflow: string[];     // Approval workflow
  rejectionReasons: string[];    // Valid rejection reasons
  specialConditions?: string;    // Special conditions
}
```

### Feature DTOs

#### `FacilityFeatureDto`
**Used for**: Facility features

```typescript
interface FacilityFeatureDto {
  id: string;                    // Feature ID
  name: string;                  // Feature name
  code: string;                  // Feature code
  description?: string;          // Feature description
  isActive: boolean;             // Is feature active
  isRequired: boolean;           // Is feature required
}
```

#### `FacilityCapabilityPolicyDto`
**Used for**: Facility capability policies

```typescript
interface FacilityCapabilityPolicyDto {
  id: string;                    // Policy ID
  name: string;                  // Policy name
  code: string;                  // Policy code
  description?: string;          // Policy description
  isActive: boolean;             // Is policy active
  isRequired: boolean;           // Is policy required
}
```

---

## 🌐 API Endpoint Mapping

### Facility Endpoints

| Endpoint | Method | DTO Used | Query Parameters | Description |
|----------|--------|----------|------------------|-------------|
| `/api/v1/facilities` | GET | `FacilityDto[]` | `page=1`, `pageSize=10`, `type?`, `status?`, `searchTerm?`, `onlyActive=true` | Get facilities list |
| `/api/v1/facilities/{facilityId}` | GET | `FacilityDetailsDto` | `includeCycles=true`, `includeFeatures=true`, `includePolicies=true`, `includeUserRequestHistory=false` | Get facility details |

### Cycle Endpoints

| Endpoint | Method | DTO Used | Query Parameters | Description |
|----------|--------|----------|------------------|-------------|
| `/api/v1/facilities/{facilityId}/cycles` | GET | `FacilityCycleWithUserContextDto[]` | `page=1`, `pageSize=10`, `status?`, `onlyActive=true`, `onlyEligible=false`, `onlyWithUserRequests=false`, `includeUserRequestStatus=true`, `includeDetailedRequestInfo=false`, `includeStatistics=true` | Get facility cycles |
| `/api/v1/facilities/cycles/{cycleId}` | GET | `FacilityCycleDetailsDto` | `includeFacilityInfo=true`, `includeUserRequestHistory=true`, `includeEligibilityDetails=true`, `includeDependencies=true`, `includeStatistics=true` | Get cycle details |

### Request Endpoints

| Endpoint | Method | DTO Used | Query Parameters | Description |
|----------|--------|----------|------------------|-------------|
| `/api/v1/facilities/requests` | GET | `FacilityRequestDto[]` | `page=1`, `pageSize=10`, `facilityId?`, `facilityCycleId?`, `status?`, `searchTerm?`, `dateFrom?`, `dateTo?` | Get all requests |
| `/api/v1/facilities/requests/{requestId}` | GET | `FacilityRequestDetailsDto` | `includeFacility=true`, `includeCycle=true`, `includePolicySnapshot=true` | Get request details |

### User Endpoints

| Endpoint | Method | DTO Used | Query Parameters | Description |
|----------|--------|----------|------------------|-------------|
| `/api/v1/facilities/user/cycle-requests` | GET | `FacilityRequestDto[]` | `page=1`, `pageSize=10`, `facilityId?`, `facilityCycleId?`, `status?`, `statusCategory?`, `dateFrom?`, `dateTo?`, `includeFacilityInfo=true`, `includeCycleInfo=true`, `includeTimeline=true` | Get user cycle requests |
| `/api/v1/facilities/user/cycles-with-requests` | GET | `FacilityCycleWithUserContextDto[]` | `page=1`, `pageSize=10`, `facilityId?`, `requestStatus?`, `requestStatusCategory?`, `onlyActive=true`, `includeDetailedRequestInfo=true`, `includeFacilityInfo=true`, `includeStatistics=true` | Get user cycles with requests |
| `/api/v1/facilities/user/member-info` | GET | `UserMemberInfoDetailsDto` | None | Get user member info |
| `/api/v1/facilities/cycles/{cycleId}/user-request` | GET | `GetFacilityCycleDetailsResponse` | None | Check user request for cycle |

### Command Endpoints

| Endpoint | Method | Request DTO | Response DTO | Description |
|----------|--------|-------------|--------------|-------------|
| `/api/v1/facilities/requests` | POST | `CreateFacilityRequestCommand` | `CreateFacilityRequestResult` | Create facility request |
| `/api/v1/facilities/requests/{requestId}/approve` | POST | `ApproveFacilityRequestRequest` | `ApproveFacilityRequestResult` | Approve facility request |
| `/api/v1/facilities/requests/{requestId}/reject` | POST | `RejectFacilityRequestRequest` | `RejectFacilityRequestResult` | Reject facility request |
| `/api/v1/facilities/requests/{requestId}/cancel` | POST | `CancelFacilityRequestRequest` | `CancelFacilityRequestResult` | Cancel facility request |

---

## 📋 Query Parameters Reference

### Common Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number for pagination |
| `pageSize` | int | 10 | Number of items per page |
| `status` | string? | null | Filter by status (enum value) |
| `searchTerm` | string? | null | Search term for filtering |

### Facility Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `type` | string? | null | Filter by facility type |
| `onlyActive` | bool | true | Show only active facilities |

### Cycle Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `onlyActive` | bool | true | Show only active cycles |
| `onlyEligible` | bool | false | Show only eligible cycles for user |
| `onlyWithUserRequests` | bool | false | Show only cycles with user requests |
| `includeUserRequestStatus` | bool | true | Include user request status |
| `includeDetailedRequestInfo` | bool | false | Include detailed request information |
| `includeStatistics` | bool | true | Include statistics |

### Request Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `facilityId` | Guid? | null | Filter by facility ID |
| `facilityCycleId` | Guid? | null | Filter by cycle ID |
| `statusCategory` | int? | null | Filter by status category |
| `dateFrom` | DateTime? | null | Filter from date |
| `dateTo` | DateTime? | null | Filter to date |

### Include Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `includeCycles` | bool | true | Include facility cycles |
| `includeFeatures` | bool | true | Include facility features |
| `includePolicies` | bool | true | Include facility policies |
| `includeUserRequestHistory` | bool | false | Include user request history |
| `includeFacilityInfo` | bool | true | Include facility information |
| `includeCycleInfo` | bool | true | Include cycle information |
| `includeTimeline` | bool | true | Include request timeline |
| `includeFacility` | bool | true | Include facility details |
| `includeCycle` | bool | true | Include cycle details |
| `includePolicySnapshot` | bool | true | Include policy snapshot |
| `includeEligibilityDetails` | bool | true | Include eligibility details |
| `includeDependencies` | bool | true | Include dependencies |

### Status Categories

| Category | Value | Description |
|----------|-------|-------------|
| `InProgress` | 1 | Requests in progress |
| `Completed` | 2 | Completed requests |
| `Rejected` | 3 | Rejected requests |
| `Cancelled` | 4 | Cancelled requests |
| `All` | 5 | All requests |

### Example URLs

```
# Get facilities with pagination and filters
GET /api/v1/facilities?page=1&pageSize=20&type=Loan&onlyActive=true&searchTerm=تجارت

# Get facility details with all includes
GET /api/v1/facilities/123e4567-e89b-12d3-a456-426614174000?includeCycles=true&includeFeatures=true&includePolicies=true&includeUserRequestHistory=true

# Get facility cycles with user context
GET /api/v1/facilities/123e4567-e89b-12d3-a456-426614174000/cycles?page=1&pageSize=10&onlyActive=true&includeUserRequestStatus=true&includeStatistics=true

# Get cycle details with all includes
GET /api/v1/facilities/cycles/456e7890-e89b-12d3-a456-426614174000?includeFacilityInfo=true&includeUserRequestHistory=true&includeEligibilityDetails=true&includeDependencies=true&includeStatistics=true

# Get user cycle requests with filters
GET /api/v1/facilities/user/cycle-requests?page=1&pageSize=20&facilityId=123e4567-e89b-12d3-a456-426614174000&statusCategory=1&includeFacilityInfo=true&includeCycleInfo=true&includeTimeline=true

# Get user cycles with requests
GET /api/v1/facilities/user/cycles-with-requests?page=1&pageSize=10&onlyActive=true&includeDetailedRequestInfo=true&includeFacilityInfo=true&includeStatistics=true

# Get all requests with filters
GET /api/v1/facilities/requests?page=1&pageSize=20&facilityId=123e4567-e89b-12d3-a456-426614174000&status=PendingApproval&dateFrom=2024-01-01&dateTo=2024-12-31

# Get request details
GET /api/v1/facilities/requests/789e0123-e89b-12d3-a456-426614174000?includeFacility=true&includeCycle=true&includePolicySnapshot=true
```

---

## 💻 Client Implementation Guide

### TypeScript Client Types

```typescript
// Base types
type FacilityType = 'Loan' | 'Grant' | 'Card' | 'WelfareVoucher' | 'Other';
type FacilityStatus = 'Draft' | 'Active' | 'Suspended' | 'Closed' | 'Maintenance';
type CycleStatus = 'Draft' | 'Active' | 'Closed' | 'Completed' | 'Cancelled';
type RequestStatus = 'RequestSent' | 'PendingApproval' | 'PendingDocuments' | 'Waitlisted' | 'ReturnedForAmendment' | 'UnderReview' | 'Approved' | 'Rejected' | 'Cancelled' | 'BankCancelled' | 'Expired' | 'Completed';

// Additional enum types
type AdmissionStrategy = 'FIFO' | 'Score' | 'Lottery' | 'Random';
type Repeatability = 'None' | 'OncePerCycle' | 'MultiplePerCycle' | 'Unlimited';
type Exclusivity = 'None' | 'SingleFacility' | 'SingleCycle' | 'SingleRequest';
type Dependency = 'None' | 'PreviousCycle' | 'OtherFacility' | 'DocumentUpload' | 'ProfileComplete';
type EligibilityLevel = 'High' | 'Medium' | 'Low' | 'Ineligible';
type ProfileStatus = 'Incomplete' | 'Pending' | 'Complete' | 'Verified';
type CapabilityLevel = 'Basic' | 'Intermediate' | 'Advanced' | 'Expert';
type FeatureLevel = 'Basic' | 'Standard' | 'Premium' | 'Enterprise';
type DependencyType = 'Document' | 'PreviousRequest' | 'ProfileField' | 'Capability' | 'Feature';
type QuotaStatus = 'Available' | 'Limited' | 'Full' | 'Waitlist';
type CooldownStatus = 'Active' | 'Expired' | 'NotApplicable';
type ProcessingStatus = 'Pending' | 'InProgress' | 'Completed' | 'Failed';
type ApprovalStatus = 'Pending' | 'Approved' | 'Rejected' | 'Cancelled';
type BankStatus = 'NotRequired' | 'Scheduled' | 'Completed' | 'Failed' | 'Cancelled';

// API Response wrapper
interface ApiResponse<T> {
  data: T;
  isSuccess: boolean;
  message: string;
  errors: string[];
}

// Pagination wrapper
interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
```


---

## 📝 Notes for Client Developers

### 1. **Enum Handling**
- Always use `*Text` fields for display (e.g., `statusText`, `typeText`)
- Raw enum values are for programmatic logic
- Status fields include computed boolean flags (`isInProgress`, `isCompleted`, etc.)

### 2. **Date Handling**
- All dates are in ISO 8601 format
- Use `createdAt`, `lastModifiedAt` for timestamps
- Use `daysSinceCreated`, `daysUntilEnd` for relative time calculations

### 3. **Null Safety**
- Optional fields are marked with `?`
- Arrays are always initialized (never null)
- Use `isAvailable` computed properties for complex null checks

### 4. **Pagination**
- Use `page`, `pageSize` query parameters
- Response includes `totalCount`, `totalPages` for UI pagination
- Default page size is usually 10-20 items

### 5. **User Context**
- Endpoints with user context require authentication
- User-specific data appears when `NationalNumber` is provided
- `UserEligibility`, `UserRequestHistory` are user-specific fields

### 6. **Performance Tips**
- Use Simple DTOs for lists and summaries
- Use Details DTOs only for single entity views
- Leverage query parameters to control data inclusion (`includeCycles`, `includeFeatures`, etc.)

### Additional Supporting DTOs

#### `UserMemberInfoDto` (Simple)
**Used for**: Basic user member information

```typescript
interface UserMemberInfoDto {
  id: string;                    // Member ID
  fullName: string;              // Member full name
  nationalId: string;            // Member national ID
  phoneNumber?: string;          // Member phone number
}
```

#### `UserMemberInfoDetailsDto` (Details)
**Used for**: Detailed user member information
**Inherits from**: `UserMemberInfoDto`
**Endpoints**: `GET /api/v1/facilities/user/member-info`

```typescript
interface UserMemberInfoDetailsDto extends UserMemberInfoDto {
  capabilities: MemberCapabilityDto[]; // Member capabilities
  features: MemberFeatureDto[];        // Member features
  eligibilitySummary: MemberEligibilitySummaryDto; // Eligibility summary
  requestStatistics: MemberRequestStatisticsDto; // Request statistics
  profileCompleteness: MemberProfileCompletenessDto; // Profile completeness
  metadata: Record<string, string>;    // Member metadata
  lastLoginDate?: string;              // Last login date
  lastModifiedAt?: string;             // Last modification timestamp
  preferences: Record<string, object>; // Member preferences
}
```

#### `RequestTimelineDto`
**Used for**: Request timeline information

```typescript
interface RequestTimelineDto {
  createdAt: string;            // Request creation date
  approvedAt?: string;          // Request approval date
  rejectedAt?: string;          // Request rejection date
  bankAppointmentScheduledAt?: string; // Bank appointment scheduled date
  bankAppointmentDate?: string; // Bank appointment actual date
  disbursedAt?: string;        // Disbursement date
  completedAt?: string;         // Completion date
  daysSinceCreated: number;     // Days since request creation
  daysUntilBankAppointment?: number; // Days until bank appointment
  isBankAppointmentOverdue: boolean; // Is bank appointment overdue
  processingTimeDays?: number;  // Processing time in days
}
```

#### `RequestFinancialInfoDto`
**Used for**: Request financial information

```typescript
interface RequestFinancialInfoDto {
  requestedAmountRials: number;  // Requested amount
  approvedAmountRials?: number;  // Approved amount
  currency: string;              // Currency code
  interestRate?: number;          // Interest rate
  processingFee?: number;         // Processing fee
  totalAmount?: number;          // Total amount (including fees)
  monthlyPayment?: number;       // Monthly payment amount
  paymentMonths: number;         // Number of payment months
}
```

#### `RequestStatusDto`
**Used for**: Request status details

```typescript
interface RequestStatusDto {
  status: string;                // Current status
  statusText: string;           // Human-readable status text
  isInProgress: boolean;        // Is in progress
  isCompleted: boolean;         // Is completed
  isRejected: boolean;          // Is rejected
  isCancelled: boolean;         // Is cancelled
  isTerminal: boolean;          // Is status terminal
  canBeCancelled: boolean;       // Can be cancelled
  requiresApplicantAction: boolean; // Requires applicant action
  requiresBankAction: boolean;   // Requires bank action
}
```

#### `MemberCapabilityDto`
**Used for**: Member capabilities

```typescript
interface MemberCapabilityDto {
  id: string;                    // Capability ID
  name: string;                  // Capability name
  code: string;                  // Capability code
  description?: string;          // Capability description
  isActive: boolean;             // Is capability active
  isRequired: boolean;           // Is capability required
  level: string;                 // Capability level
  acquiredAt?: string;          // When capability was acquired
}
```

#### `MemberFeatureDto`
**Used for**: Member features

```typescript
interface MemberFeatureDto {
  id: string;                    // Feature ID
  name: string;                  // Feature name
  code: string;                  // Feature code
  description?: string;          // Feature description
  isActive: boolean;             // Is feature active
  isRequired: boolean;           // Is feature required
  level: string;                 // Feature level
  acquiredAt?: string;          // When feature was acquired
}
```

#### `MemberEligibilitySummaryDto`
**Used for**: Member eligibility summary

```typescript
interface MemberEligibilitySummaryDto {
  isEligible: boolean;           // Is member eligible
  eligibilityScore: number;      // Eligibility score
  eligibilityLevel: string;      // Eligibility level
  lastEligibilityCheck: string;  // Last eligibility check date
  eligibilityReasons: string[];  // Eligibility reasons
  ineligibilityReasons: string[]; // Ineligibility reasons
  requiredDocuments: string[];  // Required documents
  missingDocuments: string[];    // Missing documents
}
```

#### `MemberRequestStatisticsDto`
**Used for**: Member request statistics

```typescript
interface MemberRequestStatisticsDto {
  totalRequests: number;         // Total requests
  pendingRequests: number;       // Pending requests
  approvedRequests: number;      // Approved requests
  rejectedRequests: number;     // Rejected requests
  cancelledRequests: number;    // Cancelled requests
  completedRequests: number;     // Completed requests
  averageProcessingTime: number; // Average processing time (days)
  lastRequestDate?: string;     // Last request date
}
```

#### `MemberProfileCompletenessDto`
**Used for**: Member profile completeness

```typescript
interface MemberProfileCompletenessDto {
  completenessPercentage: number; // Profile completeness percentage
  status: string;                // Profile status
  statusText: string;            // Human-readable status text
  completenessItems: ProfileCompletenessItemDto[]; // Completeness items
  missingFields: string[];       // Missing fields
  lastUpdated: string;           // Last profile update
}
```

#### `ProfileCompletenessItemDto`
**Used for**: Individual profile completeness items

```typescript
interface ProfileCompletenessItemDto {
  fieldName: string;             // Field name
  fieldLabel: string;            // Field label
  isComplete: boolean;           // Is field complete
  isRequired: boolean;           // Is field required
  completionPercentage: number;  // Field completion percentage
  lastUpdated?: string;          // Last field update
}
```

#### `DetailedEligibilityDto`
**Used for**: Detailed eligibility information

```typescript
interface DetailedEligibilityDto {
  isEligible: boolean;           // Is user eligible
  eligibilityScore: number;      // Eligibility score
  eligibilityLevel: string;     // Eligibility level
  eligibilityReasons: string[];  // Eligibility reasons
  ineligibilityReasons: string[]; // Ineligibility reasons
  requiredDocuments: string[];   // Required documents
  missingDocuments: string[];    // Missing documents
  lastEligibilityCheck: string;  // Last eligibility check date
  eligibilityDetails: Record<string, object>; // Detailed eligibility info
}
```

#### `FacilityCycleDependencyDto`
**Used for**: Facility cycle dependencies

```typescript
interface FacilityCycleDependencyDto {
  id: string;                    // Dependency ID
  name: string;                  // Dependency name
  code: string;                  // Dependency code
  description?: string;          // Dependency description
  isRequired: boolean;           // Is dependency required
  isSatisfied: boolean;          // Is dependency satisfied
  dependencyType: string;        // Dependency type
  requiredValue?: string;        // Required value
  actualValue?: string;          // Actual value
}
```

#### `CycleDependencyDto`
**Used for**: Cycle dependencies

```typescript
interface CycleDependencyDto {
  id: string;                    // Dependency ID
  name: string;                  // Dependency name
  code: string;                  // Dependency code
  description?: string;          // Dependency description
  isRequired: boolean;           // Is dependency required
  isSatisfied: boolean;          // Is dependency satisfied
  dependencyType: string;        // Dependency type
  requiredValue?: string;        // Required value
  actualValue?: string;          // Actual value
}
```

#### `DependencyRequirementsDto`
**Used for**: Dependency requirements

```typescript
interface DependencyRequirementsDto {
  totalDependencies: number;    // Total dependencies
  requiredDependencies: CycleDependencyDto[]; // Required dependencies
  satisfiedDependencies: CycleDependencyDto[]; // Satisfied dependencies
  missingDependencies: CycleDependencyDto[]; // Missing dependencies
  dependencySatisfactionPercentage: number; // Dependency satisfaction %
}
```

#### `CapabilityRequirementsDto`
**Used for**: Capability requirements

```typescript
interface CapabilityRequirementsDto {
  totalCapabilities: number;     // Total capabilities
  requiredCapabilities: string[]; // Required capabilities
  satisfiedCapabilities: string[]; // Satisfied capabilities
  missingCapabilities: string[]; // Missing capabilities
  capabilitySatisfactionPercentage: number; // Capability satisfaction %
}
```

#### `FeatureRequirementsDto`
**Used for**: Feature requirements

```typescript
interface FeatureRequirementsDto {
  totalFeatures: number;         // Total features
  requiredFeatures: string[];    // Required features
  satisfiedFeatures: string[];   // Satisfied features
  missingFeatures: string[];    // Missing features
  featureSatisfactionPercentage: number; // Feature satisfaction %
}
```

#### `QuotaAvailabilityDto`
**Used for**: Quota availability information

```typescript
interface QuotaAvailabilityDto {
  totalQuota: number;            // Total quota
  usedQuota: number;            // Used quota
  availableQuota: number;        // Available quota
  quotaUtilizationPercentage: number; // Quota utilization %
  isQuotaAvailable: boolean;     // Is quota available
  quotaReserved?: number;        // Reserved quota
  quotaWaitlist?: number;        // Waitlist quota
}
```

#### `CooldownRequirementsDto`
**Used for**: Cooldown requirements

```typescript
interface CooldownRequirementsDto {
  cooldownDays: number;          // Cooldown period in days
  lastRequestDate?: string;      // Last request date
  daysUntilEligible?: number;    // Days until eligible
  isInCooldown: boolean;         // Is in cooldown period
  cooldownEndDate?: string;      // Cooldown end date
}
```

#### `CycleSummaryDto`
**Used for**: Cycle summary information

```typescript
interface CycleSummaryDto {
  totalCycles: number;           // Total cycles
  activeCycles: number;          // Active cycles
  completedCycles: number;       // Completed cycles
  totalQuota: number;            // Total quota
  usedQuota: number;             // Used quota
  availableQuota: number;        // Available quota
  quotaUtilizationPercentage: number; // Quota utilization %
}
```

---

## 🔄 DTO Evolution

### Versioning Strategy
- **Breaking changes**: New major version (v2, v3, etc.)
- **Additive changes**: New fields added to existing DTOs
- **Deprecation**: Fields marked as deprecated but still supported

### Migration Guide
When DTOs change:
1. **Check breaking changes** in release notes
2. **Update client types** accordingly
3. **Test thoroughly** with new API version
4. **Handle deprecated fields** gracefully

---

This documentation covers all DTOs in the Facilities module. For specific implementation details or questions, refer to the individual DTO files or contact the development team.

