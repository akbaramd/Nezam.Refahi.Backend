using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Application.Mappers;

/// <summary>
/// Professional extension methods for mapping remaining Domain entities to DTOs
/// Provides clean, error-free mapping with proper null handling
/// </summary>
public static class AdditionalMappers
{
    #region QuestionAnswer Mappings

    /// <summary>
    /// Maps QuestionAnswer entity to simple QuestionAnswerDto (for lists)
    /// </summary>
    public static QuestionAnswerDto ToDto(this QuestionAnswer questionAnswer)
    {
        ArgumentNullException.ThrowIfNull(questionAnswer);

        return new QuestionAnswerDto
        {
            Id = questionAnswer.Id,
            ResponseId = questionAnswer.ResponseId,
            QuestionId = questionAnswer.QuestionId,
            RepeatIndex = questionAnswer.RepeatIndex,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
            SelectedOptions = questionAnswer.SelectedOptions.Select(so => so.ToDto()).ToList(),
            HasAnswer = questionAnswer.HasAnswer()
        };
    }

    /// <summary>
    /// Maps QuestionAnswer entity to detailed QuestionAnswerDetailsDto (for single queries)
    /// </summary>
    public static QuestionAnswerDetailsDto ToDetailsDto(this QuestionAnswer questionAnswer)
    {
        ArgumentNullException.ThrowIfNull(questionAnswer);

        return new QuestionAnswerDetailsDto
        {
            Id = questionAnswer.Id,
            ResponseId = questionAnswer.ResponseId,
            QuestionId = questionAnswer.QuestionId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
            SelectedOptions = questionAnswer.SelectedOptions.Select(so => new QuestionOptionDto
            {
                Id = so.Id,
                QuestionId = so.QuestionAnswerId, // Map from QuestionAnswerOption to QuestionOption
                Text = so.OptionText,
                Order = 0, // TODO: Get from question option when available
                IsActive = true,
                IsSelected = true
            }).ToList(),
            IsAnswered = questionAnswer.HasAnswer(),
            IsComplete = questionAnswer.HasAnswer(),
            Question = null // Will be set by caller if needed
        };
    }

    #endregion

    #region QuestionAnswerOption Mappings

    /// <summary>
    /// Maps QuestionAnswerOption entity to QuestionAnswerOptionDto
    /// </summary>
    public static QuestionAnswerOptionDto ToDto(this QuestionAnswerOption answerOption)
    {
        ArgumentNullException.ThrowIfNull(answerOption);

        return new QuestionAnswerOptionDto
        {
            Id = answerOption.Id,
            QuestionAnswerId = answerOption.QuestionAnswerId,
            OptionId = answerOption.OptionId,
            OptionText = answerOption.OptionText
        };
    }

    #endregion

    #region QuestionOption Mappings

    /// <summary>
    /// Maps QuestionOption entity to QuestionOptionDto
    /// </summary>
    public static QuestionOptionDto ToDto(this QuestionOption questionOption)
    {
        ArgumentNullException.ThrowIfNull(questionOption);

        return new QuestionOptionDto
        {
            Id = questionOption.Id,
            QuestionId = questionOption.QuestionId,
            Text = questionOption.Text,
            Order = questionOption.Order,
            IsActive = questionOption.IsActive,
            IsSelected = false // Will be set by caller based on user selection
        };
    }

    #endregion

    #region SurveyFeature Mappings

    /// <summary>
    /// Maps SurveyFeature entity to SurveyFeatureDto
    /// </summary>
    public static SurveyFeatureDto ToDto(this SurveyFeature surveyFeature)
    {
        ArgumentNullException.ThrowIfNull(surveyFeature);

        return new SurveyFeatureDto
        {
            Id = surveyFeature.Id,
            SurveyId = surveyFeature.SurveyId,
            FeatureCode = surveyFeature.FeatureCode,
            FeatureTitleSnapshot = surveyFeature.FeatureTitleSnapshot
        };
    }

    #endregion

    #region SurveyCapability Mappings

    /// <summary>
    /// Maps SurveyCapability entity to SurveyCapabilityDto
    /// </summary>
    public static SurveyCapabilityDto ToDto(this SurveyCapability surveyCapability)
    {
        ArgumentNullException.ThrowIfNull(surveyCapability);

        return new SurveyCapabilityDto
        {
            Id = surveyCapability.Id,
            SurveyId = surveyCapability.SurveyId,
            CapabilityCode = surveyCapability.CapabilityCode,
            CapabilityTitleSnapshot = surveyCapability.CapabilityTitleSnapshot
        };
    }

    #endregion

    #region ParticipantInfo Mappings

    /// <summary>
    /// Maps ParticipantInfo value object to ParticipantInfoDto
    /// </summary>
    public static ParticipantInfoDto ToDto(this ParticipantInfo participantInfo)
    {
        ArgumentNullException.ThrowIfNull(participantInfo);

        return new ParticipantInfoDto
        {
            MemberId = participantInfo.MemberId,
            ParticipantHash = participantInfo.ParticipantHash,
            NationalCode = null, // TODO: Extract from demography when available
            FullName = null, // TODO: Extract from demography when available
            IsAnonymous = participantInfo.IsAnonymous,
            DemographyData = null // TODO: Map from DemographySnapshot when available
        };
    }

    #endregion

    #region Collection Mappings

    /// <summary>
    /// Maps collection of Survey entities to simple DTOs (for lists)
    /// </summary>
    public static List<SurveyDto> ToDtoList(this IEnumerable<Survey> surveys, Dictionary<Guid, Response>? userResponses = null)
    {
        ArgumentNullException.ThrowIfNull(surveys);

        return surveys.Select(survey => 
        {
            var userResponse = userResponses?.GetValueOrDefault(survey.Id);
            return survey.ToDto(userResponse);
        }).ToList();
    }

    /// <summary>
    /// Maps collection of Survey entities to detailed DTOs (for single queries)
    /// </summary>
    public static List<SurveyDetailsDto> ToDetailsDtoList(this IEnumerable<Survey> surveys, Dictionary<Guid, Response>? userResponses = null)
    {
        ArgumentNullException.ThrowIfNull(surveys);

        return surveys.Select(survey => 
        {
            var userResponse = userResponses?.GetValueOrDefault(survey.Id);
            return survey.ToDetailsDto(userResponse);
        }).ToList();
    }

    /// <summary>
    /// Maps collection of Question entities to simple DTOs (for lists)
    /// </summary>
    public static List<QuestionDto> ToDtoList(this IEnumerable<Question> questions, Dictionary<Guid, QuestionAnswer>? userAnswers = null)
    {
        ArgumentNullException.ThrowIfNull(questions);

        return questions.Select(question => 
        {
            var userAnswer = userAnswers?.GetValueOrDefault(question.Id);
            return question.ToDto(userAnswer);
        }).ToList();
    }

    /// <summary>
    /// Maps collection of Question entities to detailed DTOs (for single queries)
    /// </summary>
    public static List<QuestionDetailsDto> ToDetailsDtoList(this IEnumerable<Question> questions, Dictionary<Guid, List<QuestionAnswer>>? userAnswers = null)
    {
        ArgumentNullException.ThrowIfNull(questions);

        return questions.Select(question => 
        {
            var answers = userAnswers?.GetValueOrDefault(question.Id) ?? new List<QuestionAnswer>();
            return question.ToDetailsDto(answers);
        }).ToList();
    }

    /// <summary>
    /// Maps collection of Response entities to simple DTOs (for lists)
    /// </summary>
    public static List<ResponseDto> ToDtoList(this IEnumerable<Response> responses)
    {
        ArgumentNullException.ThrowIfNull(responses);

        return responses.Select(response => response.ToDto()).ToList();
    }

    /// <summary>
    /// Maps collection of Response entities to detailed DTOs (for single queries)
    /// </summary>
    public static List<ResponseDetailsDto> ToDetailsDtoList(this IEnumerable<Response> responses)
    {
        ArgumentNullException.ThrowIfNull(responses);

        return responses.Select(response => response.ToDetailsDto()).ToList();
    }

    /// <summary>
    /// Maps collection of QuestionAnswer entities to simple DTOs (for lists)
    /// </summary>
    public static List<QuestionAnswerDto> ToDtoList(this IEnumerable<QuestionAnswer> questionAnswers)
    {
        ArgumentNullException.ThrowIfNull(questionAnswers);

        return questionAnswers.Select(qa => qa.ToDto()).ToList();
    }

    /// <summary>
    /// Maps collection of QuestionAnswer entities to detailed DTOs (for single queries)
    /// </summary>
    public static List<QuestionAnswerDetailsDto> ToDetailsDtoList(this IEnumerable<QuestionAnswer> questionAnswers)
    {
        ArgumentNullException.ThrowIfNull(questionAnswers);

        return questionAnswers.Select(qa => qa.ToDetailsDto()).ToList();
    }

    /// <summary>
    /// Maps collection of QuestionOption entities to DTOs
    /// </summary>
    public static List<QuestionOptionDto> ToDtoList(this IEnumerable<QuestionOption> questionOptions)
    {
        ArgumentNullException.ThrowIfNull(questionOptions);

        return questionOptions.Select(qo => qo.ToDto()).ToList();
    }

    /// <summary>
    /// Maps collection of SurveyFeature entities to DTOs
    /// </summary>
    public static List<SurveyFeatureDto> ToDtoList(this IEnumerable<SurveyFeature> surveyFeatures)
    {
        ArgumentNullException.ThrowIfNull(surveyFeatures);

        return surveyFeatures.Select(sf => sf.ToDto()).ToList();
    }

    /// <summary>
    /// Maps collection of SurveyCapability entities to DTOs
    /// </summary>
    public static List<SurveyCapabilityDto> ToDtoList(this IEnumerable<SurveyCapability> surveyCapabilities)
    {
        ArgumentNullException.ThrowIfNull(surveyCapabilities);

        return surveyCapabilities.Select(sc => sc.ToDto()).ToList();
    }

    #endregion

    #region Safe Mapping Helpers

    /// <summary>
    /// Safely maps a nullable Survey to DTO, returns null if survey is null
    /// </summary>
    public static SurveyDto? ToDtoSafe(this Survey? survey, Response? userResponse = null)
    {
        return survey?.ToDto(userResponse);
    }

    /// <summary>
    /// Safely maps a nullable Survey to detailed DTO, returns null if survey is null
    /// </summary>
    public static SurveyDetailsDto? ToDetailsDtoSafe(this Survey? survey, Response? userResponse = null)
    {
        return survey?.ToDetailsDto(userResponse);
    }

    /// <summary>
    /// Safely maps a nullable Question to DTO, returns null if question is null
    /// </summary>
    public static QuestionDto? ToDtoSafe(this Question? question, QuestionAnswer? userAnswer = null)
    {
        return question?.ToDto(userAnswer);
    }

    /// <summary>
    /// Safely maps a nullable Question to detailed DTO, returns null if question is null
    /// </summary>
    public static QuestionDetailsDto? ToDetailsDtoSafe(this Question? question, List<QuestionAnswer>? userAnswers = null)
    {
        return question?.ToDetailsDto(userAnswers);
    }

    /// <summary>
    /// Safely maps a nullable Response to DTO, returns null if response is null
    /// </summary>
    public static ResponseDto? ToDtoSafe(this Response? response)
    {
        return response?.ToDto();
    }

    /// <summary>
    /// Safely maps a nullable Response to detailed DTO, returns null if response is null
    /// </summary>
    public static ResponseDetailsDto? ToDetailsDtoSafe(this Response? response)
    {
        return response?.ToDetailsDto();
    }

    #endregion
}