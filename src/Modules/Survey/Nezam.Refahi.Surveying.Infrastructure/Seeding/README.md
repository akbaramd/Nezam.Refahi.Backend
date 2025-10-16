# Survey Data Seeding

This directory contains the seeding functionality for the Survey module.

## Overview

The seeding system automatically creates sample survey data when the application starts up. This includes:

- **3 Survey Types**: Employee Satisfaction, Organization Culture, and Service Quality
- **4 Questions per Survey**: Mix of Fixed MCQ4, Single Choice, Multiple Choice, and Textual questions
- **Persian Language**: All questions and options are in Persian (Farsi)
- **Organization Focus**: Questions are designed for organizational surveys

## Files

### `SurveySeeder.cs`
Main seeding class that creates the survey data:
- Creates 3 different surveys with appropriate metadata
- Adds features and capabilities to each survey
- Creates questions with options for each survey type

### `SurveySeedingHostedService.cs`
Hosted service that runs the seeding on application startup:
- Ensures database exists
- Checks if data already exists (prevents duplicate seeding)
- Runs the seeding process
- Handles errors gracefully

### `SurveySeedingExtensions.cs`
Extension methods for manual seeding (if needed):
- `SeedSurveyDataAsync()`: Manual seeding method
- `SeedSurveyDataOnStartupAsync()`: Startup seeding method

## Survey Types

### 1. Employee Satisfaction Survey (نظرسنجی رضایت شغلی کارکنان)
- **Type**: Non-anonymous
- **Questions**: Job satisfaction, work environment, management quality, suggestions
- **Features**: employee_satisfaction, hr_management
- **Capabilities**: feedback_collection, performance_evaluation

### 2. Organization Culture Survey (نظرسنجی فرهنگ سازمانی)
- **Type**: Anonymous
- **Questions**: Communication, teamwork, organizational values, improvement suggestions
- **Features**: organizational_culture, work_environment
- **Capabilities**: culture_assessment, improvement_planning

### 3. Service Quality Survey (نظرسنجی کیفیت خدمات)
- **Type**: Anonymous
- **Questions**: Service quality, response time, service aspects, improvement suggestions
- **Features**: service_quality, customer_satisfaction
- **Capabilities**: service_evaluation, quality_improvement

## Question Types

Each survey includes 4 questions with different types:

1. **Fixed MCQ4**: 4-option multiple choice questions
2. **Single Choice**: Single selection questions
3. **Multiple Choice**: Multi-selection questions
4. **Textual**: Open-ended text questions

## Usage

The seeding runs automatically when the application starts. No manual intervention is required.

If you need to run seeding manually, you can use the extension methods:

```csharp
// Manual seeding
await serviceProvider.SeedSurveyDataAsync();

// Startup seeding
await serviceProvider.SeedSurveyDataOnStartupAsync();
```

## Configuration

The seeding is configured in `NezamRefahiSurveyingInfrastructureModule.cs`:

```csharp
// Register seeding hosted service
context.Services.AddHostedService<SurveySeedingHostedService>();
```

## Notes

- Seeding only runs if no surveys exist in the database
- All text is in Persian (Farsi) as requested
- Questions are designed for organizational surveys
- The system prevents duplicate seeding automatically
