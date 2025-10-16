using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Facilities.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "facilities");

            migrationBuilder.CreateTable(
                name: "Facilities",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DefaultAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "IRR"),
                    MinAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "IRR"),
                    MaxAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "IRR"),
                    CooldownDays = table.Column<int>(type: "int", nullable: true),
                    IsRepeatable = table.Column<bool>(type: "bit", nullable: false),
                    IsExclusive = table.Column<bool>(type: "bit", nullable: false),
                    ExclusiveSetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxActiveAcrossCycles = table.Column<int>(type: "int", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityCapabilityPolicies",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PolicyType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModifierValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FacilityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityCapabilityPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityCapabilityPolicies_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityCapabilityPolicies_Facilities_FacilityId1",
                        column: x => x.FacilityId1,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityCycles",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quota = table.Column<int>(type: "int", nullable: false),
                    UsedQuota = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OverrideMinAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OverrideMinCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "IRR"),
                    OverrideMaxAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OverrideMaxCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "IRR"),
                    OverrideCooldownDays = table.Column<int>(type: "int", nullable: true),
                    OverrideIsRepeatable = table.Column<bool>(type: "bit", nullable: true),
                    OverrideExclusiveSetId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverrideMaxActiveAcrossCycles = table.Column<int>(type: "int", nullable: true),
                    AdmissionStrategy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "FIFO"),
                    WaitlistCapacity = table.Column<int>(type: "int", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityCycles_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacilityFeatures",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequirementType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FacilityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityFeatures_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityFeatures_Facilities_FacilityId1",
                        column: x => x.FacilityId1,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityRequests",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserNationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RequestedAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequestedCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "IRR"),
                    ApprovedAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ApprovedCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true, defaultValue: "IRR"),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchedToBankAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedByBankAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankAppointmentScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankAppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankAppointmentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisbursedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisbursementReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PolicySnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityRequests_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityRequests_FacilityCycles_FacilityCycleId",
                        column: x => x.FacilityCycleId,
                        principalSchema: "facilities",
                        principalTable: "FacilityCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_Code",
                schema: "facilities",
                table: "Facilities",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_Name",
                schema: "facilities",
                table: "Facilities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_Status",
                schema: "facilities",
                table: "Facilities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_Type",
                schema: "facilities",
                table: "Facilities",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapabilityPolicies_CapabilityId",
                schema: "facilities",
                table: "FacilityCapabilityPolicies",
                column: "CapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapabilityPolicies_FacilityId",
                schema: "facilities",
                table: "FacilityCapabilityPolicies",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapabilityPolicies_FacilityId_CapabilityId",
                schema: "facilities",
                table: "FacilityCapabilityPolicies",
                columns: new[] { "FacilityId", "CapabilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapabilityPolicies_FacilityId1",
                schema: "facilities",
                table: "FacilityCapabilityPolicies",
                column: "FacilityId1");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapabilityPolicies_PolicyType",
                schema: "facilities",
                table: "FacilityCapabilityPolicies",
                column: "PolicyType");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycles_EndDate",
                schema: "facilities",
                table: "FacilityCycles",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycles_FacilityId",
                schema: "facilities",
                table: "FacilityCycles",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycles_FacilityId_StartDate_EndDate",
                schema: "facilities",
                table: "FacilityCycles",
                columns: new[] { "FacilityId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycles_Name",
                schema: "facilities",
                table: "FacilityCycles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycles_StartDate",
                schema: "facilities",
                table: "FacilityCycles",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycles_Status",
                schema: "facilities",
                table: "FacilityCycles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityFeatures_FacilityId",
                schema: "facilities",
                table: "FacilityFeatures",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityFeatures_FacilityId_FeatureId",
                schema: "facilities",
                table: "FacilityFeatures",
                columns: new[] { "FacilityId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityFeatures_FacilityId1",
                schema: "facilities",
                table: "FacilityFeatures",
                column: "FacilityId1");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityFeatures_FeatureId",
                schema: "facilities",
                table: "FacilityFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityFeatures_RequirementType",
                schema: "facilities",
                table: "FacilityFeatures",
                column: "RequirementType");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_ApprovedAt",
                schema: "facilities",
                table: "FacilityRequests",
                column: "ApprovedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_BankAppointmentDate",
                schema: "facilities",
                table: "FacilityRequests",
                column: "BankAppointmentDate");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_CorrelationId",
                schema: "facilities",
                table: "FacilityRequests",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_CreatedAt",
                schema: "facilities",
                table: "FacilityRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_ExternalUserId",
                schema: "facilities",
                table: "FacilityRequests",
                column: "ExternalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_FacilityCycleId",
                schema: "facilities",
                table: "FacilityRequests",
                column: "FacilityCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_FacilityId",
                schema: "facilities",
                table: "FacilityRequests",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_IdempotencyKey",
                schema: "facilities",
                table: "FacilityRequests",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_RequestNumber",
                schema: "facilities",
                table: "FacilityRequests",
                column: "RequestNumber",
                unique: true,
                filter: "[RequestNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityRequests_Status",
                schema: "facilities",
                table: "FacilityRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityCapabilityPolicies",
                schema: "facilities");

            migrationBuilder.DropTable(
                name: "FacilityFeatures",
                schema: "facilities");

            migrationBuilder.DropTable(
                name: "FacilityRequests",
                schema: "facilities");

            migrationBuilder.DropTable(
                name: "FacilityCycles",
                schema: "facilities");

            migrationBuilder.DropTable(
                name: "Facilities",
                schema: "facilities");
        }
    }
}
