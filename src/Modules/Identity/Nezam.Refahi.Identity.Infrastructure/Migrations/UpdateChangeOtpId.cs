using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChangeOtpId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OtpChallenges_ChallengeId",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "ChallengeId",
                schema: "identity",
                table: "OtpChallenges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChallengeId",
                schema: "identity",
                table: "OtpChallenges",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_ChallengeId",
                schema: "identity",
                table: "OtpChallenges",
                column: "ChallengeId",
                unique: true);
        }
    }
}
