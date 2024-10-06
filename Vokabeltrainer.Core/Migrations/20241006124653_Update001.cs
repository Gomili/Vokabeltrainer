using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vokabeltrainer.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StopTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Anzahl = table.Column<int>(type: "INTEGER", nullable: false),
                    Richtige = table.Column<int>(type: "INTEGER", nullable: false),
                    Falsche = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}
