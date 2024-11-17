using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vokabeltrainer.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Cdt",
                table: "Vokabeln",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Mdt",
                table: "Vokabeln",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Cdt",
                table: "Sessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Mdt",
                table: "Sessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cdt",
                table: "Vokabeln");

            migrationBuilder.DropColumn(
                name: "Mdt",
                table: "Vokabeln");

            migrationBuilder.DropColumn(
                name: "Cdt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Mdt",
                table: "Sessions");
        }
    }
}
