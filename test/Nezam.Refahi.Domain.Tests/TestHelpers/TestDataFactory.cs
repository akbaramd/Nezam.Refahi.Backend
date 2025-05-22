using System;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;

namespace Nezam.Refahi.Domain.Tests.TestHelpers;

/// <summary>
/// Factory class for creating test data objects
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Creates a Money value object with the specified amount and currency
    /// </summary>
    public static Money CreateMoney(decimal amount, string currency = "USD")
    {
        return new Money(amount, currency);
    }
    
    /// <summary>
    /// Creates a DateRange value object with the specified check-in and check-out dates
    /// </summary>
    public static DateRange CreateDateRange(DateTime checkIn, DateTime checkOut)
    {
        return new DateRange(DateOnly.FromDateTime(checkIn), DateOnly.FromDateTime(checkOut));
    }
    
    /// <summary>
    /// Creates a LocationReference value object with the specified IDs and names
    /// </summary>
    public static LocationReference CreateLocationReference(
        Guid? cityId = null, 
        Guid? provinceId = null, 
        string cityName = "Tehran", 
        string provinceName = "Tehran Province")
    {
        return new LocationReference(
            cityId ?? Guid.NewGuid(), 
            provinceId ?? Guid.NewGuid(), 
            cityName, 
            provinceName,"Iran - Urmia");
    }
    
    /// <summary>
    /// Creates a Hotel entity with default values
    /// </summary>
    public static Hotel CreateHotel(
        string name = "Grand Hotel",
        string description = "A luxury hotel in the heart of the city",
        int capacity = 100,
        decimal pricePerNight = 100.50m,
        string currency = "USD")
    {
        var locationReference = CreateLocationReference();
        var money = CreateMoney(pricePerNight, currency);
        
        return new Hotel(Guid.NewGuid(),name, description, locationReference, money, capacity);
    }
    
    /// <summary>
    /// Creates a Guest entity with default values
    /// </summary>
    public static Guest CreateGuest(
        string firstName = "Mohammad",
        string lastName = "Ahmadi",
        string nationalId = "2741153671",
        int age = 30)
    {
        return new Guest(firstName, lastName, nationalId, age);
    }
}
