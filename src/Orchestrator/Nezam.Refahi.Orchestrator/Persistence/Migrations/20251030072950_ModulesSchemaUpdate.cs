using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Orchestrator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModulesSchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "modules");

            migrationBuilder.RenameTable(
                name: "ReservationPaymentSagas",
                newName: "ReservationPaymentSagas",
                newSchema: "modules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ReservationPaymentSagas",
                schema: "modules",
                newName: "ReservationPaymentSagas");
        }
    }
}
