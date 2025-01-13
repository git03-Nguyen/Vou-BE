using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameService.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncEventInGameService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CounterPartId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ShakeVoucherId = table.Column<string>(type: "text", nullable: true),
                    ShakePrice = table.Column<long>(type: "bigint", nullable: true),
                    ShakeWinRate = table.Column<int>(type: "integer", nullable: true),
                    ShakeAverageDiamond = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_EventId",
                schema: "game",
                table: "QuizSessions",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizSessions_Events_EventId",
                schema: "game",
                table: "QuizSessions",
                column: "EventId",
                principalSchema: "game",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizSessions_Events_EventId",
                schema: "game",
                table: "QuizSessions");

            migrationBuilder.DropTable(
                name: "Events",
                schema: "game");

            migrationBuilder.DropIndex(
                name: "IX_QuizSessions_EventId",
                schema: "game",
                table: "QuizSessions");
        }
    }
}
