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
                    _logger.LogInformation("Added cycle: {CycleName} for facility {FacilityCode}", cycle.Name, cycle.FacilityId);
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
            type: FacilityType.Loan,
            description: "تسهیلات تجارت برای راه‌اندازی کسب و کار با مبلغ ۱۰۰ میلیون تومان و بازپرداخت ۳۰ ماهه",
            bankName: "بانک تجارت",
            bankCode: "TEJARAT",
            bankAccountNumber: "0987654321"
        );

        // Activate the facility
        tejaratFacility.Activate();

        // Add metadata
        tejaratFacility.Metadata["loanTermMonths"] = "30";
        tejaratFacility.Metadata["interestRate"] = "0.23"; // 23% annual interest
        tejaratFacility.Metadata["purpose"] = "کسب و کار";
        tejaratFacility.Metadata["targetGroup"] = "کارآفرینان";
        tejaratFacility.Metadata["businessPlanRequired"] = "true";
        tejaratFacility.Metadata["cooldownYears"] = "2"; // 2 years cooldown after receiving loan

        facilities.Add(tejaratFacility);

        return facilities;
    }

    private List<FacilityCycle> CreateFacilityCycles(List<Facility> facilities)
    {
        var cycles = new List<FacilityCycle>();
        
        // Persian dates: 1 Aban to 30 Aban (1403) = October 22, 2024 to November 20, 2024
        // Registration period: 1 month before cycle start
        var cycle1StartDate = new DateTime(2024, 10, 22); // 1 Aban 1403
        var cycle1EndDate = new DateTime(2024, 11, 20);   // 30 Aban 1403
        var cycle1RegistrationStart = cycle1StartDate.AddMonths(-1); // 1 month before
        var cycle1RegistrationEnd = cycle1StartDate.AddDays(-1); // Day before cycle starts

        foreach (var facility in facilities)
        {
            // Create first cycle for Trade Facilities
            var cycle1 = new FacilityCycle(
                facilityId: facility.Id,
                name: $"دوره اول {facility.Name}",
                startDate: cycle1StartDate,
                endDate: cycle1EndDate,
                quota: 100, // 100 applications per cycle
                minAmount: new Money(20_000_000),
                maxAmount: new Money(100_000_000),
                defaultAmount: new Money(100_000_000),
                paymentMonths: 30,
                interestRate: 0.23m, // 23% annual interest
                cooldownDays: 730, // 2 years = 730 days
                isRepeatable: true,
                isExclusive: false,
                exclusiveSetId: null,
                maxActiveAcrossCycles: 2,
                description: $"دوره اول ارائه {facility.Name} - ظرفیت ۱۰۰ درخواست - ثبت نام از {cycle1RegistrationStart:yyyy/MM/dd} تا {cycle1RegistrationEnd:yyyy/MM/dd}"
            );

            // Activate the cycle
            cycle1.Activate();

            // Add cycle-specific metadata
            cycle1.Metadata["cycleNumber"] = "1";
            cycle1.Metadata["priority"] = "high";
            cycle1.Metadata["announcementDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            cycle1.Metadata["admissionStrategy"] = "FIFO";
            cycle1.Metadata["waitlistCapacity"] = "50";
            cycle1.Metadata["registrationStartDate"] = cycle1RegistrationStart.ToString("yyyy-MM-dd");
            cycle1.Metadata["registrationEndDate"] = cycle1RegistrationEnd.ToString("yyyy-MM-dd");
            cycle1.Metadata["availableSlots"] = "100";
            cycle1.Metadata["registeredCount"] = "0";

            cycles.Add(cycle1);

            // Create second cycle for Trade Facilities (same period for both cycles as requested)
            var cycle2 = new FacilityCycle(
                facilityId: facility.Id,
                name: $"دوره دوم {facility.Name}",
                startDate: cycle1StartDate, // Same period for both cycles
                endDate: cycle1EndDate,
                quota: 100, // 100 applications per cycle
                minAmount: new Money(20_000_000),
                maxAmount: new Money(100_000_000),
                defaultAmount: new Money(100_000_000),
                paymentMonths: 30,
                interestRate: 0.23m, // 23% annual interest
                cooldownDays: 730, // 2 years = 730 days
                isRepeatable: true,
                isExclusive: false,
                exclusiveSetId: null,
                maxActiveAcrossCycles: 2,
                description: $"دوره دوم ارائه {facility.Name} - ظرفیت ۱۰۰ درخواست - ثبت نام از {cycle1RegistrationStart:yyyy/MM/dd} تا {cycle1RegistrationEnd:yyyy/MM/dd}"
            );

            // Activate the cycle
            cycle2.Activate();

            // Add cycle-specific metadata
            cycle2.Metadata["cycleNumber"] = "2";
            cycle2.Metadata["priority"] = "high";
            cycle2.Metadata["announcementDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
            cycle2.Metadata["admissionStrategy"] = "FIFO";
            cycle2.Metadata["waitlistCapacity"] = "50";
            cycle2.Metadata["registrationStartDate"] = cycle1RegistrationStart.ToString("yyyy-MM-dd");
            cycle2.Metadata["registrationEndDate"] = cycle1RegistrationEnd.ToString("yyyy-MM-dd");
            cycle2.Metadata["availableSlots"] = "100";
            cycle2.Metadata["registeredCount"] = "0";

            cycles.Add(cycle2);

            _logger.LogInformation("Created cycles for facility {FacilityCode}: {Cycle1Name} and {Cycle2Name} (Quota: 100 each)", 
                facility.Code, cycle1.Name, cycle2.Name);
        }

        return cycles;
    }
}
