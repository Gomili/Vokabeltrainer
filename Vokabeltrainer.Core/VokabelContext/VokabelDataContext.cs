using Microsoft.EntityFrameworkCore;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.VokabelContext;

public class VokabelDataContext : DbContext
{
    public DbSet<Vokabel> Vokabeln { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string filePath = Path.Combine(documentsPath, "Vokabeln.db");
        optionsBuilder.UseSqlite($"Data Source={filePath}");
    }
}