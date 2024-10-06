using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vokabeltrainer.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "Zeit",
                table: "Sessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Zeit",
                table: "Sessions");
        }
    }
}
