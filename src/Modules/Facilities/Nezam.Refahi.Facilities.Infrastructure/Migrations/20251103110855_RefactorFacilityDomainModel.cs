using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Facilities.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorFacilityDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityCapability",
                schema: "facilities");

            migrationBuilder.DropTable(
                name: "FacilityFeatures",
                schema: "facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_Status",
                schema: "facilities",
                table: "Facilities");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_Type",
                schema: "facilities",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "AdmissionStrategy",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "CooldownDays",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "DefaultAmountRials",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "ExclusiveSetId",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "IsRepeatable",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "MaxActiveAcrossCycles",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "MaxAmountRials",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "MaxCurrency",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "MinAmountRials",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "MinCurrency",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "WaitlistCapacity",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.DropColumn(
                name: "Metadata",
                schema: "facilities",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "facilities",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "facilities",
                table: "Facilities");

            migrationBuilder.RenameColumn(
                name: "IsExclusive",
                schema: "facilities",
                table: "FacilityCycles",
                newName: "RestrictToPreviousCycles");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMonths",
                schema: "facilities",
                table: "FacilityCycles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 12);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalMessage",
                schema: "facilities",
                table: "FacilityCycles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FacilityCycleCapabilities",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityCycleCapabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityCycleCapabilities_FacilityCycles_FacilityCycleId",
                        column: x => x.FacilityCycleId,
                        principalSchema: "facilities",
                        principalTable: "FacilityCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityCycleFeatures",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityCycleFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityCycleFeatures_FacilityCycles_FacilityCycleId",
                        column: x => x.FacilityCycleId,
                        principalSchema: "facilities",
                        principalTable: "FacilityCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityCyclePriceOptions",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "IRR"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityCyclePriceOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityCyclePriceOptions_FacilityCycles_FacilityCycleId",
                        column: x => x.FacilityCycleId,
                        principalSchema: "facilities",
                        principalTable: "FacilityCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycleCapabilities_CapabilityId",
                schema: "facilities",
                table: "FacilityCycleCapabilities",
                column: "CapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycleCapabilities_FacilityCycleId",
                schema: "facilities",
                table: "FacilityCycleCapabilities",
                column: "FacilityCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycleCapabilities_FacilityCycleId_CapabilityId",
                schema: "facilities",
                table: "FacilityCycleCapabilities",
                columns: new[] { "FacilityCycleId", "CapabilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycleFeatures_FacilityCycleId",
                schema: "facilities",
                table: "FacilityCycleFeatures",
                column: "FacilityCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycleFeatures_FacilityCycleId_FeatureId",
                schema: "facilities",
                table: "FacilityCycleFeatures",
                columns: new[] { "FacilityCycleId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCycleFeatures_FeatureId",
                schema: "facilities",
                table: "FacilityCycleFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCyclePriceOptions_FacilityCycleId",
                schema: "facilities",
                table: "FacilityCyclePriceOptions",
                column: "FacilityCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCyclePriceOptions_FacilityCycleId_DisplayOrder",
                schema: "facilities",
                table: "FacilityCyclePriceOptions",
                columns: new[] { "FacilityCycleId", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityCycleCapabilities",
                schema: "facilities");

            migrationBuilder.DropTable(
                name: "FacilityCycleFeatures",
                schema: "facilities");

            migrationBuilder.DropTable(
                name: "FacilityCyclePriceOptions",
                schema: "facilities");

            migrationBuilder.DropColumn(
                name: "ApprovalMessage",
                schema: "facilities",
                table: "FacilityCycles");

            migrationBuilder.RenameColumn(
                name: "RestrictToPreviousCycles",
                schema: "facilities",
                table: "FacilityCycles",
                newName: "IsExclusive");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMonths",
                schema: "facilities",
                table: "FacilityCycles",
                type: "int",
                nullable: false,
                defaultValue: 12,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdmissionStrategy",
                schema: "facilities",
                table: "FacilityCycles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "FIFO");

            migrationBuilder.AddColumn<int>(
                name: "CooldownDays",
                schema: "facilities",
                table: "FacilityCycles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultAmountRials",
                schema: "facilities",
                table: "FacilityCycles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                schema: "facilities",
                table: "FacilityCycles",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<string>(
                name: "ExclusiveSetId",
                schema: "facilities",
                table: "FacilityCycles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeatable",
                schema: "facilities",
                table: "FacilityCycles",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxActiveAcrossCycles",
                schema: "facilities",
                table: "FacilityCycles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAmountRials",
                schema: "facilities",
                table: "FacilityCycles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaxCurrency",
                schema: "facilities",
                table: "FacilityCycles",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<decimal>(
                name: "MinAmountRials",
                schema: "facilities",
                table: "FacilityCycles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinCurrency",
                schema: "facilities",
                table: "FacilityCycles",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                defaultValue: "IRR");

            migrationBuilder.AddColumn<int>(
                name: "WaitlistCapacity",
                schema: "facilities",
                table: "FacilityCycles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "facilities",
                table: "Facilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "facilities",
                table: "Facilities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "facilities",
                table: "Facilities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FacilityCapability",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityCapability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityCapability_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FacilityCapability_Facilities_FacilityId1",
                        column: x => x.FacilityId1,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityFeatures",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequirementType = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacilityFeatures_Facilities_FacilityId1",
                        column: x => x.FacilityId1,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_FacilityCapability_CapabilityId",
                schema: "facilities",
                table: "FacilityCapability",
                column: "CapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapability_FacilityId",
                schema: "facilities",
                table: "FacilityCapability",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapability_FacilityId_CapabilityId",
                schema: "facilities",
                table: "FacilityCapability",
                columns: new[] { "FacilityId", "CapabilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityCapability_FacilityId1",
                schema: "facilities",
                table: "FacilityCapability",
                column: "FacilityId1");

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
        }
    }
}
