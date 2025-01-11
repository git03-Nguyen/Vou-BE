using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherInEvents_Events_Id",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherInEvents_Vouchers_Id",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherToPlayers_Vouchers_Id",
                schema: "event",
                table: "VoucherToPlayers");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherToPlayers_VoucherId",
                schema: "event",
                table: "VoucherToPlayers",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherInEvents_EventId",
                schema: "event",
                table: "VoucherInEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherInEvents_VoucherId",
                schema: "event",
                table: "VoucherInEvents",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherInEvents_Events_EventId",
                schema: "event",
                table: "VoucherInEvents",
                column: "EventId",
                principalSchema: "event",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherInEvents_Vouchers_VoucherId",
                schema: "event",
                table: "VoucherInEvents",
                column: "VoucherId",
                principalSchema: "event",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherToPlayers_Vouchers_VoucherId",
                schema: "event",
                table: "VoucherToPlayers",
                column: "VoucherId",
                principalSchema: "event",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherInEvents_Events_EventId",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherInEvents_Vouchers_VoucherId",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherToPlayers_Vouchers_VoucherId",
                schema: "event",
                table: "VoucherToPlayers");

            migrationBuilder.DropIndex(
                name: "IX_VoucherToPlayers_VoucherId",
                schema: "event",
                table: "VoucherToPlayers");

            migrationBuilder.DropIndex(
                name: "IX_VoucherInEvents_EventId",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.DropIndex(
                name: "IX_VoucherInEvents_VoucherId",
                schema: "event",
                table: "VoucherInEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherInEvents_Events_Id",
                schema: "event",
                table: "VoucherInEvents",
                column: "Id",
                principalSchema: "event",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherInEvents_Vouchers_Id",
                schema: "event",
                table: "VoucherInEvents",
                column: "Id",
                principalSchema: "event",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherToPlayers_Vouchers_Id",
                schema: "event",
                table: "VoucherToPlayers",
                column: "Id",
                principalSchema: "event",
                principalTable: "Vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
