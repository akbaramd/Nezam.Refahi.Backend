using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTheNewDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Responses_CreatedAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SubmittedAt",
                schema: "surveying",
                table: "Responses",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_SubmittedAt",
                schema: "surveying",
                table: "Responses",
                column: "SubmittedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Responses_SubmittedAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SubmittedAt",
                schema: "surveying",
                table: "Responses",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "surveying",
                table: "Responses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "surveying",
                table: "Responses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "surveying",
                table: "Responses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "surveying",
                table: "Responses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "surveying",
                table: "Responses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                schema: "surveying",
                table: "Responses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "surveying",
                table: "Responses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "surveying",
                table: "Responses",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "surveying",
                table: "Responses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_CreatedAt",
                schema: "surveying",
                table: "Responses",
                column: "CreatedAt");
        }
    }
}
