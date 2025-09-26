using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBiilIdAndDepositAddTrackingcodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletDeposits_Bills_BillId",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.DropIndex(
                name: "IX_WalletDeposits_BillId",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.DropColumn(
                name: "BillId",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.AddColumn<string>(
                name: "TrackingCode",
                schema: "Finance",
                table: "WalletDeposits",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_TrackingCode",
                schema: "Finance",
                table: "WalletDeposits",
                column: "TrackingCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletDeposits_TrackingCode",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.DropColumn(
                name: "TrackingCode",
                schema: "Finance",
                table: "WalletDeposits");

            migrationBuilder.AddColumn<Guid>(
                name: "BillId",
                schema: "Finance",
                table: "WalletDeposits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_BillId",
                schema: "Finance",
                table: "WalletDeposits",
                column: "BillId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletDeposits_Bills_BillId",
                schema: "Finance",
                table: "WalletDeposits",
                column: "BillId",
                principalSchema: "finance",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
