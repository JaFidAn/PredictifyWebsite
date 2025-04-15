using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxStreakAndRatioToTeamOutcomeStreak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxStreak",
                table: "TeamOutcomeStreaks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Ratio",
                table: "TeamOutcomeStreaks",
                type: "decimal(4,3)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxStreak",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "Ratio",
                table: "TeamOutcomeStreaks");
        }
    }
}
