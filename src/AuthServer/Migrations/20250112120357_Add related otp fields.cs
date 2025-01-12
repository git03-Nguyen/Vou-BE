using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthServer.Migrations
{
    /// <inheritdoc />
    public partial class Addrelatedotpfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Addresses",
                schema: "auth",
                table: "CounterParts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "OtpActivateCode",
                schema: "auth",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpActivateExpiredTime",
                schema: "auth",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpResetPasswordCode",
                schema: "auth",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpResetPasswordExpiredTime",
                schema: "auth",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                columns: new[] { "CreatedDate", "ModifiedDate", "OtpActivateCode", "OtpActivateExpiredTime", "OtpResetPasswordCode", "OtpResetPasswordExpiredTime" },
                values: new object[] { new DateTime(2025, 1, 12, 19, 3, 55, 541, DateTimeKind.Local).AddTicks(1539), new DateTime(2025, 1, 12, 19, 3, 55, 541, DateTimeKind.Local).AddTicks(1573), null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpActivateCode",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpActivateExpiredTime",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpResetPasswordCode",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpResetPasswordExpiredTime",
                schema: "auth",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Addresses",
                schema: "auth",
                table: "CounterParts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 12, 12, 9, 52, 226, DateTimeKind.Local).AddTicks(7922), new DateTime(2025, 1, 12, 12, 9, 52, 226, DateTimeKind.Local).AddTicks(7935) });
        }
    }
}
