using Vokabeltrainer.Core.Contracts.Services;

namespace Vokabeltrainer.Core.Models;

public class Session : IEntity
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime StopTime { get; set; }
    public int Anzahl { get; set; }
    public int Richtige { get; set; }
    public int Falsche { get; set; }
}