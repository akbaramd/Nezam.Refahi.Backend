using System.Globalization;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Domain.Entities;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Infrastructure.Seeding;

/// <summary>
/// Seeder for Facilities module data
/// </summary>
public class FacilitiesSeeder
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IFacilityCycleRepository _facilityCycleRepository;
    private readonly IFacilitiesUnitOfWork _unitOfWork;
    private readonly ILogger<FacilitiesSeeder> _logger;

    public FacilitiesSeeder(
        IFacilityRepository facilityRepository,
        IFacilityCycleRepository facilityCycleRepository,
        IFacilitiesUnitOfWork unitOfWork,
        ILogger<FacilitiesSeeder> logger)
    {
        _facilityRepository = facilityRepository;
        _facilityCycleRepository = facilityCycleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting Facilities data seeding...");

            // Check if data already exists
            var existingFacilitiesCount = await _facilityRepository.CountAsync();
            if (existingFacilitiesCount > 0)
            {
                _logger.LogInformation("Facilities data already exists. Skipping seeding.");
                return;
            }

            // Begin transaction
            await _unitOfWork.BeginAsync();

            try
            {
                // Create facilities
                var facilities = CreateFacilities();
                
                // Add facilities to repository
                foreach (var facility in facilities)
                {
                    await _facilityRepository.AddAsync(facility);
                    _logger.LogInformation("Added facility: {FacilityName} ({FacilityCode})", facility.Name, facility.Code);
                }

                // Create cycles for facilities
                var cycles = CreateFacilityCycles(facilities);
                
                // Add cycles to repository
                foreach (var cycle in cycles)
                {
                    await _facilityCycleRepository.AddAsync(cycle);
                    
                    // Explicitly access PriceOptions collection to ensure EF Core tracks them
                    // This is necessary because EF Core needs to discover the related entities
                    var priceOptionsCount = cycle.PriceOptions.Count;
                    _logger.LogInformation(
                        "Added cycle: {CycleName} for facility {FacilityCode} with {PriceOptionCount} price options", 
                        cycle.Name, cycle.FacilityId, priceOptionsCount);
                }

                // Commit transaction
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Facilities data seeding completed successfully. Created {FacilityCount} facilities and {CycleCount} cycles.", 
                    facilities.Count, cycles.Count);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Facilities data seeding");
            throw;
        }
    }

    private List<Facility> CreateFacilities()
    {
        var facilities = new List<Facility>();

        // Only Tejarat Facility - Trade Facilities
        var tejaratFacility = new Facility(
            name: "تسهیلات تجارت",
            code: "TEJARAT-001",
            description: "تسهیلات تجارت برای راه‌اندازی کسب و کار با مبلغ ۱۰۰ میلیون تومان و بازپرداخت ۳۰ ماهه",
            bankName: "بانک تجارت",
            bankCode: "TEJARAT",
            bankAccountNumber: "0987654321"
        );

        facilities.Add(tejaratFacility);

        return facilities;
    }

    private List<FacilityCycle> CreateFacilityCycles(List<Facility> facilities)
    {
        var cycles = new List<FacilityCycle>();
        
        // Use PersianCalendar to calculate dates for year 1404
        var persianCalendar = new PersianCalendar();
        const int persianYear = 1404;
        const int abanMonth = 8;  // آبان
        const int azarMonth = 9;  // آذر
        
        // Calculate dates: 1 Aban 1404 and 30 Aban 1404
        var cycle1StartDate = persianCalendar.ToDateTime(persianYear, abanMonth, 1, 0, 0, 0, 0); // 1 Aban 1404
        var cycle1EndDate = persianCalendar.ToDateTime(persianYear, abanMonth, 30, 0, 0, 0, 0);   // 30 Aban 1404
        
        // Calculate dates: 1 Azar 1404 and 30 Azar 1404
        var cycle2StartDate = persianCalendar.ToDateTime(persianYear, azarMonth, 1, 0, 0, 0, 0); // 1 Azar 1404
        var cycle2EndDate = persianCalendar.ToDateTime(persianYear, azarMonth, 30, 0, 0, 0, 0);   // 30 Azar 1404

        // Common capability IDs - these should match actual capabilities in the system
        var commonCapabilities = new List<string>
        {
            "structure_supervisor_grade1",
            "architecture_supervisor_grade1",
            "has_license"
        };

        foreach (var facility in facilities)
        {
            // Create first cycle: 1 Aban to 30 Aban - Status: Active (در حال ثبت نام)
            var cycle1 = new FacilityCycle(
                facilityId: facility.Id,
                name: $"دوره اول {facility.Name}",
                startDate: cycle1StartDate,
                endDate: cycle1EndDate,
                quota: 100,
                restrictToPreviousCycles: false,
                description: $"دوره اول ارائه {facility.Name} - 1 آبان تا 30 آبان 1404",
                paymentMonths: 30,
                interestRate: 0.23m // 23% annual interest
            );

            // Add price options: 1,000,000,000 and 500,000,000
            cycle1.AddPriceOption(new Money(1_000_000_000), displayOrder: 1, description: "یک میلیارد تومان");
            cycle1.AddPriceOption(new Money(500_000_000), displayOrder: 2, description: "پانصد میلیون تومان");

            // Add capabilities to the cycle
            foreach (var capabilityId in commonCapabilities)
            {
                try
                {
                    cycle1.AddCapability(capabilityId);
                }
                catch (InvalidOperationException)
                {
                    // Capability already exists, skip
                    _logger.LogWarning("Capability {CapabilityId} already exists for cycle {CycleName}", capabilityId, cycle1.Name);
                }
            }

            // Activate the cycle (Status: Active - در حال ثبت نام)
            cycle1.Activate();

            // Add cycle-specific metadata
            cycle1.Metadata["cycleNumber"] = "1";
            cycle1.Metadata["priority"] = "high";
            cycle1.Metadata["announcementDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            cycle1.Metadata["statusText"] = "در حال ثبت نام";

            cycles.Add(cycle1);

            // Create second cycle: 1 Azar to 30 Azar - Status: Draft (منتظر ثبت نام)
            var cycle2 = new FacilityCycle(
                facilityId: facility.Id,
                name: $"دوره دوم {facility.Name}",
                startDate: cycle2StartDate,
                endDate: cycle2EndDate,
                quota: 100,
                restrictToPreviousCycles: false,
                description: $"دوره دوم ارائه {facility.Name} - 1 آذر تا 30 آذر 1404",
                paymentMonths: 30,
                interestRate: 0.23m // 23% annual interest
            );

            // Add price options: 1,000,000,000 and 500,000,000
            cycle2.AddPriceOption(new Money(1_000_000_000), displayOrder: 1, description: "یک میلیارد تومان");
            cycle2.AddPriceOption(new Money(500_000_000), displayOrder: 2, description: "پانصد میلیون تومان");

            // Add capabilities to the cycle
            foreach (var capabilityId in commonCapabilities)
            {
                try
                {
                    cycle2.AddCapability(capabilityId);
                }
                catch (InvalidOperationException)
                {
                    // Capability already exists, skip
                    _logger.LogWarning("Capability {CapabilityId} already exists for cycle {CycleName}", capabilityId, cycle2.Name);
                }
            }

            // Keep cycle2 in Draft status (منتظر ثبت نام)
            // Status is already Draft by default, no need to activate

            // Add cycle-specific metadata
            cycle2.Metadata["cycleNumber"] = "2";
            cycle2.Metadata["priority"] = "high";
            cycle2.Metadata["announcementDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            cycle2.Metadata["statusText"] = "منتظر ثبت نام";

            cycles.Add(cycle2);

            _logger.LogInformation("Created cycles for facility {FacilityCode}: {Cycle1Name} (Active - 1-30 Aban) and {Cycle2Name} (Draft - 1-30 Azar)", 
                facility.Code, cycle1.Name, cycle2.Name);
        }

        return cycles;
    }
}
