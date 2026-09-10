using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Contracts.Services;

public interface ILernspracheService
{
    event Action<Lernsprache>? SpracheGeaendert;

    Lernsprache AktuelleSprache { get; }

    Task InitializeAsync();

    Task SetzeSpracheAsync(Lernsprache sprache);
}
