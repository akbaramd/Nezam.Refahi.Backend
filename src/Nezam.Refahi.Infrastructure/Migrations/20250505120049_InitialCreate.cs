using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProvinceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CityName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProvinceName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PricePerNight_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerNight_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReservationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Amount_Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Amount_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionReference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AuthorizationCode = table.Column<string>(type: "TEXT", nullable: true),
                    CaptureReference = table.Column<string>(type: "TEXT", nullable: true),
                    Gateway = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TransactionDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CompletionDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ReceiptNumber = table.Column<string>(type: "TEXT", nullable: true),
                    IsRefund = table.Column<bool>(type: "INTEGER", nullable: false),
                    RefundedTransactionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RefundReason = table.Column<string>(type: "TEXT", nullable: true),
                    RefundFee_Value = table.Column<decimal>(type: "TEXT", nullable: true),
                    RefundFee_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    OriginalAmount_Value = table.Column<decimal>(type: "TEXT", nullable: true),
                    OriginalAmount_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", nullable: true),
                    OriginalCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    AuthorizedAmount_Value = table.Column<decimal>(type: "TEXT", nullable: true),
                    AuthorizedAmount_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    CapturedAmount_Value = table.Column<decimal>(type: "TEXT", nullable: true),
                    CapturedAmount_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    DisputeStatus = table.Column<string>(type: "TEXT", nullable: true),
                    DisputeReason = table.Column<string>(type: "TEXT", nullable: true),
                    DisputeDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    DisputeResolutionDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    InstallmentPlanId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InstallmentNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    TotalInstallments = table.Column<int>(type: "INTEGER", nullable: true),
                    PreviousAttemptId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AttemptNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NationalIdValue = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotelFeature",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    HotelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelFeature_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AltText = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsMainPhoto = table.Column<bool>(type: "INTEGER", nullable: false),
                    HotelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelPhoto_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", nullable: true),
                    ProvinceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    ReservationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HotelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PrimaryGuestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CheckInDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    CheckOutDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TotalPrice_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice_Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    SpecialRequests = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LastPaymentTransactionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastPaymentDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LockExpirationTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Guest_PrimaryGuestId",
                        column: x => x.PrimaryGuestId,
                        principalTable: "Guest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Guest_ReservationId",
                table: "Guest",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelFeature_HotelId_Name",
                table: "HotelFeature",
                columns: new[] { "HotelId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelPhoto_HotelId_IsMainPhoto",
                table: "HotelPhoto",
                columns: new[] { "HotelId", "IsMainPhoto" },
                unique: true,
                filter: "\"IsMainPhoto\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_HotelPhoto_Url",
                table: "HotelPhoto",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_IsAvailable",
                table: "Hotels",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_Name",
                table: "Hotels",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_CustomerId",
                table: "PaymentTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_ReservationId",
                table: "PaymentTransactions",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Status",
                table: "PaymentTransactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_TransactionReference",
                table: "PaymentTransactions",
                column: "TransactionReference");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_HotelId_Status",
                table: "Reservations",
                columns: new[] { "HotelId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_LastPaymentTransactionId",
                table: "Reservations",
                column: "LastPaymentTransactionId",
                filter: "[LastPaymentTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PrimaryGuestId",
                table: "Reservations",
                column: "PrimaryGuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Status",
                table: "Reservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NationalIdValue",
                table: "Users",
                column: "NationalIdValue");

            migrationBuilder.AddForeignKey(
                name: "FK_Guest_Reservations_ReservationId",
                table: "Guest",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guest_Reservations_ReservationId",
                table: "Guest");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "HotelFeature");

            migrationBuilder.DropTable(
                name: "HotelPhoto");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Guest");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
