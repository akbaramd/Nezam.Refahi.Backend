# گزارش تغییرات API - مدیریت وضعیت پاسخ‌ها (Response Status Management)

## خلاصه تغییرات
این گزارش شامل تمام تغییرات انجام شده در API های مربوط به مدیریت وضعیت پاسخ‌ها (Response Status) می‌باشد. تغییرات شامل اضافه کردن فیلدهای جدید `ResponseStatus` و `ResponseStatusText` به تمام DTO های مربوط به پاسخ‌ها است.

---

## 1. تغییرات در Enum های دامنه

### ResponseStatus Enum (جدید)
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Domain/Enums/ResponseStatus.cs`

```csharp
public enum ResponseStatus
{
    Answering = 1,      // در حال جواب
    Reviewing = 2,      // در حال ریویو  
    Completed = 3,      // اتمام شده
    Cancelled = 4,      // لغو شده
    Expired = 5         // منقضی شده
}
```

### ResponseStatusHelper (جدید)
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Domain/Enums/ResponseStatusHelper.cs`

کلاس کمکی برای تبدیل وضعیت‌ها به متن فارسی و انگلیسی.

---

## 2. تغییرات در Entity های دامنه

### Response Entity
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Domain/Entities/Response.cs`

**فیلدهای اضافه شده:**
- `public ResponseStatus Status { get; private set; }`

**متدهای اضافه شده:**
- `StartReviewing()` - تغییر وضعیت به ریویو
- `ResumeAnswering()` - بازگشت به وضعیت جواب دادن
- `CanBeModified()` - بررسی امکان ویرایش
- `IsReviewing()` - بررسی وضعیت ریویو
- `IsCompleted()` - بررسی وضعیت تکمیل

---

## 3. تغییرات در DTO ها

### 3.1 ResponseDto
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Dtos/ResponseDto.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 3.2 ResponseDetailsDto
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Dtos/ResponseDetailsDto.cs`

**تغییرات در ResponseStatusDto:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 3.3 UserResponseStatusDto
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Dtos/UserResponseStatusDto.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusTextFa { get; set; } = string.Empty;  // متن فارسی وضعیت
```

---

## 4. تغییرات در Command Response ها

### 4.1 AnswerQuestionResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/AnswerQuestionCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 4.2 SubmitSurveyResponseResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/SubmitSurveyResponseCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 4.3 AutoSaveAnswersResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/AutoSaveAnswersCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 4.4 StartSurveyResponseResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/StartSurveyResponseCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 4.5 SubmitResponseResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/SubmitResponseCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 4.6 GoNextQuestionResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/GoNextQuestionCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 4.7 GoPreviousQuestionResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/GoPreviousQuestionCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

### 4.8 CancelResponseResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Commands/CancelResponseCommand.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

---

## 5. تغییرات در Query Response ها

### 5.1 ResponseProgressResponse
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Contracts/Queries/GetResponseProgressQuery.cs`

**فیلدهای اضافه شده:**
```csharp
public string ResponseStatus { get; set; } = string.Empty;        // وضعیت جدید پاسخ
public string ResponseStatusText { get; set; } = string.Empty;    // متن فارسی وضعیت
```

---

## 6. تغییرات در Handler ها

### 6.1 AnswerQuestionCommandHandler
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Application/Commands/AnswerQuestionCommandHandler.cs`

**تغییرات:**
- اضافه شدن منطق مدیریت وضعیت بر اساس درصد تکمیل
- انتقال خودکار به وضعیت "ریویو" در 80% تکمیل
- بازگشت به وضعیت "جواب دادن" در کمتر از 80% تکمیل
- اضافه شدن `ResponseStatus` و `ResponseStatusText` به پاسخ

### 6.2 SubmitSurveyResponseCommandHandler
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Application/Commands/SubmitSurveyResponseCommandHandler.cs`

**تغییرات:**
- اضافه شدن `ResponseStatus` و `ResponseStatusText` به پاسخ

### 6.3 AutoSaveAnswersCommandHandler
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Application/Commands/AutoSaveAnswersCommandHandler.cs`

**تغییرات:**
- اضافه شدن منطق مدیریت وضعیت بر اساس درصد تکمیل
- اضافه شدن `ResponseStatus` و `ResponseStatusText` به پاسخ

### 6.4 GetResponseProgressQueryHandler
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Application/Queries/GetResponseProgressQueryHandler.cs`

**تغییرات:**
- اضافه شدن `ResponseStatus` و `ResponseStatusText` به پاسخ

### 6.5 GetSpecificQuestionQueryHandler
**مسیر:** `src/Modules/Survey/Nezam.Refahi.Surveying.Application/Queries/GetSpecificQuestionQueryHandler.cs`

**تغییرات:**
- اضافه شدن `ResponseStatus` و `ResponseStatusText` به `UserResponseStatusDto`

---

## 7. منطق مدیریت وضعیت

### 7.1 انتقال خودکار وضعیت
- **80% تکمیل یا بیشتر:** انتقال از "در حال جواب" به "در حال ریویو"
- **کمتر از 80% تکمیل:** بازگشت از "در حال ریویو" به "در حال جواب"
- **ارسال نهایی:** انتقال به "اتمام شده"

### 7.2 وضعیت‌های ممکن
1. **Answering (در حال جواب):** وضعیت اولیه هنگام شروع پاسخ‌دهی
2. **Reviewing (در حال ریویو):** وضعیت بررسی پاسخ‌ها قبل از ارسال نهایی
3. **Completed (اتمام شده):** وضعیت پس از ارسال موفق پاسخ
4. **Cancelled (لغو شده):** وضعیت لغو شده توسط کاربر
5. **Expired (منقضی شده):** وضعیت منقضی شده

---

## 8. نکات مهم برای کلاینت

### 8.1 فیلدهای جدید در تمام Response ها
تمام Response های مربوط به پاسخ‌ها حالا شامل دو فیلد جدید هستند:
- `ResponseStatus`: مقدار enum به صورت string
- `ResponseStatusText`: متن فارسی وضعیت

### 8.2 تغییرات در رفتار API
- وضعیت پاسخ به صورت خودکار بر اساس درصد تکمیل تغییر می‌کند
- امکان ویرایش پاسخ فقط در وضعیت "در حال جواب" وجود دارد
- وضعیت "در حال ریویو" نشان‌دهنده نزدیک بودن به تکمیل است

### 8.3 سازگاری با گذشته
- تمام فیلدهای قبلی حفظ شده‌اند
- فیلدهای جدید اختیاری هستند و مقدار پیش‌فرض خالی دارند
- هیچ تغییر breaking در API وجود ندارد

---

## 9. مثال Response جدید

```json
{
  "data": {
    "responseId": "123e4567-e89b-12d3-a456-426614174000",
    "questionId": "123e4567-e89b-12d3-a456-426614174001",
    "isAnswered": true,
    "answeredQuestions": 8,
    "completionPercentage": 80.0,
    "responseStatus": "Reviewing",
    "responseStatusText": "در حال ریویو",
    "message": "پاسخ با موفقیت ثبت شد"
  },
  "isSuccess": true,
  "message": "Operation completed successfully"
}
```

---

## 10. خلاصه تغییرات برای کلاینت

### فیلدهای اضافه شده به تمام Response های مربوط به پاسخ:
1. `responseStatus` (string): وضعیت فعلی پاسخ
2. `responseStatusText` (string): متن فارسی وضعیت

### مقادیر ممکن برای ResponseStatus:
- `"Answering"` → `"در حال جواب"`
- `"Reviewing"` → `"در حال ریویو"`
- `"Completed"` → `"اتمام شده"`
- `"Cancelled"` → `"لغو شده"`
- `"Expired"` → `"منقضی شده"`

### تغییرات در رفتار:
- وضعیت به صورت خودکار بر اساس درصد تکمیل تغییر می‌کند
- در 80% تکمیل، وضعیت به "ریویو" تغییر می‌کند
- امکان ویرایش فقط در وضعیت "جواب دادن" وجود دارد

---

**تاریخ:** 2024-01-XX  
**نسخه:** 1.0.0  
**وضعیت:** تکمیل شده و آماده برای استفاده
