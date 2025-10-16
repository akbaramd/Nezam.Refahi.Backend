using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Shared.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageBox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssemblyName",
                schema: "shared",
                table: "OutboxMessages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullTypeName",
                schema: "shared",
                table: "OutboxMessages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_AssemblyName",
                schema: "shared",
                table: "OutboxMessages",
                column: "AssemblyName");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_FullTypeName",
                schema: "shared",
                table: "OutboxMessages",
                column: "FullTypeName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_AssemblyName",
                schema: "shared",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_FullTypeName",
                schema: "shared",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "AssemblyName",
                schema: "shared",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "FullTypeName",
                schema: "shared",
                table: "OutboxMessages");
        }
    }
}
