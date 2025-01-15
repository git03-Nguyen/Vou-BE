﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventService.Data.Contexts.Migrations
{
    /// <inheritdoc />
    public partial class AddHasNotifiedInFavoriteEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasNotified",
                schema: "event",
                table: "FavoriteEvents",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasNotified",
                schema: "event",
                table: "FavoriteEvents");
        }
    }
}
