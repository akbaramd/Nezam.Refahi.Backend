using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Surveis;

/// <summary>
/// An answer to a single question within a response.
/// </summary>
public class SurveyAnswer : BaseEntity
{
  public Guid ResponseId { get; private set; }
  public SurveyResponse Response { get; private set; } = null!;

  public Guid QuestionId { get; private set; }
  public SurveyQuestion Question { get; private set; } = null!;

  public Guid? OptionId { get; private set; }
  public SurveyOptions? Option { get; private set; }

  public string? TextAnswer { get; private set; }
  public string? FilePath { get; private set; }

  // Private constructor for EF Core
  private SurveyAnswer() : base() { }

  public SurveyAnswer(SurveyResponse response, SurveyQuestion question, SurveyOptions? option = null,
                  string? textAnswer = null, string? filePath = null) : base()
  {
    Response = response ?? throw new ArgumentNullException(nameof(response));
    ResponseId = response.Id;

    Question = question ?? throw new ArgumentNullException(nameof(question));
    QuestionId = question.Id;

    Option = option;
    OptionId = option?.Id;

    // Ensure the answer is valid for the question type
    ValidateAnswer(question, option, textAnswer, filePath);

    TextAnswer = textAnswer;
    FilePath = filePath;
  }

  private void ValidateAnswer(SurveyQuestion question, SurveyOptions? option, string? textAnswer, string? filePath)
  {
    switch (question.Type)
    {
      case QuestionType.SingleChoice:
        if (option == null)
          throw new ArgumentException("Single choice question requires an option selection");
        if (!string.IsNullOrEmpty(textAnswer) || !string.IsNullOrEmpty(filePath))
          throw new ArgumentException("Single choice question cannot have text or file answers");
        break;

      case QuestionType.MultipleChoice:
        if (option == null)
          throw new ArgumentException("Multiple choice question requires an option selection");
        if (!string.IsNullOrEmpty(textAnswer) || !string.IsNullOrEmpty(filePath))
          throw new ArgumentException("Multiple choice question cannot have text or file answers");
        break;

      case QuestionType.Rating:
        if (option == null)
          throw new ArgumentException("Rating question requires an option selection");
        if (!string.IsNullOrEmpty(textAnswer) || !string.IsNullOrEmpty(filePath))
          throw new ArgumentException("Rating question cannot have text or file answers");
        break;

      case QuestionType.OpenText:
        if (option != null)
          throw new ArgumentException("Open text question cannot have option selection");
        if (string.IsNullOrEmpty(textAnswer))
          throw new ArgumentException("Open text question requires text answer");
        if (!string.IsNullOrEmpty(filePath))
          throw new ArgumentException("Open text question cannot have file answers");
        break;

      case QuestionType.FileUpload:
        if (option != null)
          throw new ArgumentException("File upload question cannot have option selection");
        if (!string.IsNullOrEmpty(textAnswer))
          throw new ArgumentException("File upload question cannot have text answers");
        if (string.IsNullOrEmpty(filePath))
          throw new ArgumentException("File upload question requires file path");
        break;

      default:
        throw new ArgumentException("Invalid question type");
    }
  }

  public void UpdateTextAnswer(string textAnswer)
  {
    if (Question.Type != QuestionType.OpenText)
      throw new InvalidOperationException("Cannot update text answer for non-text question type");

    TextAnswer = textAnswer;
    UpdateModifiedAt();
  }

  public void UpdateFileAnswer(string filePath)
  {
    if (Question.Type != QuestionType.FileUpload)
      throw new InvalidOperationException("Cannot update file answer for non-file question type");

    FilePath = filePath;
    UpdateModifiedAt();
  }

  public void UpdateOptionAnswer(SurveyOptions option)
  {
    if (Question.Type != QuestionType.SingleChoice &&
        Question.Type != QuestionType.MultipleChoice &&
        Question.Type != QuestionType.Rating)
      throw new InvalidOperationException("Cannot update option for non-choice question type");

    Option = option ?? throw new ArgumentNullException(nameof(option));
    OptionId = option.Id;
    UpdateModifiedAt();
  }
}