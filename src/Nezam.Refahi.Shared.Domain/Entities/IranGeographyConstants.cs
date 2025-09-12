namespace Nezam.Refahi.Shared.Domain.Entities;

/// <summary>
/// Comprehensive constants for Iranian geography - provinces and cities
/// </summary>
public static class IranGeographyConstants
{
    /// <summary>
    /// Gets all provinces with their basic information
    /// </summary>
    public static readonly (string Name, string Code, string EnglishName)[] AllProvinces = IranProvinces.AllProvinces;

    /// <summary>
    /// Gets all cities for a specific province by name
    /// </summary>
    /// <param name="provinceName">Name of the province in Persian</param>
    /// <returns>Array of city names</returns>
    public static string[] GetCitiesByProvince(string provinceName)
    {
        // Try main cities first
        var mainCities = IranCities.GetCitiesByProvince(provinceName);
        if (mainCities.Length > 0)
            return mainCities;

        // Try extended cities
        var extendedCities = IranProvincesExtended.GetCitiesByProvinceExtended(provinceName);
        if (extendedCities.Length > 0)
            return extendedCities;

        return Array.Empty<string>();
    }

    /// <summary>
    /// Gets all cities for a specific province by code
    /// </summary>
    /// <param name="provinceCode">Code of the province (e.g., "TEH", "ESF")</param>
    /// <returns>Array of city names</returns>
    public static string[] GetCitiesByProvinceCode(string provinceCode)
    {
        var province = AllProvinces.FirstOrDefault(p => p.Code == provinceCode);
        return province.Name != null ? GetCitiesByProvince(province.Name) : Array.Empty<string>();
    }

    /// <summary>
    /// Gets all cities for a specific province by English name
    /// </summary>
    /// <param name="provinceEnglishName">English name of the province</param>
    /// <returns>Array of city names</returns>
    public static string[] GetCitiesByProvinceEnglishName(string provinceEnglishName)
    {
        var province = AllProvinces.FirstOrDefault(p => p.EnglishName == provinceEnglishName);
        return province.Name != null ? GetCitiesByProvince(province.Name) : Array.Empty<string>();
    }

    /// <summary>
    /// Gets province information by name
    /// </summary>
    /// <param name="provinceName">Name of the province in Persian</param>
    /// <returns>Province information tuple or null if not found</returns>
    public static (string Name, string Code, string EnglishName)? GetProvinceByName(string provinceName)
    {
        return AllProvinces.FirstOrDefault(p => p.Name == provinceName);
    }

    /// <summary>
    /// Gets province information by code
    /// </summary>
    /// <param name="provinceCode">Code of the province</param>
    /// <returns>Province information tuple or null if not found</returns>
    public static (string Name, string Code, string EnglishName)? GetProvinceByCode(string provinceCode)
    {
        return AllProvinces.FirstOrDefault(p => p.Code == provinceCode);
    }

    /// <summary>
    /// Gets province information by English name
    /// </summary>
    /// <param name="provinceEnglishName">English name of the province</param>
    /// <returns>Province information tuple or null if not found</returns>
    public static (string Name, string Code, string EnglishName)? GetProvinceByEnglishName(string provinceEnglishName)
    {
        return AllProvinces.FirstOrDefault(p => p.EnglishName == provinceEnglishName);
    }

    /// <summary>
    /// Checks if a city belongs to a specific province
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <param name="provinceName">Name of the province</param>
    /// <returns>True if the city belongs to the province</returns>
    public static bool IsCityInProvince(string cityName, string provinceName)
    {
        var cities = GetCitiesByProvince(provinceName);
        return cities.Contains(cityName);
    }

    /// <summary>
    /// Gets the total number of provinces
    /// </summary>
    public static int TotalProvinces => AllProvinces.Length;

    /// <summary>
    /// Gets the total number of cities across all provinces
    /// </summary>
    public static int TotalCities => AllProvinces.Sum(p => GetCitiesByProvince(p.Name).Length);

    /// <summary>
    /// Gets all unique city names across all provinces
    /// </summary>
    public static string[] AllCities => AllProvinces
        .SelectMany(p => GetCitiesByProvince(p.Name))
        .Distinct()
        .OrderBy(c => c)
        .ToArray();

    /// <summary>
    /// Gets provinces sorted by name
    /// </summary>
    public static (string Name, string Code, string EnglishName)[] ProvincesSortedByName => 
        AllProvinces.OrderBy(p => p.Name).ToArray();

    /// <summary>
    /// Gets provinces sorted by code
    /// </summary>
    public static (string Name, string Code, string EnglishName)[] ProvincesSortedByCode => 
        AllProvinces.OrderBy(p => p.Code).ToArray();

    /// <summary>
    /// Gets provinces sorted by English name
    /// </summary>
    public static (string Name, string Code, string EnglishName)[] ProvincesSortedByEnglishName => 
        AllProvinces.OrderBy(p => p.EnglishName).ToArray();

    /// <summary>
    /// Gets all provinces with their cities
    /// </summary>
    public static (string ProvinceName, string ProvinceCode, string ProvinceEnglishName, string[] Cities)[] ProvincesWithCities =>
        AllProvinces.Select(p => (p.Name, p.Code, p.EnglishName, GetCitiesByProvince(p.Name))).ToArray();

    /// <summary>
    /// Gets provinces with city count
    /// </summary>
    public static (string ProvinceName, string ProvinceCode, int CityCount)[] ProvincesWithCityCount =>
        AllProvinces.Select(p => (p.Name, p.Code, GetCitiesByProvince(p.Name).Length)).ToArray();

    /// <summary>
    /// Gets the province with the most cities
    /// </summary>
    public static (string ProvinceName, int CityCount)? ProvinceWithMostCities
    {
        get
        {
            var provincesWithCount = ProvincesWithCityCount;
            if (!provincesWithCount.Any()) return null;
            
            var max = provincesWithCount.Max(p => p.CityCount);
            var province = provincesWithCount.First(p => p.CityCount == max);
            return (province.ProvinceName, province.CityCount);
        }
    }

    /// <summary>
    /// Gets the province with the least cities
    /// </summary>
    public static (string ProvinceName, int CityCount)? ProvinceWithLeastCities
    {
        get
        {
            var provincesWithCount = ProvincesWithCityCount;
            if (!provincesWithCount.Any()) return null;
            
            var min = provincesWithCount.Min(p => p.CityCount);
            var province = provincesWithCount.First(p => p.CityCount == min);
            return (province.ProvinceName, province.CityCount);
        }
    }

    /// <summary>
    /// Gets average number of cities per province
    /// </summary>
    public static double AverageCitiesPerProvince => 
        AllProvinces.Any() ? (double)TotalCities / TotalProvinces : 0;

    /// <summary>
    /// Gets provinces with city count above average
    /// </summary>
    public static (string ProvinceName, string ProvinceCode, int CityCount)[] ProvincesAboveAverageCities
    {
        get
        {
            var average = AverageCitiesPerProvince;
            return ProvincesWithCityCount
                .Where(p => p.CityCount > average)
                .OrderByDescending(p => p.CityCount)
                .ToArray();
        }
    }

    /// <summary>
    /// Gets provinces with city count below average
    /// </summary>
    public static (string ProvinceName, string ProvinceCode, int CityCount)[] ProvincesBelowAverageCities
    {
        get
        {
            var average = AverageCitiesPerProvince;
            return ProvincesWithCityCount
                .Where(p => p.CityCount < average)
                .OrderBy(p => p.CityCount)
                .ToArray();
        }
    }
}
