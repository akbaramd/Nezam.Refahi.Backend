# گزارش تحلیل DDD - Facilities Module

## مشکلات شناسایی شده

### 🔴 **مشکل 1: نقض Aggregate Boundary - Facility و FacilityCycle**

**موقعیت:** `Facility.cs` خط 24-25, 130-175

**مشکل:**
```csharp
// Facility.cs
private readonly List<FacilityCycle> _cycles = new();
public IReadOnlyCollection<FacilityCycle> Cycles => _cycles.AsReadOnly();

public FacilityCycle CreateCycle(...)
{
    var cycle = new FacilityCycle(...);
    _cycles.Add(cycle);  // ❌ مشکل: اضافه کردن Aggregate Root به لیست
    return cycle;
}
```

**نقض قوانین DDD:**
- ❌ `FacilityCycle` خودش Aggregate Root است (Repository دارد: `IFacilityCycleRepository`)
- ❌ `Facility` دارای لیست `FacilityCycle` است که Aggregate Root هستند
- ❌ یک Aggregate Root نمی‌تواند Aggregate Root دیگری را به عنوان child نگه دارد

**قانون نقض شده:**
> "ARs must not directly manipulate other ARs; use IDs + coordination"

**راه حل:**
- اگر `FacilityCycle` Aggregate Root مستقل است، `Facility` نباید لیست `_cycles` داشته باشد
- `Facility` فقط باید `FacilityId` را نگه دارد
- ارتباط از طریق ID و Event-driven coordination

---

### 🔴 **مشکل 2: نقض Cross-Aggregate Manipulation - FacilityCycle و FacilityRequest**

**موقعیت:** `FacilityCycle.cs` خط 59-60, 484-507

**مشکل:**
```csharp
// FacilityCycle.cs
private readonly List<FacilityRequest> _applications = new();
public IReadOnlyCollection<FacilityRequest> Applications => _applications.AsReadOnly();

public void AddApplication(FacilityRequest application)
{
    _applications.Add(application);  // ❌ مشکل: manipulate کردن Aggregate Root دیگر
    UsedQuota++;
}

public void RemoveApplication(FacilityRequest application)
{
    if (_applications.Remove(application))  // ❌ مشکل: manipulate کردن Aggregate Root دیگر
    {
        UsedQuota--;
    }
}
```

**نقض قوانین DDD:**
- ❌ `FacilityRequest` خودش Aggregate Root است (Repository دارد: `IFacilityRequestRepository`)
- ❌ `FacilityCycle` دارای لیست `FacilityRequest` است که Aggregate Root هستند
- ❌ `FacilityCycle` مستقیماً `FacilityRequest` را manipulate می‌کند

**قانون نقض شده:**
> "ARs must not directly manipulate other ARs; use IDs + coordination"

**راه حل:**
- `FacilityCycle` نباید لیست `_applications` داشته باشد
- فقط `UsedQuota` را از طریق Domain Events یا Application Service مدیریت کنید
- از Event-driven coordination استفاده کنید

---

### 🔴 **مشکل 3: Repository برای Internal Entity - FacilityCycleDependency**

**موقعیت:** `IFacilityCycleDependencyRepository.cs`

**مشکل:**
```csharp
// IFacilityCycleDependencyRepository.cs
public interface IFacilityCycleDependencyRepository : IRepository<FacilityCycleDependency, Guid>
```

**نقض قوانین DDD:**
- ❌ `FacilityCycleDependency` Entity داخلی `FacilityCycle` است (خط 39-40 در `FacilityCycle.cs`)
- ❌ Repository فقط باید برای Aggregate Roots باشد
- ❌ طبق قوانین: "Never create repositories for internal Entities; only Aggregate Roots"

**قانون نقض شده:**
> "Never create repositories for internal Entities; only Aggregate Roots"

**راه حل:**
- Repository `IFacilityCycleDependencyRepository` را حذف کنید
- دسترسی به Dependencies از طریق `FacilityCycle` Aggregate Root انجام شود

---

### 🟡 **مشکل 4: Navigation Properties به Aggregate Roots**

**موقعیت:** `FacilityRequest.cs` خط 52-53

**مشکل:**
```csharp
// FacilityRequest.cs
public Facility Facility { get; private set; } = null!;
public FacilityCycle FacilityCycle { get; private set; } = null!;
```

**نقض قوانین DDD:**
- 🟡 Navigation properties به Aggregate Roots دیگر (EF Core نیاز دارد اما برای Domain مناسب نیست)
- 🟡 بهتر است فقط ID نگه داریم: "Hard Rule: No direct Entity↔Entity across Aggregates"

**قانون نقض شده:**
> "Hard Rule: No direct Entity↔Entity across Aggregates"

**راه حل:**
- Navigation properties را برای EF Core نگه دارید (read-only)
- در Domain logic فقط از IDs استفاده کنید

---

### 🟡 **مشکل 5: FacilityRejection - Entity یا Value Object؟**

**موقعیت:** `FacilityRejection.cs`

**تحلیل:**
- `FacilityRejection` دارای Identity است (`Entity<Guid>`)
- `FacilityRejection` دارای Repository است (`IFacilityRejectionRepository`)
- اما `FacilityRejection` به نظر می‌رسد Entity داخلی `FacilityRequest` باشد

**سوال:**
- آیا `FacilityRejection` lifecycle مستقل دارد؟
- آیا باید Repository داشته باشد؟

**راه حل:**
- اگر lifecycle مستقل ندارد → باید Entity داخلی `FacilityRequest` باشد
- اگر lifecycle مستقل دارد → باید Aggregate Root باشد

---

## خلاصه مشکلات

| مشکل | Entity | شدت | قانون نقض شده |
|------|--------|-----|----------------|
| 1 | Facility | 🔴 Critical | Aggregate Boundary |
| 2 | FacilityCycle | 🔴 Critical | Cross-Aggregate Manipulation |
| 3 | FacilityCycleDependency | 🔴 Critical | Repository for Internal Entity |
| 4 | FacilityRequest | 🟡 Medium | Navigation Properties |
| 5 | FacilityRejection | 🟡 Medium | Entity Classification |

---

## توصیه‌های اصلاحی

### 1. جداسازی Aggregate Boundaries

**Facility و FacilityCycle:**
- `Facility` نباید لیست `_cycles` داشته باشد
- `Facility.CreateCycle()` باید فقط `FacilityCycle` را ایجاد کند و Event منتشر کند
- `Facility` فقط باید `FacilityId` را نگه دارد

**FacilityCycle و FacilityRequest:**
- `FacilityCycle` نباید لیست `_applications` داشته باشد
- `UsedQuota` باید از طریق Domain Events یا Application Service مدیریت شود
- از Event-driven coordination استفاده کنید

### 2. حذف Repository برای Internal Entities

- `IFacilityCycleDependencyRepository` را حذف کنید
- دسترسی از طریق `FacilityCycle` Aggregate Root

### 3. استفاده از Domain Events

- برای coordination بین Aggregates از Domain Events استفاده کنید
- Example: `FacilityRequestCreatedEvent` → `FacilityCycle` listener → Update `UsedQuota`

---

## وضعیت فعلی

- ✅ **Aggregate Roots:** `Facility`, `FacilityCycle`, `FacilityRequest`
- ✅ **Internal Entities:** `FacilityCycleDependency`, `FacilityCycleCapability`, `FacilityCycleFeature`, `FacilityCyclePriceOption`
- ❌ **مشکلات:** Cross-aggregate references, Repository for internal entities

