using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttemptAndRepeatableFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_ResponseId_QuestionId",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.AddColumn<string>(
                name: "AttemptStatus",
                schema: "surveying",
                table: "Responses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CanceledAt",
                schema: "surveying",
                table: "Responses",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiredAt",
                schema: "surveying",
                table: "Responses",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRepeats",
                schema: "surveying",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepeatPolicyKind",
                schema: "surveying",
                table: "Questions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RepeatIndex",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_AttemptStatus",
                schema: "surveying",
                table: "Responses",
                column: "AttemptStatus");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_RepeatIndex",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "RepeatIndex");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_ResponseId_QuestionId_RepeatIndex",
                schema: "surveying",
                table: "QuestionAnswers",
                columns: new[] { "ResponseId", "QuestionId", "RepeatIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Responses_AttemptStatus",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_RepeatIndex",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_ResponseId_QuestionId_RepeatIndex",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "AttemptStatus",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CanceledAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "MaxRepeats",
                schema: "surveying",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "RepeatPolicyKind",
                schema: "surveying",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "RepeatIndex",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_ResponseId_QuestionId",
                schema: "surveying",
                table: "QuestionAnswers",
                columns: new[] { "ResponseId", "QuestionId" },
                unique: true);
        }
    }
}
