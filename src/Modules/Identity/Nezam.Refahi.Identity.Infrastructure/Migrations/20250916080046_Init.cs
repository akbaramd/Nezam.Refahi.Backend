using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "OtpChallenges",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceFingerprint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, defaultValue: ""),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    OtpLength = table.Column<int>(type: "int", nullable: false),
                    TtlSeconds = table.Column<int>(type: "int", nullable: false),
                    MaxVerifyAttempts = table.Column<int>(type: "int", nullable: false),
                    MaxResends = table.Column<int>(type: "int", nullable: false),
                    MaxPerPhonePerHour = table.Column<int>(type: "int", nullable: false),
                    MaxPerIpPerHour = table.Column<int>(type: "int", nullable: false),
                    OtpHash = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    Nonce = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    ChallengeStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeliveryStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttemptsLeft = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    ResendLeft = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    LastSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsumedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockReason = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "00000000-0000-0000-0000-000000000001"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "00000000-0000-0000-0000-000000000001"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPhoneVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PhoneVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastAuthenticatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UnlockAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDeviceFingerprint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastIpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    LastUserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ClaimValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceFingerprintValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TokenHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TokenAlgorithm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TokenSalt = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rotation = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshSessions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ClaimValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "General")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenValue = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: false),
                    TokenType = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    DeviceFingerprint = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: true),
                    SessionFamilyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Salt = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_ClientId",
                schema: "identity",
                table: "OtpChallenges",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_CreatedAt",
                schema: "identity",
                table: "OtpChallenges",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_ExpiresAt",
                schema: "identity",
                table: "OtpChallenges",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_PhoneNumber",
                schema: "identity",
                table: "OtpChallenges",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_Status",
                schema: "identity",
                table: "OtpChallenges",
                column: "ChallengeStatus");

            migrationBuilder.CreateIndex(
                name: "IX_OtpChallenges_Status_ExpiresAt",
                schema: "identity",
                table: "OtpChallenges",
                columns: new[] { "ChallengeStatus", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_ClientId",
                table: "RefreshSessions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_LastUsedAt",
                table: "RefreshSessions",
                column: "LastUsedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_UserId",
                table: "RefreshSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_UserId_ClientId",
                table: "RefreshSessions",
                columns: new[] { "UserId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_ClaimType",
                schema: "identity",
                table: "RoleClaims",
                column: "ClaimType");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_ClaimValue",
                schema: "identity",
                table: "RoleClaims",
                column: "ClaimValue");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DisplayOrder",
                schema: "identity",
                table: "Roles",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsActive",
                schema: "identity",
                table: "Roles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsActive_DisplayOrder",
                schema: "identity",
                table: "Roles",
                columns: new[] { "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsSystemRole",
                schema: "identity",
                table: "Roles",
                column: "IsSystemRole");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "identity",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_AssignedAt",
                schema: "identity",
                table: "UserClaims",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_AssignedBy",
                schema: "identity",
                table: "UserClaims",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_ClaimType",
                schema: "identity",
                table: "UserClaims",
                column: "ClaimType");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_ClaimValue",
                schema: "identity",
                table: "UserClaims",
                column: "ClaimValue");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_ExpiresAt",
                schema: "identity",
                table: "UserClaims",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_IsActive",
                schema: "identity",
                table: "UserClaims",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_IsActive_ExpiresAt",
                schema: "identity",
                table: "UserClaims",
                columns: new[] { "IsActive", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId_IsActive",
                schema: "identity",
                table: "UserClaims",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_Category",
                schema: "identity",
                table: "UserPreferences",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_DisplayOrder",
                schema: "identity",
                table: "UserPreferences",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_IsActive",
                schema: "identity",
                table: "UserPreferences",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                schema: "identity",
                table: "UserPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId_Category",
                schema: "identity",
                table: "UserPreferences",
                columns: new[] { "UserId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_ValueType",
                schema: "identity",
                table: "UserPreferences",
                column: "ValueType");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedAt",
                schema: "identity",
                table: "UserRoles",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedBy",
                schema: "identity",
                table: "UserRoles",
                column: "AssignedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_ExpiresAt",
                schema: "identity",
                table: "UserRoles",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_IsActive",
                schema: "identity",
                table: "UserRoles",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_IsActive_ExpiresAt",
                schema: "identity",
                table: "UserRoles",
                columns: new[] { "IsActive", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId_IsActive",
                schema: "identity",
                table: "UserRoles",
                columns: new[] { "RoleId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                schema: "identity",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_IsActive",
                schema: "identity",
                table: "UserRoles",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                schema: "identity",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedAt",
                schema: "identity",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedBy",
                schema: "identity",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_User_DeletedAt",
                schema: "identity",
                table: "Users",
                column: "DeletedAt",
                filter: "[DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_DeletedBy",
                schema: "identity",
                table: "Users",
                column: "DeletedBy",
                filter: "[DeletedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_IsDeleted",
                schema: "identity",
                table: "Users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_User_LastModifiedAt",
                schema: "identity",
                table: "Users",
                column: "LastModifiedAt",
                filter: "[LastModifiedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_LastModifiedBy",
                schema: "identity",
                table: "Users",
                column: "LastModifiedBy",
                filter: "[LastModifiedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                schema: "identity",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsPhoneVerified",
                schema: "identity",
                table: "Users",
                column: "IsPhoneVerified");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NationalId",
                schema: "identity",
                table: "Users",
                column: "NationalId",
                unique: true,
                filter: "[NationalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "identity",
                table: "Users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_ExpiresAt",
                schema: "identity",
                table: "UserTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_IsRevoked_IsUsed",
                schema: "identity",
                table: "UserTokens",
                columns: new[] { "IsRevoked", "IsUsed" },
                filter: "[IsRevoked] = 0 AND [IsUsed] = 0");

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
                name: "IX_UserTokens_TokenValue_TokenType",
                schema: "identity",
                table: "UserTokens",
                columns: new[] { "TokenValue", "TokenType" },
                unique: true)
                .Annotation("SqlServer:FillFactor", 90);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId_DeviceFingerprint_TokenType",
                schema: "identity",
                table: "UserTokens",
                columns: new[] { "UserId", "DeviceFingerprint", "TokenType" },
                filter: "[DeviceFingerprint] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId_TokenType",
                schema: "identity",
                table: "UserTokens",
                columns: new[] { "UserId", "TokenType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtpChallenges",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "RefreshSessions");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserPreferences",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "identity");
        }
    }
}
