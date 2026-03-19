using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePlayerEventShakeSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerShakeSessions_PlayerId",
                schema: "game",
                table: "PlayerShakeSessions");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerShakeSessions_PlayerId_EventId",
                schema: "game",
                table: "PlayerShakeSessions",
                columns: new[] { "PlayerId", "EventId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerShakeSessions_PlayerId_EventId",
                schema: "game",
                table: "PlayerShakeSessions");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerShakeSessions_PlayerId",
                schema: "game",
                table: "PlayerShakeSessions",
                column: "PlayerId");
        }
    }
}
