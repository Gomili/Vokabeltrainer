using Microsoft.EntityFrameworkCore;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.VokabelContext;

public class VokabelDataContext : DbContext
{
    public DbSet<Vokabel> Vokabeln { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Vokabel.db");
    }
}