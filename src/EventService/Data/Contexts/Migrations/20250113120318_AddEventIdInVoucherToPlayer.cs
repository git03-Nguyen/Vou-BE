using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddEventIdInVoucherToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventId",
                schema: "event",
                table: "VoucherToPlayers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventId",
                schema: "event",
                table: "VoucherToPlayers");
        }
    }
}
