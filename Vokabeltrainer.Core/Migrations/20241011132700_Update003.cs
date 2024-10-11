using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vokabeltrainer.Core.Migrations
{
    /// <inheritdoc />
    public partial class Update003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMarked",
                table: "Vokabeln",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMarked",
                table: "Vokabeln");
        }
    }
}
