using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSomeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessCount",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "LastAccessedAt",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "LastAccessedBy",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "LastArchivedAt",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "LastBackupAt",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "SnapshotVersion",
                table: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "AccessCount",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "LastAccessedAt",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "LastAccessedBy",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "LastArchivedAt",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "LastBackupAt",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "Metadata",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.DropColumn(
                name: "SnapshotVersion",
                schema: "identity",
                table: "OtpChallenges");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccessCount",
                table: "RefreshSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                table: "RefreshSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RefreshSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RefreshSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "RefreshSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RefreshSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessedAt",
                table: "RefreshSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastAccessedBy",
                table: "RefreshSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastArchivedAt",
                table: "RefreshSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBackupAt",
                table: "RefreshSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedUtc",
                table: "RefreshSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "RefreshSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "RefreshSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "RefreshSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SnapshotVersion",
                table: "RefreshSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AccessCount",
                schema: "identity",
                table: "OtpChallenges",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                schema: "identity",
                table: "OtpChallenges",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "identity",
                table: "OtpChallenges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "identity",
                table: "OtpChallenges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "identity",
                table: "OtpChallenges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "identity",
                table: "OtpChallenges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessedAt",
                schema: "identity",
                table: "OtpChallenges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastAccessedBy",
                schema: "identity",
                table: "OtpChallenges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastArchivedAt",
                schema: "identity",
                table: "OtpChallenges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBackupAt",
                schema: "identity",
                table: "OtpChallenges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedUtc",
                schema: "identity",
                table: "OtpChallenges",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "identity",
                table: "OtpChallenges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                schema: "identity",
                table: "OtpChallenges",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "identity",
                table: "OtpChallenges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SnapshotVersion",
                schema: "identity",
                table: "OtpChallenges",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
