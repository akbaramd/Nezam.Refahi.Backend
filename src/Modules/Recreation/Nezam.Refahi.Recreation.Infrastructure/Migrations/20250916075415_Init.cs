using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "recreation");

            migrationBuilder.EnsureSchema(
                name: "Recreation");

            migrationBuilder.CreateTable(
                name: "FeatureCategories",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IconClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RegistrationStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TourStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TourEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false),
                    CurrentRegisteredCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinAge = table.Column<int>(type: "int", nullable: true),
                    MaxAge = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IconClass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidationRules = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Features_FeatureCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "recreation",
                        principalTable: "FeatureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TourMemberCapabilities",
                schema: "Recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourMemberCapabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourMemberCapabilities_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourMemberFeatures",
                schema: "Recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourMemberFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourMemberFeatures_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPhotos",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPhotos_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPricing",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantType = table.Column<int>(type: "int", nullable: false),
                    PriceRials = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "date", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    MinQuantity = table.Column<int>(type: "int", nullable: true),
                    MaxQuantity = table.Column<int>(type: "int", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPricing_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourReservations",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalAmountRials = table.Column<long>(type: "bigint", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_TourReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourReservations_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourRestrictedTours",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestrictedTourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourRestrictedTours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourRestrictedTours_Tours_RestrictedTourId",
                        column: x => x.RestrictedTourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TourRestrictedTours_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourRestrictedTours_Tours_TourId1",
                        column: x => x.TourId1,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TourFeatures",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsHighlighted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "recreation",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourFeatures_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "recreation",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ParticipantType = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmergencyContactPhone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiredAmountRials = table.Column<long>(type: "bigint", nullable: false),
                    PaidAmountRials = table.Column<long>(type: "bigint", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_TourReservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "recreation",
                        principalTable: "TourReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureCategories_DisplayOrder",
                schema: "recreation",
                table: "FeatureCategories",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureCategories_IsActive",
                schema: "recreation",
                table: "FeatureCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureCategories_Name",
                schema: "recreation",
                table: "FeatureCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Features_CategoryId",
                schema: "recreation",
                table: "Features",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_CategoryId_Name",
                schema: "recreation",
                table: "Features",
                columns: new[] { "CategoryId", "Name" },
                unique: true,
                filter: "[CategoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Features_DisplayOrder",
                schema: "recreation",
                table: "Features",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Features_IsActive",
                schema: "recreation",
                table: "Features",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_NationalNumber",
                schema: "recreation",
                table: "Participants",
                column: "NationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ParticipantType",
                schema: "recreation",
                table: "Participants",
                column: "ParticipantType");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ReservationId",
                schema: "recreation",
                table: "Participants",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ReservationId_NationalNumber",
                schema: "recreation",
                table: "Participants",
                columns: new[] { "ReservationId", "NationalNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourFeatures_AssignedAt",
                schema: "recreation",
                table: "TourFeatures",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TourFeatures_FeatureId",
                schema: "recreation",
                table: "TourFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TourFeatures_IsHighlighted",
                schema: "recreation",
                table: "TourFeatures",
                column: "IsHighlighted");

            migrationBuilder.CreateIndex(
                name: "IX_TourFeatures_TourId",
                schema: "recreation",
                table: "TourFeatures",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourFeatures_TourId_FeatureId",
                schema: "recreation",
                table: "TourFeatures",
                columns: new[] { "TourId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourMemberCapabilities_CapabilityId",
                schema: "Recreation",
                table: "TourMemberCapabilities",
                column: "CapabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_TourMemberCapabilities_TourId",
                schema: "Recreation",
                table: "TourMemberCapabilities",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourMemberCapabilities_TourId_CapabilityId_Unique",
                schema: "Recreation",
                table: "TourMemberCapabilities",
                columns: new[] { "TourId", "CapabilityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourMemberFeatures_FeatureId",
                schema: "Recreation",
                table: "TourMemberFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TourMemberFeatures_TourId",
                schema: "Recreation",
                table: "TourMemberFeatures",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourMemberFeatures_TourId_FeatureId_Unique",
                schema: "Recreation",
                table: "TourMemberFeatures",
                columns: new[] { "TourId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourPhotos_DisplayOrder",
                schema: "recreation",
                table: "TourPhotos",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TourPhotos_TourId",
                schema: "recreation",
                table: "TourPhotos",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricing_IsActive",
                schema: "recreation",
                table: "TourPricing",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricing_ParticipantType",
                schema: "recreation",
                table: "TourPricing",
                column: "ParticipantType");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricing_TourId",
                schema: "recreation",
                table: "TourPricing",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricing_TourId_ParticipantType",
                schema: "recreation",
                table: "TourPricing",
                columns: new[] { "TourId", "ParticipantType" });

            migrationBuilder.CreateIndex(
                name: "IX_TourPricing_ValidFrom",
                schema: "recreation",
                table: "TourPricing",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_TourPricing_ValidTo",
                schema: "recreation",
                table: "TourPricing",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservation_CreatedAt",
                schema: "recreation",
                table: "TourReservations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservation_CreatedBy",
                schema: "recreation",
                table: "TourReservations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservation_DeletedAt",
                schema: "recreation",
                table: "TourReservations",
                column: "DeletedAt",
                filter: "[DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservation_DeletedBy",
                schema: "recreation",
                table: "TourReservations",
                column: "DeletedBy",
                filter: "[DeletedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservation_IsDeleted",
                schema: "recreation",
                table: "TourReservations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservation_LastModifiedAt",
                schema: "recreation",
                table: "TourReservations",
                column: "LastModifiedAt",
                filter: "[LastModifiedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservation_LastModifiedBy",
                schema: "recreation",
                table: "TourReservations",
                column: "LastModifiedBy",
                filter: "[LastModifiedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_ExpiryDate",
                schema: "recreation",
                table: "TourReservations",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_ReservationDate",
                schema: "recreation",
                table: "TourReservations",
                column: "ReservationDate");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_Status",
                schema: "recreation",
                table: "TourReservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TourId",
                schema: "recreation",
                table: "TourReservations",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TrackingCode",
                schema: "recreation",
                table: "TourReservations",
                column: "TrackingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourRestrictedTours_RestrictedTourId",
                schema: "recreation",
                table: "TourRestrictedTours",
                column: "RestrictedTourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourRestrictedTours_TourId",
                schema: "recreation",
                table: "TourRestrictedTours",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourRestrictedTours_TourId_RestrictedTourId_Unique",
                schema: "recreation",
                table: "TourRestrictedTours",
                columns: new[] { "TourId", "RestrictedTourId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourRestrictedTours_TourId1",
                schema: "recreation",
                table: "TourRestrictedTours",
                column: "TourId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_CreatedAt",
                schema: "recreation",
                table: "Tours",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_CreatedBy",
                schema: "recreation",
                table: "Tours",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_DeletedAt",
                schema: "recreation",
                table: "Tours",
                column: "DeletedAt",
                filter: "[DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_DeletedBy",
                schema: "recreation",
                table: "Tours",
                column: "DeletedBy",
                filter: "[DeletedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_IsDeleted",
                schema: "recreation",
                table: "Tours",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_LastModifiedAt",
                schema: "recreation",
                table: "Tours",
                column: "LastModifiedAt",
                filter: "[LastModifiedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tour_LastModifiedBy",
                schema: "recreation",
                table: "Tours",
                column: "LastModifiedBy",
                filter: "[LastModifiedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_IsActive",
                schema: "recreation",
                table: "Tours",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_RegistrationStart_RegistrationEnd",
                schema: "recreation",
                table: "Tours",
                columns: new[] { "RegistrationStart", "RegistrationEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_Tours_TourStart",
                schema: "recreation",
                table: "Tours",
                column: "TourStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participants",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "TourFeatures",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "TourMemberCapabilities",
                schema: "Recreation");

            migrationBuilder.DropTable(
                name: "TourMemberFeatures",
                schema: "Recreation");

            migrationBuilder.DropTable(
                name: "TourPhotos",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "TourPricing",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "TourRestrictedTours",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "TourReservations",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "Tours",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "FeatureCategories",
                schema: "recreation");
        }
    }
}
