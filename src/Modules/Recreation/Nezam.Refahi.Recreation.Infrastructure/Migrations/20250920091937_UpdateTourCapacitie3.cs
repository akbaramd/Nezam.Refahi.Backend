using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Recreation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTourCapacitie3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TourReservations_ExpiryDate",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_MemberId",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_Status",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_TrackingCode",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourCapacities_IsActive",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropIndex(
                name: "IX_TourCapacities_RegistrationStart_RegistrationEnd",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropIndex(
                name: "IX_TourCapacities_TourId_IsActive",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.RenameIndex(
                name: "IX_Participants_ReservationId_NationalNumber",
                schema: "recreation",
                table: "Participants",
                newName: "UX_Participants_ReservationNationalNumber");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationDate",
                schema: "recreation",
                table: "TourReservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                schema: "recreation",
                table: "TourReservations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PaidAmountRials",
                schema: "recreation",
                table: "TourReservations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "recreation",
                table: "TourReservations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipantsPerReservation",
                schema: "recreation",
                table: "TourCapacities",
                type: "int",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<int>(
                name: "MinParticipantsPerReservation",
                schema: "recreation",
                table: "TourCapacities",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "RemainingParticipants",
                schema: "recreation",
                table: "TourCapacities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "recreation",
                table: "TourCapacities",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "recreation",
                table: "TourCapacities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiIdempotency",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequestPayloadHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ResponseData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ClientIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiIdempotency", x => x.Id);
                    table.CheckConstraint("CK_ApiIdempotency_ExpiresAfterCreated", "[ExpiresAt] > [CreatedAt]");
                    table.CheckConstraint("CK_ApiIdempotency_StatusCodeValid", "[StatusCode] >= 100 AND [StatusCode] <= 599");
                });

            migrationBuilder.CreateTable(
                name: "ReservationPriceSnapshots",
                schema: "recreation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantType = table.Column<int>(type: "int", nullable: false),
                    BasePriceRials = table.Column<long>(type: "bigint", nullable: false),
                    DiscountAmountRials = table.Column<long>(type: "bigint", nullable: true),
                    FinalPriceRials = table.Column<long>(type: "bigint", nullable: false),
                    DiscountCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiscountDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PricingRules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationPriceSnapshots", x => x.Id);
                    table.CheckConstraint("CK_ReservationPriceSnapshots_BasePricePositive", "[BasePriceRials] >= 0");
                    table.CheckConstraint("CK_ReservationPriceSnapshots_DiscountValid", "[DiscountAmountRials] IS NULL OR [DiscountAmountRials] >= 0");
                    table.CheckConstraint("CK_ReservationPriceSnapshots_FinalPricePositive", "[FinalPriceRials] >= 0");
                    table.ForeignKey(
                        name: "FK_ReservationPriceSnapshots_TourReservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "recreation",
                        principalTable: "TourReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_ExpiryDate",
                schema: "recreation",
                table: "TourReservations",
                column: "ExpiryDate",
                filter: "[ExpiryDate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TenantCapacityStatus",
                schema: "recreation",
                table: "TourReservations",
                columns: new[] { "TenantId", "CapacityId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TenantMemberDate",
                schema: "recreation",
                table: "TourReservations",
                columns: new[] { "TenantId", "MemberId", "ReservationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TenantStatusExpiry",
                schema: "recreation",
                table: "TourReservations",
                columns: new[] { "TenantId", "Status", "ExpiryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TenantTourStatus",
                schema: "recreation",
                table: "TourReservations",
                columns: new[] { "TenantId", "TourId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_TourReservations_TenantTourMember",
                schema: "recreation",
                table: "TourReservations",
                columns: new[] { "TenantId", "TourId", "MemberId" },
                unique: true,
                filter: "[Status] IN (1, 2, 3)");

            migrationBuilder.CreateIndex(
                name: "UX_TourReservations_TenantTrackingCode",
                schema: "recreation",
                table: "TourReservations",
                columns: new[] { "TenantId", "TrackingCode" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TourReservations_CancellationAfterReservation",
                schema: "recreation",
                table: "TourReservations",
                sql: "[CancellationDate] IS NULL OR [CancellationDate] >= [ReservationDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TourReservations_ConfirmationAfterReservation",
                schema: "recreation",
                table: "TourReservations",
                sql: "[ConfirmationDate] IS NULL OR [ConfirmationDate] >= [ReservationDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TourReservations_ExpiryAfterReservation",
                schema: "recreation",
                table: "TourReservations",
                sql: "[ExpiryDate] IS NULL OR [ExpiryDate] > [ReservationDate]");

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacity_RemainingParticipants",
                schema: "recreation",
                table: "TourCapacities",
                column: "RemainingParticipants");

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacity_TenantRegistrationWindow",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "TenantId", "RegistrationStart", "RegistrationEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacity_TenantTour",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "TenantId", "TourId" });

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacity_TenantTourActive",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "TenantId", "TourId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "UX_TourCapacity_TenantTourPeriod",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "TenantId", "TourId", "RegistrationStart", "RegistrationEnd" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TourCapacity_MaxParticipants_Positive",
                schema: "recreation",
                table: "TourCapacities",
                sql: "[MaxParticipants] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TourCapacity_ParticipantLimits_Valid",
                schema: "recreation",
                table: "TourCapacities",
                sql: "[MinParticipantsPerReservation] > 0 AND [MaxParticipantsPerReservation] >= [MinParticipantsPerReservation]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TourCapacity_RegistrationDates_Valid",
                schema: "recreation",
                table: "TourCapacities",
                sql: "[RegistrationStart] < [RegistrationEnd]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TourCapacity_RemainingParticipants_Valid",
                schema: "recreation",
                table: "TourCapacities",
                sql: "[RemainingParticipants] >= 0 AND [RemainingParticipants] <= [MaxParticipants]");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_RegistrationDate",
                schema: "recreation",
                table: "Participants",
                column: "RegistrationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ApiIdempotency_ExpiresAt",
                schema: "recreation",
                table: "ApiIdempotency",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApiIdempotency_TenantCreatedAt",
                schema: "recreation",
                table: "ApiIdempotency",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiIdempotency_TenantProcessedCreated",
                schema: "recreation",
                table: "ApiIdempotency",
                columns: new[] { "TenantId", "IsProcessed", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiIdempotency_UserId",
                schema: "recreation",
                table: "ApiIdempotency",
                column: "UserId",
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_ApiIdempotency_TenantEndpointKey",
                schema: "recreation",
                table: "ApiIdempotency",
                columns: new[] { "TenantId", "Endpoint", "IdempotencyKey" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationPriceSnapshots_DiscountCode",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                column: "DiscountCode",
                filter: "[DiscountCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationPriceSnapshots_ReservationId",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationPriceSnapshots_SnapshotDate",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                column: "SnapshotDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationPriceSnapshots_TenantReservation",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                columns: new[] { "TenantId", "ReservationId" });

            migrationBuilder.CreateIndex(
                name: "UX_ReservationPriceSnapshots_TenantReservationParticipantType",
                schema: "recreation",
                table: "ReservationPriceSnapshots",
                columns: new[] { "TenantId", "ReservationId", "ParticipantType" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiIdempotency",
                schema: "recreation");

            migrationBuilder.DropTable(
                name: "ReservationPriceSnapshots",
                schema: "recreation");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_ExpiryDate",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_TenantCapacityStatus",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_TenantMemberDate",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_TenantStatusExpiry",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourReservations_TenantTourStatus",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "UX_TourReservations_TenantTourMember",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "UX_TourReservations_TenantTrackingCode",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TourReservations_CancellationAfterReservation",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TourReservations_ConfirmationAfterReservation",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TourReservations_ExpiryAfterReservation",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropIndex(
                name: "IX_TourCapacity_RemainingParticipants",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropIndex(
                name: "IX_TourCapacity_TenantRegistrationWindow",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropIndex(
                name: "IX_TourCapacity_TenantTour",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropIndex(
                name: "IX_TourCapacity_TenantTourActive",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropIndex(
                name: "UX_TourCapacity_TenantTourPeriod",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TourCapacity_MaxParticipants_Positive",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TourCapacity_ParticipantLimits_Valid",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TourCapacity_RegistrationDates_Valid",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TourCapacity_RemainingParticipants_Valid",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropIndex(
                name: "IX_Participants_RegistrationDate",
                schema: "recreation",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "CancellationDate",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropColumn(
                name: "PaidAmountRials",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "recreation",
                table: "TourReservations");

            migrationBuilder.DropColumn(
                name: "MaxParticipantsPerReservation",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropColumn(
                name: "MinParticipantsPerReservation",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropColumn(
                name: "RemainingParticipants",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "recreation",
                table: "TourCapacities");

            migrationBuilder.RenameIndex(
                name: "UX_Participants_ReservationNationalNumber",
                schema: "recreation",
                table: "Participants",
                newName: "IX_Participants_ReservationId_NationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_ExpiryDate",
                schema: "recreation",
                table: "TourReservations",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_MemberId",
                schema: "recreation",
                table: "TourReservations",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_Status",
                schema: "recreation",
                table: "TourReservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TourReservations_TrackingCode",
                schema: "recreation",
                table: "TourReservations",
                column: "TrackingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacities_IsActive",
                schema: "recreation",
                table: "TourCapacities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacities_RegistrationStart_RegistrationEnd",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "RegistrationStart", "RegistrationEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_TourCapacities_TourId_IsActive",
                schema: "recreation",
                table: "TourCapacities",
                columns: new[] { "TourId", "IsActive" });
        }
    }
}
