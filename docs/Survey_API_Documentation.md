# نظام رفاهی - مستندات API نظرسنجی

## مقدمه
این مستند، API های مربوط به ماژول نظرسنجی سیستم نظام رفاهی را توضیح می‌دهد. این API ها امکان ایجاد، مدیریت و پاسخ به نظرسنجی‌ها را فراهم می‌کنند.

## مدل داده‌ای
سیستم نظرسنجی از مدل‌های داده‌ای زیر استفاده می‌کند:

### Survey (نظرسنجی)
- **Id**: شناسه منحصر به فرد نظرسنجی (Guid)
- **Title**: عنوان نظرسنجی
- **Description**: توضیحات نظرسنجی (اختیاری)
- **Status**: وضعیت نظرسنجی (Draft، Published، Closed، Archived)
- **OpensAt**: زمان شروع نظرسنجی (اختیاری)
- **ClosesAt**: زمان پایان نظرسنجی (اختیاری)
- **TimeLimit**: محدودیت زمانی برای پاسخ به نظرسنجی به دقیقه (اختیاری)
- **CreatedAt**: زمان ایجاد نظرسنجی
- **CreatedBy**: کاربر ایجاد کننده نظرسنجی
- **LastModifiedAt**: زمان آخرین ویرایش نظرسنجی
- **LastModifiedBy**: کاربر آخرین ویرایش کننده نظرسنجی

### Question (سوال)
- **Id**: شناسه منحصر به فرد سوال (Guid)
- **SurveyId**: شناسه نظرسنجی مربوطه
- **Text**: متن سوال
- **Type**: نوع سوال (MultipleChoice، SingleChoice، Text، File)
- **IsRequired**: آیا پاسخ به سوال اجباری است؟
- **Order**: ترتیب نمایش سوال در نظرسنجی
- **Options**: گزینه‌های سوال (برای سوالات چند گزینه‌ای)

### QuestionOption (گزینه سوال)
- **Id**: شناسه منحصر به فرد گزینه (Guid)
- **QuestionId**: شناسه سوال مربوطه
- **Text**: متن گزینه
- **DisplayOrder**: ترتیب نمایش گزینه

### SurveyResponse (پاسخ نظرسنجی)
- **Id**: شناسه منحصر به فرد پاسخ نظرسنجی (Guid)
- **SurveyId**: شناسه نظرسنجی مربوطه
- **RespondentName**: نام پاسخ دهنده (اختیاری)
- **RespondentEmail**: ایمیل پاسخ دهنده (اختیاری)
- **StartedAt**: زمان شروع پاسخگویی
- **CompletedAt**: زمان تکمیل پاسخگویی (اختیاری)
- **Status**: وضعیت پاسخ (InProgress، Completed)

### SurveyAnswer (پاسخ سوال)
- **Id**: شناسه منحصر به فرد پاسخ سوال (Guid)
- **ResponseId**: شناسه پاسخ نظرسنجی مربوطه
- **QuestionId**: شناسه سوال مربوطه
- **OptionId**: شناسه گزینه انتخاب شده (برای سوالات چند گزینه‌ای)
- **TextAnswer**: متن پاسخ (برای سوالات متنی)
- **FilePath**: مسیر فایل پاسخ (برای سوالات فایلی)
- **AnsweredAt**: زمان پاسخگویی به سوال

## نقش‌ها و دسترسی‌ها
سیستم از سه نقش اصلی پشتیبانی می‌کند:

1. **Administrator**: دسترسی کامل به تمام قابلیت‌های نظرسنجی
2. **Editor**: توانایی ایجاد، ویرایش و مدیریت نظرسنجی‌ها
3. **User**: توانایی مشاهده و پاسخ به نظرسنجی‌های منتشر شده

## API Endpoints

### مدیریت نظرسنجی

#### دریافت لیست نظرسنجی‌ها
```
GET /api/surveys
```
- **توضیحات**: دریافت لیست نظرسنجی‌ها با قابلیت صفحه‌بندی و فیلتر
- **پارامترها**:
  - `PageNumber`: شماره صفحه (پیش‌فرض: 1)
  - `PageSize`: تعداد آیتم در هر صفحه (پیش‌فرض: 10)
  - `SearchTerm`: عبارت جستجو در عنوان و توضیحات (اختیاری)
  - `Status`: فیلتر بر اساس وضعیت (اختیاری)
  - `SortBy`: مرتب‌سازی بر اساس فیلد (پیش‌فرض: CreatedAt)
  - `SortDirection`: جهت مرتب‌سازی (پیش‌فرض: Descending)
- **دسترسی**: Administrator، Editor، User
- **پاسخ**: لیست نظرسنجی‌ها با اطلاعات صفحه‌بندی

#### دریافت جزئیات نظرسنجی
```
GET /api/surveys/{surveyId}
```
- **توضیحات**: دریافت اطلاعات کامل یک نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `includeQuestions`: شامل سوالات باشد؟ (پیش‌فرض: false)
  - `includeResponseStats`: شامل آمار پاسخ‌ها باشد؟ (پیش‌فرض: false)
- **دسترسی**: Administrator، Editor، User
- **پاسخ**: اطلاعات کامل نظرسنجی

#### ایجاد نظرسنجی جدید
```
POST /api/surveys
```
- **توضیحات**: ایجاد یک نظرسنجی جدید
- **بدنه درخواست**:
  ```json
  {
    "title": "عنوان نظرسنجی",
    "description": "توضیحات نظرسنجی",
    "opensAt": "2025-06-01T00:00:00Z",
    "closesAt": "2025-06-30T23:59:59Z",
    "timeLimit": 30
  }
  ```
- **دسترسی**: Administrator، Editor
- **پاسخ**: اطلاعات نظرسنجی ایجاد شده با شناسه جدید

#### ویرایش نظرسنجی
```
PUT /api/surveys/{surveyId}
```
- **توضیحات**: ویرایش اطلاعات یک نظرسنجی موجود
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
- **بدنه درخواست**: مشابه ایجاد نظرسنجی
- **دسترسی**: Administrator، Editor
- **پاسخ**: اطلاعات به‌روزرسانی شده نظرسنجی

#### انتشار نظرسنجی
```
POST /api/surveys/{surveyId}/publish
```
- **توضیحات**: تغییر وضعیت نظرسنجی به "منتشر شده"
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
- **دسترسی**: Administrator، Editor
- **پاسخ**: نتیجه عملیات

#### بستن نظرسنجی
```
POST /api/surveys/{surveyId}/close
```
- **توضیحات**: تغییر وضعیت نظرسنجی به "بسته شده"
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
- **دسترسی**: Administrator، Editor
- **پاسخ**: نتیجه عملیات

#### آرشیو نظرسنجی
```
POST /api/surveys/{surveyId}/archive
```
- **توضیحات**: تغییر وضعیت نظرسنجی به "آرشیو شده"
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
- **دسترسی**: Administrator، Editor
- **پاسخ**: نتیجه عملیات

### مدیریت سوالات

#### دریافت سوالات نظرسنجی
```
GET /api/surveys/{surveyId}/questions
```
- **توضیحات**: دریافت لیست سوالات یک نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `includeOptions`: شامل گزینه‌ها باشد؟ (پیش‌فرض: true)
- **دسترسی**: Administrator، Editor، User
- **پاسخ**: لیست سوالات نظرسنجی

#### افزودن سوال به نظرسنجی
```
POST /api/surveys/{surveyId}/questions
```
- **توضیحات**: افزودن یک سوال جدید به نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
- **بدنه درخواست**:
  ```json
  {
    "text": "متن سوال",
    "type": "MultipleChoice",
    "isRequired": true,
    "order": 1,
    "options": [
      {
        "text": "گزینه اول",
        "displayOrder": 1
      },
      {
        "text": "گزینه دوم",
        "displayOrder": 2
      }
    ]
  }
  ```
- **دسترسی**: Administrator، Editor
- **پاسخ**: اطلاعات سوال ایجاد شده با شناسه جدید

#### ویرایش سوال
```
PUT /api/surveys/{surveyId}/questions/{questionId}
```
- **توضیحات**: ویرایش یک سوال موجود
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `questionId`: شناسه سوال
- **بدنه درخواست**: مشابه افزودن سوال
- **دسترسی**: Administrator، Editor
- **پاسخ**: اطلاعات به‌روزرسانی شده سوال

#### حذف سوال
```
DELETE /api/surveys/{surveyId}/questions/{questionId}
```
- **توضیحات**: حذف یک سوال از نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `questionId`: شناسه سوال
- **دسترسی**: Administrator، Editor
- **پاسخ**: نتیجه عملیات

### پاسخ به نظرسنجی

#### دریافت پاسخ‌های نظرسنجی
```
GET /api/surveys/{surveyId}/responses
```
- **توضیحات**: دریافت لیست پاسخ‌های یک نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `includeAnswers`: شامل پاسخ‌های سوالات باشد؟ (پیش‌فرض: false)
  - `pageNumber`: شماره صفحه (پیش‌فرض: 1)
  - `pageSize`: تعداد آیتم در هر صفحه (پیش‌فرض: 10)
- **دسترسی**: Administrator، Editor
- **پاسخ**: لیست پاسخ‌های نظرسنجی

#### دریافت پاسخ‌های سوالات
```
GET /api/surveys/{surveyId}/answers
```
- **توضیحات**: دریافت پاسخ‌های سوالات یک نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `questionId`: فیلتر بر اساس سوال (اختیاری)
  - `responseId`: فیلتر بر اساس پاسخ نظرسنجی (اختیاری)
  - `maxResults`: حداکثر تعداد نتایج (پیش‌فرض: 100)
- **دسترسی**: Administrator، Editor
- **پاسخ**: لیست پاسخ‌های سوالات

#### شروع پاسخ به نظرسنجی
```
POST /api/surveys/{surveyId}/responses
```
- **توضیحات**: شروع یک پاسخ جدید به نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
- **بدنه درخواست**:
  ```json
  {
    "respondentName": "نام پاسخ دهنده",
    "respondentEmail": "email@example.com"
  }
  ```
- **دسترسی**: Administrator، Editor، User
- **پاسخ**: اطلاعات پاسخ نظرسنجی ایجاد شده با شناسه جدید

#### افزودن پاسخ به سوال
```
POST /api/surveys/{surveyId}/responses/{responseId}/answers
```
- **توضیحات**: افزودن پاسخ به یک سوال
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `responseId`: شناسه پاسخ نظرسنجی
- **بدنه درخواست**:
  ```json
  {
    "questionId": "شناسه سوال",
    "optionId": "شناسه گزینه انتخاب شده",
    "textAnswer": "متن پاسخ",
    "filePath": "مسیر فایل"
  }
  ```
- **دسترسی**: Administrator، Editor، User
- **پاسخ**: اطلاعات پاسخ سوال ایجاد شده با شناسه جدید

#### ثبت نهایی پاسخ نظرسنجی
```
POST /api/surveys/{surveyId}/responses/{responseId}/submit
```
- **توضیحات**: ثبت نهایی پاسخ نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `responseId`: شناسه پاسخ نظرسنجی
- **دسترسی**: Administrator، Editor، User
- **پاسخ**: نتیجه عملیات

#### ثبت کامل نظرسنجی در یک درخواست
```
POST /api/surveys/{surveyId}/submit-complete
```
- **توضیحات**: ثبت کامل پاسخ نظرسنجی در یک درخواست
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
- **بدنه درخواست**:
  ```json
  {
    "respondentName": "نام پاسخ دهنده",
    "respondentEmail": "email@example.com",
    "answers": [
      {
        "questionId": "شناسه سوال 1",
        "optionId": "شناسه گزینه انتخاب شده",
        "textAnswer": null,
        "filePath": null
      },
      {
        "questionId": "شناسه سوال 2",
        "optionId": null,
        "textAnswer": "متن پاسخ",
        "filePath": null
      }
    ]
  }
  ```
- **دسترسی**: Administrator، Editor، User
- **پاسخ**: اطلاعات پاسخ نظرسنجی ایجاد شده با شناسه جدید

### آمار نظرسنجی

#### دریافت آمار نظرسنجی
```
GET /api/surveys/{surveyId}/statistics
```
- **توضیحات**: دریافت آمار و اطلاعات تحلیلی یک نظرسنجی
- **پارامترها**:
  - `surveyId`: شناسه نظرسنجی
  - `textAnswerSampleCount`: تعداد نمونه پاسخ‌های متنی (پیش‌فرض: 5)
- **دسترسی**: Administrator، Editor
- **پاسخ**: آمار و اطلاعات تحلیلی نظرسنجی

## کدهای پاسخ HTTP

- **200 OK**: درخواست با موفقیت انجام شد
- **201 Created**: منبع جدید با موفقیت ایجاد شد
- **400 Bad Request**: درخواست نامعتبر است
- **401 Unauthorized**: احراز هویت نشده
- **403 Forbidden**: دسترسی غیرمجاز
- **404 Not Found**: منبع مورد نظر یافت نشد
- **500 Internal Server Error**: خطای داخلی سرور

## مدل‌های پاسخ

تمام پاسخ‌های API از ساختار `ApplicationResult` پیروی می‌کنند:

```json
{
  "succeeded": true,
  "data": { /* داده‌های پاسخ */ },
  "message": "عملیات با موفقیت انجام شد",
  "errors": null
}
```

در صورت بروز خطا:

```json
{
  "succeeded": false,
  "data": null,
  "message": "پیام خطا",
  "errors": [
    "جزئیات خطا 1",
    "جزئیات خطا 2"
  ]
}
```

## نکات پیاده‌سازی

1. **احراز هویت**: تمام درخواست‌ها نیاز به توکن JWT معتبر دارند که در هدر `Authorization` ارسال می‌شود.
2. **مدیریت خطا**: خطاها به صورت یکپارچه با استفاده از کلاس `ApplicationResult` مدیریت می‌شوند.
3. **اعتبارسنجی**: اعتبارسنجی درخواست‌ها با استفاده از FluentValidation انجام می‌شود.
4. **معماری**: این API بر اساس معماری DDD (Domain-Driven Design) پیاده‌سازی شده است.
5. **کش**: برای بهبود کارایی، برخی از درخواست‌های GET کش می‌شوند.

## مثال‌های کاربردی

### ایجاد و انتشار یک نظرسنجی

1. ایجاد نظرسنجی:
```
POST /api/surveys
```
```json
{
  "title": "نظرسنجی رضایت کارکنان",
  "description": "این نظرسنجی برای سنجش میزان رضایت کارکنان از خدمات رفاهی طراحی شده است",
  "opensAt": "2025-06-01T00:00:00Z",
  "closesAt": "2025-06-30T23:59:59Z"
}
```

2. افزودن سوالات:
```
POST /api/surveys/{surveyId}/questions
```
```json
{
  "text": "میزان رضایت شما از خدمات رفاهی چقدر است؟",
  "type": "SingleChoice",
  "isRequired": true,
  "order": 1,
  "options": [
    {
      "text": "بسیار راضی",
      "displayOrder": 1
    },
    {
      "text": "راضی",
      "displayOrder": 2
    },
    {
      "text": "متوسط",
      "displayOrder": 3
    },
    {
      "text": "ناراضی",
      "displayOrder": 4
    },
    {
      "text": "بسیار ناراضی",
      "displayOrder": 5
    }
  ]
}
```

3. انتشار نظرسنجی:
```
POST /api/surveys/{surveyId}/publish
```

### پاسخ به نظرسنجی

1. شروع پاسخ:
```
POST /api/surveys/{surveyId}/responses
```
```json
{
  "respondentName": "علی محمدی",
  "respondentEmail": "ali@example.com"
}
```

2. ثبت پاسخ‌ها:
```
POST /api/surveys/{surveyId}/responses/{responseId}/answers
```
```json
{
  "questionId": "شناسه سوال",
  "optionId": "شناسه گزینه انتخاب شده"
}
```

3. ثبت نهایی:
```
POST /api/surveys/{surveyId}/responses/{responseId}/submit
```

## منابع بیشتر

- [مستندات API کامل](https://api.nezam-refahi.ir/swagger)
- [راهنمای توسعه‌دهندگان](https://docs.nezam-refahi.ir/developer-guide)
- [نمونه کدهای کلاینت](https://github.com/nezam-refahi/client-examples)
