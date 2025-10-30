# Modal Select Component - Enhanced Version

## Overview

The Modal Select component is a reusable, feature-rich dropdown component that allows users to search and select items from a paginated list. This enhanced version includes significant improvements in UI/UX, functionality, and bug fixes.

## Key Features

### 🎯 Core Functionality
- **Search & Select**: Search through items with real-time filtering
- **Multiple Selection Modes**: Support for both single and multiple selection
- **Pagination**: Efficient handling of large datasets
- **API Integration**: Flexible API endpoint configuration

### 🚀 Enhanced Features
- **Smart Sorting**: Selected items automatically appear at the top of the list
- **Visual Selection**: Selected items are highlighted with beautiful gradients
- **Debounced Search**: Improved performance with search debouncing
- **Selection Count**: Button shows the number of selected items
- **Better Error Handling**: Comprehensive error handling and user feedback

### 🎨 UI/UX Improvements
- **Modern Design**: Clean, modern interface with gradient colors
- **Responsive Layout**: Works perfectly on all screen sizes
- **RTL Support**: Full support for Persian/Arabic languages
- **Accessibility**: WCAG compliant with proper focus management
- **Loading States**: Clear loading indicators and error messages
- **Beautiful Animations**: Smooth transitions and hover effects

## Usage

### Basic Usage

```html
<modal-select asp-for="RecipientIds"
              api-url="/api/recipients/lookup"
              placeholder="کاربران را انتخاب کنید..."
              button-text="انتخاب کاربران"
              modal-title="انتخاب کاربران" />
```

### Advanced Usage

```html
<modal-select asp-for="DepartmentIds"
              api-url="/api/departments/lookup"
              placeholder="دپارتمان‌ها را انتخاب کنید..."
              button-text="انتخاب دپارتمان"
              modal-title="انتخاب دپارتمان‌ها"
              selection-mode="multiple"
              page-size="20"
              show-selected-first="true"
              allow-deselect="true"
              min-search-length="2"
              extra-filters="active=true&type=main" />
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `asp-for` | ModelExpression | Required | The model property to bind to |
| `api-url` | string | "/api/lookup" | API endpoint for fetching data |
| `text` | string | "text" | Field name for display text |
| `selection-mode` | string | "multiple" | "single" or "multiple" |
| `placeholder` | string | "انتخاب..." | Placeholder text for input |
| `button-text` | string | "انتخاب" | Text for the select button |
| `modal-title` | string | "انتخاب" | Title of the modal |
| `page-size` | int | 10 | Number of items per page |
| `show-selected-first` | bool | true | Show selected items at top |
| `allow-deselect` | bool | true | Allow removing selected items |
| `min-search-length` | int | 2 | Minimum characters for search |
| `extra-filters` | string | "" | Additional query parameters |
| `selected-json` | string | "" | Pre-selected items JSON |

## API Requirements

The component expects the API to return data in the following format:

```json
{
  "items": [
    {
      "id": "1",
      "text": "Item Name",
      "description": "Optional description"
    }
  ],
  "totalItems": 100,
  "totalPages": 10
}
```

### API Endpoints

1. **Main Lookup**: `GET /api/endpoint?page=1&pageSize=10&search=term`
2. **Fetch by IDs**: `GET /api/endpoint/byIds?ids=1,2,3`

## Visual Design

### Color Scheme
- **Primary Colors**: Purple gradient (#667eea to #764ba2)
- **Success Colors**: Green gradient for selected items (#28a745 to #20c997)
- **Hover Effects**: Subtle animations and color transitions
- **Modern Gradients**: Beautiful gradient backgrounds throughout

### Selected Items
- Selected items are highlighted with a green gradient background
- Left border accent for visual distinction
- Smooth hover animations
- Checkbox with gradient styling

## Styling

The component uses CSS classes that can be customized:

- `.modal-select-container` - Main container
- `.modal-select-display` - Display input field
- `.modal-select-btn` - Select button
- `.search-section` - Search input area
- `.table` - Data table
- `.table-success` - Selected row styling

## JavaScript Events

The component triggers the following events:

- `modal:shown` - When modal opens
- `modal:hidden` - When modal closes
- `selection:changed` - When selection changes

## Browser Support

- Chrome 60+
- Firefox 55+
- Safari 12+
- Edge 79+

## Migration from Previous Version

### Breaking Changes
- Removed selected items section display
- Updated CSS class names (see styling section)
- Changed `text-field` parameter to `text`
- Some JavaScript methods have been renamed
- Modal structure has been enhanced

### Migration Steps
1. Update parameter name from `text-field` to `text`
2. Update CSS class references
3. Review JavaScript event handlers
4. Test existing implementations
5. Update any custom styling

## Troubleshooting

### Common Issues

1. **Modal not opening**
   - Check if Bootstrap JS is loaded
   - Verify modal ID uniqueness

2. **API not responding**
   - Check network tab for errors
   - Verify API endpoint URL
   - Ensure CORS is configured

3. **Selected items not showing**
   - Check API response format
   - Verify `text` parameter (not `text-field`)
   - Ensure proper JSON serialization

### Debug Mode

Enable debug logging by adding this to your page:

```javascript
window.ModalSelectDebug = true;
```

## Examples

See `Views/Example/ModalSelectExample.cshtml` for complete usage examples.

## Contributing

When contributing to this component:

1. Follow the existing code style
2. Add comprehensive tests
3. Update documentation
4. Test in multiple browsers
5. Ensure RTL compatibility

## License

This component is part of the Nezam.New.EES project. 