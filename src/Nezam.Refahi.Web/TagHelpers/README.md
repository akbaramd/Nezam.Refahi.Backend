# Pagination Tag Helper

This tag helper simplifies pagination implementation across the application, ensuring consistent pagination behavior and appearance while preserving all route parameters.

## Usage

```cshtml
<pagination 
    page-current="@Model.CurrentPage"
    page-total="@Model.TotalPages"
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    asp-controller="ControllerName"
    asp-action="ActionName"
    asp-area="AreaName"
    asp-route-param1="@Model.Param1"
    asp-route-param2="@Model.Param2"
    ...>
</pagination>
```

## Parameters

### Required Parameters

- `page-current`: Current page number (1-based)
- `page-total`: Total number of pages
- `page-size`: Number of items per page
- `total-items`: Total number of items across all pages
- `asp-controller`: The controller name
- `asp-action`: The action name

### Optional Parameters

- `asp-area`: The area name (if applicable)
- `asp-route-*`: Additional route values to preserve during pagination

## Examples

### Basic Example

```cshtml
<pagination 
    page-current="@Model.CurrentPage"
    page-total="@Model.TotalPages"
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    asp-controller="Users"
    asp-action="Index">
</pagination>
```

### With Filtering and Sorting Parameters

```cshtml
<pagination 
    page-current="@Model.CurrentPage"
    page-total="@Model.TotalPages"
    page-size="@Model.PageSize"
    total-items="@Model.TotalItems"
    asp-controller="Documents"
    asp-action="Index"
    asp-area="Panel"
    asp-route-searchTerm="@Model.SearchTerm"
    asp-route-isRead="@Model.IsRead"
    asp-route-sortBy="@Model.SortBy"
    asp-route-sortOrder="@Model.SortOrder">
</pagination>
```

## Design

The pagination tag helper will:

1. Display a summary of the current visible items (e.g., "Showing 1-10 of 100 items")
2. Generate first page link (arrow icon)
3. Generate numbered page links for pages around the current page
4. Generate last page link (arrow icon)

All links preserve the current filtering, sorting, and other parameters.

## Implementation Notes

The tag helper automatically suppresses output if there is only one page or less to display. 