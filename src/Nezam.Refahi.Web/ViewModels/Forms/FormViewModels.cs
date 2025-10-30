using System.ComponentModel.DataAnnotations;

namespace Nezam.Refahi.Web.ViewModels.Forms;

#region Create and Edit Models

/// <summary>
/// ViewModel for creating a new form
/// </summary>
public class CreateFormViewModel
{
    [Required(ErrorMessage = "عنوان فرم الزامی است")]
    [StringLength(200, ErrorMessage = "عنوان فرم نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "عنوان فرم")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "توضیحات فرم نمی‌تواند بیشتر از 1000 کاراکتر باشد")]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }
}

/// <summary>
/// ViewModel for editing form basic information
/// </summary>
public class EditFormViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "عنوان فرم الزامی است")]
    [StringLength(200, ErrorMessage = "عنوان فرم نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "عنوان فرم")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "توضیحات فرم نمی‌تواند بیشتر از 1000 کاراکتر باشد")]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "وضعیت")]
    public string Status { get; set; } = string.Empty;

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for adding a form group
/// </summary>
public class AddFormGroupViewModel
{
    [Required(ErrorMessage = "عنوان گروه الزامی است")]
    [StringLength(200, ErrorMessage = "عنوان گروه نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "عنوان گروه")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "توضیحات گروه نمی‌تواند بیشتر از 500 کاراکتر باشد")]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "گروه والد")]
    public Guid? ParentGroupId { get; set; }

    public List<FormGroupSelectItem> AvailableGroups { get; set; } = new();
}

/// <summary>
/// ViewModel for adding a form field
/// </summary>
public class AddFormFieldViewModel
{
    [Required(ErrorMessage = "برچسب فیلد الزامی است")]
    [StringLength(200, ErrorMessage = "برچسب فیلد نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "برچسب فیلد")]
    public string Label { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع فیلد الزامی است")]
    [Display(Name = "نوع فیلد")]
    public string FieldType { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "متن راهنما نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "متن راهنما")]
    public string? Placeholder { get; set; }

    [StringLength(500, ErrorMessage = "متن راهنما نمی‌تواند بیشتر از 500 کاراکتر باشد")]
    [Display(Name = "متن راهنما")]
    public string? HelpText { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
    [Display(Name = "ترتیب نمایش")]
    public int Order { get; set; }

    [Display(Name = "گروه")]
    public Guid? GroupId { get; set; }

    public List<FormGroupSelectItem> AvailableGroups { get; set; } = new();
    public List<FieldTypeSelectItem> AvailableFieldTypes { get; set; } = new();
}

/// <summary>
/// ViewModel for editing a form field
/// </summary>
public class EditFormFieldViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "برچسب فیلد الزامی است")]
    [StringLength(200, ErrorMessage = "برچسب فیلد نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "برچسب فیلد")]
    public string Label { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "متن راهنما نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "متن راهنما")]
    public string? Placeholder { get; set; }

    [StringLength(500, ErrorMessage = "متن راهنما نمی‌تواند بیشتر از 500 کاراکتر باشد")]
    [Display(Name = "متن راهنما")]
    public string? HelpText { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
    [Display(Name = "ترتیب نمایش")]
    public int Order { get; set; }

    [Display(Name = "قابل مشاهده")]
    public bool IsVisible { get; set; } = true;

    [Display(Name = "اجباری")]
    public bool IsRequired { get; set; }

    [Display(Name = "حداقل طول")]
    public int? MinLength { get; set; }

    [Display(Name = "حداکثر طول")]
    public int? MaxLength { get; set; }

    [Display(Name = "حداقل مقدار")]
    public decimal? MinValue { get; set; }

    [Display(Name = "حداکثر مقدار")]
    public decimal? MaxValue { get; set; }

    [Display(Name = "عبارت منظم")]
    public string? ValidationRegex { get; set; }

    [Display(Name = "پیام خطا")]
    public string? ValidationErrorMessage { get; set; }

    public string FieldType { get; set; } = string.Empty;
    public bool SupportsOptions { get; set; }
    public bool SupportsValidation { get; set; }
}

/// <summary>
/// ViewModel for managing a form field (advanced settings)
/// </summary>
public class ManageFormFieldViewModel
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public bool SupportsOptions { get; set; }
    public bool SupportsValidation { get; set; }

    // Basic Info
    [Required(ErrorMessage = "برچسب فیلد الزامی است")]
    [StringLength(200, ErrorMessage = "برچسب فیلد نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "برچسب فیلد")]
    public string FieldLabel { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "متن راهنما نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "متن راهنما")]
    public string? Placeholder { get; set; }

    [StringLength(500, ErrorMessage = "متن راهنما نمی‌تواند بیشتر از 500 کاراکتر باشد")]
    [Display(Name = "متن راهنما")]
    public string? HelpText { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
    [Display(Name = "ترتیب نمایش")]
    public int Order { get; set; }

    [Display(Name = "قابل مشاهده")]
    public bool IsVisible { get; set; } = true;

    // Validation Settings
    [Display(Name = "اجباری")]
    public bool IsRequired { get; set; }

    [Display(Name = "حداقل طول")]
    public int? MinLength { get; set; }

    [Display(Name = "حداکثر طول")]
    public int? MaxLength { get; set; }

    [Display(Name = "حداقل مقدار")]
    public decimal? MinValue { get; set; }

    [Display(Name = "حداکثر مقدار")]
    public decimal? MaxValue { get; set; }

    [Display(Name = "عبارت منظم")]
    public string? ValidationRegex { get; set; }

    [Display(Name = "پیام خطا")]
    public string? ValidationErrorMessage { get; set; }

    // Options
    public List<FieldOptionViewModel> Options { get; set; } = new();
}

/// <summary>
/// ViewModel for field option
/// </summary>
public class FieldOptionViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "برچسب گزینه الزامی است")]
    [StringLength(200, ErrorMessage = "برچسب گزینه نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "برچسب")]
    public string Label { get; set; } = string.Empty;

    [Required(ErrorMessage = "مقدار گزینه الزامی است")]
    [StringLength(200, ErrorMessage = "مقدار گزینه نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "مقدار")]
    public string Value { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
    [Display(Name = "ترتیب نمایش")]
    public int DisplayOrder { get; set; }

    [Display(Name = "پیش‌فرض")]
    public bool IsDefault { get; set; }
}

/// <summary>
/// ViewModel for adding a field option
/// </summary>
public class AddFieldOptionViewModel
{
    public Guid FieldId { get; set; }

    [Required(ErrorMessage = "برچسب گزینه الزامی است")]
    [StringLength(200, ErrorMessage = "برچسب گزینه نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "برچسب")]
    public string Label { get; set; } = string.Empty;

    [Required(ErrorMessage = "مقدار گزینه الزامی است")]
    [StringLength(200, ErrorMessage = "مقدار گزینه نمی‌تواند بیشتر از 200 کاراکتر باشد")]
    [Display(Name = "مقدار")]
    public string Value { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش نمی‌تواند منفی باشد")]
    [Display(Name = "ترتیب نمایش")]
    public int DisplayOrder { get; set; }

    [Display(Name = "پیش‌فرض")]
    public bool IsDefault { get; set; }
}

#endregion

#region Display Models

/// <summary>
/// ViewModel for displaying form details
/// </summary>
public class FormDetailsViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<FormVersionViewModel> Versions { get; set; } = new();
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanPublish { get; set; }
}

/// <summary>
/// ViewModel for displaying form version
/// </summary>
public class FormVersionViewModel
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<FormGroupViewModel> Groups { get; set; } = new();
    public List<FormFieldViewModel> Fields { get; set; } = new();
    public int ResponseCount { get; set; }
}

/// <summary>
/// ViewModel for displaying form group
/// </summary>
public class FormGroupViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentGroupId { get; set; }
    public int DisplayOrder { get; set; }
    public List<FormGroupViewModel> SubGroups { get; set; } = new();
    public List<FormFieldViewModel> Fields { get; set; } = new();
}

/// <summary>
/// ViewModel for displaying form field
/// </summary>
public class FormFieldViewModel
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVisible { get; set; }
    public Guid? GroupId { get; set; }
    public FieldValidationViewModel? Validation { get; set; }
    public List<FieldOptionViewModel> Options { get; set; } = new();
    public bool SupportsOptions { get; set; }
    public bool SupportsValidation { get; set; }
}

/// <summary>
/// ViewModel for displaying field validation
/// </summary>
public class FieldValidationViewModel
{
    public bool IsRequired { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public string? ValidationRegex { get; set; }
    public string? ValidationErrorMessage { get; set; }
}

/// <summary>
/// ViewModel for displaying form list item
/// </summary>
public class FormListItemViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public int VersionCount { get; set; }
    public int ResponseCount { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanPublish { get; set; }
}

#endregion

#region Select Items

/// <summary>
/// Select item for form groups
/// </summary>
public class FormGroupSelectItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Level { get; set; }
}

/// <summary>
/// Select item for field types
/// </summary>
public class FieldTypeSelectItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool SupportsOptions { get; set; }
    public bool SupportsValidation { get; set; }
}

#endregion

#region Filter and Search Models

/// <summary>
/// Filter model for forms list
/// </summary>
public class FormListFilterViewModel
{
    [Display(Name = "جستجو")]
    public string? SearchTerm { get; set; }

    [Display(Name = "وضعیت")]
    public string? Status { get; set; }

    [Display(Name = "فعال")]
    public bool? IsActive { get; set; }

    [Display(Name = "تاریخ ایجاد از")]
    public DateTime? CreatedFrom { get; set; }

    [Display(Name = "تاریخ ایجاد تا")]
    public DateTime? CreatedTo { get; set; }

    [Range(1, 100, ErrorMessage = "شماره صفحه باید بین 1 تا 100 باشد")]
    public int Page { get; set; } = 1;

    [Range(10, 100, ErrorMessage = "تعداد آیتم در صفحه باید بین 10 تا 100 باشد")]
    public int PageSize { get; set; } = 20;

    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;

    public List<FormListItemViewModel> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

#endregion