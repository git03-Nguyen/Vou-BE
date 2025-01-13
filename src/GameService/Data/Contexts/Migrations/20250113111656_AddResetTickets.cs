using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddResetTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextResetTicketsTime",
                schema: "game",
                table: "PlayerShakeSessions",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextResetTicketsTime",
                schema: "game",
                table: "PlayerShakeSessions");
        }
    }
}
