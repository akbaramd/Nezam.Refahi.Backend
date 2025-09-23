using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAggregaterootOfPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "finance",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "finance",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "finance",
                table: "Payments",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "finance",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                schema: "finance",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "finance",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "finance",
                table: "Payments",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "finance",
                table: "Payments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_DeletedAt",
                schema: "finance",
                table: "Payments",
                column: "DeletedAt",
                filter: "[DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_DeletedBy",
                schema: "finance",
                table: "Payments",
                column: "DeletedBy",
                filter: "[DeletedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IsDeleted",
                schema: "finance",
                table: "Payments",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payment_DeletedAt",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payment_DeletedBy",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payment_IsDeleted",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "finance",
                table: "Payments");
        }
    }
}
