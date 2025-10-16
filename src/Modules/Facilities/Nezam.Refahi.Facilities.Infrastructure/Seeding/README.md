# Facilities Module Seeding

This directory contains the seeding functionality for the Facilities module, which automatically creates initial data when the application starts.

## Overview

The Facilities seeding system creates essential data for the application to function properly, including:

- **Facilities**: Core facility definitions (Mehr and Tejarat loans)
- **Facility Cycles**: Active periods for each facility with quotas and schedules

## Files

### `FacilitiesSeeder.cs`
The main seeder class that creates:
- **Mehr Facility**: 100 million Toman loan for 30 months (housing purpose)
- **Tejarat Facility**: 100 million Toman loan for 30 months (business purpose)
- **First Cycles**: Initial active cycles for both facilities

### `FacilitiesSeedingExtensions.cs`
Extension methods for seeding:
- `SeedFacilitiesDataAsync()`: Manual seeding method
- `SeedFacilitiesDataOnStartupAsync()`: Startup seeding method

### `FacilitiesSeedingHostedService.cs`
Background service that runs seeding automatically on application startup.

## Seeded Data

### Mehr Facility (تسهیلات مهر)
- **Type**: Loan
- **Amount**: 100 million Toman (50M-100M range)
- **Term**: 30 months
- **Interest**: 18% annual
- **Purpose**: Housing
- **Target**: Government employees
- **Cooldown**: 1 year
- **Repeatable**: No
- **Exclusive**: Yes

### Tejarat Facility (تسهیلات تجارت)
- **Type**: Loan
- **Amount**: 100 million Toman (20M-100M range)
- **Term**: 30 months
- **Interest**: 20% annual
- **Purpose**: Business
- **Target**: Entrepreneurs
- **Cooldown**: 6 months
- **Repeatable**: Yes
- **Exclusive**: No

### Facility Cycles
- **Duration**: 3 months each
- **Quota**: 100 applications per cycle
- **Waitlist**: 50 additional applications
- **Strategy**: FIFO (First In, First Out)
- **Start**: 1 week from seeding date

## Usage

### Automatic Seeding (Recommended)
The seeding runs automatically when the application starts via `FacilitiesSeedingHostedService`.

### Manual Seeding
```csharp
// In Program.cs or Startup.cs
await serviceProvider.SeedFacilitiesDataAsync();
```

### Startup Seeding
```csharp
// In Program.cs or Startup.cs
await serviceProvider.SeedFacilitiesDataOnStartupAsync();
```

## Configuration

The seeder checks if data already exists before creating new records, preventing duplicate data on subsequent runs.

## Dependencies

- `IFacilityRepository`: For facility management
- `IFacilityCycleRepository`: For cycle management
- `IFacilitiesUnitOfWork`: For transaction management
- `ILogger<FacilitiesSeeder>`: For logging

## Notes

- All amounts are in Toman (Iranian currency)
- Interest rates are annual percentages
- Dates are in UTC
- The seeder uses proper transaction management with rollback on errors
- Persian descriptions and metadata are included for localization
