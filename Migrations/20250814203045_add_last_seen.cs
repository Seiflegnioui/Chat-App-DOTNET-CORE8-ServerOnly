using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P4___Websockets.Migrations
{
    /// <inheritdoc />
    public partial class add_last_seen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "seen_time",
                table: "Msgs",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "seen_time",
                table: "Msgs");
        }
    }
}
