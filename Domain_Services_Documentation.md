# 📚 مستند Domain Services - Facilities Module

## 🎯 مقدمه

Domain Services برای پیاده‌سازی **قوانین کسب‌وکار** که به یک Aggregate Root خاص تعلق ندارند استفاده می‌شوند.

### ✅ قوانین Domain Services (طبق DDD):

1. **Stateless** - نباید state داشته باشند
2. **Repository Injection مجاز است** - اما فقط برای **query/validation** نه save
3. **فقط Abstractions** - باید از Interfaces استفاده کنند نه Concrete classes
4. **Domain Logic Only** - فقط Domain Logic، نه Application Logic
5. **Use IDs & Entities** - می‌توانند Domain Entities را به عنوان parameter دریافت کنند
6. **No I/O Directly** - نباید مستقیماً به Database دسترسی داشته باشند (مگر از طریق Repository abstractions)

### ⚠️ مرزها:

- ✅ **مجاز:** Repository injection برای validation و query
- ✅ **مجاز:** دریافت Domain Entities به عنوان parameter
- ❌ **ممنوع:** Save/Persist operations (باید در Application Service باشد)
- ❌ **ممنوع:** Stateful operations
- ❌ **ممنوع:** Infrastructure concerns

---

## 🔧 Domain Services ایجاد شده

### 1. **IFacilityCycleEligibilityService** ✅

**هدف:** بررسی واجد شرایط بودن عضو برای دریافت تسهیلات

**مکان:** `Domain/Services/FacilityCycleEligibilityService.cs`

**متدها:**
- `CheckEligibility()` - بررسی کلی واجد شرایط بودن
- `CheckDependencies()` - بررسی وابستگی‌های دوره
- `CheckRequiredFeatures()` - بررسی ویژگی‌های مورد نیاز
- `CheckRequiredCapabilities()` - بررسی قابلیت‌های مورد نیاز

**استفاده:**
```csharp
// در Application Service
var cycle = await _cycleRepository.GetByIdAsync(cycleId);
var eligibilityResult = _eligibilityService.CheckEligibilityForCycle(
    cycle,  // Domain Entity که از Repository لود شده
    memberId,
    completedFacilities,
    activeFacilities,
    memberFeatures,
    memberCapabilities);

if (!eligibilityResult.IsEligible)
{
    // Handle rejection with reasons
    foreach (var reason in eligibilityResult.Reasons)
    {
        // Log or return to user
    }
}
```

**چرا Domain Service:**
- ✅ این logic نیاز به هماهنگی بین چند Aggregate دارد
- ✅ نیاز به بررسی `FacilityCycle` (Dependencies, Features, Capabilities)
- ✅ نیاز به اطلاعات عضو (completedFacilities, features, capabilities)
- ✅ Stateless - هیچ state ندارد
- ✅ Pure Domain Logic - فقط validation و business rules

---

### 2. **IFacilityCycleValidationService** ✅

**هدف:** اعتبارسنجی قوانین کسب‌وکار برای دوره‌ها

**مکان:** `Domain/Services/FacilityCycleValidationService.cs`

**متدها:**
- `IsCycleNameUniqueAsync()` - بررسی یکتایی نام دوره
- `GetOverlappingCyclesAsync()` - بررسی تداخل زمانی
- `HasCircularDependencyAsync()` - بررسی وابستگی دایره‌ای

**استفاده:**
```csharp
// در Application Service
var isUnique = await _validationService.IsCycleNameUniqueAsync(
    facilityId,
    cycleName,
    excludeCycleId);

var overlappingCycles = await _validationService.GetOverlappingCyclesAsync(
    facilityId,
    startDate,
    endDate,
    excludeCycleId);
```

**چرا Domain Service:**
- نیاز به دسترسی به Repository برای بررسی یکتایی
- نیاز به بررسی چند Aggregate Root (چند Cycle)
- Business rule که به یک Aggregate خاص تعلق ندارد

---

### 3. **IFacilityQuotaService** ✅

**هدف:** مدیریت و محاسبات سهمیه

**مکان:** `Domain/Services/FacilityQuotaService.cs`

**متدها:**
- `CanAcceptNewRequest()` - بررسی امکان پذیرش درخواست جدید
- `CalculateRemainingQuota()` - محاسبه سهمیه باقیمانده
- `CanIncrementQuota()` - بررسی امکان افزایش سهمیه

**استفاده:**
```csharp
// در Application Service یا Domain Event Handler
var canAccept = _quotaService.CanAcceptNewRequest(
    cycle.Quota,
    cycle.UsedQuota,
    cycle.Status,
    cycle.StartDate,
    cycle.EndDate);

if (canAccept)
{
    cycle.IncrementUsedQuota();
}
```

**چرا Domain Service:**
- Logic محاسباتی که می‌تواند در چند جا استفاده شود
- Business rule برای مدیریت سهمیه
- Stateless و قابل تست

---

### 4. **IFacilityDependencyService** ✅

**هدف:** مدیریت و اعتبارسنجی وابستگی‌های دوره‌ها

**مکان:** `Domain/Services/FacilityDependencyService.cs`

**متدها:**
- `HasCircularDependency()` - بررسی وابستگی دایره‌ای
- `IsValidDependency()` - بررسی معتبر بودن وابستگی
- `AreDependenciesSatisfied()` - بررسی برآورده شدن وابستگی‌ها

**استفاده:**
```csharp
// در Application Service
var cycle = await _cycleRepository.GetByIdAsync(cycleId);
var hasCircular = _dependencyService.HasCircularDependency(
    cycle,
    requiredFacilityId,
    cycle.FacilityId);

if (hasCircular)
{
    throw new InvalidOperationException("Circular dependency detected");
}
```

**چرا Domain Service:**
- ✅ Logic پیچیده برای بررسی وابستگی‌ها
- ✅ Stateless - هیچ state ندارد
- ✅ Pure Domain Logic - فقط validation

---

### 5. **IUniqueConstraintManager** ✅ (موجود)

**هدف:** مدیریت قیود یکتای پویا

**مکان:** `Domain/Services/UniqueConstraintManager.cs`

**متدها:**
- `GeneratePolicyBasedUniqueKeyAsync()` - تولید کلید یکتا
- `IsPerCycleOnceUniqueAsync()` - بررسی یکتایی PerCycleOnce
- `IsExclusiveSetUniqueAsync()` - بررسی یکتایی ExclusiveSet

**چرا Domain Service:**
- ✅ Logic پیچیده برای تولید کلیدهای یکتا
- ✅ Stateless - هیچ state ندارد
- ✅ Business rule برای idempotency

---

## 🚫 Domain Services که **نمی‌سازیم**

### ❌ **FacilityCreationService**
**دلیل:** ایجاد `Facility` یک کار ساده است که در Constructor انجام می‌شود. نیازی به Domain Service نیست.

### ❌ **FacilityCycleCreationService**
**دلیل:** ایجاد `FacilityCycle` باید در Application Service انجام شود (نیاز به Repository). Domain Service مناسب نیست.

### ❌ **FacilityRequestCreationService**
**دلیل:** ایجاد `FacilityRequest` باید در Application Service انجام شود (نیاز به چند Repository). Domain Service مناسب نیست.

---

## 📋 **الگوی استفاده از Domain Services**

### ✅ **صحیح:**
```csharp
// Application Service
public class CreateFacilityCycleCommandHandler
{
    private readonly IFacilityCycleRepository _cycleRepository;
    private readonly IFacilityCycleValidationService _validationService;

    public async Task<FacilityCycle> Handle(CreateFacilityCycleCommand command)
    {
        // 1. Validate using Domain Service
        var isUnique = await _validationService.IsCycleNameUniqueAsync(
            command.FacilityId,
            command.Name);

        if (!isUnique)
            throw new InvalidOperationException("Cycle name must be unique");

        // 2. Create Aggregate Root
        var cycle = new FacilityCycle(
            command.FacilityId,
            command.Name,
            command.StartDate,
            command.EndDate,
            command.Quota);

        // 3. Save
        await _cycleRepository.AddAsync(cycle);
        return cycle;
    }
}
```

### ❌ **غلط:**
```csharp
// ❌ Domain Service نباید Repository داشته باشد (مگر برای validation)
public class BadDomainService
{
    private readonly IFacilityRepository _facilityRepository; // ❌

    public async Task<Facility> CreateFacility(...) // ❌ این باید در Application Service باشد
    {
        // ...
    }
}
```

---

## 🎯 **خلاصه**

| Domain Service | هدف | وابستگی | Repository |
|---------------|-----|----------|-----------|
| `IFacilityCycleEligibilityService` | بررسی واجد شرایط بودن | Stateless | ❌ No |
| `IFacilityCycleValidationService` | اعتبارسنجی قوانین | Stateless | ✅ Yes (for validation) |
| `IFacilityQuotaService` | مدیریت سهمیه | Stateless | ❌ No |
| `IFacilityDependencyService` | مدیریت وابستگی‌ها | Stateless | ❌ No |
| `IUniqueConstraintManager` | مدیریت قیود یکتا | Stateless | ✅ Yes (for checking) |

---

## ✅ **قوانین Domain Services**

1. **Stateless** - نباید state داشته باشند
2. **Repository Injection مجاز** - اما فقط برای **query/validation** نه save
3. **Domain Logic Only** - فقط Domain Logic، نه Application Logic
4. **Use Entities** - می‌توانند Domain Entities را به عنوان parameter دریافت کنند
5. **Abstractions Only** - باید از Interfaces استفاده کنند نه Concrete classes
6. **Testable** - باید قابل تست باشند

### ✅ **چه چیزی مجاز است:**

- ✅ Injection کردن Repository interfaces برای validation
- ✅ دریافت Domain Entities به عنوان parameter
- ✅ استفاده از Value Objects
- ✅ Query کردن از Repository (برای validation)

### ❌ **چه چیزی ممنوع است:**

- ❌ Save/Persist operations (باید در Application Service باشد)
- ❌ Stateful operations
- ❌ Infrastructure concerns
- ❌ Transaction management

---

## 📝 **نکات مهم**

- Domain Services برای **coordination** بین Aggregates استفاده می‌شوند
- Application Services برای **orchestration** و **transaction management** استفاده می‌شوند
- Domain Services می‌توانند Repository داشته باشند **فقط** برای validation (مثل `FacilityCycleValidationService`)
- Domain Services باید **stateless** باشند

