using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAiForecastChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PredictedOutcomeName",
                table: "AiForecasts");

            migrationBuilder.RenameColumn(
                name: "IsCorrect",
                table: "AiForecasts",
                newName: "IsActuallyCorrect");

            migrationBuilder.RenameColumn(
                name: "Confidence",
                table: "AiForecasts",
                newName: "ProbabilityOfCorrectness");

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "AiForecasts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "AiForecasts");

            migrationBuilder.RenameColumn(
                name: "ProbabilityOfCorrectness",
                table: "AiForecasts",
                newName: "Confidence");

            migrationBuilder.RenameColumn(
                name: "IsActuallyCorrect",
                table: "AiForecasts",
                newName: "IsCorrect");

            migrationBuilder.AddColumn<string>(
                name: "PredictedOutcomeName",
                table: "AiForecasts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
