using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWallletModelsToFinanacial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Finance");

            migrationBuilder.CreateTable(
                name: "Wallets",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NationalNumber = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WalletName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastTransactionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletDeposits",
                schema: "Finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserNationalNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletDeposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletDeposits_Bills_BillId",
                        column: x => x.BillId,
                        principalSchema: "finance",
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WalletDeposits_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "finance",
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletSnapshots",
                schema: "Finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserNationalNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SnapshotDate = table.Column<DateTime>(type: "date", nullable: false),
                    TransactionCount = table.Column<int>(type: "int", nullable: false),
                    TotalDeposits = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalWithdrawals = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetChange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastTransactionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletSnapshots_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "finance",
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WalletSnapshots_Wallets_WalletId1",
                        column: x => x.WalletId1,
                        principalSchema: "finance",
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                schema: "finance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AmountRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceAfterRials = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "finance",
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_BillId",
                schema: "Finance",
                table: "WalletDeposits",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_ExternalReference",
                schema: "Finance",
                table: "WalletDeposits",
                column: "ExternalReference");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_RequestedAt",
                schema: "Finance",
                table: "WalletDeposits",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_Status",
                schema: "Finance",
                table: "WalletDeposits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_UserNationalNumber",
                schema: "Finance",
                table: "WalletDeposits",
                column: "UserNationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_UserNationalNumber_RequestedAt",
                schema: "Finance",
                table: "WalletDeposits",
                columns: new[] { "UserNationalNumber", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_WalletId",
                schema: "Finance",
                table: "WalletDeposits",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletDeposits_WalletId_Status",
                schema: "Finance",
                table: "WalletDeposits",
                columns: new[] { "WalletId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CreatedAt",
                schema: "finance",
                table: "Wallets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CreatedBy",
                schema: "finance",
                table: "Wallets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_DeletedAt",
                schema: "finance",
                table: "Wallets",
                column: "DeletedAt",
                filter: "[DeletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_DeletedBy",
                schema: "finance",
                table: "Wallets",
                column: "DeletedBy",
                filter: "[DeletedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_IsDeleted",
                schema: "finance",
                table: "Wallets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_LastModifiedAt",
                schema: "finance",
                table: "Wallets",
                column: "LastModifiedAt",
                filter: "[LastModifiedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_LastModifiedBy",
                schema: "finance",
                table: "Wallets",
                column: "LastModifiedBy",
                filter: "[LastModifiedBy] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_LastTransactionAt",
                schema: "finance",
                table: "Wallets",
                column: "LastTransactionAt");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_NationalNumber",
                schema: "finance",
                table: "Wallets",
                column: "NationalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_NationalNumber_Status",
                schema: "finance",
                table: "Wallets",
                columns: new[] { "NationalNumber", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Status",
                schema: "finance",
                table: "Wallets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_SnapshotDate",
                schema: "Finance",
                table: "WalletSnapshots",
                column: "SnapshotDate");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_UserNationalNumber",
                schema: "Finance",
                table: "WalletSnapshots",
                column: "UserNationalNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_UserNationalNumber_SnapshotDate",
                schema: "Finance",
                table: "WalletSnapshots",
                columns: new[] { "UserNationalNumber", "SnapshotDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_WalletId",
                schema: "Finance",
                table: "WalletSnapshots",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_WalletId_SnapshotDate",
                schema: "Finance",
                table: "WalletSnapshots",
                columns: new[] { "WalletId", "SnapshotDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletSnapshots_WalletId1",
                schema: "Finance",
                table: "WalletSnapshots",
                column: "WalletId1");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_CreatedAt",
                schema: "finance",
                table: "WalletTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ExternalReference",
                schema: "finance",
                table: "WalletTransactions",
                column: "ExternalReference");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReferenceId",
                schema: "finance",
                table: "WalletTransactions",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_TransactionType",
                schema: "finance",
                table: "WalletTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_TransactionType_CreatedAt",
                schema: "finance",
                table: "WalletTransactions",
                columns: new[] { "TransactionType", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                schema: "finance",
                table: "WalletTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId_CreatedAt",
                schema: "finance",
                table: "WalletTransactions",
                columns: new[] { "WalletId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId_TransactionType",
                schema: "finance",
                table: "WalletTransactions",
                columns: new[] { "WalletId", "TransactionType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletDeposits",
                schema: "Finance");

            migrationBuilder.DropTable(
                name: "WalletSnapshots",
                schema: "Finance");

            migrationBuilder.DropTable(
                name: "WalletTransactions",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "finance");
        }
    }
}
