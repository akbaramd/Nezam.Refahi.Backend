using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTourCapacities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tours_RegistrationStart_RegistrationEnd",
                schema: "recreation",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "MaxParticipants",
                schema: "recreation",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "RegistrationEnd",
                schema: "recreation",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "RegistrationStart",
                schema: "recreation",
                table: "Tours");

            migrationBuilder.AddColumn<Guid>(
                name: "CapacityId",
                schema: "recreation",
                table: "TourReservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MemberId",
                schema: "recreation",
                table: "TourReservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TourCapacities",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    RegistrationStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourCapacities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourCapacities_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tours_TourStart_TourEnd",
                schema: "recreation",
                table: "Tours",
                columns: new[] { "TourStart", "TourEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_CapacityId",
                schema: "recreation",
                table: "TourReservations",
                column: "CapacityId");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_MemberId",
                schema: "recreation",
                table: "TourReservations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacities_IsActive",
                schema: "recreation",
                table: "TourCapacities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacities_RegistrationStart_RegistrationEnd",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "RegistrationStart", "RegistrationEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacities_TourId",
                schema: "recreation",
                table: "TourCapacities",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacities_TourId_IsActive",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "TourId", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_TourReservations_TourCapacities_CapacityId",
                schema: "recreation",
                table: "TourReservations",
                column: "CapacityId",
                principalSchema: "recreation",
                principalTable: "TourCapacities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourReservations_TourCapacities_CapacityId",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropTable(
                name: "TourCapacities",
                schema: "recreation");

            migrationBuilder.DropIndex(
                name: "IX_Tours_TourStart_TourEnd",
                schema: "recreation",
                table: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_CapacityId",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_MemberId",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropColumn(
                name: "CapacityId",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropColumn(
                name: "MemberId",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                schema: "recreation",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationEnd",
                schema: "recreation",
                table: "Tours",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationStart",
                schema: "recreation",
                table: "Tours",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Tours_RegistrationStart_RegistrationEnd",
                schema: "recreation",
                table: "Tours",
                columns: new[] { "RegistrationStart", "RegistrationEnd" });
        }
    }
}
