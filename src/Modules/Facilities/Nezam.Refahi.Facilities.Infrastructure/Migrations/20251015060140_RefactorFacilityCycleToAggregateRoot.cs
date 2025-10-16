using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Facilities.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class RefactorFacilityCycleToAggregateRoot : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropColumn(
              name: "OverrideCooldownDays",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "OverrideExclusiveSetId",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "OverrideIsRepeatable",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "CooldownDays",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "DefaultAmountRials",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "DefaultCurrency",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "ExclusiveSetId",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "IsExclusive",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "IsRepeatable",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "MaxActiveAcrossCycles",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "MaxAmountRials",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "MaxCurrency",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "MinAmountRials",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "MinCurrency",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.RenameColumn(
              name: "OverrideMinCurrency",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "MinCurrency");

          migrationBuilder.RenameColumn(
              name: "OverrideMinAmountRials",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "MinAmountRials");

          migrationBuilder.RenameColumn(
              name: "OverrideMaxCurrency",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "MaxCurrency");

          migrationBuilder.RenameColumn(
              name: "OverrideMaxAmountRials",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "MaxAmountRials");

          migrationBuilder.RenameColumn(
              name: "OverrideMaxActiveAcrossCycles",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "MaxActiveAcrossCycles");

          migrationBuilder.AlterColumn<int>(
              name: "UsedQuota",
              schema: "facilities",
              table: "FacilityCycles",
              type: "int",
              nullable: false,
              defaultValue: 0,
              oldClrType: typeof(int),
              oldType: "int");

          migrationBuilder.AddColumn<int>(
              name: "CooldownDays",
              schema: "facilities",
              table: "FacilityCycles",
              type: "int",
              nullable: false,
              defaultValue: 0);

          migrationBuilder.AddColumn<decimal>(
              name: "DefaultAmountRials",
              schema: "facilities",
              table: "FacilityCycles",
              type: "decimal(18,2)",
              precision: 18,
              scale: 2,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "DefaultCurrency",
              schema: "facilities",
              table: "FacilityCycles",
              type: "nvarchar(3)",
              maxLength: 3,
              nullable: true,
              defaultValue: "IRR");

          migrationBuilder.AddColumn<string>(
              name: "ExclusiveSetId",
              schema: "facilities",
              table: "FacilityCycles",
              type: "nvarchar(100)",
              maxLength: 100,
              nullable: true);

          migrationBuilder.AddColumn<decimal>(
              name: "InterestRate",
              schema: "facilities",
              table: "FacilityCycles",
              type: "decimal(5,4)",
              precision: 5,
              scale: 4,
              nullable: true);

          migrationBuilder.AddColumn<bool>(
              name: "IsExclusive",
              schema: "facilities",
              table: "FacilityCycles",
              type: "bit",
              nullable: false,
              defaultValue: false);

          migrationBuilder.AddColumn<bool>(
              name: "IsRepeatable",
              schema: "facilities",
              table: "FacilityCycles",
              type: "bit",
              nullable: false,
              defaultValue: true);

          migrationBuilder.AddColumn<int>(
              name: "PaymentMonths",
              schema: "facilities",
              table: "FacilityCycles",
              type: "int",
              nullable: false,
              defaultValue: 12);

          migrationBuilder.AddColumn<string>(
              name: "BankAccountNumber",
              schema: "facilities",
              table: "Facilities",
              type: "nvarchar(50)",
              maxLength: 50,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "BankCode",
              schema: "facilities",
              table: "Facilities",
              type: "nvarchar(50)",
              maxLength: 50,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "BankName",
              schema: "facilities",
              table: "Facilities",
              type: "nvarchar(200)",
              maxLength: 200,
              nullable: true);

          migrationBuilder.CreateTable(
              name: "FacilityCycleDependencies",
              schema: "facilities",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  CycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  RequiredFacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  RequiredFacilityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                  MustBeCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                  CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                  FacilityCycleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_FacilityCycleDependencies", x => x.Id);
                  table.CheckConstraint("CK_FacilityCycleDependencies_CycleId_NotEqual_RequiredFacilityId", "CycleId != RequiredFacilityId");
                  table.ForeignKey(
                      name: "FK_FacilityCycleDependencies_FacilityCycles_CycleId",
                      column: x => x.CycleId,
                      principalSchema: "facilities",
                      principalTable: "FacilityCycles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
                  table.ForeignKey(
                      name: "FK_FacilityCycleDependencies_FacilityCycles_FacilityCycleId",
                      column: x => x.FacilityCycleId,
                      principalSchema: "facilities",
                      principalTable: "FacilityCycles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Restrict);
              });

          migrationBuilder.CreateIndex(
              name: "IX_FacilityCycleDependencies_CycleId",
              schema: "facilities",
              table: "FacilityCycleDependencies",
              column: "CycleId");

          migrationBuilder.CreateIndex(
              name: "IX_FacilityCycleDependencies_CycleId_RequiredFacilityId",
              schema: "facilities",
              table: "FacilityCycleDependencies",
              columns: new[] { "CycleId", "RequiredFacilityId" },
              unique: true);

          migrationBuilder.CreateIndex(
              name: "IX_FacilityCycleDependencies_FacilityCycleId",
              schema: "facilities",
              table: "FacilityCycleDependencies",
              column: "FacilityCycleId");

          migrationBuilder.CreateIndex(
              name: "IX_FacilityCycleDependencies_RequiredFacilityId",
              schema: "facilities",
              table: "FacilityCycleDependencies",
              column: "RequiredFacilityId");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "FacilityCycleDependencies",
              schema: "facilities");

          migrationBuilder.DropColumn(
              name: "CooldownDays",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "DefaultAmountRials",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "DefaultCurrency",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "ExclusiveSetId",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "InterestRate",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "IsExclusive",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "IsRepeatable",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "PaymentMonths",
              schema: "facilities",
              table: "FacilityCycles");

          migrationBuilder.DropColumn(
              name: "BankAccountNumber",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "BankCode",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.DropColumn(
              name: "BankName",
              schema: "facilities",
              table: "Facilities");

          migrationBuilder.RenameColumn(
              name: "MinCurrency",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "OverrideMinCurrency");

          migrationBuilder.RenameColumn(
              name: "MinAmountRials",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "OverrideMinAmountRials");

          migrationBuilder.RenameColumn(
              name: "MaxCurrency",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "OverrideMaxCurrency");

          migrationBuilder.RenameColumn(
              name: "MaxAmountRials",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "OverrideMaxAmountRials");

          migrationBuilder.RenameColumn(
              name: "MaxActiveAcrossCycles",
              schema: "facilities",
              table: "FacilityCycles",
              newName: "OverrideMaxActiveAcrossCycles");

          migrationBuilder.AlterColumn<int>(
              name: "UsedQuota",
              schema: "facilities",
              table: "FacilityCycles",
              type: "int",
              nullable: false,
              oldClrType: typeof(int),
              oldType: "int",
              oldDefaultValue: 0);

          migrationBuilder.AddColumn<int>(
              name: "OverrideCooldownDays",
              schema: "facilities",
              table: "FacilityCycles",
              type: "int",
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "OverrideExclusiveSetId",
              schema: "facilities",
              table: "FacilityCycles",
              type: "nvarchar(max)",
              nullable: true);

          migrationBuilder.AddColumn<bool>(
              name: "OverrideIsRepeatable",
              schema: "facilities",
              table: "FacilityCycles",
              type: "bit",
              nullable: true);

          migrationBuilder.AddColumn<int>(
              name: "CooldownDays",
              schema: "facilities",
              table: "Facilities",
              type: "int",
              nullable: true);

          migrationBuilder.AddColumn<decimal>(
              name: "DefaultAmountRials",
              schema: "facilities",
              table: "Facilities",
              type: "decimal(18,2)",
              precision: 18,
              scale: 2,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "DefaultCurrency",
              schema: "facilities",
              table: "Facilities",
              type: "nvarchar(3)",
              maxLength: 3,
              nullable: true,
              defaultValue: "IRR");

          migrationBuilder.AddColumn<string>(
              name: "ExclusiveSetId",
              schema: "facilities",
              table: "Facilities",
              type: "nvarchar(max)",
              nullable: true);

          migrationBuilder.AddColumn<bool>(
              name: "IsExclusive",
              schema: "facilities",
              table: "Facilities",
              type: "bit",
              nullable: false,
              defaultValue: false);

          migrationBuilder.AddColumn<bool>(
              name: "IsRepeatable",
              schema: "facilities",
              table: "Facilities",
              type: "bit",
              nullable: false,
              defaultValue: false);

          migrationBuilder.AddColumn<int>(
              name: "MaxActiveAcrossCycles",
              schema: "facilities",
              table: "Facilities",
              type: "int",
              nullable: true);

          migrationBuilder.AddColumn<decimal>(
              name: "MaxAmountRials",
              schema: "facilities",
              table: "Facilities",
              type: "decimal(18,2)",
              precision: 18,
              scale: 2,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "MaxCurrency",
              schema: "facilities",
              table: "Facilities",
              type: "nvarchar(3)",
              maxLength: 3,
              nullable: true,
              defaultValue: "IRR");

          migrationBuilder.AddColumn<decimal>(
              name: "MinAmountRials",
              schema: "facilities",
              table: "Facilities",
              type: "decimal(18,2)",
              precision: 18,
              scale: 2,
              nullable: true);

          migrationBuilder.AddColumn<string>(
              name: "MinCurrency",
              schema: "facilities",
              table: "Facilities",
              type: "nvarchar(3)",
              maxLength: 3,
              nullable: true,
              defaultValue: "IRR");
      }
  }
