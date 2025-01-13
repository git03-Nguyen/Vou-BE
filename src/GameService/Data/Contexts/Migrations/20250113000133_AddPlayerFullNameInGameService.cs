using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerFullNameInGameService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                schema: "game",
                table: "Players",
                newName: "AvatarUrl");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "game",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "game",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                schema: "game",
                table: "Players",
                newName: "ImageUrl");
        }
    }
}
