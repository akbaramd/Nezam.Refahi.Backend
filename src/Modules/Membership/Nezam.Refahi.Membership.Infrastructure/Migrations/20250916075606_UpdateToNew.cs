using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessCount",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastAccessedAt",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastAccessedBy",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastArchivedAt",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastBackupAt",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Metadata",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "membership",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "SnapshotVersion",
                schema: "membership",
                table: "Members",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "membership",
                table: "Members",
                newName: "LastModifiedAt");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "membership",
                table: "Members",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "membership",
                table: "Members",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "membership",
                table: "Members",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "membership",
                table: "Members",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "membership",
                table: "Members",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Member_CreatedAt",
                schema: "membership",
                table: "Members",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Member_CreatedBy",
                schema: "membership",
                table: "Members",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Member_DeletedAt",
                schema: "membership",
                table: "Members",
                column: "DeletedAt",
                filter: "[DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Member_DeletedBy",
                schema: "membership",
                table: "Members",
                column: "DeletedBy",
                filter: "[DeletedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Member_IsDeleted",
                schema: "membership",
                table: "Members",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Member_LastModifiedAt",
                schema: "membership",
                table: "Members",
                column: "LastModifiedAt",
                filter: "[LastModifiedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Member_LastModifiedBy",
                schema: "membership",
                table: "Members",
                column: "LastModifiedBy",
                filter: "[LastModifiedBy] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Member_CreatedAt",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Member_CreatedBy",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Member_DeletedAt",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Member_DeletedBy",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Member_IsDeleted",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Member_LastModifiedAt",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Member_LastModifiedBy",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "membership",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "membership",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "Version",
                schema: "membership",
                table: "Members",
                newName: "SnapshotVersion");

            migrationBuilder.RenameColumn(
                name: "LastModifiedAt",
                schema: "membership",
                table: "Members",
                newName: "ModifiedAt");

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "membership",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "membership",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "membership",
                table: "Members",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<long>(
                name: "AccessCount",
                schema: "membership",
                table: "Members",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAtUtc",
                schema: "membership",
                table: "Members",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessedAt",
                schema: "membership",
                table: "Members",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastAccessedBy",
                schema: "membership",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastArchivedAt",
                schema: "membership",
                table: "Members",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBackupAt",
                schema: "membership",
                table: "Members",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedUtc",
                schema: "membership",
                table: "Members",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "membership",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "membership",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
