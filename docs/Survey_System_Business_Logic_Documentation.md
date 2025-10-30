# Survey System Business Logic Documentation

## Overview
This document provides comprehensive documentation of the Survey System's business logic, domain rules, and architectural patterns for client developers.

## Architecture Pattern
The Survey System follows **Domain-Driven Design (DDD)** principles with:
- **Aggregate Root**: `Survey` (single aggregate containing all related entities)
- **Entities**: `Question`, `Response`, `QuestionAnswer`, `SurveyFeature`, `SurveyCapability`
- **Value Objects**: `ParticipationPolicy`, `ParticipantInfo`, `DemographySnapshot`, `AudienceFilter`
- **Domain Services**: `ParticipationRulesDomainService`, `SurveyDomainService`, `SurveyValidationService`
- **Domain Events**: `ResponseSubmittedEvent`, `SurveyStructureFrozenEvent`

---

## 1. Survey Lifecycle Management

### 1.1 Survey States
The survey follows a strict state machine:

```typescript
enum SurveyState {
  Draft = 0,        // Being created and configured
  Scheduled = 1,    // Scheduled to start at specific time
  Active = 2,       // Currently accepting responses
  Paused = 3,       // Temporarily paused
  Closed = 4,       // Ended, no longer accepting responses
  Archived = 5      // Archived for historical purposes
}
```

### 1.2 State Transitions
- **Draft → Scheduled**: `Schedule(startAt, endAt?)`
- **Draft → Active**: `Activate()` (immediate activation)
- **Scheduled → Active**: Automatic when `startAt` time reached
- **Active → Closed**: `Close()` (manual or automatic when `endAt` reached)
- **Closed → Archived**: `Archive()`

### 1.3 Business Rules
- Only **Draft** surveys can be modified (questions, features, capabilities)
- **Active** surveys cannot be modified (structure frozen)
- Surveys must have at least one question before activation
- Time validation: `startAt` cannot be after `endAt`

---

## 2. Participation Management

### 2.1 Participation Policy
```typescript
interface ParticipationPolicy {
  maxAttemptsPerMember: number;        // Maximum attempts per participant
  allowMultipleSubmissions: boolean;   // Allow multiple submissions
  coolDownSeconds?: number;           // Cool down period between attempts
  allowBackNavigation: boolean;       // Allow back navigation during survey
}
```

### 2.2 Participation Rules
1. **Attempt Limits**: Participants cannot exceed `maxAttemptsPerMember`
2. **Cool Down Period**: If set, participants must wait before next attempt
3. **Multiple Submissions**: If `false`, only one submission allowed per participant
4. **Back Navigation**: Controls whether participants can go back to previous questions

### 2.3 Participant Types
- **Authenticated Members**: Identified by `MemberId`
- **Anonymous Participants**: Identified by `ParticipantHash`

---

## 3. Response Management

### 3.1 Response States
```typescript
enum AttemptStatus {
  Active = 0,      // Currently being answered
  Submitted = 1,   // Submitted and immutable
  Canceled = 2,    // Canceled/abandoned
  Expired = 3      // Expired due to time constraints
}
```

### 3.2 Response Lifecycle
1. **Start Response**: `StartResponse(participant, demographySnapshot?)`
2. **Answer Questions**: `SetResponseAnswer(questionId, textAnswer?, selectedOptionIds?)`
3. **Submit Response**: `Submit()` → Raises `ResponseSubmittedEvent`
4. **Cancel/Expire**: `Cancel()` or `Expire()`

### 3.3 Response Validation
- **Required Questions**: All required questions must be answered before submission
- **Question Types**: Validation based on question kind (text, single choice, multiple choice)
- **Repeat Questions**: Support for repeated questions with `repeatIndex`

---

## 4. Question Management

### 4.1 Question Types
```typescript
enum QuestionKind {
  FixedMCQ4 = 0,      // Fixed 4-option multiple choice
  ChoiceSingle = 1,   // Single choice (radio buttons)
  ChoiceMulti = 2,    // Multiple choice (checkboxes)
  Textual = 3         // Text input
}
```

### 4.2 Question Structure
- **Order**: Questions have an `order` property for sequencing
- **Required**: Questions can be marked as required
- **Options**: Choice questions must have options
- **Repeat Policy**: Questions can be repeated multiple times

### 4.3 Question Validation Rules
- **FixedMCQ4**: Must have exactly 4 options
- **Choice Questions**: Must have at least one option
- **Text Questions**: No options required
- **Required Questions**: Must be answered before submission

---

## 5. Authorization & Access Control

### 5.1 Feature-Based Authorization
Surveys can be linked to **Features** (capabilities):
```typescript
interface SurveyFeature {
  featureCode: string;           // Stable feature code
  featureTitleSnapshot?: string; // Snapshot of feature title
}
```

### 5.2 Capability-Based Authorization
Surveys can be linked to **Capabilities** (permissions):
```typescript
interface SurveyCapability {
  capabilityCode: string;           // Stable capability code
  capabilityTitleSnapshot?: string; // Snapshot of capability title
}
```

### 5.3 Authorization Logic
- **OR Logic**: Participants need at least ONE required feature OR capability
- **No Restrictions**: If no features/capabilities specified, all participants allowed
- **Combined Requirements**: If both features and capabilities specified, participant needs at least one of each

---

## 6. Multiple Response Handling

### 6.1 Latest Response Logic
The system distinguishes between different types of "latest" responses:

#### 6.1.1 Latest Valid Response
- **Definition**: Latest **submitted** response
- **Purpose**: For business logic that requires completed responses
- **Method**: `GetLatestValidResponse(participant)`
- **Ordering**: By `SubmittedAt` timestamp

#### 6.1.2 Latest Response (Any Status)
- **Definition**: Latest response of any status (Active, Submitted, Canceled, Expired)
- **Purpose**: For UI display and navigation
- **Method**: `GetLatestResponse(participant)`
- **Ordering**: By `SubmittedAt ?? CanceledAt ?? ExpiredAt`

### 6.2 Response Counting
- **Submitted Responses**: Count only `Submitted` responses for business rules
- **All Responses**: Count all responses for display purposes
- **Attempt Numbers**: Sequential numbering (1, 2, 3, ...)

---

## 7. Survey Structure Management

### 7.1 Structure Freezing
- **Purpose**: Prevent modifications to active surveys
- **Trigger**: Automatic when survey becomes active
- **Manual Control**: `FreezeStructure()` and `UnfreezeStructure()`
- **Versioning**: Structure version increments on freeze/unfreeze

### 7.2 Structure Validation
Before activation, surveys must pass:
- At least one question exists
- All choice questions have options
- FixedMCQ4 questions have exactly 4 options
- No structural inconsistencies

---

## 8. Time Management

### 8.1 Survey Timing
- **Start Time**: When survey becomes available
- **End Time**: When survey stops accepting responses
- **Current Time Check**: `IsAcceptingResponses()` considers both state and timing

### 8.2 Response Timing
- **Submission Time**: When response was submitted
- **Expiration**: Responses can expire based on survey rules
- **Cool Down**: Time between attempts

---

## 9. Data Integrity & Consistency

### 9.1 Aggregate Invariants
The `Survey` aggregate enforces:
- Survey state consistency
- Question structure validity
- Response attempt numbering
- Participant uniqueness

### 9.2 Domain Events
- **ResponseSubmittedEvent**: Raised when response is submitted
- **SurveyStructureFrozenEvent**: Raised when structure is frozen
- **SurveyStructureUnfrozenEvent**: Raised when structure is unfrozen

---

## 10. Client Integration Guidelines

### 10.1 API Usage Patterns

#### 10.1.1 Starting a Survey
```typescript
// Check participation eligibility
const participationStatus = await getParticipationStatus(surveyId);

if (participationStatus.canParticipate) {
  // Start new response or resume existing
  const response = await startSurveyResponse(surveyId, {
    forceNewAttempt: false,
    resumeActiveIfAny: true
  });
}
```

#### 10.1.2 Answering Questions
```typescript
// Answer a question
await answerQuestion(surveyId, responseId, questionId, {
  textAnswer: "User's text answer",
  selectedOptionIds: ["option1", "option2"]
});
```

#### 10.1.3 Navigation
```typescript
// Get current question
const currentQuestion = await getCurrentQuestion(surveyId, responseId);

// Navigate to specific question
await jumpToQuestion(surveyId, responseId, targetQuestionId);

// Get previous questions
const previousQuestions = await getPreviousQuestions(surveyId, {
  currentQuestionIndex: 5,
  maxCount: 10
});
```

### 10.2 Error Handling

#### 10.2.1 Common Error Scenarios
- **Survey Not Active**: Survey is not accepting responses
- **Max Attempts Reached**: Participant exceeded attempt limit
- **Cool Down Period**: Participant must wait before next attempt
- **Authorization Failed**: Participant lacks required features/capabilities
- **Question Required**: Required question not answered
- **Response Immutable**: Trying to modify submitted response

#### 10.2.2 Error Response Format
```typescript
interface ApplicationResult<T> {
  isSuccess: boolean;
  data?: T;
  errorCode?: string;
  errorMessageFa?: string;
  meta?: {
    correlationId?: string;
    timestamp?: string;
  };
}
```

### 10.3 State Management

#### 10.3.1 Response State Tracking
```typescript
interface ResponseState {
  responseId: string;
  attemptNumber: number;
  status: AttemptStatus;
  questionsAnswered: number;
  questionsTotal: number;
  completionPercentage: number;
  canContinue: boolean;
  nextQuestionId?: string;
}
```

#### 10.3.2 Progress Tracking
```typescript
interface ProgressInfo {
  questionsAnswered: number;
  questionsRemaining: number;
  progressPercentage: number;
  requiredQuestionsAnswered: number;
  requiredQuestionsRemaining: number;
}
```

---

## 11. Business Rules Summary

### 11.1 Survey Rules
- Surveys must have at least one question before activation
- Only draft surveys can be modified
- Active surveys cannot be modified (structure frozen)
- Time constraints must be valid (start ≤ end)

### 11.2 Participation Rules
- Participants cannot exceed maximum attempts
- Cool down period must be respected
- Authorization requirements must be met
- Only one active response per participant

### 11.3 Response Rules
- Required questions must be answered before submission
- Only active responses can be modified
- Submitted responses are immutable
- Response states follow strict transition rules

### 11.4 Question Rules
- Choice questions must have options
- FixedMCQ4 questions must have exactly 4 options
- Questions are ordered by their order property
- Repeat questions follow repeat policy rules

---

## 12. Performance Considerations

### 12.1 Data Loading
- Use projection queries for list views
- Load full aggregates only when needed
- Implement pagination for large datasets
- Cache survey structure when possible

### 12.2 Response Handling
- Batch answer updates when possible
- Use auto-save for long surveys
- Implement optimistic UI updates
- Handle network failures gracefully

---

## 13. Security Considerations

### 13.1 Authorization
- Always validate participant authorization
- Check survey state before operations
- Validate attempt limits and cool down
- Ensure response ownership

### 13.2 Data Protection
- Handle anonymous participants securely
- Protect demographic data
- Implement proper audit logging
- Use idempotency keys for sensitive operations

---

## 14. Testing Guidelines

### 14.1 Unit Testing
- Test all business rules
- Test state transitions
- Test validation logic
- Test edge cases

### 14.2 Integration Testing
- Test complete survey flows
- Test authorization scenarios
- Test error conditions
- Test performance under load

---

## 15. Migration & Versioning

### 15.1 Survey Structure Versioning
- Structure version increments on freeze/unfreeze
- Track changes for audit purposes
- Handle backward compatibility
- Plan for future enhancements

### 15.2 Data Migration
- Handle survey structure changes
- Migrate existing responses
- Preserve data integrity
- Test migration scenarios

---

This documentation provides a comprehensive understanding of the Survey System's business logic and should be used as a reference for client development, testing, and integration.
