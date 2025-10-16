using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Infrastructure.Persistence;

namespace Nezam.Refahi.Surveying.Infrastructure.Seeding;

/// <summary>
/// Seeder for Survey module data
/// </summary>
public class SurveySeeder
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<SurveySeeder> _logger;

    public SurveySeeder(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<SurveySeeder> logger)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting Survey data seeding...");

            // Check if data already exists
            var existingCount = await _surveyRepository.CountAsync();
            if (existingCount > 0)
            {
                _logger.LogInformation("Survey data already exists. Skipping seeding.");
                return;
            }

            // Create surveys
            var surveys = await CreateSurveysAsync();
            
            // Add surveys to repository
            foreach (var survey in surveys)
            {
                await _surveyRepository.AddAsync(survey);
            }

            // Save changes using Unit of Work
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Survey data seeding completed successfully. Created {Count} surveys.", surveys.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Survey data seeding");
            throw;
        }
    }

    private Task<List<Survey>> CreateSurveysAsync()
    {
        var surveys = new List<Survey>();

        // Survey 1: Employee Satisfaction Survey (رضایت شغلی)
        var employeeSatisfactionSurvey = CreateEmployeeSatisfactionSurvey();
        surveys.Add(employeeSatisfactionSurvey);

        // Survey 2: Organization Culture Survey (فرهنگ سازمانی)
        var organizationCultureSurvey = CreateOrganizationCultureSurvey();
        surveys.Add(organizationCultureSurvey);

        // Survey 3: Service Quality Survey (کیفیت خدمات)
        var serviceQualitySurvey = CreateServiceQualitySurvey();
        surveys.Add(serviceQualitySurvey);

        return Task.FromResult(surveys);
    }

    private Survey CreateEmployeeSatisfactionSurvey()
    {
        var survey = new Survey(
            "نظرسنجی رضایت شغلی کارکنان",
            "این نظرسنجی برای ارزیابی سطح رضایت کارکنان از محیط کار، همکاران، مدیریت و شرایط کاری طراحی شده است.",
            false,
            new ParticipationPolicy(
                maxAttemptsPerMember: 1,
                allowMultipleSubmissions: false,
                coolDownSeconds: null
            )
        );
        

        // Add features
        survey.AddFeature("has_license", "دارای پروانه"); 
 



        // Add questions
        CreateEmployeeSatisfactionQuestions(survey);
        survey.Activate();
        return survey;
    }

    private Survey CreateOrganizationCultureSurvey()
    {
        var survey = new Survey(
            "نظرسنجی فرهنگ سازمانی",
            "این نظرسنجی برای بررسی و ارزیابی فرهنگ سازمانی، ارزش‌ها، هنجارها و رفتارهای حاکم بر محیط کار طراحی شده است.",
            true,
            new ParticipationPolicy(
                maxAttemptsPerMember: 1,
                allowMultipleSubmissions: false,
                coolDownSeconds: null
            )
        );
 

        // Add features
        survey.AddFeature("has_license", "دارای پروانه");  

        // Add questions
        CreateOrganizationCultureQuestions(survey);
        survey.Activate();
        return survey;
    }

    private Survey CreateServiceQualitySurvey()
    {
        var survey = new Survey(
            "نظرسنجی کیفیت خدمات",
            "این نظرسنجی برای ارزیابی کیفیت خدمات ارائه شده، رضایت مشتریان و بهبود مستمر خدمات طراحی شده است.",
            true,
            new ParticipationPolicy(
                maxAttemptsPerMember: 2,
                allowMultipleSubmissions: true,
                coolDownSeconds: 3600 // 1 hour
            )
        );


        // Add features
        survey.AddFeature("has_license", "دارای پروانه");



        // Add questions
        CreateServiceQualityQuestions(survey);
        survey.Activate();
        return survey;
    }

    private void CreateEmployeeSatisfactionQuestions(Survey survey)
    {
        // Question 1: Multiple Choice - Job Satisfaction Level
        var q1 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceSingle, "سطح رضایت شما از شغل فعلی‌تان چقدر است؟", 1, true)); 
        q1.AddOption("خیلی راضی", 1);
        q1.AddOption("راضی", 2);
        q1.AddOption("نسبتاً راضی", 3);
        q1.AddOption("ناراضی", 4);
        q1.AddOption("خیلی ناراضی", 5);

        // Question 2: Fixed MCQ4 - Work Environment
        var q2 = survey.AddQuestion(new QuestionSpecification(QuestionKind.FixedMCQ4, "محیط کاری شما چگونه است؟", 2, true));
        q2.AddOption("عالی", 1);
        q2.AddOption("خوب", 2);
        q2.AddOption("متوسط", 3);
        q2.AddOption("ضعیف", 4);

        // Question 3: Multiple Choice - Management Quality
        var q3 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceMulti, "کدام موارد در مورد مدیریت شما صادق است؟ (می‌توانید چند گزینه انتخاب کنید)", 3, false));
        q3.AddOption("مدیریت حمایت‌گر است", 1);
        q3.AddOption("مدیریت منصف است", 2);
        q3.AddOption("مدیریت ارتباط خوبی دارد", 3);
        q3.AddOption("مدیریت به پیشرفت کارکنان اهمیت می‌دهد", 4);
        q3.AddOption("مدیریت بازخورد مناسب می‌دهد", 5);

        // Question 4: Textual - Suggestions
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "پیشنهادات شما برای بهبود محیط کاری چیست؟", 4, false));
    }

    private void CreateOrganizationCultureQuestions(Survey survey)
    {
        // Question 1: Fixed MCQ4 - Communication
        var q1 = survey.AddQuestion(new QuestionSpecification(QuestionKind.FixedMCQ4, "سطح ارتباطات در سازمان چگونه است؟", 1, true));
        q1.AddOption("عالی", 1);
        q1.AddOption("خوب", 2);
        q1.AddOption("متوسط", 3);
        q1.AddOption("ضعیف", 4);

        // Question 2: Multiple Choice - Teamwork
        var q2 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceSingle, "سطح همکاری تیمی در سازمان چگونه است؟", 2, true));
        q2.AddOption("خیلی خوب", 1);
        q2.AddOption("خوب", 2);
        q2.AddOption("متوسط", 3);
        q2.AddOption("ضعیف", 4);

        // Question 3: Multiple Choice - Values
        var q3 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceMulti, "کدام ارزش‌های سازمانی در محیط کار شما رعایت می‌شود؟", 3, false));
        q3.AddOption("صداقت و شفافیت", 1);
        q3.AddOption("احترام متقابل", 2);
        q3.AddOption("عدالت و برابری", 3);
        q3.AddOption("ابتکار و نوآوری", 4);
        q3.AddOption("مسئولیت‌پذیری", 5);

        // Question 4: Textual - Culture Improvement
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "برای بهبود فرهنگ سازمانی چه پیشنهادی دارید؟", 4, false));
    }

    private void CreateServiceQualityQuestions(Survey survey)
    {
        // Question 1: Fixed MCQ4 - Service Quality
        var q1 = survey.AddQuestion(new QuestionSpecification(QuestionKind.FixedMCQ4, "کیفیت خدمات ارائه شده چگونه است؟", 1, true));
        q1.AddOption("عالی", 1);
        q1.AddOption("خوب", 2);
        q1.AddOption("متوسط", 3);
        q1.AddOption("ضعیف", 4);

        // Question 2: Multiple Choice - Response Time
        var q2 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceSingle, "سرعت پاسخگویی به درخواست‌های شما چگونه است؟", 2, true));
        q2.AddOption("خیلی سریع", 1);
        q2.AddOption("سریع", 2);
        q2.AddOption("متوسط", 3);
        q2.AddOption("کند", 4);

        // Question 3: Multiple Choice - Service Aspects
        var q3 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceMulti, "کدام جنبه‌های خدمات رضایت‌بخش است؟", 3, false));
        q3.AddOption("کیفیت خدمات", 1);
        q3.AddOption("سرعت ارائه", 2);
        q3.AddOption("دوستی و احترام", 3);
        q3.AddOption("دقت و صحت", 4);
        q3.AddOption("قیمت مناسب", 5);

        // Question 4: Textual - Service Improvement
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "برای بهبود کیفیت خدمات چه پیشنهادی دارید؟", 4, false));
    }
}
