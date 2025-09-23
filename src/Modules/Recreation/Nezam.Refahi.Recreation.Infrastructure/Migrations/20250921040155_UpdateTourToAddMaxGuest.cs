using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTourToAddMaxGuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxGuestsPerReservation",
                schema: "recreation",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                schema: "recreation",
                table: "Participants",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxGuestsPerReservation",
                schema: "recreation",
                table: "Tours");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                schema: "recreation",
                table: "Participants",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
