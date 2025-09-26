using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWalletNationalnumberToExternalid2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletDeposits_UserNationalNumber",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.DropIndex(
                name: "IX_WalletDeposits_UserNationalNumber_RequestedAt",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.DropColumn(
                name: "UserNationalNumber",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalUserId",
                schema: "Finance",
                table: "WalletDeposits",
                type: "uniqueidentifier",
                maxLength: 20,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_ExternalUserId",
                schema: "Finance",
                table: "WalletDeposits",
                column: "ExternalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_ExternalUserId_RequestedAt",
                schema: "Finance",
                table: "WalletDeposits",
                columns: new[] { "ExternalUserId", "RequestedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletDeposits_ExternalUserId",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.DropIndex(
                name: "IX_WalletDeposits_ExternalUserId_RequestedAt",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.AddColumn<string>(
                name: "UserNationalNumber",
                schema: "Finance",
                table: "WalletDeposits",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_UserNationalNumber",
                schema: "Finance",
                table: "WalletDeposits",
                column: "UserNationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_UserNationalNumber_RequestedAt",
                schema: "Finance",
                table: "WalletDeposits",
                columns: new[] { "UserNationalNumber", "RequestedAt" });
        }
    }
}
