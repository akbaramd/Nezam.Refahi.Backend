using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixQuestionAnswerRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_ResponseId1",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "ResponseId1");

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
    }
}
