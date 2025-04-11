using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_TeamOutcomeStreak_WithMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamOutcomeStreaks_TeamId_OutcomeId",
                table: "TeamOutcomeStreaks");

            migrationBuilder.AddColumn<DateTime>(
                name: "MatchDate",
                table: "TeamOutcomeStreaks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "TeamOutcomeStreaks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeamOutcomeStreaks_MatchId",
                table: "TeamOutcomeStreaks",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamOutcomeStreaks_TeamId_OutcomeId_MatchId",
                table: "TeamOutcomeStreaks",
                columns: new[] { "TeamId", "OutcomeId", "MatchId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamOutcomeStreaks_Matches_MatchId",
                table: "TeamOutcomeStreaks",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamOutcomeStreaks_Matches_MatchId",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropIndex(
                name: "IX_TeamOutcomeStreaks_MatchId",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropIndex(
                name: "IX_TeamOutcomeStreaks_TeamId_OutcomeId_MatchId",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "MatchDate",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "TeamOutcomeStreaks");

            migrationBuilder.CreateIndex(
                name: "IX_TeamOutcomeStreaks_TeamId_OutcomeId",
                table: "TeamOutcomeStreaks",
                columns: new[] { "TeamId", "OutcomeId" },
                unique: true);
        }
    }
}
