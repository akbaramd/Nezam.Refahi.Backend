using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseTokenValueLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TokenValue",
                schema: "identity",
                table: "UserTokens",
                type: "varchar(2048)",
                unicode: false,
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldUnicode: false,
                oldMaxLength: 256);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TokenValue",
                schema: "identity",
                table: "UserTokens",
                type: "varchar(256)",
                unicode: false,
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(2048)",
                oldUnicode: false,
                oldMaxLength: 2048);
        }
    }
}
