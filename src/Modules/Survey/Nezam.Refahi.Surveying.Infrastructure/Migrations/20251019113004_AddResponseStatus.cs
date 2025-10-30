using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nezam.Refahi.Surveys.Infrastructure.Migrations;

  /// <inheritdoc />
  public partial class AddResponseStatus : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.AddColumn<string>(
              name: "Status",
              schema: "surveying",
              table: "Responses",
              type: "nvarchar(20)",
              maxLength: 20,
              nullable: false,
              defaultValue: "Answering");

          // Update existing records to have the correct status based on their attempt status
          migrationBuilder.Sql(@"
                UPDATE [surveying].[Responses] 
                SET [Status] = CASE 
                    WHEN [AttemptStatus] = 'Active' THEN 'Answering'
                    WHEN [AttemptStatus] = 'Submitted' THEN 'Completed'
                    WHEN [AttemptStatus] = 'Canceled' THEN 'Cancelled'
                    WHEN [AttemptStatus] = 'Expired' THEN 'Expired'
                    ELSE 'Answering'
                END");

          migrationBuilder.CreateIndex(
              name: "IX_Responses_Status",
              schema: "surveying",
              table: "Responses",
              column: "Status");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropIndex(
              name: "IX_Responses_Status",
              schema: "surveying",
              table: "Responses");

          migrationBuilder.DropColumn(
              name: "Status",
              schema: "surveying",
              table: "Responses");
      }
  }
