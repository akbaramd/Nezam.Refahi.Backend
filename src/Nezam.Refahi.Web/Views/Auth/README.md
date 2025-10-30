# Authentication Pages Design System

## Overview
This document outlines the design principles, components, and guidelines for all authentication pages in the Nezam.New.EES system. All auth pages follow a consistent design system to ensure a unified user experience with Persian culture alignment.

## Design Principles

### 1. **Consistent Layout Structure**
All authentication pages follow the same structural pattern:
- **Wrapper**: Full-height centered container
- **Container**: Responsive width container (max-width: 450px for forms, 600px for selection pages)
- **Card**: White background card with rounded corners and shadow
- **Header**: Icon, title, and subtitle section
- **Content**: Main content area
- **Footer**: Organization branding

### 2. **Visual Hierarchy**
- **Primary Icon**: Large circular icon with gradient background
- **Title**: Bold, prominent heading (1.5rem)
- **Subtitle**: Muted descriptive text (0.95rem)
- **Content**: Clear, readable body text
- **Actions**: Prominent call-to-action buttons

### 3. **Color System**
- **Primary Gradient**: `linear-gradient(135deg, #667eea 0%, #764ba2 100%)`
- **Success Gradient**: `linear-gradient(135deg, #28a745 0%, #20c997 100%)`
- **Error Gradient**: `linear-gradient(135deg, #dc3545 0%, #c82333 100%)`
- **Warning Gradient**: `linear-gradient(135deg, #ffc107 0%, #fd7e14 100%)`
- **Info Gradient**: `linear-gradient(135deg, #17a2b8 0%, #138496 100%)`
- **Text Colors**: 
  - Primary: `#2c3e50`
  - Secondary: `#6c757d`
  - Muted: `#495057`

### 4. **Typography**
- **Font Family**: System fonts with RTL support
- **Title**: 700 weight, 1.5rem size
- **Subtitle**: 400 weight, 0.95rem size
- **Body**: 400 weight, 0.9rem size
- **Small Text**: 0.85rem size

### 5. **Persian Culture Alignment**
- **RTL Layout**: Right-to-left text direction
- **Persian Icons**: Culturally appropriate iconography
- **Persian Text**: Natural Persian language flow
- **Cultural Colors**: Warm, welcoming color palette
- **Respectful Design**: Professional and dignified appearance

### 6. **Responsive Sizing**
- **Mobile**: Container max-width 450px, padding 2rem 1.5rem
- **Tablet**: Container max-width 600px, padding 2.5rem
- **Desktop**: Container max-width 700px, padding 3rem
- **Large Desktop**: Container max-width 800px, padding 3.5rem

## Component Guidelines

### 1. **Authentication Wrapper**
```css
.authentication-wrapper {
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 2rem 1rem;
}
```

### 2. **Auth Card**
```css
.auth-card {
    background: white;
    border-radius: 16px;
    box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
    padding: 2.5rem;
    border: 1px solid rgba(0, 0, 0, 0.05);
}
```

### 3. **Icon Wrapper**
```css
.icon-wrapper {
    width: 80px;
    height: 80px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 2rem;
    box-shadow: 0 8px 25px rgba(102, 126, 234, 0.3);
}
```

### 4. **Form Elements**
- **Input Groups**: Consistent styling with icons
- **Buttons**: Gradient backgrounds with hover effects
- **Validation**: Clear error messages with proper spacing

### 5. **Info Cards**
```css
.info-card {
    background: #f8f9fa;
    border-radius: 12px;
    padding: 1.25rem;
    border-left: 4px solid #667eea;
}
```

### 6. **Selection Cards**
```css
.area-card {
    background: #f8f9fa;
    border: 2px solid #e9ecef;
    border-radius: 12px;
    padding: 1.5rem;
    display: flex;
    align-items: center;
    gap: 1rem;
    transition: all 0.3s ease;
}
```

## Page-Specific Guidelines

### 1. **Login Page**
- **Icon**: User icon (`ri-user-line`)
- **Form**: Phone number input with validation
- **Info Cards**: Separate cards for engineers and owners
- **Submit Button**: "دریافت کد تایید" with message icon

### 2. **OTP Verification Page**
- **Icon**: Shield check icon (`ri-shield-check-line`)
- **OTP Input**: 5 individual digit inputs with auto-focus
- **Timer**: Countdown display with resend functionality
- **Auto-submit**: Form submits when all digits are entered

### 3. **Area Selection Page**
- **Icon**: Layout grid icon (`ri-layout-grid-line`)
- **Content**: Interactive area selection cards
- **Cards**: Hover effects with gradient backgrounds
- **Info Section**: Guidance card for users
- **Actions**: Logout button with proper styling

### 4. **User Information Page**
- **Icon**: User add icon (`ri-user-add-line`)
- **Form**: National code, first name, last name, and email inputs
- **Validation**: Real-time validation with Persian character support
- **National Code**: Algorithm validation with proper error messages
- **Info Cards**: Registration information and security details
- **Submit Button**: "ادامه و ثبت نام" with loading state

### 5. **Logout Confirmation Page**
- **Icon**: Logout icon (`ri-logout-box-r-line`) with warning gradient
- **Content**: Confirmation message asking user to confirm logout
- **Actions**: Two buttons - "انصراف" (Cancel) and "تایید خروج" (Confirm Logout)
- **Cancel Behavior**: Returns to previous page using `javascript:history.back()`
- **Confirm Action**: Redirects to `ConfirmLogout` action

### 6. **Logout Success Page**
- **Icon**: Check double icon (`ri-check-double-line`) with success gradient
- **Content**: Success message confirming logout completion
- **Info Card**: Security information about session cleanup
- **Action**: Single "ورود مجدد" button to return to login

### 6. **Access Denied / Area Selection Page**
- **Icon**: Map pin icon (`ri-map-pin-line`) with info gradient
- **Content**: Guidance to select appropriate area with permissions
- **Actions**: "بازگشت" (Return) and "انتخاب ناحیه" (Select Area) buttons
- **Purpose**: Redirects users to area selection instead of showing error

## User Flow Patterns

### **Login Flow**
1. **User enters phone** → Redirects to OTP verification
2. **User enters OTP** → Auto-submit when complete
3. **Verification successful** → Redirects to area selection
4. **User selects area** → Enters dashboard

### **Logout Flow**
1. **User clicks logout** → Redirects to `Logout.cshtml` (confirmation page)
2. **User cancels** → Returns to previous page via `history.back()`
3. **User confirms** → Redirects to `ConfirmLogout` action
4. **Logout processed** → Redirects to `LogoutSuccess.cshtml` (success page)
5. **User can login again** → "ورود مجدد" button

### **Access Denied Flow**
1. **User lacks permissions** → Redirects to `AccessDenied.cshtml`
2. **Page shows area selection guidance** → No error message
3. **User selects area** → Redirects to `AreaSelection` action
4. **User returns** → Can go back to previous page

## Responsive Design

### Container Sizing Guidelines
```css
/* Mobile (default) */
.authentication-container {
    max-width: 450px;
}

/* Tablet */
@media (min-width: 768px) {
    .authentication-container {
        max-width: 600px;
    }
}

/* Desktop */
@media (min-width: 1200px) {
    .authentication-container {
        max-width: 700px;
    }
}

/* Large Desktop */
@media (min-width: 1400px) {
    .authentication-container {
        max-width: 800px;
    }
}
```

### Mobile Breakpoints
```css
@media (max-width: 576px) {
    .authentication-wrapper {
        padding: 1rem;
    }
    
    .auth-card {
        padding: 2rem 1.5rem;
    }
    
    .icon-wrapper {
        width: 60px;
        height: 60px;
        font-size: 1.5rem;
    }

    .area-card {
        padding: 1.25rem;
        flex-direction: column;
        text-align: center;
        gap: 0.75rem;
    }
}
```

### Responsive Considerations
- **Container Width**: Progressive sizing from 450px (mobile) to 800px (large desktop)
- **Padding**: Reduced on mobile devices (2rem 1.5rem)
- **Icon Size**: Smaller icons on mobile (60px vs 80px)
- **Button Layout**: Stack buttons vertically on small screens
- **Text Size**: Maintain readability across devices
- **Card Layout**: Vertical stacking on mobile for selection cards
- **Form Elements**: Consistent sizing across all breakpoints

## Animation Guidelines

### 1. **Hover Effects**
- **Buttons**: `translateY(-2px)` with enhanced shadow
- **Inputs**: Focus states with color transitions
- **Cards**: Subtle shadow changes and border color transitions
- **Selection Cards**: Scale and color animations

### 2. **Loading States**
- **Spinner**: Rotating icon animation
- **Button States**: Disabled state with loading text
- **Transitions**: 0.3s ease for all interactions

### 3. **Keyframe Animations**
```css
@keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
}

@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}
```

## Accessibility Guidelines

### 1. **Keyboard Navigation**
- **Tab Order**: Logical tab sequence
- **Focus Indicators**: Clear focus states
- **Skip Links**: Proper heading structure

### 2. **Screen Reader Support**
- **Alt Text**: Descriptive text for icons
- **ARIA Labels**: Proper labeling for form elements
- **Semantic HTML**: Use appropriate HTML elements

### 3. **Color Contrast**
- **Text**: Minimum 4.5:1 contrast ratio
- **Interactive Elements**: Clear visual feedback
- **Error States**: High contrast error indicators

## Persian Culture Considerations

### 1. **Language and Text**
- **RTL Support**: Proper right-to-left text direction
- **Persian Typography**: Appropriate font choices
- **Natural Language**: Persian text flows naturally

### 2. **Visual Design**
- **Cultural Colors**: Warm, welcoming color palette
- **Respectful Icons**: Culturally appropriate iconography
- **Professional Appearance**: Dignified and trustworthy design

### 3. **User Experience**
- **Familiar Patterns**: Design patterns familiar to Persian users
- **Clear Communication**: Straightforward and respectful messaging
- **Easy Navigation**: Intuitive interaction patterns

## Implementation Checklist

### Before Creating New Auth Pages
- [ ] Use `_AuthLayout` as the layout
- [ ] Follow the wrapper-container-card structure
- [ ] Include appropriate icon with gradient
- [ ] Implement responsive design
- [ ] Add proper form validation
- [ ] Include loading states
- [ ] Test keyboard navigation
- [ ] Verify screen reader compatibility
- [ ] Ensure RTL support
- [ ] Use Persian-appropriate content

### Code Standards
- [ ] Use consistent CSS class naming
- [ ] Include proper RTL support
- [ ] Implement error handling
- [ ] Add success/error feedback
- [ ] Use semantic HTML elements
- [ ] Include proper meta tags
- [ ] Follow Persian culture guidelines

## File Structure
```
Views/Auth/
├── Login.cshtml          # Phone number input
├── VerifyOtp.cshtml      # OTP verification
├── UserInformation.cshtml # User registration form
├── AreaSelection.cshtml  # Area selection with cards
├── Logout.cshtml         # Logout confirmation
├── LogoutSuccess.cshtml  # Logout success page
├── AccessDenied.cshtml   # Area selection guidance
└── README.md            # This documentation
```

## Dependencies
- **Layout**: `_AuthLayout.cshtml`
- **CSS Framework**: Bootstrap 5
- **Icons**: Remix Icons (ri-*)
- **Validation**: jQuery Validation
- **Notifications**: Toastr (for OTP page)

## Controller Actions Required

### Auth Controller Actions
```csharp
// Login flow
public IActionResult Login() // Phone input
public IActionResult VerifyOtp() // OTP verification
public IActionResult UserInformation() // User registration
public IActionResult AreaSelection() // Area selection

// Logout flow
public IActionResult Logout() // Shows confirmation page
public IActionResult ConfirmLogout() // Processes logout
public IActionResult LogoutSuccess() // Shows success page

// Area selection
public IActionResult SelectArea() // Processes area selection
```

## Maintenance Notes
- Keep all auth pages consistent with this design system
- Update this README when adding new patterns
- Test all pages across different devices and browsers
- Maintain accessibility standards
- Review and update color schemes as needed
- Ensure proper logout flow with confirmation
- Handle access denied gracefully with area selection
- Maintain Persian culture alignment
- Test RTL functionality regularly

---

**Last Updated**: December 2024
**Version**: 1.3
**Maintainer**: Development Team 