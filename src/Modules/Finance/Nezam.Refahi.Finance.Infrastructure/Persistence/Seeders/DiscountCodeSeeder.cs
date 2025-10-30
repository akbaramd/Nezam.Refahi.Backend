using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder for discount codes - creates sample discount codes for testing and development
/// </summary>
public static class DiscountCodeSeeder
{
    /// <summary>
    /// Seeds discount codes into the database
    /// </summary>
    /// <param name="context">Database context</param>
    public static async Task SeedAsync(DbContext context)
    {
        var discountCodesDbSet = context.Set<DiscountCode>();
        
        // Check if discount codes already exist
        if (await discountCodesDbSet.AnyAsync())
        {
            return; // Already seeded
        }

        // Set dates for Friday, October 17, 2025 (today) to Friday, October 24, 2025 (one week)
        var today = new DateTime(2025, 10, 17); // Friday, October 17, 2025
        var validFrom = today;
        var validTo = today.AddDays(7); // Valid for one week until Friday, October 24, 2025

        var discountCodes = new List<DiscountCode>
        {
            // 100% discount with maximum 50,000 Rials - Single Use
            new DiscountCode(
                code: "FULL100",
                title: "تخفیف کامل 100%",
                type: DiscountType.Percentage,
                discountValue: 100,
                validFrom: validFrom,
                validTo: validTo,
                description: "تخفیف کامل 100% - حداکثر 50,000 ریال - یکبار مصرف",
                maximumDiscountAmount: Money.FromRials(50000),
                minimumBillAmount: Money.FromRials(10000),
                usageLimit: 1,
                isSingleUse: true,
                createdByExternalUserId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                createdByUserFullName: "سیستم",
                metadata: new Dictionary<string, string>
                {
                    ["Category"] = "FullDiscount",
                    ["Priority"] = "High",
                    ["TargetAudience"] = "AllUsers",
                    ["SingleUse"] = "True",
                    ["MaxDiscount"] = "50000"
                }
            ),

            // 10% discount with maximum 50,000 Rials - Multi Use
            new DiscountCode(
                code: "PERCENT10",
                title: "تخفیف 10 درصدی",
                type: DiscountType.Percentage,
                discountValue: 10,
                validFrom: validFrom,
                validTo: validTo,
                description: "تخفیف 10 درصدی - حداکثر 50,000 ریال - چندبار مصرف",
                maximumDiscountAmount: Money.FromRials(50000),
                minimumBillAmount: Money.FromRials(10000),
                usageLimit: 100,
                isSingleUse: false,
                createdByExternalUserId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                createdByUserFullName: "سیستم",
                metadata: new Dictionary<string, string>
                {
                    ["Category"] = "Percentage",
                    ["Priority"] = "Medium",
                    ["TargetAudience"] = "AllUsers",
                    ["MaxDiscount"] = "50000"
                }
            ),

            // Fixed amount discount for 50,000 Rials with minimum bill 100,000 - Multi Use
            new DiscountCode(
                code: "FIXED50K",
                title: "تخفیف ثابت 50,000 ریال",
                type: DiscountType.FixedAmount,
                discountValue: 50000,
                validFrom: validFrom,
                validTo: validTo,
                description: "تخفیف ثابت 50,000 ریال - حداقل فاکتور 100,000 ریال - چندبار مصرف",
                maximumDiscountAmount: null,
                minimumBillAmount: Money.FromRials(100000),
                usageLimit: 50,
                isSingleUse: false,
                createdByExternalUserId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                createdByUserFullName: "سیستم",
                metadata: new Dictionary<string, string>
                {
                    ["Category"] = "FixedAmount",
                    ["Priority"] = "High",
                    ["TargetAudience"] = "HighValueUsers",
                    ["MinBillAmount"] = "100000"
                }
            ),

            // Additional single-use discount codes
            new DiscountCode(
                code: "FIRSTTIME",
                title: "تخفیف اولین خرید",
                type: DiscountType.Percentage,
                discountValue: 25,
                validFrom: validFrom,
                validTo: validTo,
                description: "تخفیف 25 درصدی برای اولین خرید - یکبار مصرف",
                maximumDiscountAmount: Money.FromRials(25000),
                minimumBillAmount: Money.FromRials(20000),
                usageLimit: 1,
                isSingleUse: true,
                createdByExternalUserId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                createdByUserFullName: "سیستم",
                metadata: new Dictionary<string, string>
                {
                    ["Category"] = "FirstTime",
                    ["Priority"] = "High",
                    ["TargetAudience"] = "NewUsers",
                    ["SingleUse"] = "True"
                }
            ),

            // Single-use fixed amount discount
            new DiscountCode(
                code: "ONETIME30K",
                title: "تخفیف یکباره 30,000 ریال",
                type: DiscountType.FixedAmount,
                discountValue: 30000,
                validFrom: validFrom,
                validTo: validTo,
                description: "تخفیف ثابت 30,000 ریال - یکبار مصرف",
                maximumDiscountAmount: null,
                minimumBillAmount: Money.FromRials(50000),
                usageLimit: 1,
                isSingleUse: true,
                createdByExternalUserId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                createdByUserFullName: "سیستم",
                metadata: new Dictionary<string, string>
                {
                    ["Category"] = "OneTime",
                    ["Priority"] = "Medium",
                    ["TargetAudience"] = "AllUsers",
                    ["SingleUse"] = "True"
                }
            ),

            // Multi-use percentage discount
            new DiscountCode(
                code: "WEEKEND20",
                title: "تخفیف آخر هفته",
                type: DiscountType.Percentage,
                discountValue: 20,
                validFrom: validFrom,
                validTo: validTo,
                description: "تخفیف ویژه آخر هفته - 20 درصد - چندبار مصرف",
                maximumDiscountAmount: Money.FromRials(30000),
                minimumBillAmount: Money.FromRials(30000),
                usageLimit: 150,
                isSingleUse: false,
                createdByExternalUserId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                createdByUserFullName: "سیستم",
                metadata: new Dictionary<string, string>
                {
                    ["Category"] = "Weekend",
                    ["Priority"] = "Medium",
                    ["TargetAudience"] = "AllUsers",
                    ["ValidDays"] = "Friday,Saturday,Sunday"
                }
            ),

            // Multi-use fixed amount discount
            new DiscountCode(
                code: "REGULAR20K",
                title: "تخفیف منظم 20,000 ریال",
                type: DiscountType.FixedAmount,
                discountValue: 20000,
                validFrom: validFrom,
                validTo: validTo,
                description: "تخفیف ثابت 20,000 ریال - چندبار مصرف",
                maximumDiscountAmount: null,
                minimumBillAmount: Money.FromRials(40000),
                usageLimit: 200,
                isSingleUse: false,
                createdByExternalUserId: Guid.Parse("00000000-0000-0000-0000-000000000001"),
                createdByUserFullName: "سیستم",
                metadata: new Dictionary<string, string>
                {
                    ["Category"] = "Regular",
                    ["Priority"] = "Medium",
                    ["TargetAudience"] = "AllUsers"
                }
            )
        };

        await discountCodesDbSet.AddRangeAsync(discountCodes);
        await context.SaveChangesAsync();
    }
}
