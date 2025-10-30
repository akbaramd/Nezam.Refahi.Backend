using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Facilities.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFacilityCapabilityPolicyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityCapabilityPolicies",
                schema: "facilities");

            migrationBuilder.CreateTable(
                name: "FacilityCapability",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacilityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityCapability",
                schema: "facilities");

            migrationBuilder.CreateTable(
                name: "FacilityCapabilityPolicies",
                schema: "facilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifierValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PolicyType = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacilityCapabilityPolicies_Facilities_FacilityId1",
                        column: x => x.FacilityId1,
                        principalSchema: "facilities",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
        }
    }
}
