using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Shared.Contracts;

#nullable disable

namespace AuthServer.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAddresstostring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Addresses",
                schema: "auth",
                table: "CounterParts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Address[]),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 11, 19, 24, 39, 401, DateTimeKind.Local).AddTicks(8203), new DateTime(2025, 1, 11, 19, 24, 39, 401, DateTimeKind.Local).AddTicks(8217) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Address[]>(
                name: "Addresses",
                schema: "auth",
                table: "CounterParts",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 11, 19, 4, 18, 680, DateTimeKind.Local).AddTicks(4579), new DateTime(2025, 1, 11, 19, 4, 18, 680, DateTimeKind.Local).AddTicks(4594) });
        }
    }
}
