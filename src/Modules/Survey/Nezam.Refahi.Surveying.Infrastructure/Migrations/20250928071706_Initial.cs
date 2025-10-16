using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "surveying");

            migrationBuilder.CreateTable(
                name: "Surveys",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    State = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MaxAttemptsPerMember = table.Column<int>(type: "int", nullable: false),
                    AllowMultipleSubmissions = table.Column<bool>(type: "bit", nullable: false),
                    CoolDownSeconds = table.Column<int>(type: "int", nullable: true),
                    AudienceFilter = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
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
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalSchema: "surveying",
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParticipantHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NationalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    DemographyData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_Responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responses_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalSchema: "surveying",
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyCapabilities",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CapabilityTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyCapabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyCapabilities_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalSchema: "surveying",
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyFeatures",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FeatureTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyFeatures_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalSchema: "surveying",
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "surveying",
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswers",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextAnswer = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswers_Responses_ResponseId",
                        column: x => x.ResponseId,
                        principalSchema: "surveying",
                        principalTable: "Responses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswerOptions",
                schema: "surveying",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswerOptions_QuestionAnswers_QuestionAnswerId",
                        column: x => x.QuestionAnswerId,
                        principalSchema: "surveying",
                        principalTable: "QuestionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerOptions_OptionId",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerOptions_QuestionAnswerId",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerOptions_QuestionAnswerId_OptionId",
                schema: "surveying",
                table: "QuestionAnswerOptions",
                columns: new[] { "QuestionAnswerId", "OptionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_QuestionId",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_ResponseId",
                schema: "surveying",
                table: "QuestionAnswers",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_ResponseId_QuestionId",
                schema: "surveying",
                table: "QuestionAnswers",
                columns: new[] { "ResponseId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_IsActive",
                schema: "surveying",
                table: "QuestionOptions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                schema: "surveying",
                table: "QuestionOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId_Order",
                schema: "surveying",
                table: "QuestionOptions",
                columns: new[] { "QuestionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Kind",
                schema: "surveying",
                table: "Questions",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SurveyId",
                schema: "surveying",
                table: "Questions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SurveyId_Order",
                schema: "surveying",
                table: "Questions",
                columns: new[] { "SurveyId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Responses_CreatedAt",
                schema: "surveying",
                table: "Responses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_MemberId",
                schema: "surveying",
                table: "Responses",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_ParticipantHash",
                schema: "surveying",
                table: "Responses",
                column: "ParticipantHash");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_SurveyId",
                schema: "surveying",
                table: "Responses",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_SurveyId_AttemptNumber",
                schema: "surveying",
                table: "Responses",
                columns: new[] { "SurveyId", "AttemptNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCapabilities_CapabilityKey",
                schema: "surveying",
                table: "SurveyCapabilities",
                column: "CapabilityKey");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCapabilities_SurveyId",
                schema: "surveying",
                table: "SurveyCapabilities",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyCapabilities_SurveyId_CapabilityKey",
                schema: "surveying",
                table: "SurveyCapabilities",
                columns: new[] { "SurveyId", "CapabilityKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyFeatures_FeatureKey",
                schema: "surveying",
                table: "SurveyFeatures",
                column: "FeatureKey");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyFeatures_SurveyId",
                schema: "surveying",
                table: "SurveyFeatures",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyFeatures_SurveyId_FeatureKey",
                schema: "surveying",
                table: "SurveyFeatures",
                columns: new[] { "SurveyId", "FeatureKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_EndAt",
                schema: "surveying",
                table: "Surveys",
                column: "EndAt");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_IsAnonymous",
                schema: "surveying",
                table: "Surveys",
                column: "IsAnonymous");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_StartAt",
                schema: "surveying",
                table: "Surveys",
                column: "StartAt");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_State",
                schema: "surveying",
                table: "Surveys",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionAnswerOptions",
                schema: "surveying");

            migrationBuilder.DropTable(
                name: "QuestionOptions",
                schema: "surveying");

            migrationBuilder.DropTable(
                name: "SurveyCapabilities",
                schema: "surveying");

            migrationBuilder.DropTable(
                name: "SurveyFeatures",
                schema: "surveying");

            migrationBuilder.DropTable(
                name: "QuestionAnswers",
                schema: "surveying");

            migrationBuilder.DropTable(
                name: "Questions",
                schema: "surveying");

            migrationBuilder.DropTable(
                name: "Responses",
                schema: "surveying");

            migrationBuilder.DropTable(
                name: "Surveys",
                schema: "surveying");
        }
    }
}
