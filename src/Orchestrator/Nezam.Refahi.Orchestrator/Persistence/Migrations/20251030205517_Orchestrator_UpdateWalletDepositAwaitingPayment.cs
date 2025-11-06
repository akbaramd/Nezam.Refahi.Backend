using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Orchestrator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Orchestrator_UpdateWalletDepositAwaitingPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountRials",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.AddColumn<long>(
                name: "AmountMinor",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "BillCreationRetryCount",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "BillCreationTimeoutTokenId",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentTimeoutTokenId",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountMinor",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.DropColumn(
                name: "BillCreationRetryCount",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.DropColumn(
                name: "BillCreationTimeoutTokenId",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.DropColumn(
                name: "PaymentTimeoutTokenId",
                schema: "orchestrator",
                table: "WalletDepositSagas");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountRials",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                schema: "orchestrator",
                table: "WalletDepositSagas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
