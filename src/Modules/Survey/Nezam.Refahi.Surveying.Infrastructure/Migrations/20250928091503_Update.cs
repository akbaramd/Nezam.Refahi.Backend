using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionAnswers_QuestionAnswerId",
                schema: "surveying",
                table: "QuestionAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.AddColumn<Guid>(
                name: "ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OptionText",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "ResponseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionAnswers_QuestionAnswerId",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                column: "QuestionAnswerId",
                principalSchema: "surveying",
                principalTable: "QuestionAnswers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionOptions_OptionId",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                column: "OptionId",
                principalSchema: "surveying",
                principalTable: "QuestionOptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "QuestionId",
                principalSchema: "surveying",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "ResponseId",
                principalSchema: "surveying",
                principalTable: "Responses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "ResponseId1",
                principalSchema: "surveying",
                principalTable: "Responses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionAnswers_QuestionAnswerId",
                schema: "surveying",
                table: "QuestionAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionOptions_OptionId",
                schema: "surveying",
                table: "QuestionAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "OptionText",
                schema: "surveying",
                table: "QuestionAnswerOptions");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionAnswers_QuestionAnswerId",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                column: "QuestionAnswerId",
                principalSchema: "surveying",
                principalTable: "QuestionAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "ResponseId",
                principalSchema: "surveying",
                principalTable: "Responses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
