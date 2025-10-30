# تحلیل جامع کانتکست Membership - ساختار دامنه و توضیحات

## 🎯 **هدف کلی کانتکست Membership**

کانتکست Membership مسئول مدیریت **اعضای سازمان** و **صلاحیت‌های آن‌ها** است. این کانتکست شامل:
- مدیریت اطلاعات اعضا
- مدیریت نقش‌ها (Roles)
- مدیریت قابلیت‌ها (Capabilities) 
- مدیریت ویژگی‌ها (Features)
- ارتباط با سیستم‌های خارجی

---

## 🏗️ **ساختار کلی کانتکست**

### **1. لایه‌های معماری (Clean Architecture)**

```
📁 Membership/
├── 📁 Domain/           # هسته کسب‌وکار
├── 📁 Application/      # منطق کاربردی
├── 📁 Infrastructure/   # پیاده‌سازی فنی
├── 📁 Contracts/        # قراردادهای بین‌کانتکستی
└── 📁 Presentation/     # رابط کاربری
```

---

## 🎭 **مدل دامنه (Domain Model)**

### **1. Aggregate Root اصلی: Member**

```csharp
public sealed class Member : FullAggregateRoot<Guid>
{
    // شناسه‌های اصلی
    public Guid? UserId { get; private set; }           // ارتباط با Identity
    public string MembershipNumber { get; private set; } // شماره عضویت
    public NationalId NationalCode { get; private set; } // کد ملی
    
    // اطلاعات شخصی
    public FullName FullName { get; private set; }      // نام کامل
    public Email Email { get; private set; }           // ایمیل
    public PhoneNumber PhoneNumber { get; private set; } // شماره تلفن
    public DateTime? BirthDate { get; private set; }   // تاریخ تولد
    
    // روابط
    private readonly List<MemberRole> _roles;          // نقش‌های عضو
    private readonly List<MemberCapability> _capabilities; // قابلیت‌های عضو
}
```

**ویژگی‌های کلیدی Member:**
- ✅ **Aggregate Root**: کنترل کامل بر داده‌های عضو
- ✅ **Encapsulation**: تغییرات فقط از طریق متدهای دامنه
- ✅ **Business Logic**: منطق کسب‌وکار در خود entity
- ✅ **Value Objects**: استفاده از FullName, Email, PhoneNumber

### **2. Entity های پشتیبان**

#### **Role (نقش)**
```csharp
public sealed class Role : Entity<Guid>
{
    public string Key { get; private set; }           // کلید یکتا (مثل "Engineer")
    public string Title { get; private set; }         // عنوان انگلیسی
    public string? EmployerName { get; private set; } // نام کارفرما
    public string? EmployerCode { get; private set; } // کد کارفرما
    public bool IsActive { get; private set; }        // وضعیت فعال
}
```

#### **Capability (قابلیت)**
```csharp
public sealed class Capability : Entity<string>
{
    public string Name { get; private set; }          // نام قابلیت
    public string Description { get; private set; }   // توضیحات
    public DateTime? ValidFrom { get; private set; }  // تاریخ شروع اعتبار
    public DateTime? ValidTo { get; private set; }    // تاریخ پایان اعتبار
    private readonly List<Features> _features;       // ویژگی‌های قابلیت
}
```

#### **Features (ویژگی)**
```csharp
public sealed class Features : Entity<string>
{
    public string Title { get; private set; }         // عنوان ویژگی
    public string Type { get; private set; }         // نوع ویژگی
    
    // انواع ویژگی‌ها
    public static class FeatureTypes
    {
        public const string ServiceField = "service_field";
        public const string ServiceType = "service_type";
        public const string LicenseStatus = "license_status";
        public const string Grade = "grade";
        public const string SpecialCapability = "special_capability";
    }
}
```

### **3. Junction Entities (ارتباطات)**

#### **MemberRole (نقش عضو)**
```csharp
public sealed class MemberRole : Entity<Guid>
{
    public Guid MemberId { get; private set; }       // شناسه عضو
    public Guid RoleId { get; private set; }          // شناسه نقش
    public DateTime? ValidFrom { get; private set; }  // تاریخ شروع اعتبار
    public DateTime? ValidTo { get; private set; }    // تاریخ پایان اعتبار
    public string? AssignedBy { get; private set; }   // اختصاص‌دهنده
    public bool IsActive { get; private set; }        // وضعیت فعال
}
```

#### **MemberCapability (قابلیت عضو)**
```csharp
public sealed class MemberCapability : Entity<Guid>
{
    public Guid MemberId { get; private set; }        // شناسه عضو
    public string CapabilityId { get; private set; } // شناسه قابلیت
    public DateTime? ValidFrom { get; private set; } // تاریخ شروع اعتبار
    public DateTime? ValidTo { get; private set; }   // تاریخ پایان اعتبار
    public string? AssignedBy { get; private set; }  // اختصاص‌دهنده
    public bool IsActive { get; private set; }       // وضعیت فعال
}
```

---

## 🔄 **روابط دامنه (Domain Relationships)**

### **1. روابط اصلی**

```
Member (1) ←→ (N) MemberRole ←→ (1) Role
Member (1) ←→ (N) MemberCapability ←→ (1) Capability
Capability (1) ←→ (N) Features
```

### **2. الگوی دسترسی**

```
Member → MemberCapability → Capability → Features
```

**مثال عملی:**
- عضو "احمد احمدی" → قابلیت "مهندس عمران" → ویژگی "مجوز طراحی ساختمان"

---

## 🎯 **Value Objects**

### **1. FullName**
```csharp
public class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    
    public override string ToString() => $"{FirstName} {LastName}";
}
```

### **2. Email**
```csharp
public class Email : ValueObject
{
    private readonly string _value;
    
    // اعتبارسنجی خودکار ایمیل
    private static bool IsValid(string email)
    {
        const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }
}
```

---

## 🔧 **سرویس‌های دامنه (Domain Services)**

### **1. IMemberService (سرویس اصلی)**

```csharp
public interface IMemberService
{
    // جستجوی عضو
    Task<MemberDto?> GetMemberByNationalCodeAsync(NationalId nationalCode);
    Task<MemberDto?> GetMemberByMembershipNumberAsync(string membershipNumber);
    Task<MemberDto?> GetMemberByExternalIdAsync(string externalId);
    
    // بررسی صلاحیت‌ها
    Task<bool> HasCapabilityAsync(NationalId nationalCode, string capabilityId);
    Task<bool> HasFeatureAsync(NationalId nationalCode, string featureId);
    Task<IEnumerable<string>> GetMemberCapabilitiesAsync(NationalId nationalCode);
    
    // بررسی عضویت
    Task<bool> HasActiveMembershipAsync(NationalId nationalCode);
    Task<BasicMemberInfoDto?> GetBasicMemberInfoAsync(NationalId nationalCode);
}
```

### **2. IExternalMemberStorage (ذخیره‌سازی خارجی)**

```csharp
public interface IExternalMemberStorage
{
    Task<ExternalMemberResponseDto?> GetMemberByNationalCodeAsync(NationalId nationalCode);
    Task<IEnumerable<ExternalMemberResponseDto>> SearchMembersAsync(ExternalMemberSearchCriteria criteria);
}
```

---

## 🏛️ **Repository Pattern**

### **1. Repository های اصلی**

```csharp
// Member Repository
public interface IMemberRepository : IRepository<Member, Guid>
{
    Task<Member?> GetByNationalCodeAsync(NationalId nationalCode);
    Task<Member?> GetByMembershipNumberAsync(string membershipNumber);
    Task<Member?> GetByPhoneNumberAsync(PhoneNumber phoneNumber);
    Task<bool> IsNationalCodeExistsAsync(NationalId nationalCode);
}

// Role Repository
public interface IRoleRepository : IRepository<Role, Guid>
{
    Task<Role?> GetByKeyAsync(string key);
    Task<IEnumerable<Role>> GetActiveRolesAsync();
}

// Capability Repository
public interface ICapabilityRepository : IRepository<Capability, string>
{
    Task<Capability?> GetByIdAsync(string id);
    Task<IEnumerable<Capability>> GetActiveCapabilitiesAsync();
}
```

---

## 🔄 **Unit of Work Pattern**

### **1. IMembershipUnitOfWork**

```csharp
public interface IMembershipUnitOfWork : IUnitOfWork
{
    // Repository ها
    IMemberRepository Members { get; }
    IRoleRepository Roles { get; }
    ICapabilityRepository Capabilities { get; }
    IFeatureRepository Features { get; }
    IMemberRoleRepository MemberRoles { get; }
    IMemberCapabilityRepository MemberCapabilities { get; }
    
    // مدیریت تراکنش
    Task BeginAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
```

---

## 🌐 **ارتباط با کانتکست‌های دیگر**

### **1. Identity Context**
- **ارتباط**: `Member.UserId` → `Identity.User.Id`
- **هدف**: ارتباط عضو با حساب کاربری

### **2. Recreation Context**
- **ارتباط**: `IMemberService` → `TourReservation`
- **هدف**: بررسی صلاحیت عضو برای رزرو تور

### **3. Finance Context**
- **ارتباط**: `IMemberService` → `Bill`
- **هدف**: ارتباط فاکتور با عضو

---

## 📊 **مدل داده (Data Model)**

### **1. جداول اصلی**

```sql
-- جدول اعضا
CREATE TABLE [membership].[Members] (
    [Id] uniqueidentifier PRIMARY KEY,
    [UserId] uniqueidentifier NULL,           -- ارتباط با Identity
    [MembershipNumber] nvarchar(50) NOT NULL,
    [NationalCode] nvarchar(10) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [BirthDate] date NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(100) NOT NULL
);

-- جدول نقش‌ها
CREATE TABLE [membership].[Roles] (
    [Id] uniqueidentifier PRIMARY KEY,
    [Key] nvarchar(50) NOT NULL UNIQUE,
    [Title] nvarchar(100) NOT NULL,
    [EmployerName] nvarchar(100) NULL,
    [EmployerCode] nvarchar(50) NULL,
    [IsActive] bit NOT NULL DEFAULT 1
);

-- جدول قابلیت‌ها
CREATE TABLE [membership].[Capabilities] (
    [Id] nvarchar(50) PRIMARY KEY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [ValidFrom] datetime2 NULL,
    [ValidTo] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT 1
);

-- جدول ویژگی‌ها
CREATE TABLE [membership].[Features] (
    [Id] nvarchar(50) PRIMARY KEY,
    [Title] nvarchar(100) NOT NULL,
    [Type] nvarchar(50) NOT NULL
);

-- جدول ارتباط عضو-نقش
CREATE TABLE [membership].[MemberRoles] (
    [Id] uniqueidentifier PRIMARY KEY,
    [MemberId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    [ValidFrom] datetime2 NULL,
    [ValidTo] datetime2 NULL,
    [AssignedBy] nvarchar(100) NULL,
    [IsActive] bit NOT NULL DEFAULT 1
);

-- جدول ارتباط عضو-قابلیت
CREATE TABLE [membership].[MemberCapabilities] (
    [Id] uniqueidentifier PRIMARY KEY,
    [MemberId] uniqueidentifier NOT NULL,
    [CapabilityId] nvarchar(50) NOT NULL,
    [ValidFrom] datetime2 NULL,
    [ValidTo] datetime2 NULL,
    [AssignedBy] nvarchar(100) NULL,
    [IsActive] bit NOT NULL DEFAULT 1
);

-- جدول ارتباط قابلیت-ویژگی
CREATE TABLE [membership].[CapabilityFeatures] (
    [CapabilityId] nvarchar(50) NOT NULL,
    [FeatureId] nvarchar(50) NOT NULL,
    PRIMARY KEY ([CapabilityId], [FeatureId])
);
```

---

## 🔐 **قوانین کسب‌وکار (Business Rules)**

### **1. قوانین Member**

```csharp
// ✅ اعتبارسنجی کد ملی
if (string.IsNullOrWhiteSpace(nationalCode))
    throw new ArgumentException("کد ملی الزامی است");

// ✅ اعتبارسنجی ایمیل
if (!IsValid(email))
    throw new ArgumentException("فرمت ایمیل نامعتبر است");

// ✅ بررسی تکراری بودن کد ملی
if (await IsNationalCodeExistsAsync(nationalCode))
    throw new InvalidOperationException("عضو با این کد ملی قبلاً ثبت شده است");
```

### **2. قوانین Role Assignment**

```csharp
// ✅ بررسی اعتبار نقش
public bool IsValid()
{
    if (!IsActive) return false;
    
    var now = DateTimeOffset.UtcNow;
    if (ValidFrom.HasValue && now < ValidFrom.Value) return false;
    if (ValidTo.HasValue && now > ValidTo.Value) return false;
    
    return true;
}
```

### **3. قوانین Capability Assignment**

```csharp
// ✅ بررسی اعتبار قابلیت
public bool IsValid()
{
    if (!IsActive) return false;
    
    var now = DateTimeOffset.UtcNow;
    if (ValidFrom.HasValue && now < ValidFrom.Value) return false;
    if (ValidTo.HasValue && now > ValidTo.Value) return false;
    
    return true;
}
```

---

## 🔄 **جریان داده (Data Flow)**

### **1. ایجاد عضو جدید**

```
1. درخواست ایجاد عضو
2. اعتبارسنجی داده‌ها
3. ایجاد Member entity
4. اختصاص نقش‌ها
5. اختصاص قابلیت‌ها
6. ذخیره در دیتابیس
7. بازگشت MemberDto
```

### **2. جستجوی عضو**

```
1. درخواست جستجو (کد ملی)
2. جستجو در دیتابیس محلی
3. اگر یافت نشد → جستجو در سیستم خارجی
4. اگر در سیستم خارجی یافت شد → ایجاد عضو محلی
5. بازگشت MemberDto
```

### **3. بررسی صلاحیت**

```
1. درخواست بررسی صلاحیت
2. جستجوی عضو
3. بررسی قابلیت‌های فعال
4. بررسی ویژگی‌های قابلیت
5. بازگشت نتیجه boolean
```

---

## 🎯 **مثال‌های عملی**

### **1. ایجاد عضو مهندس**

```csharp
// ایجاد عضو
var member = new Member(
    membershipNumber: "ENG001",
    nationalCode: new NationalId("1234567890"),
    fullName: new FullName("احمد", "احمدی"),
    email: new Email("ahmad@example.com"),
    phoneNumber: new PhoneNumber("09123456789")
);

// اختصاص نقش
member.AssignRole(engineerRoleId, assignedBy: "admin");

// اختصاص قابلیت
member.AssignCapability("civil_engineer", assignedBy: "admin");
```

### **2. بررسی صلاحیت برای رزرو تور**

```csharp
// در Recreation Context
var hasCapability = await _memberService.HasCapabilityAsync(
    nationalCode, 
    "civil_engineer"
);

if (!hasCapability)
{
    return ApplicationResult.Failure("عضو فاقد صلاحیت مهندسی عمران است");
}
```

---

## 🔧 **نکات فنی مهم**

### **1. External System Integration**

- **Plugin Pattern**: استفاده از `IExternalMemberStorage` برای سیستم‌های خارجی
- **Fallback Strategy**: جستجو در سیستم محلی → سیستم خارجی
- **Data Synchronization**: همگام‌سازی خودکار داده‌ها

### **2. Performance Considerations**

- **Caching**: کش کردن اطلاعات عضو
- **Lazy Loading**: بارگذاری تنبل قابلیت‌ها
- **Indexing**: ایندکس روی کد ملی و شماره عضویت

### **3. Security**

- **Data Validation**: اعتبارسنجی کامل داده‌ها
- **Access Control**: کنترل دسترسی بر اساس قابلیت‌ها
- **Audit Trail**: ردیابی کامل تغییرات

---

## 📈 **نقاط قوت طراحی**

### **1. DDD Compliance**
- ✅ **Aggregate Root**: Member کنترل کامل دارد
- ✅ **Value Objects**: FullName, Email, PhoneNumber
- ✅ **Domain Services**: IMemberService
- ✅ **Repository Pattern**: جداسازی دسترسی به داده

### **2. Clean Architecture**
- ✅ **Dependency Inversion**: وابستگی به abstractions
- ✅ **Separation of Concerns**: جداسازی لایه‌ها
- ✅ **Testability**: قابلیت تست بالا

### **3. Extensibility**
- ✅ **Plugin System**: قابلیت افزودن سیستم‌های خارجی
- ✅ **Flexible Capabilities**: قابلیت‌های قابل تنظیم
- ✅ **Multi-tenant Ready**: آماده برای چند مستأجری

---

## 🎯 **خلاصه**

کانتکست Membership یک **مدل دامنه قوی و انعطاف‌پذیر** ارائه می‌دهد که:

1. **مدیریت کامل اعضا** با اطلاعات شخصی و حرفه‌ای
2. **سیستم نقش‌بندی انعطاف‌پذیر** با پشتیبانی از کارفرمایان مختلف
3. **سیستم قابلیت‌های قابل تنظیم** برای کنترل دسترسی
4. **ارتباط با سیستم‌های خارجی** از طریق Plugin Pattern
5. **معماری تمیز** با رعایت اصول DDD و Clean Architecture

این کانتکست به عنوان **مرکز مدیریت هویت و صلاحیت** در سیستم عمل می‌کند و پایه‌ای محکم برای سایر کانتکست‌ها فراهم می‌آورد.
