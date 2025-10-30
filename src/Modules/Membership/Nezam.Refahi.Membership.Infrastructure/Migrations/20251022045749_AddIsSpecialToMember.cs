using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSpecialToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpecial",
                schema: "membership",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if member has special VIP status");

            migrationBuilder.CreateIndex(
                name: "IX_Members_IsSpecial",
                schema: "membership",
                table: "Members",
                column: "IsSpecial");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Members_IsSpecial",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IsSpecial",
                schema: "membership",
                table: "Members");
        }
    }
}
