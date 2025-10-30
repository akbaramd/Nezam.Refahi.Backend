
namespace Nezam.Refahi.Web.ViewModels.Forms;

#region AJAX Response Models

/// <summary>
/// Base AJAX response model
/// </summary>
public class AjaxResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public object? Data { get; set; }
}

/// <summary>
/// Generic AJAX response model
/// </summary>
public class AjaxResponse<T> : AjaxResponse
{
    public new T? Data { get; set; }
}

#endregion

#region Form Management AJAX Models

/// <summary>
/// AJAX response for form operations
/// </summary>
public class FormAjaxResponse : AjaxResponse<FormDetailsViewModel>
{
}

/// <summary>
/// AJAX response for form list
/// </summary>
public class FormListAjaxResponse : AjaxResponse<List<FormListItemViewModel>>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// AJAX response for form field operations
/// </summary>
public class FormFieldAjaxResponse : AjaxResponse<FormFieldViewModel>
{
}

/// <summary>
/// AJAX response for form field list
/// </summary>
public class FormFieldListAjaxResponse : AjaxResponse<List<FormFieldViewModel>>
{
}

/// <summary>
/// AJAX response for form group operations
/// </summary>
public class FormGroupAjaxResponse : AjaxResponse<FormGroupViewModel>
{
}

/// <summary>
/// AJAX response for form group list
/// </summary>
public class FormGroupListAjaxResponse : AjaxResponse<List<FormGroupViewModel>>
{
}

/// <summary>
/// AJAX response for field option operations
/// </summary>
public class FieldOptionAjaxResponse : AjaxResponse<FieldOptionViewModel>
{
}

/// <summary>
/// AJAX response for field option list
/// </summary>
public class FieldOptionListAjaxResponse : AjaxResponse<List<FieldOptionViewModel>>
{
}

#endregion

#region Form Submission Models

/// <summary>
/// AJAX response for form submission
/// </summary>
public class FormSubmissionAjaxResponse : AjaxResponse
{
    public Guid? ResponseId { get; set; }
    public Guid FormId { get; set; }
    public Guid FormVersionId { get; set; }
    public DateTime SubmittedAt { get; set; }
}

/// <summary>
/// AJAX response for form response list
/// </summary>
public class FormResponseListAjaxResponse : AjaxResponse<List<FormResponseViewModel>>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// ViewModel for form response
/// </summary>
public class FormResponseViewModel
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public Guid FormVersionId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public List<FieldResponseViewModel> FieldResponses { get; set; } = new();
}

/// <summary>
/// ViewModel for field response
/// </summary>
public class FieldResponseViewModel
{
    public Guid FieldId { get; set; }
    public string FieldLabel { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public List<string> FileUrls { get; set; } = new();
}

#endregion

#region Utility Models

/// <summary>
/// Model for field details request
/// </summary>
public class FieldDetailsRequest
{
    public Guid FieldId { get; set; }
}

/// <summary>
/// Model for field details response
/// </summary>
public class FieldDetailsResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public FormFieldViewModel? Field { get; set; }
}

/// <summary>
/// Model for form statistics
/// </summary>
public class FormStatisticsViewModel
{
    public Guid FormId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalResponses { get; set; }
    public int TodayResponses { get; set; }
    public int ThisWeekResponses { get; set; }
    public int ThisMonthResponses { get; set; }
    public DateTime LastResponseAt { get; set; }
    public List<FieldStatisticsViewModel> FieldStatistics { get; set; } = new();
}

/// <summary>
/// Model for field statistics
/// </summary>
public class FieldStatisticsViewModel
{
    public Guid FieldId { get; set; }
    public string FieldLabel { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public int ResponseCount { get; set; }
    public int CompletionRate { get; set; } // Percentage
    public List<OptionStatisticsViewModel> OptionStatistics { get; set; } = new();
}

/// <summary>
/// Model for option statistics
/// </summary>
public class OptionStatisticsViewModel
{
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionValue { get; set; } = string.Empty;
    public int SelectionCount { get; set; }
    public int SelectionPercentage { get; set; }
}

#endregion