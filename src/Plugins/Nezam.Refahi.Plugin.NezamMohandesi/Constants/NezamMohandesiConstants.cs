using Nezam.Refahi.Membership.Domain.Entities;

namespace Nezam.Refahi.Plugin.NezamMohandesi.Constants;

public static class NezamMohandesiConstants
{
    #region Plugin Information
    public static class PluginInfo
    {
        public const string Name = "NezamMohandesi";
        public const string DisplayName = "Organization of Engineering System";
        public const int SeedPriority = 100;
    }
    #endregion

    #region Features

    /// <summary>
    /// Service Fields Features
    /// </summary>
    public static class ServiceFieldsFeatures
    {
        public static readonly Features Structure = new("structure", "Structure Engineering / سازه", Features.FeatureTypes.ServiceField);
        public static readonly Features Mechanic = new("mechanic", "Mechanic Engineering / مکانیک", Features.FeatureTypes.ServiceField);
        public static readonly Features Electricity = new("electricity", "Electricity Engineering / برق", Features.FeatureTypes.ServiceField);
        public static readonly Features Architecture = new("architecture", "Architecture / معماری", Features.FeatureTypes.ServiceField);

        public static IEnumerable<Features> All => new[]
        {
            Structure, Mechanic, Electricity, Architecture
        };
    }

    /// <summary>
    /// Service Types Features
    /// </summary>
    public static class ServiceTypesFeatures
    {
        public static readonly Features Design = new("designer", "Design / طراح", Features.FeatureTypes.ServiceType);
        public static readonly Features Supervision = new("supervisor", "Supervision / ناظر", Features.FeatureTypes.ServiceType);
        public static readonly Features Execute = new("execute", "Execute / اجرا", Features.FeatureTypes.ServiceType);
        public static readonly Features Executor = new("executor", "Executor / مجری", Features.FeatureTypes.ServiceType);

        public static IEnumerable<Features> All => new[]
        {
            Design, Supervision, Execute, Executor
        };
    }

    /// <summary>
    /// License Status Features
    /// </summary>
    public static class LicenseStatusFeatures
    {
        public static readonly Features HasLicense = new("has_license", "Licensed Professional / دارای پروانه", Features.FeatureTypes.LicenseStatus);
        public static readonly Features NoLicense = new("no_license", "Non-Licensed Professional / بدون پروانه", Features.FeatureTypes.LicenseStatus);

        public static IEnumerable<Features> All => new[]
        {
            HasLicense, NoLicense
        };
    }

    /// <summary>
    /// Grade Features
    /// </summary>
    public static class GradesFeatures
    {
        public static readonly Features Master = new("master", "Master Grade / پایه ارشد", Features.FeatureTypes.Grade);
        public static readonly Features Grade1 = new("grade1", "Grade 1 / پایه ۱", Features.FeatureTypes.Grade);
        public static readonly Features Grade2 = new("grade2", "Grade 2 / پایه ۲", Features.FeatureTypes.Grade);
        public static readonly Features Grade3 = new("grade3", "Grade 3 / پایه ۳", Features.FeatureTypes.Grade);

        public static IEnumerable<Features> All => new[]
        {
            Master, Grade1, Grade2, Grade3
        };
    }

    

    /// <summary>
    /// Gets all predefined features
    /// </summary>
    public static IEnumerable<Features> AllPredefinedFeatures
    {
        get
        {
            foreach (var feature in ServiceFieldsFeatures.All)
                yield return feature;

            foreach (var feature in ServiceTypesFeatures.All)
                yield return feature;

            foreach (var feature in LicenseStatusFeatures.All)
                yield return feature;

            foreach (var feature in GradesFeatures.All)
                yield return feature;

    
        }
    }

    #endregion

    #region Capabilities

    /// <summary>
    /// Gets all predefined capabilities for the NezamMohandesi plugin
    /// </summary>
    public static IEnumerable<Capability> AllPredefinedCapabilities
    {
        get
        {
           

            // License Status Capabilities
            var hasLicense = new Capability(
                "has_license",
                "Licensed Professional / دارای پروانه",
                "Capability for professionals with valid licenses",
                DateTime.UtcNow,
                null);
            hasLicense.AddFeature(new Features(LicenseStatusFeatures.HasLicense.Id, LicenseStatusFeatures.HasLicense.Title, LicenseStatusFeatures.HasLicense.Type));
            yield return hasLicense;

            var noLicense = new Capability(
                "no_license",
                "Non-Licensed Professional / بدون پروانه",
                "Capability for professionals without licenses",
                DateTime.UtcNow,
                null);
            noLicense.AddFeature(new Features(LicenseStatusFeatures.NoLicense.Id, LicenseStatusFeatures.NoLicense.Title, LicenseStatusFeatures.NoLicense.Type));
            yield return noLicense;

            // Create capabilities for all combinations of ServiceFields + ServiceTypes + Grades
            foreach (var serviceField in ServiceFieldsFeatures.All)
            {
                foreach (var serviceType in ServiceTypesFeatures.All)
                {
                    foreach (var grade in GradesFeatures.All)
                    {
                        var capabilityKey = $"{serviceField.Id}_{serviceType.Id}_{grade.Id}";
                        var capabilityName = $"{serviceField.Title} - {serviceType.Title} - {grade.Title}";
                        var capabilityDescription = $"Capability for {serviceField.Title} {serviceType.Title} at {grade.Title} level";

                        var capability = new Capability(
                            capabilityKey,
                            capabilityName,
                            capabilityDescription,
                            DateTime.UtcNow,
                            null);

                        // Create new feature instances for each capability to avoid EF tracking conflicts
                        var serviceFieldFeature = new Features(serviceField.Id, serviceField.Title, serviceField.Type);
                        var serviceTypeFeature = new Features(serviceType.Id, serviceType.Title, serviceType.Type);
                        var gradeFeature = new Features(grade.Id, grade.Title, grade.Type);

                        capability.AddFeature(serviceFieldFeature);
                        capability.AddFeature(serviceTypeFeature);
                        capability.AddFeature(gradeFeature);

                        yield return capability;
                    }
                }
            }
        }
    }

    #endregion
}