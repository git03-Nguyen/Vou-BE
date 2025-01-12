using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Shared.Contracts;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "event");

            migrationBuilder.CreateTable(
                name: "CounterParts",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Field = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterParts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CounterPartId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ShakeVoucherId = table.Column<string>(type: "text", nullable: true),
                    ShakePrice = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    ShakeWinRate = table.Column<long>(type: "bigint", nullable: true),
                    ShakeAverageDiamond = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_CounterParts_CounterPartId",
                        column: x => x.CounterPartId,
                        principalSchema: "event",
                        principalTable: "CounterParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizSets",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CounterPartId = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Quizes = table.Column<List<Quiz>>(type: "jsonb", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSets_CounterParts_CounterPartId",
                        column: x => x.CounterPartId,
                        principalSchema: "event",
                        principalTable: "CounterParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CounterPartId = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vouchers_CounterParts_CounterPartId",
                        column: x => x.CounterPartId,
                        principalSchema: "event",
                        principalTable: "CounterParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteEvents",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteEvents_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "event",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteEvents_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "event",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizSessions",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    VoucherId = table.Column<string>(type: "text", nullable: false),
                    QuizSetId = table.Column<string>(type: "text", nullable: false),
                    TakeTop = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizSessions_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "event",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizSessions_QuizSets_QuizSetId",
                        column: x => x.QuizSetId,
                        principalSchema: "event",
                        principalTable: "QuizSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizSessions_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalSchema: "event",
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherToPlayers",
                schema: "event",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    VoucherId = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ExpiredDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherToPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherToPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalSchema: "event",
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherToPlayers_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalSchema: "event",
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_CounterPartId",
                schema: "event",
                table: "Events",
                column: "CounterPartId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteEvents_EventId",
                schema: "event",
                table: "FavoriteEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteEvents_PlayerId",
                schema: "event",
                table: "FavoriteEvents",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_EventId",
                schema: "event",
                table: "QuizSessions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_QuizSetId",
                schema: "event",
                table: "QuizSessions",
                column: "QuizSetId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSessions_VoucherId",
                schema: "event",
                table: "QuizSessions",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizSets_CounterPartId",
                schema: "event",
                table: "QuizSets",
                column: "CounterPartId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_CounterPartId",
                schema: "event",
                table: "Vouchers",
                column: "CounterPartId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherToPlayers_PlayerId",
                schema: "event",
                table: "VoucherToPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherToPlayers_VoucherId",
                schema: "event",
                table: "VoucherToPlayers",
                column: "VoucherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteEvents",
                schema: "event");

            migrationBuilder.DropTable(
                name: "QuizSessions",
                schema: "event");

            migrationBuilder.DropTable(
                name: "VoucherToPlayers",
                schema: "event");

            migrationBuilder.DropTable(
                name: "Events",
                schema: "event");

            migrationBuilder.DropTable(
                name: "QuizSets",
                schema: "event");

            migrationBuilder.DropTable(
                name: "Players",
                schema: "event");

            migrationBuilder.DropTable(
                name: "Vouchers",
                schema: "event");

            migrationBuilder.DropTable(
                name: "CounterParts",
                schema: "event");
        }
    }
}
