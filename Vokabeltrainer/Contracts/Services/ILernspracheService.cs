using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Contracts.Services;

public interface ILernspracheService
{
    Lernsprache AktuelleSprache { get; }

    Task SetzeSpracheAsync(Lernsprache sprache);
}
