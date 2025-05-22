using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_NationalIdValue",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NationalIdValue",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "RefundFeeValue",
                table: "PaymentTransactions",
                newName: "RefundFee");

            migrationBuilder.RenameColumn(
                name: "OriginalAmountValue",
                table: "PaymentTransactions",
                newName: "OriginalAmount");

            migrationBuilder.RenameColumn(
                name: "CapturedAmountValue",
                table: "PaymentTransactions",
                newName: "CapturedAmount");

            migrationBuilder.RenameColumn(
                name: "AuthorizedAmountValue",
                table: "PaymentTransactions",
                newName: "AuthorizedAmount");

            migrationBuilder.RenameColumn(
                name: "AmountValue",
                table: "PaymentTransactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PricePerNightAmount",
                table: "Hotels",
                newName: "PricePerNight");

            migrationBuilder.RenameColumn(
                name: "NationalId",
                table: "Guest",
                newName: "NationalIdValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefundFee",
                table: "PaymentTransactions",
                newName: "RefundFeeValue");

            migrationBuilder.RenameColumn(
                name: "OriginalAmount",
                table: "PaymentTransactions",
                newName: "OriginalAmountValue");

            migrationBuilder.RenameColumn(
                name: "CapturedAmount",
                table: "PaymentTransactions",
                newName: "CapturedAmountValue");

            migrationBuilder.RenameColumn(
                name: "AuthorizedAmount",
                table: "PaymentTransactions",
                newName: "AuthorizedAmountValue");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "PaymentTransactions",
                newName: "AmountValue");

            migrationBuilder.RenameColumn(
                name: "PricePerNight",
                table: "Hotels",
                newName: "PricePerNightAmount");

            migrationBuilder.RenameColumn(
                name: "NationalIdValue",
                table: "Guest",
                newName: "NationalId");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdValue",
                table: "Users",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NationalIdValue",
                table: "Users",
                column: "NationalIdValue");
        }
    }
}
