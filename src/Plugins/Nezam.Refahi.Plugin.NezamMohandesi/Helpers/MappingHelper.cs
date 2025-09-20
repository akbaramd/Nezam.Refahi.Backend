namespace Nezam.Refahi.Plugin.NezamMohandesi.Helpers;

public static class MappingHelper
{
    public static class ServiceFields
    {
        public const string Structure = "structure";
        public const string Mechanic = "mechanic";
        public const string Electricity = "electricity";
        public const string Architecture = "architecture";

        public static readonly Dictionary<string, string> DisplayNames = new()
        {
            { Structure, "Structure Engineering / سازه" },
            { Mechanic, "Mechanic Engineering / مکانیک" },
            { Electricity, "Electricity Engineering / برق" },
            { Architecture, "Architecture / معماری" }
        };
    }

    public static class ServiceTypes
    {
        public const string Design = "designer";
        public const string Supervision = "supervisor";
        public const string Execute = "execute";
        public const string Executor = "executor";

        public static readonly Dictionary<string, string> DisplayNames = new()
        {
            { Design, "Design / طراح" },
            { Supervision, "Supervision / ناظر" },
            { Execute, "Execute / اجرا" },
            { Executor, "Executor / مجری" }
        };
    }

    public static class Grades
    {
        public const string Master = "master";
        public const string Grade1 = "grade1";
        public const string Grade2 = "grade2";
        public const string Grade3 = "grade3";

        public static readonly Dictionary<string, string> DisplayNames = new()
        {
            { Master, "Master Grade / پایه ارشد" },
            { Grade1, "Grade 1 / پایه ۱" },
            { Grade2, "Grade 2 / پایه ۲" },
            { Grade3, "Grade 3 / پایه ۳" }
        };

        public static readonly Dictionary<string, int> Hierarchy = new()
        {
            { Master, 4 },
            { Grade1, 3 },
            { Grade2, 2 },
            { Grade3, 1 }
        };
    }

    public static class LicenseStatus
    {
        public const string NoLicense = "no_license";
        public const string HasLicense = "has_license";
    }

    public static class ClaimTypes
    {
        public const string ServiceTypes = "service_types";
        public const string ServiceFields = "service_fields";
        public const string LicenseStatus = "license_status";
        public const string LicenseGrade = "license_grade";
        public const string ProfessionalId = "professional_id";
    }

    public static class ClaimTypeTitles
    {
        public static string ServiceFields = "Service Fields / رشته های مهندسی";
        public const string ServiceTypes = "Service Types / انواع خدمات";
        public const string LicenseGrade = "License Grade / پایه پروانه";
        public const string LicenseStatus = "License Status / وضعیت پروانه";
        public const string ProfessionalId = "Professional ID / شماره نظام مهندسی";
    }

    public static class CapabilityKeys
    {
        public const string GeneralContractor = "general_contractor";
        public const string Employer = "employer_capability";
        public const string HasLicense = "has_license";
        public const string NoLicense = "no_license";

        public static string GenerateKey(string field, string serviceType, string grade)
        {
            return $"{field}_{serviceType}_{grade}";
        }

        public static IEnumerable<(string Field, string ServiceType, string Grade, string Key)> GetAllCombinations()
        {
            var allFields = new[] { ServiceFields.Structure, ServiceFields.Mechanic, ServiceFields.Electricity, ServiceFields.Architecture };
            var allTypes = new[] { ServiceTypes.Design, ServiceTypes.Supervision, ServiceTypes.Execute, ServiceTypes.Executor };
            var allGrades = new[] { Grades.Master, Grades.Grade1, Grades.Grade2, Grades.Grade3 };

            foreach (var field in allFields)
            {
                foreach (var serviceType in allTypes)
                {
                    foreach (var grade in allGrades)
                    {
                        yield return (field, serviceType, grade, GenerateKey(field, serviceType, grade));
                    }
                }
            }
        }
    }

    public static class CapabilityDisplayNames
    {
        public static readonly Dictionary<string, string> SpecialNames = new()
        {
   
            { CapabilityKeys.HasLicense, "Licensed Professional / دارای پروانه" },
            { CapabilityKeys.NoLicense, "Non-Licensed Professional / بدون پروانه" }
        };

        public static string GenerateDisplayName(string field, string serviceType, string grade)
        {
            var fieldName = ServiceFields.DisplayNames[field];
            var serviceTypeName = ServiceTypes.DisplayNames[serviceType];
            var gradeName = Grades.DisplayNames[grade];
            return $"{fieldName} {serviceTypeName} {gradeName}";
        }

        public static string GetDisplayName(string capabilityKey)
        {
            if (SpecialNames.ContainsKey(capabilityKey))
                return SpecialNames[capabilityKey];

            // Parse the key to extract field, service type, and grade
            var parts = capabilityKey.Split('_');
            if (parts.Length == 3)
            {
                var field = parts[0];
                var serviceType = parts[1];
                var grade = parts[2];
                return GenerateDisplayName(field, serviceType, grade);
            }

            return capabilityKey;
        }
    }

    public static class RoleKeys
    {
        public const string Member = "member";
        public const string Employer = "employer";
    }

    public static class RoleDisplayNames
    {
        public static readonly Dictionary<string, string> Names = new()
        {
            { RoleKeys.Member, "Member / مهندس" },
            { RoleKeys.Employer, "Employer / کارمند" }
        };
    }

    public static class ExternalMappings
    {
        public static readonly Dictionary<string, string> ServiceFieldMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { "سازه", ServiceFields.Structure },
            { "مکانیک", ServiceFields.Mechanic },
            { "برق", ServiceFields.Electricity },
            { "معماری", ServiceFields.Architecture },
            { "civil", ServiceFields.Structure },
            { "mechanical", ServiceFields.Mechanic },
            { "electrical", ServiceFields.Electricity },
            { "architecture", ServiceFields.Architecture }
        };

        public static readonly Dictionary<string, string> ServiceTypeMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { "طراح", ServiceTypes.Design },
            { "ناظر", ServiceTypes.Supervision },
            { "اجرا", ServiceTypes.Execute },
            { "مجری", ServiceTypes.Executor },
            { "design", ServiceTypes.Design },
            { "supervision", ServiceTypes.Supervision },
            { "consultation", ServiceTypes.Execute },
            { "execution", ServiceTypes.Executor }
        };

        public static readonly Dictionary<string, string> GradeMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { "استاد", Grades.Master },
            { "پایه یک", Grades.Grade1 },
            { "پایه ۱", Grades.Grade1 },
            { "پایه 1", Grades.Grade1 },
            { "پایه دو", Grades.Grade2 },
            { "پایه ۲", Grades.Grade2 },
            { "پایه 2", Grades.Grade2 },
            { "پایه سه", Grades.Grade3 },
            { "پایه ۳", Grades.Grade3 },
            { "پایه 3", Grades.Grade3 },
            { "master", Grades.Master },
            { "grade 1", Grades.Grade1 },
            { "grade 2", Grades.Grade2 },
            { "grade 3", Grades.Grade3 }
        };
    }
}