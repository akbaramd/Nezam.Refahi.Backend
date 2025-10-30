using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UdpateThePricings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEarlyBird",
                schema: "recreation",
                table: "TourPricing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLastMinute",
                schema: "recreation",
                table: "TourPricing",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEarlyBird",
                schema: "recreation",
                table: "TourPricing");

            migrationBuilder.DropColumn(
                name: "IsLastMinute",
                schema: "recreation",
                table: "TourPricing");
        }
    }
}
