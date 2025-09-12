using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Settings.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class UpdateSomeFields2 : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.AlterColumn<string>(
              name: "ModifiedBy",
              schema: "settings",
              table: "Settings",
              type: "nvarchar(max)",
              nullable: true,
              defaultValue: "00000000-0000-0000-0000-000000000001",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "Settings",
              type: "datetime2",
              nullable: true,
              defaultValueSql: "GETUTCDATE()",
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldNullable: true);

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "Settings",
              type: "nvarchar(max)",
              nullable: true,
              defaultValue: "00000000-0000-0000-0000-000000000001",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);

          migrationBuilder.AlterColumn<DateTime>(
              name: "CreatedAt",
              schema: "settings",
              table: "Settings",
              type: "datetime2",
              nullable: false,
              defaultValueSql: "GETUTCDATE()",
              oldClrType: typeof(DateTime),
              oldType: "datetime2");

          migrationBuilder.AlterColumn<string>(
              name: "ModifiedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: true,
              defaultValue: "00000000-0000-0000-0000-000000000001",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "datetime2",
              nullable: true,
              defaultValueSql: "GETUTCDATE()",
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldNullable: true);

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: true,
              defaultValue: "00000000-0000-0000-0000-000000000001",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);

          migrationBuilder.AlterColumn<DateTime>(
              name: "CreatedAt",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "datetime2",
              nullable: false,
              defaultValueSql: "GETUTCDATE()",
              oldClrType: typeof(DateTime),
              oldType: "datetime2");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.AlterColumn<string>(
              name: "ModifiedBy",
              schema: "settings",
              table: "Settings",
              type: "nvarchar(max)",
              nullable: true,
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true,
              oldDefaultValue: "00000000-0000-0000-0000-000000000001");

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "Settings",
              type: "datetime2",
              nullable: true,
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldNullable: true,
              oldDefaultValueSql: "GETUTCDATE()");

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "Settings",
              type: "nvarchar(max)",
              nullable: true,
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true,
              oldDefaultValue: "00000000-0000-0000-0000-000000000001");

          migrationBuilder.AlterColumn<DateTime>(
              name: "CreatedAt",
              schema: "settings",
              table: "Settings",
              type: "datetime2",
              nullable: false,
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldDefaultValueSql: "GETUTCDATE()");

          migrationBuilder.AlterColumn<string>(
              name: "ModifiedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: true,
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true,
              oldDefaultValue: "00000000-0000-0000-0000-000000000001");

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "datetime2",
              nullable: true,
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldNullable: true,
              oldDefaultValueSql: "GETUTCDATE()");

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: true,
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true,
              oldDefaultValue: "00000000-0000-0000-0000-000000000001");

          migrationBuilder.AlterColumn<DateTime>(
              name: "CreatedAt",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "datetime2",
              nullable: false,
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldDefaultValueSql: "GETUTCDATE()");
      }
  }
