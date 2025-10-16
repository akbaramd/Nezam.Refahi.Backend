using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTheNewChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CapabilityName",
                schema: "membership",
                table: "MemberCapabilities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MemberAgencies",
                schema: "membership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OfficeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OfficeTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AccessLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAgencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberAgencies_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "membership",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_AccessLevel",
                schema: "membership",
                table: "MemberAgencies",
                column: "AccessLevel");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_AssignedAt",
                schema: "membership",
                table: "MemberAgencies",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_IsActive",
                schema: "membership",
                table: "MemberAgencies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_Member_Office_Active",
                schema: "membership",
                table: "MemberAgencies",
                columns: new[] { "MemberId", "AgencyId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_MemberId",
                schema: "membership",
                table: "MemberAgencies",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_AgencyId",
                schema: "membership",
                table: "MemberAgencies",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberAgencies_ValidityPeriod",
                schema: "membership",
                table: "MemberAgencies",
                columns: new[] { "ValidFrom", "ValidTo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberAgencies",
                schema: "membership");

            migrationBuilder.DropColumn(
                name: "CapabilityName",
                schema: "membership",
                table: "MemberCapabilities");
        }
    }
}
