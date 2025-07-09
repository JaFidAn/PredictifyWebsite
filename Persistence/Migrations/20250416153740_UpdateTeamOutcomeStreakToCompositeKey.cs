using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeamOutcomeStreakToCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamOutcomeStreaks",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropIndex(
                name: "IX_TeamOutcomeStreaks_TeamId_OutcomeId_MatchId",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TeamOutcomeStreaks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "TeamOutcomeStreaks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamOutcomeStreaks",
                table: "TeamOutcomeStreaks",
                columns: new[] { "TeamId", "OutcomeId", "MatchId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamOutcomeStreaks",
                table: "TeamOutcomeStreaks");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TeamOutcomeStreaks",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TeamOutcomeStreaks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "TeamOutcomeStreaks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TeamOutcomeStreaks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TeamOutcomeStreaks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "TeamOutcomeStreaks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamOutcomeStreaks",
                table: "TeamOutcomeStreaks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamOutcomeStreaks_TeamId_OutcomeId_MatchId",
                table: "TeamOutcomeStreaks",
                columns: new[] { "TeamId", "OutcomeId", "MatchId" },
                unique: true);
        }
    }
}
