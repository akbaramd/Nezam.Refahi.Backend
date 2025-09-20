using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBiilsIdToReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BillId",
                schema: "recreation",
                table: "TourReservations",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillId",
                schema: "recreation",
                table: "TourReservations");
        }
    }
}
