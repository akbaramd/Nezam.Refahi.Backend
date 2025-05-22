using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice_Currency",
                table: "Reservations",
                newName: "TotalPriceCurrency");

            migrationBuilder.RenameColumn(
                name: "TotalPrice_Amount",
                table: "Reservations",
                newName: "TotalPriceAmount");

            migrationBuilder.RenameColumn(
                name: "RefundFee_Value",
                table: "PaymentTransactions",
                newName: "RefundFeeValue");

            migrationBuilder.RenameColumn(
                name: "RefundFee_Currency",
                table: "PaymentTransactions",
                newName: "RefundFeeCurrency");

            migrationBuilder.RenameColumn(
                name: "OriginalAmount_Value",
                table: "PaymentTransactions",
                newName: "OriginalAmountValue");

            migrationBuilder.RenameColumn(
                name: "OriginalAmount_Currency",
                table: "PaymentTransactions",
                newName: "OriginalAmountCurrency");

            migrationBuilder.RenameColumn(
                name: "CapturedAmount_Value",
                table: "PaymentTransactions",
                newName: "CapturedAmountValue");

            migrationBuilder.RenameColumn(
                name: "CapturedAmount_Currency",
                table: "PaymentTransactions",
                newName: "CapturedAmountCurrency");

            migrationBuilder.RenameColumn(
                name: "AuthorizedAmount_Value",
                table: "PaymentTransactions",
                newName: "AuthorizedAmountValue");

            migrationBuilder.RenameColumn(
                name: "AuthorizedAmount_Currency",
                table: "PaymentTransactions",
                newName: "AuthorizedAmountCurrency");

            migrationBuilder.RenameColumn(
                name: "Amount_Value",
                table: "PaymentTransactions",
                newName: "AmountValue");

            migrationBuilder.RenameColumn(
                name: "Amount_Currency",
                table: "PaymentTransactions",
                newName: "AmountCurrency");

            migrationBuilder.RenameColumn(
                name: "PricePerNight_Currency",
                table: "Hotels",
                newName: "PricePerNightCurrency");

            migrationBuilder.RenameColumn(
                name: "PricePerNight_Amount",
                table: "Hotels",
                newName: "PricePerNightAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPriceCurrency",
                table: "Reservations",
                newName: "TotalPrice_Currency");

            migrationBuilder.RenameColumn(
                name: "TotalPriceAmount",
                table: "Reservations",
                newName: "TotalPrice_Amount");

            migrationBuilder.RenameColumn(
                name: "RefundFeeValue",
                table: "PaymentTransactions",
                newName: "RefundFee_Value");

            migrationBuilder.RenameColumn(
                name: "RefundFeeCurrency",
                table: "PaymentTransactions",
                newName: "RefundFee_Currency");

            migrationBuilder.RenameColumn(
                name: "OriginalAmountValue",
                table: "PaymentTransactions",
                newName: "OriginalAmount_Value");

            migrationBuilder.RenameColumn(
                name: "OriginalAmountCurrency",
                table: "PaymentTransactions",
                newName: "OriginalAmount_Currency");

            migrationBuilder.RenameColumn(
                name: "CapturedAmountValue",
                table: "PaymentTransactions",
                newName: "CapturedAmount_Value");

            migrationBuilder.RenameColumn(
                name: "CapturedAmountCurrency",
                table: "PaymentTransactions",
                newName: "CapturedAmount_Currency");

            migrationBuilder.RenameColumn(
                name: "AuthorizedAmountValue",
                table: "PaymentTransactions",
                newName: "AuthorizedAmount_Value");

            migrationBuilder.RenameColumn(
                name: "AuthorizedAmountCurrency",
                table: "PaymentTransactions",
                newName: "AuthorizedAmount_Currency");

            migrationBuilder.RenameColumn(
                name: "AmountValue",
                table: "PaymentTransactions",
                newName: "Amount_Value");

            migrationBuilder.RenameColumn(
                name: "AmountCurrency",
                table: "PaymentTransactions",
                newName: "Amount_Currency");

            migrationBuilder.RenameColumn(
                name: "PricePerNightCurrency",
                table: "Hotels",
                newName: "PricePerNight_Currency");

            migrationBuilder.RenameColumn(
                name: "PricePerNightAmount",
                table: "Hotels",
                newName: "PricePerNight_Amount");
        }
    }
}
