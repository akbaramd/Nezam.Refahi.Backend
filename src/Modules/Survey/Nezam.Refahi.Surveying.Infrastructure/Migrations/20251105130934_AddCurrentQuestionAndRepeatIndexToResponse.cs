using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentQuestionAndRepeatIndexToResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentQuestionId",
                schema: "surveying",
                table: "Responses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentRepeatIndex",
                schema: "surveying",
                table: "Responses",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentQuestionId",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CurrentRepeatIndex",
                schema: "surveying",
                table: "Responses");
        }
    }
}
