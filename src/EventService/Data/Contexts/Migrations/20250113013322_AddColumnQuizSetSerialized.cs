using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnQuizSetSerialized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quizes",
                schema: "event",
                table: "QuizSets",
                newName: "QuizesSerialized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuizesSerialized",
                schema: "event",
                table: "QuizSets",
                newName: "Quizes");
        }
    }
}
