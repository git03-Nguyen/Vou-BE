using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    public partial class AddVoucherInventoryAndRedemption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                schema: "event",
                table: "Vouchers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RedemptionInstructions",
                schema: "event",
                table: "Vouchers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuantity",
                schema: "event",
                table: "Vouchers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherToPlayers_VoucherId_PlayerId",
                schema: "event",
                table: "VoucherToPlayers",
                columns: new[] { "VoucherId", "PlayerId" });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherToPlayers_VoucherId_UsedDate",
                schema: "event",
                table: "VoucherToPlayers",
                columns: new[] { "VoucherId", "UsedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VoucherToPlayers_VoucherId_PlayerId",
                schema: "event",
                table: "VoucherToPlayers");

            migrationBuilder.DropIndex(
                name: "IX_VoucherToPlayers_VoucherId_UsedDate",
                schema: "event",
                table: "VoucherToPlayers");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                schema: "event",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "RedemptionInstructions",
                schema: "event",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "TotalQuantity",
                schema: "event",
                table: "Vouchers");
        }
    }
}
