namespace Vokabeltrainer.Core.Models;

public class Vokabel
{
    public Guid Id { get; set; }
    public string Deutsch { get; set; }
    public string Englisch { get; set; }
    public int Zaehler { get; set; }
}