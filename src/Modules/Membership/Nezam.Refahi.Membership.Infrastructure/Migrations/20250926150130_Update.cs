using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberCapabilities_Capabilities_CapabilityId",
                schema: "membership",
                table: "MemberCapabilities");

            migrationBuilder.DropTable(
                name: "CapabilityFeatures",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Capabilities",
                schema: "membership");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "membership");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Capabilities",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capabilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapabilityFeatures",
                schema: "membership",
                columns: table => new
                {
                    CapabilityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapabilityFeatures", x => new { x.CapabilityId, x.ClaimTypeId });
                    table.ForeignKey(
                        name: "FK_CapabilityFeatures_Capabilities_CapabilityId",
                        column: x => x.CapabilityId,
                        principalSchema: "membership",
                        principalTable: "Capabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CapabilityFeatures_Features_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalSchema: "membership",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_IsActive",
                schema: "membership",
                table: "Capabilities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_Name",
                schema: "membership",
                table: "Capabilities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Capabilities_ValidityPeriod",
                schema: "membership",
                table: "Capabilities",
                columns: new[] { "ValidFrom", "ValidTo" });

            migrationBuilder.CreateIndex(
                name: "IX_CapabilityFeatures_ClaimTypeId",
                schema: "membership",
                table: "CapabilityFeatures",
                column: "ClaimTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberCapabilities_Capabilities_CapabilityId",
                schema: "membership",
                table: "MemberCapabilities",
                column: "CapabilityId",
                principalSchema: "membership",
                principalTable: "Capabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
