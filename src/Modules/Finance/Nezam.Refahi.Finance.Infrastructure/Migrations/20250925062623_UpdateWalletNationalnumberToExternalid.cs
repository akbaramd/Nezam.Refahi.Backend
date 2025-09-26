using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWalletNationalnumberToExternalid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletSnapshots_UserNationalNumber",
                schema: "Finance",
                table: "WalletSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_WalletSnapshots_UserNationalNumber_SnapshotDate",
                schema: "Finance",
                table: "WalletSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_NationalNumber",
                schema: "finance",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_NationalNumber_Status",
                schema: "finance",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_RequestedByNationalNumber",
                schema: "finance",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Bills_UserNationalNumber",
                schema: "finance",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "UserNationalNumber",
                schema: "Finance",
                table: "WalletSnapshots");

            migrationBuilder.DropColumn(
                name: "NationalNumber",
                schema: "finance",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "RequestedByNationalNumber",
                schema: "finance",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "UserNationalNumber",
                schema: "finance",
                table: "Bills");

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalUserId",
                schema: "Finance",
                table: "WalletSnapshots",
                type: "uniqueidentifier",
                maxLength: 20,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalUserId",
                schema: "finance",
                table: "Wallets",
                type: "uniqueidentifier",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RequestedByExternalUserId",
                schema: "finance",
                table: "Refunds",
                type: "uniqueidentifier",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalUserId",
                schema: "finance",
                table: "Bills",
                type: "uniqueidentifier",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_ExternalUserId",
                schema: "Finance",
                table: "WalletSnapshots",
                column: "ExternalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_ExternalUserId_SnapshotDate",
                schema: "Finance",
                table: "WalletSnapshots",
                columns: new[] { "ExternalUserId", "SnapshotDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_ExternalUserId",
                schema: "finance",
                table: "Wallets",
                column: "ExternalUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_ExternalUserId_Status",
                schema: "finance",
                table: "Wallets",
                columns: new[] { "ExternalUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RequestedByExternalUserId",
                schema: "finance",
                table: "Refunds",
                column: "RequestedByExternalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_ExternalUserId",
                schema: "finance",
                table: "Bills",
                column: "ExternalUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletSnapshots_ExternalUserId",
                schema: "Finance",
                table: "WalletSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_WalletSnapshots_ExternalUserId_SnapshotDate",
                schema: "Finance",
                table: "WalletSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_ExternalUserId",
                schema: "finance",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_ExternalUserId_Status",
                schema: "finance",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_RequestedByExternalUserId",
                schema: "finance",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Bills_ExternalUserId",
                schema: "finance",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "Finance",
                table: "WalletSnapshots");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "finance",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "RequestedByExternalUserId",
                schema: "finance",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "finance",
                table: "Bills");

            migrationBuilder.AddColumn<string>(
                name: "UserNationalNumber",
                schema: "Finance",
                table: "WalletSnapshots",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalNumber",
                schema: "finance",
                table: "Wallets",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestedByNationalNumber",
                schema: "finance",
                table: "Refunds",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserNationalNumber",
                schema: "finance",
                table: "Bills",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_UserNationalNumber",
                schema: "Finance",
                table: "WalletSnapshots",
                column: "UserNationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_UserNationalNumber_SnapshotDate",
                schema: "Finance",
                table: "WalletSnapshots",
                columns: new[] { "UserNationalNumber", "SnapshotDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_NationalNumber",
                schema: "finance",
                table: "Wallets",
                column: "NationalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_NationalNumber_Status",
                schema: "finance",
                table: "Wallets",
                columns: new[] { "NationalNumber", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_RequestedByNationalNumber",
                schema: "finance",
                table: "Refunds",
                column: "RequestedByNationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_UserNationalNumber",
                schema: "finance",
                table: "Bills",
                column: "UserNationalNumber");
        }
    }
}
