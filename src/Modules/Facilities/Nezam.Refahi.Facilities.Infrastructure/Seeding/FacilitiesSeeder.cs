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

        // Mehr Facility - 100 million Toman loan for 30 months
        var mehrFacility = new Facility(
            name: "تسهیلات مهر",
            code: "MEHR-001",
            type: FacilityType.Loan,
            description: "تسهیلات مهر برای خرید مسکن با مبلغ ۱۰۰ میلیون تومان و بازپرداخت ۳۰ ماهه",
            bankName: "بانک مهر",
            bankCode: "MEHR",
            bankAccountNumber: "1234567890"
        );

        // Activate the facility
        mehrFacility.Activate();

        // Add metadata
        mehrFacility.Metadata["loanTermMonths"] = "30";
        mehrFacility.Metadata["interestRate"] = "0.18"; // 18% annual interest
        mehrFacility.Metadata["purpose"] = "مسکن";
        mehrFacility.Metadata["targetGroup"] = "کارمندان دولت";

        facilities.Add(mehrFacility);

        // Tejarat Facility - 100 million Toman loan for 30 months
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
        tejaratFacility.Metadata["interestRate"] = "0.20"; // 20% annual interest
        tejaratFacility.Metadata["purpose"] = "کسب و کار";
        tejaratFacility.Metadata["targetGroup"] = "کارآفرینان";
        tejaratFacility.Metadata["businessPlanRequired"] = "true";

        facilities.Add(tejaratFacility);

        return facilities;
    }

    private List<FacilityCycle> CreateFacilityCycles(List<Facility> facilities)
    {
        var cycles = new List<FacilityCycle>();
        var currentDate = DateTime.UtcNow;

        foreach (var facility in facilities)
        {
            // Create first cycle for each facility
            var cycleStartDate = currentDate.AddDays(7); // Start in 1 week
            var cycleEndDate = cycleStartDate.AddMonths(3); // 3 months duration

            var cycle = new FacilityCycle(
                facilityId: facility.Id,
                name: $"دوره اول {facility.Name}",
                startDate: cycleStartDate,
                endDate: cycleEndDate,
                quota: 100, // 100 applications per cycle
                minAmount: facility.Code == "MEHR-001" ? new Money(50_000_000) : new Money(20_000_000),
                maxAmount: new Money(100_000_000),
                defaultAmount: new Money(100_000_000),
                paymentMonths: 30,
                interestRate: facility.Code == "MEHR-001" ? 0.18m : 0.20m,
                cooldownDays: facility.Code == "MEHR-001" ? 365 : 180,
                isRepeatable: facility.Code != "MEHR-001",
                isExclusive: facility.Code == "MEHR-001",
                exclusiveSetId: facility.Code == "MEHR-001" ? "MEHR_SET" : null,
                maxActiveAcrossCycles: facility.Code == "MEHR-001" ? 1 : 2,
                description: $"دوره اول ارائه {facility.Name} - ظرفیت ۱۰۰ درخواست"
            );

            // Activate the cycle
            cycle.Activate();

            // Add cycle-specific metadata
            cycle.Metadata["cycleNumber"] = "1";
            cycle.Metadata["priority"] = "high";
            cycle.Metadata["announcementDate"] = currentDate.ToString("yyyy-MM-dd");
            cycle.Metadata["admissionStrategy"] = "FIFO";
            cycle.Metadata["waitlistCapacity"] = "50";

            cycles.Add(cycle);

            _logger.LogInformation("Created cycle for facility {FacilityCode}: {CycleName} (Quota: {Quota})", 
                facility.Code, cycle.Name, cycle.Quota);
        }

        return cycles;
    }
}
