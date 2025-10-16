using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTHeTourAgency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TourAgencies",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AgencyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AgencyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AccessLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaxReservations = table.Column<int>(type: "int", nullable: true),
                    MaxParticipants = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourAgencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourAgencies_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourAgencies_AgencyCode",
                schema: "recreation",
                table: "TourAgencies",
                column: "AgencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_TourAgencies_IsActive",
                schema: "recreation",
                table: "TourAgencies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TourAgencies_AgencyId",
                schema: "recreation",
                table: "TourAgencies",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TourAgencies_TourId",
                schema: "recreation",
                table: "TourAgencies",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourAgencies_TourId_AgencyId",
                schema: "recreation",
                table: "TourAgencies",
                columns: new[] { "TourId", "AgencyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourAgencies_ValidFrom",
                schema: "recreation",
                table: "TourAgencies",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_TourAgencies_ValidTo",
                schema: "recreation",
                table: "TourAgencies",
                column: "ValidTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourAgencies",
                schema: "recreation");
        }
    }
}
