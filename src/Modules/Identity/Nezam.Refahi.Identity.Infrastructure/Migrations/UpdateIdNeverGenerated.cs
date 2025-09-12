using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdNeverGenerated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "identity",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "identity",
                table: "UserRoles");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "UserTokens",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "Users",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "UserRoles",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "UserPreferences",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "UserClaims",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "Roles",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "RoleClaims",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RefreshSessions",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "identity",
                table: "OtpChallenges",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "identity",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "identity",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "identity",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "identity",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "identity",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "UserClaims");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "identity",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "identity",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "identity",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
