using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWalletPreviousBalalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BalanceAfterRials",
                schema: "finance",
                table: "WalletTransactions",
                newName: "PreviousBalanceRials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreviousBalanceRials",
                schema: "finance",
                table: "WalletTransactions",
                newName: "BalanceAfterRials");
        }
    }
}
