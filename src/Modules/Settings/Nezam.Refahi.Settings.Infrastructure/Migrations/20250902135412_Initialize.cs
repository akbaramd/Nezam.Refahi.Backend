using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Settings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "settings");

            migrationBuilder.CreateTable(
                name: "SettingsSections",
                schema: "settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettingsCategories",
                schema: "settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingsCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettingsCategories_SettingsSections_SectionId",
                        column: x => x.SectionId,
                        principalSchema: "settings",
                        principalTable: "SettingsSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SnapshotVersion = table.Column<long>(type: "bigint", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAccessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAccessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessCount = table.Column<long>(type: "bigint", nullable: false),
                    LastBackupAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_SettingsCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "settings",
                        principalTable: "SettingsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingChangeEvents",
                schema: "settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettingKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    OldValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    NewValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SnapshotVersion = table.Column<long>(type: "bigint", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAccessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAccessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessCount = table.Column<long>(type: "bigint", nullable: false),
                    LastBackupAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingChangeEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettingChangeEvents_Settings_SettingId",
                        column: x => x.SettingId,
                        principalSchema: "settings",
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_ChangedByUserId",
                schema: "settings",
                table: "SettingChangeEvents",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_ChangedByUserId_CreatedAt",
                schema: "settings",
                table: "SettingChangeEvents",
                columns: new[] { "ChangedByUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_CreatedAt",
                schema: "settings",
                table: "SettingChangeEvents",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_NewValueType",
                schema: "settings",
                table: "SettingChangeEvents",
                column: "NewValueType");

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_OldValueType",
                schema: "settings",
                table: "SettingChangeEvents",
                column: "OldValueType");

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_SettingId",
                schema: "settings",
                table: "SettingChangeEvents",
                column: "SettingId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_SettingId_ChangedByUserId_CreatedAt",
                schema: "settings",
                table: "SettingChangeEvents",
                columns: new[] { "SettingId", "ChangedByUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_SettingId_CreatedAt",
                schema: "settings",
                table: "SettingChangeEvents",
                columns: new[] { "SettingId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SettingChangeEvents_SettingKey",
                schema: "settings",
                table: "SettingChangeEvents",
                column: "SettingKey");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_CategoryId",
                schema: "settings",
                table: "Settings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_CategoryId_IsActive_DisplayOrder",
                schema: "settings",
                table: "Settings",
                columns: new[] { "CategoryId", "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_IsActive",
                schema: "settings",
                table: "Settings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_IsActive_IsReadOnly",
                schema: "settings",
                table: "Settings",
                columns: new[] { "IsActive", "IsReadOnly" });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_IsReadOnly",
                schema: "settings",
                table: "Settings",
                column: "IsReadOnly");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Key",
                schema: "settings",
                table: "Settings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Settings_ValueType",
                schema: "settings",
                table: "Settings",
                column: "ValueType");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsCategories_IsActive",
                schema: "settings",
                table: "SettingsCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsCategories_SectionId",
                schema: "settings",
                table: "SettingsCategories",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsCategories_SectionId_IsActive_DisplayOrder",
                schema: "settings",
                table: "SettingsCategories",
                columns: new[] { "SectionId", "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SettingsCategories_SectionId_Name",
                schema: "settings",
                table: "SettingsCategories",
                columns: new[] { "SectionId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SettingsSections_DisplayOrder",
                schema: "settings",
                table: "SettingsSections",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsSections_IsActive",
                schema: "settings",
                table: "SettingsSections",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SettingsSections_IsActive_DisplayOrder",
                schema: "settings",
                table: "SettingsSections",
                columns: new[] { "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SettingsSections_Name",
                schema: "settings",
                table: "SettingsSections",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingChangeEvents",
                schema: "settings");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "settings");

            migrationBuilder.DropTable(
                name: "SettingsCategories",
                schema: "settings");

            migrationBuilder.DropTable(
                name: "SettingsSections",
                schema: "settings");
        }
    }
}
