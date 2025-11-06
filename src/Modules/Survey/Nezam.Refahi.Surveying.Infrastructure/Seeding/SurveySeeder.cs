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



        // Survey 4: Engineering Organization Educational Needs Survey (شناسایی نیازهای آموزشی)
        var educationalNeedsSurvey = CreateEducationalNeedsSurvey();
        surveys.Add(educationalNeedsSurvey);

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
        survey.Publish();
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
        survey.Publish();
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
        survey.Publish();
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

    private Survey CreateEducationalNeedsSurvey()
    {
        var survey = new Survey(
            "شناسایی نیازهای آموزشی اعضای محترم سازمان نظام مهندسی ساختمان استان آذربایجان‌غربی",
            "به اطلاع اعضای محترم سازمان نظام مهندسی ساختمان استان آذربایجان‌غربی می‌رساند با عنایت به اهمیت به‌روز بودن دانش و مهارت‌های فنی مهندسان و متخصصان ساختمان و با هدف ارتقاء کیفیت ساخت و ساز در سطح استان، سازمان استان در نظر دارد نسبت به شناسایی نیازهای آموزشی اعضای محترم در زمینه دوره‌های بازآموزی و ترویجی اقدام نماید. از تمامی اعضای محترم سازمان دعوت می‌شود که سمینارها یا دوره‌های پیشنهادی خود را به سازمان استان اعلام فرمایند. لازم به ذکر است پس از دریافت کلیه نظرات، کمیته آموزش سازمان نسبت به برگزاری این دوره‌ها به روش مقتضی در ارومیه و دیگر شهرستان‌ها در قالب یک تقویم آموزشی اقدام خواهد نمود.",
            false,
            new ParticipationPolicy(
                maxAttemptsPerMember: 1,
                allowMultipleSubmissions: false,
                coolDownSeconds: null
            )
        );

        // Add questions
        CreateEducationalNeedsQuestions(survey);
        survey.Publish();
        return survey;
    }

    private void CreateEducationalNeedsQuestions(Survey survey)
    {
        // Question 1: Name (Textual - Required)
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "نام", 1, true));

        // Question 2: Last Name (Textual - Required)
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "نام خانوادگی", 2, true));

        // Question 3: Field/Discipline (Single Choice - Required)
        var q3 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceSingle, "رشته", 3, true));
        q3.AddOption("عمران", 1);
        q3.AddOption("معماری", 2);
        q3.AddOption("تأسیسات مکانیکی", 3);
        q3.AddOption("تأسیسات برقی", 4);
        q3.AddOption("نقشه‌برداری", 5);
        q3.AddOption("شهرسازی", 6);
        q3.AddOption("سایر", 7);

        // Question 4: City (Single Choice - Required)
        var q4 = survey.AddQuestion(new QuestionSpecification(QuestionKind.ChoiceSingle, "شهرستان مربوطه", 4, true));
        q4.AddOption("ارومیه", 1);
        q4.AddOption("خوی", 2);
        q4.AddOption("مهاباد", 3);
        q4.AddOption("میاندوآب", 4);
        q4.AddOption("بوکان", 5);
        q4.AddOption("سلماس", 6);
        q4.AddOption("پیرانشهر", 7);
        q4.AddOption("نقده", 8);
        q4.AddOption("شاهین‌دژ", 9);
        q4.AddOption("تکاب", 10);
        q4.AddOption("اشنویه", 11);
        q4.AddOption("ماکو", 12);
        q4.AddOption("سایر", 13);

        // Question 5: Course/Seminar Title (Textual - Required)
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "عنوان دوره یا سمینار پیشنهادی", 5, true));

        // Question 6: Suggested Instructors (Textual - Optional)
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "اساتید پیشنهادی (در صورت تمایل)", 6, false));

        // Question 7: Additional Descriptions (Textual - Optional)
        survey.AddQuestion(new QuestionSpecification(QuestionKind.Textual, "توضیحات تکمیلی (در صورت نیاز)", 7, false));
    }
}
