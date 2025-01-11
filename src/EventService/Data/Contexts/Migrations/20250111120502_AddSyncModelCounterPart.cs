using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncModelCounterPart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherInEvents_Events_EventId",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                schema: "event",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Events",
                schema: "event",
                newName: "Event",
                newSchema: "event");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Event",
                schema: "event",
                table: "Event",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherInEvents_Event_EventId",
                schema: "event",
                table: "VoucherInEvents",
                column: "EventId",
                principalSchema: "event",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherInEvents_Event_EventId",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Event",
                schema: "event",
                table: "Event");

            migrationBuilder.RenameTable(
                name: "Event",
                schema: "event",
                newName: "Events",
                newSchema: "event");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                schema: "event",
                table: "Events",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherInEvents_Events_EventId",
                schema: "event",
                table: "VoucherInEvents",
                column: "EventId",
                principalSchema: "event",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
