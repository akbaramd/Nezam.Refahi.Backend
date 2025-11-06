using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Orchestrator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Orchestrator_AddSagaScheduleTokensAndAmountRials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountMinor",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountRials",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "decimal(18,0)",
                precision: 18,
                scale: 0,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountRials",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.AddColumn<long>(
                name: "AmountMinor",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
