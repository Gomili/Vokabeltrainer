namespace Vokabeltrainer.Core.Contracts.Services;

public interface IEntity
{
    public Guid Id { get; set; }
    public DateTime Cdt { get; set; }
    public DateTime Mdt { get; set; }
}