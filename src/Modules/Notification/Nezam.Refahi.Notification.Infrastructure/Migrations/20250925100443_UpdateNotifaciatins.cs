using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Notifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotifaciatins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionData",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ActionText",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ActionUrl",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                schema: "notification",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Context",
                schema: "notification",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Action",
                schema: "notification",
                table: "Notifications",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Context",
                schema: "notification",
                table: "Notifications",
                column: "Context");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ExternalUserId_Action",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "ExternalUserId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ExternalUserId_Context",
                schema: "notification",
                table: "Notifications",
                columns: new[] { "ExternalUserId", "Context" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_Action",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Context",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ExternalUserId_Action",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ExternalUserId_Context",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Action",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Context",
                schema: "notification",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ActionData",
                schema: "notification",
                table: "Notifications",
                type: "NVARCHAR(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionText",
                schema: "notification",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionUrl",
                schema: "notification",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "notification",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
