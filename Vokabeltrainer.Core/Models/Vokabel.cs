using System.ComponentModel.DataAnnotations;
using Vokabeltrainer.Core.Contracts.Services;

namespace Vokabeltrainer.Core.Models;

public class Vokabel : IEntity
{
    public Guid Id { get; set; }
    public DateTime Cdt { get; set; } = DateTime.Now;
    public DateTime Mdt { get; set; } = DateTime.Now;
    public DateTime Freigabedatum { get; set; }
    [MaxLength(256)]public string Deutsch { get; set; } = "";
    [MaxLength(256)]public string Englisch { get; set; } = "";
    public Lernsprache Sprache { get; set; } = Lernsprache.Englisch;
    public int Zaehler { get; set; } = 0;
    public bool IsMarked { get; set; } = false;
}
