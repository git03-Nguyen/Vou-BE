using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Shared.Contracts;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class QuizSetQuizesToJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Quiz>>(
                name: "Quizes",
                schema: "event",
                table: "QuizSets",
                type: "json",
                nullable: false,
                oldClrType: typeof(List<Quiz>),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Quiz>>(
                name: "Quizes",
                schema: "event",
                table: "QuizSets",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(List<Quiz>),
                oldType: "json");
        }
    }
}
