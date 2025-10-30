using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDificulityToTour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                schema: "recreation",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                schema: "recreation",
                table: "Tours");
        }
    }
}
