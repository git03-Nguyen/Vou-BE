using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthServer.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCounterPartName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "auth",
                table: "CounterParts");

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 11, 19, 4, 18, 680, DateTimeKind.Local).AddTicks(4579), new DateTime(2025, 1, 11, 19, 4, 18, 680, DateTimeKind.Local).AddTicks(4594) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "auth",
                table: "CounterParts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 1, 11, 11, 54, 2, 174, DateTimeKind.Local).AddTicks(6068), new DateTime(2025, 1, 11, 11, 54, 2, 174, DateTimeKind.Local).AddTicks(6083) });
        }
    }
}
