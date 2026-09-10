namespace Vokabeltrainer.Contracts.Services;

public interface IBenutzerprofilService
{
    event Action? ProfilGeaendert;

    string Name { get; }

    int Wortfunken { get; }

    bool BelohneNeueVokabeln { get; }

    Task InitializeAsync();

    Task SetzeNameAsync(string name);

    Task SetzeBelohnungFuerNeueVokabelnAsync(bool aktiviert);

    Task FuegeWortfunkenHinzuAsync(int anzahl = 1);

    Task SetzeWortfunkenZurueckAsync();
}
