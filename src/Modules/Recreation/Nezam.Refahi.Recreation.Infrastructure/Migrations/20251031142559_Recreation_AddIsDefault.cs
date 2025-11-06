using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Recreation_AddIsDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                schema: "recreation",
                table: "TourPricing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "AppliedDiscountPercentage",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredCapabilityIds",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequiredFeatureIds",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TourPricingId",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WasDefaultPricing",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasEarlyBird",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasLastMinute",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TourPricingCapability",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourPricingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPricingCapability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPricingCapability_TourPricing_TourPricingId",
                        column: x => x.TourPricingId,
                        principalSchema: "recreation",
                        principalTable: "TourPricing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPricingFeature",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourPricingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPricingFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPricingFeature_TourPricing_TourPricingId",
                        column: x => x.TourPricingId,
                        principalSchema: "recreation",
                        principalTable: "TourPricing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourPricing_TourId_ParticipantType_IsDefault",
                schema: "recreation",
                table: "TourPricing",
                columns: new[] { "TourId", "ParticipantType", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationPriceSnapshots_TourPricingId",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                column: "TourPricingId",
                filter: "[TourPricingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricingCapability_CapabilityId",
                schema: "recreation",
                table: "TourPricingCapability",
                column: "CapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricingCapability_TourPricingId_CapabilityId",
                schema: "recreation",
                table: "TourPricingCapability",
                columns: new[] { "TourPricingId", "CapabilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourPricingFeature_FeatureId",
                schema: "recreation",
                table: "TourPricingFeature",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricingFeature_TourPricingId_FeatureId",
                schema: "recreation",
                table: "TourPricingFeature",
                columns: new[] { "TourPricingId", "FeatureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourPricingCapability",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "TourPricingFeature",
                schema: "recreation");

            migrationBuilder.DropIndex(
                name: "IX_TourPricing_TourId_ParticipantType_IsDefault",
                schema: "recreation",
                table: "TourPricing");

            migrationBuilder.DropIndex(
                name: "IX_ReservationPriceSnapshots_TourPricingId",
                schema: "recreation",
                table: "ReservationPriceSnapshots");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                schema: "recreation",
                table: "TourPricing");

            migrationBuilder.DropColumn(
                name: "AppliedDiscountPercentage",
                schema: "recreation",
                table: "ReservationPriceSnapshots");

            migrationBuilder.DropColumn(
                name: "RequiredCapabilityIds",
                schema: "recreation",
                table: "ReservationPriceSnapshots");

            migrationBuilder.DropColumn(
                name: "RequiredFeatureIds",
                schema: "recreation",
                table: "ReservationPriceSnapshots");

            migrationBuilder.DropColumn(
                name: "TourPricingId",
                schema: "recreation",
                table: "ReservationPriceSnapshots");

            migrationBuilder.DropColumn(
                name: "WasDefaultPricing",
                schema: "recreation",
                table: "ReservationPriceSnapshots");

            migrationBuilder.DropColumn(
                name: "WasEarlyBird",
                schema: "recreation",
                table: "ReservationPriceSnapshots");

            migrationBuilder.DropColumn(
                name: "WasLastMinute",
                schema: "recreation",
                table: "ReservationPriceSnapshots");
        }
    }
}
