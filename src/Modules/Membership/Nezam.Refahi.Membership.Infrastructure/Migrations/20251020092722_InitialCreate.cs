using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Membership.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class InitialCreate : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.EnsureSchema(
              name: "membership");

          migrationBuilder.CreateTable(
              name: "Members",
              schema: "membership",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  ExternalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  MembershipNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                  NationalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                  FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                  PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                  BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                  Version = table.Column<long>(type: "bigint", nullable: false),
                  RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                  CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                  CreatedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                  LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                  LastModifiedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                  IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                  DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                  DeletedBy = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Members", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "Roles",
              schema: "membership",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                  Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                  EmployerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                  EmployerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                  IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                  SortOrder = table.Column<int>(type: "int", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_Roles", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "MemberAgencies",
              schema: "membership",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  AgencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                  OfficeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                  OfficeTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                  ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                  ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                  AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                  AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                  Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                  AccessLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_MemberAgencies", x => x.Id);
                  table.ForeignKey(
                      name: "FK_MemberAgencies_Members_MemberId",
                      column: x => x.MemberId,
                      principalSchema: "membership",
                      principalTable: "Members",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "MemberCapabilities",
              schema: "membership",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  CapabilityKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                  CapabilityTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                  ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                  ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                  AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                  AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                  Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_MemberCapabilities", x => x.Id);
                  table.ForeignKey(
                      name: "FK_MemberCapabilities_Members_MemberId",
                      column: x => x.MemberId,
                      principalSchema: "membership",
                      principalTable: "Members",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "MemberFeatures",
              schema: "membership",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  FeatureKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                  IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                  FeatureTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                  ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                  ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                  AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                  AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                  Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_MemberFeatures", x => x.Id);
                  table.ForeignKey(
                      name: "FK_MemberFeatures_Members_MemberId",
                      column: x => x.MemberId,
                      principalSchema: "membership",
                      principalTable: "Members",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "MemberRoles",
              schema: "membership",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                  ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                  ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                  AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                  Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                  IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_MemberRoles", x => x.Id);
                  table.ForeignKey(
                      name: "FK_MemberRoles_Members_MemberId",
                      column: x => x.MemberId,
                      principalSchema: "membership",
                      principalTable: "Members",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
                  table.ForeignKey(
                      name: "FK_MemberRoles_Roles_RoleId",
                      column: x => x.RoleId,
                      principalSchema: "membership",
                      principalTable: "Roles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateIndex(
              name: "IX_MemberAgencies_AccessLevel",
              schema: "membership",
              table: "MemberAgencies",
              column: "AccessLevel");

          migrationBuilder.CreateIndex(
              name: "IX_MemberAgencies_AgencyId",
              schema: "membership",
              table: "MemberAgencies",
              column: "AgencyId");

          migrationBuilder.CreateIndex(
              name: "IX_MemberAgencies_AssignedAt",
              schema: "membership",
              table: "MemberAgencies",
              column: "AssignedAt");

          migrationBuilder.CreateIndex(
              name: "IX_MemberAgencies_IsActive",
              schema: "membership",
              table: "MemberAgencies",
              column: "IsActive");

          migrationBuilder.CreateIndex(
              name: "IX_MemberAgencies_Member_Office_Active",
              schema: "membership",
              table: "MemberAgencies",
              columns: new[] { "MemberId", "AgencyId" },
              unique: true,
              filter: "[IsActive] = 1");

          migrationBuilder.CreateIndex(
              name: "IX_MemberAgencies_MemberId",
              schema: "membership",
              table: "MemberAgencies",
              column: "MemberId");

          migrationBuilder.CreateIndex(
              name: "IX_MemberAgencies_ValidityPeriod",
              schema: "membership",
              table: "MemberAgencies",
              columns: new[] { "ValidFrom", "ValidTo" });

          migrationBuilder.CreateIndex(
              name: "IX_MemberCapabilities_AssignedAt",
              schema: "membership",
              table: "MemberCapabilities",
              column: "AssignedAt");

          migrationBuilder.CreateIndex(
              name: "IX_MemberCapabilities_CapabilityCapabilityKey",
              schema: "membership",
              table: "MemberCapabilities",
              column: "CapabilityKey");

          migrationBuilder.CreateIndex(
              name: "IX_MemberCapabilities_IsActive",
              schema: "membership",
              table: "MemberCapabilities",
              column: "IsActive");

          migrationBuilder.CreateIndex(
              name: "IX_MemberCapabilities_Member_Capability_Active",
              schema: "membership",
              table: "MemberCapabilities",
              columns: new[] { "MemberId", "CapabilityKey" },
              unique: true,
              filter: "[IsActive] = 1");

          migrationBuilder.CreateIndex(
              name: "IX_MemberCapabilities_MemberId",
              schema: "membership",
              table: "MemberCapabilities",
              column: "MemberId");

          migrationBuilder.CreateIndex(
              name: "IX_MemberCapabilities_ValidityPeriod",
              schema: "membership",
              table: "MemberCapabilities",
              columns: new[] { "ValidFrom", "ValidTo" });

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_AssignedAt",
              schema: "membership",
              table: "MemberFeatures",
              column: "AssignedAt");

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_AssignedBy",
              schema: "membership",
              table: "MemberFeatures",
              column: "AssignedBy");

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_FeatureKey",
              schema: "membership",
              table: "MemberFeatures",
              column: "FeatureKey");

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_IsActive",
              schema: "membership",
              table: "MemberFeatures",
              column: "IsActive");

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_MemberId",
              schema: "membership",
              table: "MemberFeatures",
              column: "MemberId");

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_MemberId_FeatureKey",
              schema: "membership",
              table: "MemberFeatures",
              columns: new[] { "MemberId", "FeatureKey" },
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_ValidFrom",
              schema: "membership",
              table: "MemberFeatures",
              column: "ValidFrom");

          migrationBuilder.CreateIndex(
              name: "IX_MemberFeatures_ValidTo",
              schema: "membership",
              table: "MemberFeatures",
              column: "ValidTo");

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_IsActive",
              schema: "membership",
              table: "MemberRoles",
              column: "IsActive");

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_Member_IsActive",
              schema: "membership",
              table: "MemberRoles",
              columns: new[] { "MemberId", "IsActive" });

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_Member_Role",
              schema: "membership",
              table: "MemberRoles",
              columns: new[] { "MemberId", "RoleId" });

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_Member_ValidTo",
              schema: "membership",
              table: "MemberRoles",
              columns: new[] { "MemberId", "ValidTo" });

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_MemberId",
              schema: "membership",
              table: "MemberRoles",
              column: "MemberId");

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_Role_IsActive",
              schema: "membership",
              table: "MemberRoles",
              columns: new[] { "RoleId", "IsActive" });

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_RoleId",
              schema: "membership",
              table: "MemberRoles",
              column: "RoleId");

          migrationBuilder.CreateIndex(
              name: "IX_MemberRoles_ValidTo",
              schema: "membership",
              table: "MemberRoles",
              column: "ValidTo");

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

          migrationBuilder.CreateIndex(
              name: "IX_Members_Email",
              schema: "membership",
              table: "Members",
              column: "Email",
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_Members_ExternalUserId",
              schema: "membership",
              table: "Members",
              column: "ExternalUserId",
              unique: true,
              filter: "[ExternalUserId] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_Members_FirstName",
              schema: "membership",
              table: "Members",
              column: "FirstName");

          migrationBuilder.CreateIndex(
              name: "IX_Members_LastName",
              schema: "membership",
              table: "Members",
              column: "LastName");

          migrationBuilder.CreateIndex(
              name: "IX_Members_NationalCode",
              schema: "membership",
              table: "Members",
              column: "NationalCode",
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_Members_PhoneNumber",
              schema: "membership",
              table: "Members",
              column: "PhoneNumber",
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_Roles_EmployerCode",
              schema: "membership",
              table: "Roles",
              column: "EmployerCode");

          migrationBuilder.CreateIndex(
              name: "IX_Roles_EmployerName",
              schema: "membership",
              table: "Roles",
              column: "EmployerName");

          migrationBuilder.CreateIndex(
              name: "IX_Roles_IsActive",
              schema: "membership",
              table: "Roles",
              column: "IsActive");

          migrationBuilder.CreateIndex(
              name: "IX_Roles_Key",
              schema: "membership",
              table: "Roles",
              column: "Key",
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_Roles_SortOrder_Title",
              schema: "membership",
              table: "Roles",
              columns: new[] { "SortOrder", "Title" });
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "MemberAgencies",
              schema: "membership");

          migrationBuilder.DropTable(
              name: "MemberCapabilities",
              schema: "membership");

          migrationBuilder.DropTable(
              name: "MemberFeatures",
              schema: "membership");

          migrationBuilder.DropTable(
              name: "MemberRoles",
              schema: "membership");

          migrationBuilder.DropTable(
              name: "Members",
              schema: "membership");

          migrationBuilder.DropTable(
              name: "Roles",
              schema: "membership");
      }
  }
