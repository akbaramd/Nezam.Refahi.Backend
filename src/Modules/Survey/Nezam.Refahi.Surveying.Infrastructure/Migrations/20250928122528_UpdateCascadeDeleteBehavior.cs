using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDeleteBehavior : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionAnswers_QuestionAnswerId",
                schema: "surveying",
                table: "QuestionAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswerOptions_QuestionAnswers_QuestionAnswerId",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                column: "QuestionAnswerId",
                principalSchema: "surveying",
                principalTable: "QuestionAnswers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Responses_ResponseId",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "ResponseId",
                principalSchema: "surveying",
                principalTable: "Responses",
                principalColumn: "Id");
        }
    }
}
