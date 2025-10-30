using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Finance.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class AddDiscountCodeEntities : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.AddColumn<decimal>(
              name: "AppliedDiscountAmountRials",
              schema: "finance",
              table: "Payments",
              type: "decimal(18,2)",
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "AppliedDiscountCode",
              schema: "finance",
              table: "Payments",
              type: "nvarchar(50)",
              maxLength: 50,
              nullable: true);

          migrationBuilder.AddColumn<Guid>(
              name: "AppliedDiscountCodeId",
              schema: "finance",
              table: "Payments",
              type: "uniqueidentifier",
              nullable: true);

          migrationBuilder.AddColumn<bool>(
              name: "IsFreePayment",
              schema: "finance",
              table: "Payments",
              type: "bit",
              nullable: false,
              defaultValue: false);

          migrationBuilder.AddColumn<decimal>(
              name: "DiscountAmountRials",
              schema: "finance",
              table: "Bills",
              type: "decimal(18,2)",
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "DiscountCode",
              schema: "finance",
              table: "Bills",
              type: "nvarchar(50)",
              maxLength: 50,
              nullable: true);

          migrationBuilder.AddColumn<Guid>(
              name: "DiscountCodeId",
              schema: "finance",
              table: "Bills",
              type: "uniqueidentifier",
              nullable: true);

          migrationBuilder.CreateTable(
              name: "DiscountCodes",
              schema: "finance",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "کد تخفیف"),
                  Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "عنوان کد تخفیف"),
                  Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "توضیحات کد تخفیف"),
                  Type = table.Column<int>(type: "int", nullable: false, comment: "نوع تخفیف (درصدی یا مبلغی)"),
                  DiscountValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "مقدار تخفیف"),
                  MaximumDiscountAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, comment: "حداکثر مبلغ تخفیف"),
                  MinimumBillAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true, comment: "حداقل مبلغ فاکتور"),
                  Status = table.Column<int>(type: "int", nullable: false, comment: "وضعیت کد تخفیف"),
                  ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "تاریخ شروع اعتبار"),
                  ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "تاریخ پایان اعتبار"),
                  UsageLimit = table.Column<int>(type: "int", nullable: true, comment: "حد مجاز استفاده"),
                  UsedCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "تعداد استفاده شده"),
                  IsSingleUse = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "آیا یکبار مصرف است"),
                  IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "آیا فعال است"),
                  CreatedByExternalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "شناسه کاربر ایجادکننده"),
                  CreatedByUserFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "نام کامل کاربر ایجادکننده"),
                  Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "اطلاعات اضافی"),
                  Version = table.Column<long>(type: "bigint", nullable: false),
                  RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                  CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "تاریخ ایجاد"),
                  CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "ایجادکننده"),
                  LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "تاریخ آخرین تغییر"),
                  LastModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "آخرین تغییردهنده"),
                  IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "آیا حذف شده"),
                  DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "تاریخ حذف"),
                  DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "حذف‌کننده")
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_DiscountCodes", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "DiscountCodeUsages",
              schema: "finance",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  DiscountCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "شناسه کد تخفیف"),
                  BillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "شناسه فاکتور"),
                  ExternalUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "شناسه کاربر"),
                  UserFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "نام کامل کاربر"),
                  BillAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "مبلغ فاکتور"),
                  DiscountAmountRials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, comment: "مبلغ تخفیف"),
                  UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "تاریخ استفاده"),
                  Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "یادداشت‌ها"),
                  Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "اطلاعات اضافی"),
                  DiscountCodeId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_DiscountCodeUsages", x => x.Id);
                  table.ForeignKey(
                      name: "FK_DiscountCodeUsages_DiscountCodes_DiscountCodeId",
                      column: x => x.DiscountCodeId,
                      principalSchema: "finance",
                      principalTable: "DiscountCodes",
                      principalColumn: "Id");
                  table.ForeignKey(
                      name: "FK_DiscountCodeUsages_DiscountCodes_DiscountCodeId1",
                      column: x => x.DiscountCodeId1,
                      principalSchema: "finance",
                      principalTable: "DiscountCodes",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodes_Code",
              schema: "finance",
              table: "DiscountCodes",
              column: "Code",
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodes_Status",
              schema: "finance",
              table: "DiscountCodes",
              column: "Status");

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodes_Status_ValidDates",
              schema: "finance",
              table: "DiscountCodes",
              columns: new[] { "Status", "ValidFrom", "ValidTo" });

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodes_ValidFrom",
              schema: "finance",
              table: "DiscountCodes",
              column: "ValidFrom");

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodes_ValidTo",
              schema: "finance",
              table: "DiscountCodes",
              column: "ValidTo");

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodeUsages_BillId",
              schema: "finance",
              table: "DiscountCodeUsages",
              column: "BillId");

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodeUsages_DiscountCodeId",
              schema: "finance",
              table: "DiscountCodeUsages",
              column: "DiscountCodeId");

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodeUsages_DiscountCodeId_BillId",
              schema: "finance",
              table: "DiscountCodeUsages",
              columns: new[] { "DiscountCodeId", "BillId" });

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodeUsages_DiscountCodeId_ExternalUserId",
              schema: "finance",
              table: "DiscountCodeUsages",
              columns: new[] { "DiscountCodeId", "ExternalUserId" });

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodeUsages_DiscountCodeId1",
              schema: "finance",
              table: "DiscountCodeUsages",
              column: "DiscountCodeId1");

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodeUsages_ExternalUserId",
              schema: "finance",
              table: "DiscountCodeUsages",
              column: "ExternalUserId");

          migrationBuilder.CreateIndex(
              name: "IX_DiscountCodeUsages_UsedAt",
              schema: "finance",
              table: "DiscountCodeUsages",
              column: "UsedAt");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "DiscountCodeUsages",
              schema: "finance");

          migrationBuilder.DropTable(
              name: "DiscountCodes",
              schema: "finance");

          migrationBuilder.DropColumn(
              name: "AppliedDiscountAmountRials",
              schema: "finance",
              table: "Payments");

          migrationBuilder.DropColumn(
              name: "AppliedDiscountCode",
              schema: "finance",
              table: "Payments");

          migrationBuilder.DropColumn(
              name: "AppliedDiscountCodeId",
              schema: "finance",
              table: "Payments");

          migrationBuilder.DropColumn(
              name: "IsFreePayment",
              schema: "finance",
              table: "Payments");

          migrationBuilder.DropColumn(
              name: "DiscountAmountRials",
              schema: "finance",
              table: "Bills");

          migrationBuilder.DropColumn(
              name: "DiscountCode",
              schema: "finance",
              table: "Bills");

          migrationBuilder.DropColumn(
              name: "DiscountCodeId",
              schema: "finance",
              table: "Bills");
      }
  }
