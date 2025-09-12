using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Settings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdNeverGenerated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "settings",
                table: "SettingsSections",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "settings",
                table: "SettingsCategories",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "settings",
                table: "Settings",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "settings",
                table: "SettingChangeEvents",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "settings",
                table: "SettingsSections");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "settings",
                table: "SettingsCategories");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "settings",
                table: "SettingChangeEvents");
        }
    }
}
