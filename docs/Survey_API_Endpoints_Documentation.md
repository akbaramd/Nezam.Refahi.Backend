# Survey API Endpoints Documentation

## Overview
This document provides comprehensive documentation for all Survey API endpoints, including request/response models, query parameters, and usage examples.

## Base URL
```
/api/v1/surveys
```

---

## 1. Get Survey Overview (Anonymous)
**Endpoint:** `GET /{surveyId}/overview`

### Description
Get basic survey information without authentication.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier

### Response
```typescript
interface SurveyOverviewResponse {
  survey: {
    id: string;
    title: string;
    description?: string;
    state: string;
    stateTextFa: string;
    isAcceptingResponses: boolean;
    totalQuestions: number;
    requiredQuestions: number;
    allowBackNavigation: boolean;
    allowMultipleSubmissions: boolean;
    maxAttemptsPerMember: number;
    coolDownSeconds?: number;
  };
}
```

### Example Request
```http
GET /api/v1/surveys/123e4567-e89b-12d3-a456-426614174000/overview
```

---

## 2. Get Survey Details (Anonymous)
**Endpoint:** `GET /{surveyId}/details`

### Description
Get detailed survey information without authentication.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier

### Response
```typescript
interface SurveyDto {
  id: string;
  title: string;
  description?: string;
  state: string;
  stateTextFa: string;
  isAcceptingResponses: boolean;
  totalQuestions: number;
  requiredQuestions: number;
  allowBackNavigation: boolean;
  allowMultipleSubmissions: boolean;
  maxAttemptsPerMember: number;
  coolDownSeconds?: number;
  questions: QuestionDto[];
  features: SurveyFeatureDto[];
  capabilities: SurveyCapabilityDto[];
}
```

---

## 3. Get Survey Details with User Context (Authenticated)
**Endpoint:** `GET /{surveyId}/details/user`

### Description
Get survey details with user-specific information and eligibility status.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- Requires authentication

### Response
Same as SurveyDto but with additional user-specific fields:
```typescript
interface SurveyDto {
  // ... same as above
  canUserParticipate: boolean;
  userEligibilityMessage?: string;
  userLastResponse?: ResponseDto;
  userResponseCount: number;
}
```

---

## 4. Get Participation Status (Authenticated)
**Endpoint:** `GET /{surveyId}/participation`

### Description
Get user's participation status for a specific survey.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- Requires authentication

### Response
```typescript
interface ParticipationStatusResponse {
  canParticipate: boolean;
  eligibilityMessage?: string;
  lastResponse?: {
    id: string;
    attemptNumber: number;
    status: string;
    statusTextFa: string;
    startedAt: string;
    submittedAt?: string;
    questionsAnswered: number;
    questionsTotal: number;
    completionPercentage: number;
  };
  cooldownInfo?: {
    remainingSeconds: number;
    cooldownEndsAt: string;
    message: string;
  };
}
```

---

## 5. Get Current Question (Authenticated)
**Endpoint:** `GET /{surveyId}/responses/{responseId}/questions/current`

### Description
Get the current question for a specific response.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- `repeatIndex` (int?, Query) - Repeat index for repeated questions
- Requires authentication

### Response
```typescript
interface CurrentQuestionResponse {
  question: QuestionDetailsDto;
  navigation: {
    currentIndex: number;
    totalQuestions: number;
    canGoNext: boolean;
    canGoPrevious: boolean;
    progressPercentage: number;
  };
  userAnswer?: QuestionAnswerDto;
}
```

---

## 6. Get Question by ID (Authenticated)
**Endpoint:** `GET /{surveyId}/responses/{responseId}/questions/{questionId}`

### Description
Get a specific question by ID for a response.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- `questionId` (Guid, Path) - Question identifier
- `repeatIndex` (int?, Query) - Repeat index for repeated questions
- Requires authentication

### Response
```typescript
interface QuestionByIdResponse {
  question: QuestionDetailsDto;
  userAnswer?: QuestionAnswerDto;
  navigation: QuestionNavigationDto;
}
```

---

## 7. Get Response Progress (Authenticated)
**Endpoint:** `GET /{surveyId}/responses/{responseId}/progress`

### Description
Get progress information for a specific response.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- Requires authentication

### Response
```typescript
interface ResponseProgressResponse {
  responseId: string;
  attemptNumber: number;
  questionsAnswered: number;
  questionsTotal: number;
  requiredQuestionsAnswered: number;
  requiredQuestionsTotal: number;
  completionPercentage: number;
  progressPercentage: number;
  isCompleted: boolean;
  canSubmit: boolean;
  nextQuestionId?: string;
  previousQuestionId?: string;
}
```

---

## 8. List Questions for Navigation (Authenticated)
**Endpoint:** `GET /{surveyId}/responses/{responseId}/questions`

### Description
Get list of questions for navigation purposes.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- `includeBackNavigation` (bool, Query, default: false) - Include back navigation info
- Requires authentication

### Response
```typescript
interface QuestionsNavigationResponse {
  questions: QuestionNavigationItemDto[];
  navigation: {
    currentIndex: number;
    totalQuestions: number;
    canGoNext: boolean;
    canGoPrevious: boolean;
    progressPercentage: number;
  };
}
```

---

## 9. Get Active Surveys (Anonymous)
**Endpoint:** `GET /active`

### Description
Get list of active surveys with pagination and filtering.

### Query Parameters
- `pageNumber` (int, default: 1) - Page number
- `pageSize` (int, default: 20) - Page size
- `featureKey` (string?, Query) - Filter by feature key
- `capabilityKey` (string?, Query) - Filter by capability key

### Response
```typescript
interface ActiveSurveysResponse {
  surveys: SurveyDto[];
  pagination: {
    pageNumber: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
  };
}
```

---

## 10. Get Surveys with User Last Response (Authenticated)
**Endpoint:** `GET /user/last-responses`

### Description
Get surveys with user's last response information.

### Query Parameters
- `pageNumber` (int, default: 1) - Page number
- `pageSize` (int, default: 20) - Page size
- `searchTerm` (string?, Query) - Search term
- `state` (string?, Query) - Filter by state
- `isAcceptingResponses` (bool?, Query) - Filter by accepting responses
- `sortBy` (string?, Query, default: "CreatedAt") - Sort field
- `sortDirection` (string?, Query, default: "Desc") - Sort direction
- `includeQuestions` (bool, Query, default: false) - Include questions
- `includeUserLastResponse` (bool, Query, default: true) - Include user last response
- Requires authentication

### Response
```typescript
interface SurveysWithUserLastResponseResponse {
  surveys: SurveyWithUserResponseDto[];
  pagination: PaginationDto;
}
```

---

## 11. Get Surveys with User Responses (Authenticated)
**Endpoint:** `GET /user/responses`

### Description
Get surveys with all user responses.

### Query Parameters
- `pageNumber` (int, default: 1) - Page number
- `pageSize` (int, default: 20) - Page size
- `searchTerm` (string?, Query) - Search term
- `state` (string?, Query) - Filter by state
- `isAcceptingResponses` (bool?, Query) - Filter by accepting responses
- `userResponseStatus` (string?, Query) - Filter by user response status
- `hasUserResponse` (bool?, Query) - Filter by has user response
- `canUserParticipate` (bool?, Query) - Filter by can user participate
- `minUserCompletionPercentage` (decimal?, Query) - Minimum completion percentage
- `maxUserCompletionPercentage` (decimal?, Query) - Maximum completion percentage
- `sortBy` (string?, Query, default: "CreatedAt") - Sort field
- `sortDirection` (string?, Query, default: "Desc") - Sort direction
- `includeQuestions` (bool, Query, default: false) - Include questions
- `includeUserResponses` (bool, Query, default: true) - Include user responses
- `includeUserLastResponse` (bool, Query, default: true) - Include user last response
- Requires authentication

### Response
```typescript
interface SurveysWithUserResponsesResponse {
  surveys: SurveyWithUserResponsesDto[];
  pagination: PaginationDto;
}
```

---

## 12. Get Survey Questions (Anonymous/Authenticated)
**Endpoint:** `GET /{surveyId}/questions`

### Description
Get survey questions with optional user answers.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `includeUserAnswers` (bool, Query, default: false) - Include user answers

### Response
```typescript
interface SurveyQuestionsResponse {
  survey: SurveyBasicInfoDto;
  questions: QuestionDetailsDto[];
  userAnswers?: QuestionAnswerDto[];
}
```

---

## 13. Get Survey Questions Details (Anonymous/Authenticated)
**Endpoint:** `GET /{surveyId}/questions/details`

### Description
Get detailed survey questions with optional user answers and statistics.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `includeUserAnswers` (bool, Query, default: false) - Include user answers
- `includeStatistics` (bool, Query, default: false) - Include statistics

### Response
```typescript
interface SurveyQuestionsDetailsResponse {
  survey: SurveyBasicInfoDto;
  questions: QuestionDetailsDto[];
  userAnswers?: QuestionAnswerDto[];
  statistics?: QuestionStatisticsDto[];
}
```

---

## 14. Get Survey Questions with Answers (Authenticated)
**Endpoint:** `GET /{surveyId}/questions/with-answers`

### Description
Get survey questions with user's answers for a specific attempt.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `attemptNumber` (int?, Query) - Specific attempt number
- Requires authentication

### Response
```typescript
interface SurveyQuestionsWithAnswersResponse {
  survey: SurveyBasicInfoDto;
  questions: QuestionDetailsDto[];
  userAnswers: QuestionAnswerDto[];
  attemptNumber: number;
}
```

---

## 15. Get User Survey Responses (Authenticated)
**Endpoint:** `GET /{surveyId}/user/responses`

### Description
Get user's responses for a specific survey.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `includeAnswers` (bool, Query, default: false) - Include answers
- `includeLastAnswersOnly` (bool, Query, default: false) - Include only last answers
- Requires authentication

### Response
```typescript
interface UserSurveyResponsesResponse {
  survey: SurveyBasicInfoDto;
  responses: ResponseDto[];
  lastResponse?: ResponseDto;
}
```

---

## 16. Get Response Details (Authenticated)
**Endpoint:** `GET /responses/{responseId}/details`

### Description
Get detailed information about a specific response.

### Parameters
- `responseId` (Guid, Path) - Response identifier
- `includeQuestionDetails` (bool, Query, default: true) - Include question details
- `includeSurveyDetails` (bool, Query, default: true) - Include survey details
- Requires authentication

### Response
```typescript
interface ResponseDetailsDto {
  response: ResponseDto;
  survey?: SurveyBasicInfoDto;
  questions?: QuestionDetailsDto[];
  answers: QuestionAnswerDto[];
}
```

---

## 17. Get Question Answer Details (Authenticated)
**Endpoint:** `GET /responses/{responseId}/questions/{questionId}/answer`

### Description
Get detailed information about a specific question answer.

### Parameters
- `responseId` (Guid, Path) - Response identifier
- `questionId` (Guid, Path) - Question identifier
- `includeQuestionDetails` (bool, Query, default: true) - Include question details
- `includeSurveyDetails` (bool, Query, default: false) - Include survey details
- Requires authentication

### Response
```typescript
interface QuestionAnswerDetailsDto {
  answer: QuestionAnswerDto;
  question?: QuestionDetailsDto;
  survey?: SurveyBasicInfoDto;
}
```

---

## 18. Get Next Question (Authenticated)
**Endpoint:** `GET /{surveyId}/responses/{responseId}/questions/next`

### Description
Get the next question in sequence for a response.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- `currentQuestionId` (Guid?, Query) - Current question ID
- `includeUserAnswer` (bool, Query, default: true) - Include user answer
- Requires authentication

### Response
```typescript
interface NextQuestionResponseDto {
  question?: QuestionDetailsDto;
  navigation: QuestionNavigationDto;
  userAnswer?: QuestionAnswerDto;
  isLastQuestion: boolean;
}
```

---

## 19. Get Specific Question by Index (Anonymous)
**Endpoint:** `GET /{surveyId}/questions/{questionIndex}`

### Description
Get a specific question by its index in the survey.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `questionIndex` (int, Path) - Question index (0-based)
- `memberId` (Guid?, Query) - Member ID for user-specific data
- `responseId` (Guid?, Query) - Response ID for user-specific data
- `includeUserAnswers` (bool, Query, default: true) - Include user answers
- `includeNavigationInfo` (bool, Query, default: true) - Include navigation info
- `includeStatistics` (bool, Query, default: false) - Include statistics

### Response
```typescript
interface GetSpecificQuestionResponse {
  survey: SurveyBasicInfoDto;
  currentQuestion: QuestionDetailsDto;
  navigation: QuestionNavigationDto;
  userAnswer?: QuestionAnswerDto;
  userResponseStatus?: UserResponseStatusDto;
  statistics?: QuestionStatisticsDto;
}
```

---

## 20. Get Previous Questions (Anonymous)
**Endpoint:** `GET /{surveyId}/questions/previous`

### Description
Get previous questions from a specific index.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `currentQuestionIndex` (int, Query) - Current question index
- `maxCount` (int, Query, default: 10) - Maximum number of previous questions
- `memberId` (Guid?, Query) - Member ID for user-specific data
- `responseId` (Guid?, Query) - Response ID for user-specific data
- `includeUserAnswers` (bool, Query, default: true) - Include user answers
- `includeNavigationInfo` (bool, Query, default: true) - Include navigation info

### Response
```typescript
interface GetPreviousQuestionsResponse {
  survey: SurveyBasicInfoDto;
  previousQuestions: PreviousQuestionDto[];
  navigation: PreviousQuestionsNavigationDto;
  userResponseStatus?: UserResponseStatusDto;
}
```

---

## 21. Start Survey Response (Authenticated)
**Endpoint:** `POST /{surveyId}/responses`

### Description
Start a new survey response or resume an existing one.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `Idempotency-Key` (string?, Header) - Idempotency key
- Requires authentication

### Request Body
```typescript
interface StartSurveyResponseRequest {
  participantHash?: string;
  forceNewAttempt: boolean;
  demographySnapshot?: Record<string, string>;
  resumeActiveIfAny: boolean; // default: true
}
```

### Response
```typescript
interface StartSurveyResponseResponse {
  responseId: string;
  attemptNumber: number;
  status: string;
  statusTextFa: string;
  message: string;
  canContinue: boolean;
  nextQuestionId?: string;
}
```

---

## 22. Answer Question (Authenticated)
**Endpoint:** `PUT /{surveyId}/responses/{responseId}/answers/{questionId}`

### Description
Answer a specific question in a response.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- `questionId` (Guid, Path) - Question identifier
- `Idempotency-Key` (string?, Header) - Idempotency key
- Requires authentication

### Request Body
```typescript
interface AnswerQuestionRequest {
  textAnswer?: string;
  selectedOptionIds?: string[];
  allowBackNavigation: boolean; // default: true
}
```

### Response
```typescript
interface AnswerQuestionResponse {
  success: boolean;
  message: string;
  nextQuestionId?: string;
  isLastQuestion: boolean;
  progressPercentage: number;
}
```

---

## 23. Go to Next Question (Authenticated)
**Endpoint:** `POST /{surveyId}/responses/{responseId}/navigation/next`

### Description
Navigate to the next question in the survey.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- Requires authentication

### Response
```typescript
interface GoNextQuestionResponse {
  nextQuestionId?: string;
  isLastQuestion: boolean;
  message: string;
  progressPercentage: number;
}
```

---

## 24. Go to Previous Question (Authenticated)
**Endpoint:** `POST /{surveyId}/responses/{responseId}/navigation/prev`

### Description
Navigate to the previous question in the survey.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- Requires authentication

### Response
```typescript
interface GoPreviousQuestionResponse {
  previousQuestionId?: string;
  isFirstQuestion: boolean;
  message: string;
  progressPercentage: number;
}
```

---

## 25. Jump to Specific Question (Authenticated)
**Endpoint:** `POST /{surveyId}/responses/{responseId}/navigation/jump`

### Description
Jump to a specific question in the survey.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- Requires authentication

### Request Body
```typescript
interface JumpToQuestionRequest {
  targetQuestionId: string;
}
```

### Response
```typescript
interface JumpToQuestionResponse {
  targetQuestionId: string;
  message: string;
  progressPercentage: number;
}
```

---

## 26. Submit Response (Authenticated)
**Endpoint:** `POST /{surveyId}/responses/{responseId}/submit`

### Description
Submit a survey response for completion.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- `Idempotency-Key` (string?, Header) - Idempotency key
- Requires authentication

### Response
```typescript
interface SubmitResponseResponse {
  success: boolean;
  message: string;
  submittedAt: string;
  completionPercentage: number;
}
```

---

## 27. Cancel Response (Authenticated)
**Endpoint:** `POST /{surveyId}/responses/{responseId}/cancel`

### Description
Cancel a survey response.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- Requires authentication

### Response
```typescript
interface CancelResponseResponse {
  success: boolean;
  message: string;
  canceledAt: string;
}
```

---

## 28. AutoSave Answers (Authenticated)
**Endpoint:** `PATCH /{surveyId}/responses/{responseId}/autosave`

### Description
Auto-save answers for a response.

### Parameters
- `surveyId` (Guid, Path) - Survey identifier
- `responseId` (Guid, Path) - Response identifier
- Requires authentication

### Request Body
```typescript
interface AutoSaveAnswersRequest {
  answers: AutoSaveAnswerRequestDto[];
  mode: AutoSaveMode; // default: Merge
}

interface AutoSaveAnswerRequestDto {
  questionId: string;
  textAnswer?: string;
  selectedOptionIds?: string[];
}

enum AutoSaveMode {
  Merge,
  Overwrite
}
```

### Response
```typescript
interface AutoSaveAnswersResponse {
  success: boolean;
  message: string;
  savedAnswersCount: number;
}
```

---

## Common Data Types

### SurveyBasicInfoDto
```typescript
interface SurveyBasicInfoDto {
  id: string;
  title: string;
  description?: string;
  state: string;
  stateTextFa: string;
  isAcceptingResponses: boolean;
  totalQuestions: number;
  requiredQuestions: number;
  allowBackNavigation: boolean;
  allowMultipleSubmissions: boolean;
  maxAttemptsPerMember: number;
  coolDownSeconds?: number;
}
```

### QuestionDetailsDto
```typescript
interface QuestionDetailsDto {
  id: string;
  text: string;
  kind: string;
  kindTextFa: string;
  isRequired: boolean;
  order: number;
  options: QuestionOptionDto[];
}
```

### QuestionOptionDto
```typescript
interface QuestionOptionDto {
  id: string;
  text: string;
  order: number;
}
```

### QuestionAnswerDto
```typescript
interface QuestionAnswerDto {
  questionId: string;
  responseId: string;
  attemptNumber: number;
  textAnswer?: string;
  selectedOptionIds: string[];
  selectedOptions: QuestionAnswerOptionDto[];
  answeredAt?: string;
  answeredAtLocal?: string;
  isAnswered: boolean;
}
```

### QuestionAnswerOptionDto
```typescript
interface QuestionAnswerOptionDto {
  id: string;
  text: string;
}
```

### QuestionNavigationDto
```typescript
interface QuestionNavigationDto {
  currentIndex: number;
  totalQuestions: number;
  previousQuestion?: QuestionNavigationItemDto;
  nextQuestion?: QuestionNavigationItemDto;
  canGoPrevious: boolean;
  canGoNext: boolean;
  progressPercentage: number;
  questionsAnswered: number;
  questionsRemaining: number;
}
```

### QuestionNavigationItemDto
```typescript
interface QuestionNavigationItemDto {
  id: string;
  index: number;
  text: string;
  kind: string;
  kindTextFa: string;
  isRequired: boolean;
  isAnswered: boolean;
  isCurrent: boolean;
}
```

### UserResponseStatusDto
```typescript
interface UserResponseStatusDto {
  responseId: string;
  attemptNumber: number;
  attemptStatus: string;
  attemptStatusTextFa: string;
  startedAt?: string;
  startedAtLocal?: string;
  submittedAt?: string;
  submittedAtLocal?: string;
  questionsAnswered: number;
  questionsTotal: number;
  completionPercentage: number;
  isActive: boolean;
  isSubmitted: boolean;
  canContinue: boolean;
  nextActionText: string;
}
```

### PreviousQuestionDto
```typescript
interface PreviousQuestionDto {
  question: QuestionDetailsDto;
  index: number;
  userAnswer?: QuestionAnswerDto;
  isAnswered: boolean;
  canNavigateTo: boolean;
}
```

### PreviousQuestionsNavigationDto
```typescript
interface PreviousQuestionsNavigationDto {
  currentIndex: number;
  totalQuestions: number;
  previousQuestionsCount: number;
  morePreviousAvailable: number;
  canNavigatePrevious: boolean;
  progress: ProgressInfoDto;
}
```

### ProgressInfoDto
```typescript
interface ProgressInfoDto {
  questionsAnswered: number;
  questionsRemaining: number;
  progressPercentage: number;
  requiredQuestionsAnswered: number;
  requiredQuestionsRemaining: number;
}
```

### QuestionStatisticsDto
```typescript
interface QuestionStatisticsDto {
  questionId: string;
  totalResponses: number;
  answeredResponses: number;
  skippedResponses: number;
  answerRate: number;
  optionStatistics: OptionStatisticsDto[];
}
```

### OptionStatisticsDto
```typescript
interface OptionStatisticsDto {
  optionId: string;
  text: string;
  selectionCount: number;
  selectionPercentage: number;
}
```

### PaginationDto
```typescript
interface PaginationDto {
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}
```

---

## Error Responses

All endpoints return standardized error responses:

```typescript
interface ApplicationResult<T> {
  isSuccess: boolean;
  data?: T;
  errorCode?: string;
  errorMessageFa?: string;
  meta?: {
    correlationId?: string;
    timestamp?: string;
    [key: string]: any;
  };
}
```

### Common Error Codes
- `NOT_FOUND` - Resource not found
- `VALIDATION_FAILED` - Validation error
- `UNAUTHORIZED` - Authentication required
- `FORBIDDEN` - Access denied
- `CONFLICT` - Resource conflict
- `INTERNAL_ERROR` - Server error

---

## Authentication

Most endpoints require authentication. Include the authorization header:
```
Authorization: Bearer <token>
```

Anonymous endpoints:
- Get Survey Overview
- Get Survey Details
- Get Active Surveys
- Get Survey Questions
- Get Survey Questions Details
- Get Specific Question by Index
- Get Previous Questions

---

## Rate Limiting

API requests are rate-limited. Include appropriate delays between requests to avoid hitting rate limits.

---

## Idempotency

For sensitive operations (Start Response, Answer Question, Submit Response), include an `Idempotency-Key` header to ensure operations are idempotent.
