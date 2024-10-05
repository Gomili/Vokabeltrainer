using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vokabeltrainer.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vokabeln",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Deutsch = table.Column<string>(type: "TEXT", nullable: true),
                    Englisch = table.Column<string>(type: "TEXT", nullable: true),
                    Zaehler = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vokabeln", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vokabeln");
        }
    }
}
