using Vokabeltrainer.Core.Contracts.Services;

namespace Vokabeltrainer.Core.Models;

public class Session : IEntity
{
    public Session()
    {
    }

    public Session(int anzahl, int richtige, int falsche, DateTime startTime, DateTime stopTime)
    {
        Anzahl = anzahl;
        Richtige = richtige;
        Falsche = falsche;
        StartTime = startTime;
        StopTime = stopTime;
        Zeit = stopTime - startTime;
    }
    
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime StopTime { get; set; }
    public TimeSpan Zeit { get; set; }
    public int Anzahl { get; set; }
    public int Richtige { get; set; }
    public int Falsche { get; set; }
}