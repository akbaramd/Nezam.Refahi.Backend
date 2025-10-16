namespace Nezam.Refahi.Surveying.Domain.Enums;

/// <summary>
/// Question kind enumeration representing different types of survey questions
/// </summary>
public enum QuestionKind
{
    /// <summary>
    /// Fixed multiple choice question with exactly 4 options
    /// </summary>
    FixedMCQ4 = 0,

    /// <summary>
    /// Single choice question (radio buttons)
    /// </summary>
    ChoiceSingle = 1,

    /// <summary>
    /// Multiple choice question (checkboxes)
    /// </summary>
    ChoiceMulti = 2,

    /// <summary>
    /// Textual question (text input)
    /// </summary>
    Textual = 3
}
