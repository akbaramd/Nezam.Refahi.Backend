using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTourCapacitie2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRegisteredCount",
                schema: "recreation",
                table: "Tours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentRegisteredCount",
                schema: "recreation",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
