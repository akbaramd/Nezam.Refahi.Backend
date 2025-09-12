using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityTableModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceId",
                schema: "identity",
                table: "UserTokens",
                newName: "Salt");

            migrationBuilder.AddColumn<string>(
                name: "DeviceFingerprint",
                schema: "identity",
                table: "UserTokens",
                type: "varchar(256)",
                unicode: false,
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUsedAt",
                schema: "identity",
                table: "UserTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentTokenId",
                schema: "identity",
                table: "UserTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionFamilyId",
                schema: "identity",
                table: "UserTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                schema: "identity",
                table: "UserTokens",
                type: "varchar(512)",
                unicode: false,
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_LastUsedAt",
                schema: "identity",
                table: "UserTokens",
                column: "LastUsedAt",
                filter: "[LastUsedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_ParentTokenId",
                schema: "identity",
                table: "UserTokens",
                column: "ParentTokenId",
                filter: "[ParentTokenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_SessionFamilyId",
                schema: "identity",
                table: "UserTokens",
                column: "SessionFamilyId",
                filter: "[SessionFamilyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId_DeviceFingerprint_TokenType",
                schema: "identity",
                table: "UserTokens",
                columns: new[] { "UserId", "DeviceFingerprint", "TokenType" },
                filter: "[DeviceFingerprint] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserTokens_LastUsedAt",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_ParentTokenId",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_SessionFamilyId",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_UserId_DeviceFingerprint_TokenType",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "DeviceFingerprint",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "ParentTokenId",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "SessionFamilyId",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                schema: "identity",
                table: "UserTokens");

            migrationBuilder.RenameColumn(
                name: "Salt",
                schema: "identity",
                table: "UserTokens",
                newName: "DeviceId");
        }
    }
}
