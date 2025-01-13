using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class Minorchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Tickets",
                schema: "game",
                table: "PlayerShakeSessions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Diamond",
                schema: "game",
                table: "PlayerShakeSessions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Tickets",
                schema: "game",
                table: "PlayerShakeSessions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Diamond",
                schema: "game",
                table: "PlayerShakeSessions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
