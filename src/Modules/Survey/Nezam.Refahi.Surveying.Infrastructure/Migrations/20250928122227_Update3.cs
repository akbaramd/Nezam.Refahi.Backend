using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureTitle",
                schema: "surveying",
                table: "SurveyFeatures");

            migrationBuilder.DropColumn(
                name: "CapabilityTitle",
                schema: "surveying",
                table: "SurveyCapabilities");

            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "NationalCode",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Version",
                schema: "surveying",
                table: "QuestionAnswers");

            migrationBuilder.RenameColumn(
                name: "FeatureKey",
                schema: "surveying",
                table: "SurveyFeatures",
                newName: "FeatureCode");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyFeatures_SurveyId_FeatureKey",
                schema: "surveying",
                table: "SurveyFeatures",
                newName: "IX_SurveyFeatures_SurveyId_FeatureCode");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyFeatures_FeatureKey",
                schema: "surveying",
                table: "SurveyFeatures",
                newName: "IX_SurveyFeatures_FeatureCode");

            migrationBuilder.RenameColumn(
                name: "CapabilityKey",
                schema: "surveying",
                table: "SurveyCapabilities",
                newName: "CapabilityCode");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyCapabilities_SurveyId_CapabilityKey",
                schema: "surveying",
                table: "SurveyCapabilities",
                newName: "IX_SurveyCapabilities_SurveyId_CapabilityCode");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyCapabilities_CapabilityKey",
                schema: "surveying",
                table: "SurveyCapabilities",
                newName: "IX_SurveyCapabilities_CapabilityCode");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartAt",
                schema: "surveying",
                table: "Surveys",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "EndAt",
                schema: "surveying",
                table: "Surveys",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AudienceFilterVersion",
                schema: "surveying",
                table: "Surveys",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "surveying",
                table: "Surveys",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsStructureFrozen",
                schema: "surveying",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ParticipationPolicy_AllowBackNavigation",
                schema: "surveying",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StructureVersion",
                schema: "surveying",
                table: "Surveys",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "FeatureTitleSnapshot",
                schema: "surveying",
                table: "SurveyFeatures",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CapabilityTitleSnapshot",
                schema: "surveying",
                table: "SurveyCapabilities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DemographySchemaVersion",
                schema: "surveying",
                table: "Responses",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                schema: "surveying",
                table: "Responses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SubmittedAt",
                schema: "surveying",
                table: "Responses",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_IsAnonymous",
                schema: "surveying",
                table: "Responses",
                column: "IsAnonymous");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Responses_IsAnonymous",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "AudienceFilterVersion",
                schema: "surveying",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "surveying",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "IsStructureFrozen",
                schema: "surveying",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "ParticipationPolicy_AllowBackNavigation",
                schema: "surveying",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "StructureVersion",
                schema: "surveying",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "FeatureTitleSnapshot",
                schema: "surveying",
                table: "SurveyFeatures");

            migrationBuilder.DropColumn(
                name: "CapabilityTitleSnapshot",
                schema: "surveying",
                table: "SurveyCapabilities");

            migrationBuilder.DropColumn(
                name: "DemographySchemaVersion",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                schema: "surveying",
                table: "Responses");

            migrationBuilder.RenameColumn(
                name: "FeatureCode",
                schema: "surveying",
                table: "SurveyFeatures",
                newName: "FeatureKey");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyFeatures_SurveyId_FeatureCode",
                schema: "surveying",
                table: "SurveyFeatures",
                newName: "IX_SurveyFeatures_SurveyId_FeatureKey");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyFeatures_FeatureCode",
                schema: "surveying",
                table: "SurveyFeatures",
                newName: "IX_SurveyFeatures_FeatureKey");

            migrationBuilder.RenameColumn(
                name: "CapabilityCode",
                schema: "surveying",
                table: "SurveyCapabilities",
                newName: "CapabilityKey");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyCapabilities_SurveyId_CapabilityCode",
                schema: "surveying",
                table: "SurveyCapabilities",
                newName: "IX_SurveyCapabilities_SurveyId_CapabilityKey");

            migrationBuilder.RenameIndex(
                name: "IX_SurveyCapabilities_CapabilityCode",
                schema: "surveying",
                table: "SurveyCapabilities",
                newName: "IX_SurveyCapabilities_CapabilityKey");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartAt",
                schema: "surveying",
                table: "Surveys",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndAt",
                schema: "surveying",
                table: "Surveys",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureTitle",
                schema: "surveying",
                table: "SurveyFeatures",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CapabilityTitle",
                schema: "surveying",
                table: "SurveyCapabilities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "surveying",
                table: "Responses",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalCode",
                schema: "surveying",
                table: "Responses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                schema: "surveying",
                table: "QuestionAnswers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
