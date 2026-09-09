using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vokabeltrainer.Core.Migrations
{
    /// <inheritdoc />
    public partial class FreigabedatumFuerVokabeln : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Freigabedatum",
                table: "Vokabeln",
                type: "TEXT",
                nullable: false,
                defaultValue: DateTime.MinValue);

            // Warum: Vorhandene Vokabeln waren bisher sofort verfügbar. Ihr ursprünglicher
            // Eingabetag erhält dieses Verhalten und verhindert eine künstliche neue Sperrfrist.
            migrationBuilder.Sql(
                """
                UPDATE Vokabeln
                SET Freigabedatum = CASE
                    WHEN Cdt <= '0001-01-02' THEN date('now', 'localtime')
                    ELSE date(Cdt)
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Freigabedatum",
                table: "Vokabeln");
        }
    }
}
