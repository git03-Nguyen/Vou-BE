using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Shared.Contracts;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "game");

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    GameId = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ShakeIntensity = table.Column<int>(type: "integer", nullable: true),
                    TotalQuiz = table.Column<int>(type: "integer", nullable: true),
                    SingleQuizTime = table.Column<int>(type: "integer", nullable: true),
                    BreakTime = table.Column<int>(type: "integer", nullable: true),
                    Questions = table.Column<Question[]>(type: "jsonb", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "game",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherInGameSessions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    GameSessionId = table.Column<string>(type: "text", nullable: false),
                    VoucherId = table.Column<string>(type: "text", nullable: false),
                    Possibility = table.Column<int>(type: "integer", nullable: true),
                    TopFrom = table.Column<int>(type: "integer", nullable: true),
                    TopTo = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherInGameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherInGameSessions_GameSessions_GameSessionId",
                        column: x => x.GameSessionId,
                        principalSchema: "game",
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "Games",
                columns: new[] { "Id", "Author", "CreatedBy", "CreatedDate", "DeletedDate", "Description", "ImageUrl", "IsDeleted", "ModifiedDate", "Name", "Type" },
                values: new object[,]
                {
                    { "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "TuanDat-ThienAn", "SYSTEM", new DateTime(2025, 1, 11, 11, 57, 15, 264, DateTimeKind.Local).AddTicks(2482), null, "This is shake game", "https://storageaccwct.blob.core.windows.net/wct-blobstorage/UserDefaultAvatar.png", false, new DateTime(2025, 1, 11, 11, 57, 15, 264, DateTimeKind.Local).AddTicks(2502), "Shake Game", 1 },
                    { "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "TuanDat-ThienAn", "SYSTEM", new DateTime(2025, 1, 11, 11, 57, 15, 264, DateTimeKind.Local).AddTicks(2510), null, "This is quiz game", "https://storageaccwct.blob.core.windows.net/wct-blobstorage/UserDefaultAvatar.png", false, new DateTime(2025, 1, 11, 11, 57, 15, 264, DateTimeKind.Local).AddTicks(2511), "Quiz Game", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GameId",
                schema: "game",
                table: "GameSessions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherInGameSessions_GameSessionId",
                schema: "game",
                table: "VoucherInGameSessions",
                column: "GameSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherInGameSessions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "GameSessions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "game");
        }
    }
}
