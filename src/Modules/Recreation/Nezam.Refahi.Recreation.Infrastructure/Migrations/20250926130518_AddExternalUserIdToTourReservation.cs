using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalUserIdToTourReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExternalUserId",
                schema: "recreation",
                table: "TourReservations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TenantExternalUserDate",
                schema: "recreation",
                table: "TourReservations",
                columns: new[] { "TenantId", "ExternalUserId", "ReservationDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TourReservations_TenantExternalUserDate",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "recreation",
                table: "TourReservations");
        }
    }
}
