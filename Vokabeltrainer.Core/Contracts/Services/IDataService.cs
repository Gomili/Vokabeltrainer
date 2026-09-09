using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.Contracts.Services;

public interface IDataService
{
    Task<List<Vokabel>> ReadAllVokabelAsync();

    /// <summary>
    /// Liest ausschließlich Vokabeln, die am angegebenen Tag bereits gelernt werden dürfen.
    /// </summary>
    Task<List<Vokabel>> ReadFreigegebeneVokabelnAsync(DateTime stichtag);

    Task<Vokabel?> ReadVokabelAsync(Guid id);
    
    Task<bool> SaveVokabelAsync(Vokabel content);

    Task<bool> DeleteVokabelAsync(Vokabel content);
    
    Task<List<Session>> ReadAllSessionAsync();

    Task<Session?> ReadSessionAsync(Guid id);
    
    Task<bool> SaveSessionAsync(Session content);

    Task<bool> DeleteSessionAsync(Session content);

    Task SaveFromImport(Vokabel? oldvokabel, Vokabel newvokabel);
    
    /// <summary>
    /// Reads the count of sessions that have started from a given date and time.
    /// </summary>
    /// <param name="dateTime">The starting date and time to filter the sessions.</param>
    /// <returns>A tuple containing three integers: the total number of sessions, the number of correct sessions, and the number of incorrect sessions.</returns>
    Task<(int, int, int)> ReadSessionCountAsync(DateTime dateTime);

    Task SaveIsChangedAsync(Vokabel? oldVokabel, Vokabel newvokabel);
    
    Task FixData();

    /// <summary>
    /// Ermittelt die Anzahl der Vokabeln, die für ein priorisiertes Training markiert sind.
    /// </summary>
    Task<int> GetAnzahlPriorisierterVokabelnAsync();
}
