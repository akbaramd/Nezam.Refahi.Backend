using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Settings.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class UpdateSomeFields : Migration
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
              oldClrType: typeof(string),
              oldType: "nvarchar(max)");

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "Settings",
              type: "datetime2",
              nullable: true,
              oldClrType: typeof(DateTime),
              oldType: "datetime2");

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "Settings",
              type: "nvarchar(max)",
              nullable: true,
              oldClrType: typeof(string),
              oldType: "nvarchar(max)");

          migrationBuilder.AlterColumn<string>(
              name: "ModifiedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: true,
              oldClrType: typeof(string),
              oldType: "nvarchar(max)");

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "datetime2",
              nullable: true,
              oldClrType: typeof(DateTime),
              oldType: "datetime2");

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: true,
              oldClrType: typeof(string),
              oldType: "nvarchar(max)");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.AlterColumn<string>(
              name: "ModifiedBy",
              schema: "settings",
              table: "Settings",
              type: "nvarchar(max)",
              nullable: false,
              defaultValue: "",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "Settings",
              type: "datetime2",
              nullable: false,
              defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldNullable: true);

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "Settings",
              type: "nvarchar(max)",
              nullable: false,
              defaultValue: "",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);

          migrationBuilder.AlterColumn<string>(
              name: "ModifiedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: false,
              defaultValue: "",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);

          migrationBuilder.AlterColumn<DateTime>(
              name: "ModifiedAt",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "datetime2",
              nullable: false,
              defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
              oldClrType: typeof(DateTime),
              oldType: "datetime2",
              oldNullable: true);

          migrationBuilder.AlterColumn<string>(
              name: "CreatedBy",
              schema: "settings",
              table: "SettingChangeEvents",
              type: "nvarchar(max)",
              nullable: false,
              defaultValue: "",
              oldClrType: typeof(string),
              oldType: "nvarchar(max)",
              oldNullable: true);
      }
  }
