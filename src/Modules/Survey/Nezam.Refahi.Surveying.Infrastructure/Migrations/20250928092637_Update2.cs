using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "surveying",
                table: "QuestionAnswers");
        }
    }
}
