using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Shared.Contracts;

#nullable disable

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
                name: "Players",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuizSets",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CounterPartId = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Question = table.Column<List<Quiz>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerShakeSessions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    Tickets = table.Column<long>(type: "bigint", nullable: false),
                    LastShareTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Diamond = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerShakeSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerShakeSessions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "game",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizSessions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    VoucherId = table.Column<string>(type: "text", nullable: false),
                    QuizSetId = table.Column<string>(type: "text", nullable: false),
                    TakeTop = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SingleQuizTime = table.Column<int>(type: "integer", nullable: true),
                    BreakTime = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSessions_QuizSets_QuizSetId",
                        column: x => x.QuizSetId,
                        principalSchema: "game",
                        principalTable: "QuizSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerQuizSessions",
                schema: "game",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    QuizSessionId = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    IsWin = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerQuizSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerQuizSessions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "game",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerQuizSessions_QuizSessions_QuizSessionId",
                        column: x => x.QuizSessionId,
                        principalSchema: "game",
                        principalTable: "QuizSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuizSessions_PlayerId",
                schema: "game",
                table: "PlayerQuizSessions",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuizSessions_QuizSessionId",
                schema: "game",
                table: "PlayerQuizSessions",
                column: "QuizSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerShakeSessions_PlayerId",
                schema: "game",
                table: "PlayerShakeSessions",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_QuizSetId",
                schema: "game",
                table: "QuizSessions",
                column: "QuizSetId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerQuizSessions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "PlayerShakeSessions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "QuizSessions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "game");

            migrationBuilder.DropTable(
                name: "QuizSets",
                schema: "game");
        }
    }
}
