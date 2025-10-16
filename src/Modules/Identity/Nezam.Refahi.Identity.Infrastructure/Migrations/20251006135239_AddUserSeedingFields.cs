using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSeedingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalUserId",
                schema: "identity",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileSnapshot",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceChecksum",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceSystem",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceVersion",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExternalUserId",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileSnapshot",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SourceChecksum",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SourceVersion",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Username",
                schema: "identity",
                table: "Users");
        }
    }
}
