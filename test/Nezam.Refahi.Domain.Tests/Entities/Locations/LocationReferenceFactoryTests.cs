using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Services;
using Nezam.Refahi.Domain.BoundedContexts.Location.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Location.Repositories;
using Xunit;

namespace Nezam.Refahi.Domain.Tests.Entities.Locations;

public class LocationReferenceFactoryTests
{
    private readonly Mock<ICityRepository> _mockCityRepository;
    private readonly Mock<IProvinceRepository> _mockProvinceRepository;
    private readonly LocationReferenceFactory _factory;

    public LocationReferenceFactoryTests()
    {
        _mockCityRepository = new Mock<ICityRepository>();
        _mockProvinceRepository = new Mock<IProvinceRepository>();
        _factory = new LocationReferenceFactory(_mockCityRepository.Object, _mockProvinceRepository.Object);
    }

    [Fact]
    public void CreateFromEntities_Returns_Valid_LocationReference()
    {
        // Arrange
        var province = new Province("Tehran Province", "THR");
        var city = new City("Tehran");
        var address = "address";
        city.SetProvince(province);

        // Act
        var result = _factory.CreateFromEntities(city, province,address);

        // Assert
        Assert.Equal(city.Id, result.CityId);
        Assert.Equal(province.Id, result.ProvinceId);
        Assert.Equal(city.Name, result.CityName);
        Assert.Equal(province.Name, result.ProvinceName);
    }

    [Fact]
    public void CreateFromEntities_With_Null_City_Throws_Exception()
    {
        // Arrange
        var province = new Province("Tehran Province", "THR");
        var address = "address";
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _factory.CreateFromEntities(null, province,address));
    }

    [Fact]
    public void CreateFromEntities_With_Null_Province_Throws_Exception()
    {
        // Arrange
        var city = new City("Tehran");
        var address = "address";
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _factory.CreateFromEntities(city, null,address));
    }

    [Fact]
    public async Task CreateFromIdsAsync_Returns_Valid_LocationReference_When_Entities_Exist()
    {
        // Arrange
        var provinceId = Guid.NewGuid();
        var cityId = Guid.NewGuid();
        var address = "address";
        var province = new Province("Tehran Province", "THR");
        var city = new City("Tehran");
        city.SetProvince(province);
        
        // Use reflection to set the IDs since they're normally generated
        typeof(Province).GetProperty("Id").SetValue(province, provinceId);
        typeof(City).GetProperty("Id").SetValue(city, cityId);

        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId)).ReturnsAsync(city);
        _mockProvinceRepository.Setup(r => r.GetByIdAsync(provinceId)).ReturnsAsync(province);

        // Act
        var result = await _factory.CreateFromIdsAsync(cityId, provinceId,address);

        // Assert
        Assert.Equal(cityId, result.CityId);
        Assert.Equal(provinceId, result.ProvinceId);
        Assert.Equal(city.Name, result.CityName);
        Assert.Equal(province.Name, result.ProvinceName);
    }

    [Fact]
    public async Task CreateFromIdsAsync_Throws_Exception_When_City_Not_Found()
    {
        // Arrange
        var provinceId = Guid.NewGuid();
        var cityId = Guid.NewGuid();
        var address = "address";
        var province = new Province("Tehran Province", "THR");
        
        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId)).ReturnsAsync((City)null);
        _mockProvinceRepository.Setup(r => r.GetByIdAsync(provinceId)).ReturnsAsync(province);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _factory.CreateFromIdsAsync(cityId, provinceId,address));
        Assert.Contains("City", exception.Message);
    }

    [Fact]
    public async Task CreateFromIdsAsync_Throws_Exception_When_Province_Not_Found()
    {
        // Arrange
        var provinceId = Guid.NewGuid();
        var cityId = Guid.NewGuid();
        
        var city = new City("Tehran");
        
        _mockCityRepository.Setup(r => r.GetByIdAsync(cityId)).ReturnsAsync(city);
        _mockProvinceRepository.Setup(r => r.GetByIdAsync(provinceId)).ReturnsAsync((Province)null);
        var address = "address";
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _factory.CreateFromIdsAsync(cityId, provinceId,address));
        Assert.Contains("Province", exception.Message);
    }

    [Fact]
    public async Task CreateFromLegacyLocationAsync_Returns_Valid_LocationReference_When_Entities_Exist()
    {
        // Arrange
        string cityName = "Tehran";
        string provinceName = "Tehran Province";
        var address = "address";
        var province = new Province(provinceName, "THR");
        var city = new City(cityName);
        city.SetProvince(province);
        
        _mockProvinceRepository.Setup(r => r.FindByNameAsync(provinceName))
            .ReturnsAsync(new List<Province> { province });
        _mockCityRepository.Setup(r => r.FindByNameAsync(cityName))
            .ReturnsAsync(new List<City> { city });

        // Act
        var result = await _factory.CreateFromLegacyLocationAsync(cityName, provinceName,address);

        // Assert
        Assert.Equal(city.Id, result.CityId);
        Assert.Equal(province.Id, result.ProvinceId);
        Assert.Equal(city.Name, result.CityName);
        Assert.Equal(province.Name, result.ProvinceName);
    }

    [Fact]
    public async Task CreateFromLegacyLocationAsync_Throws_Exception_When_Province_Not_Found()
    {
        // Arrange
        string cityName = "Tehran";
        string provinceName = "Tehran Province";
        var address = "address";
        _mockProvinceRepository.Setup(r => r.FindByNameAsync(provinceName))
            .ReturnsAsync(new List<Province>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _factory.CreateFromLegacyLocationAsync(cityName, provinceName,address));
        Assert.Contains("Province", exception.Message);
    }

    [Fact]
    public async Task CreateFromLegacyLocationAsync_Throws_Exception_When_City_Not_Found()
    {
        // Arrange
        string cityName = "Tehran";
        string provinceName = "Tehran Province";
        
        var province = new Province(provinceName, "THR");
        var address = "address";        
        _mockProvinceRepository.Setup(r => r.FindByNameAsync(provinceName))
            .ReturnsAsync(new List<Province> { province });
        _mockCityRepository.Setup(r => r.FindByNameAsync(cityName))
            .ReturnsAsync(new List<City>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _factory.CreateFromLegacyLocationAsync(cityName, provinceName,address));
        Assert.Contains("City", exception.Message);
    }
}
