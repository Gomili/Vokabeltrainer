using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.Contracts.Services;

public interface IDataService
{
    Task<List<Vokabel>> ReadAllVokabelAsync();

    Task<Vokabel?> ReadVokabelAsync(Guid id);
    
    Task<bool> SaveVokabelAsync(Vokabel content);

    Task<bool> DeleteVokabelAsync(Vokabel content);
    
    Task<List<Session>> ReadAllSessionAsync();

    Task<Session?> ReadSessionAsync(Guid id);
    
    Task<bool> SaveSessionAsync(Session content);

    Task<bool> DeleteSessionAsync(Session content);

    /// <summary>
    /// Reads the count of sessions that have started from a given date and time.
    /// </summary>
    /// <param name="dateTime">The starting date and time to filter the sessions.</param>
    /// <returns>A tuple containing three integers: the total number of sessions, the number of correct sessions, and the number of incorrect sessions.</returns>
    Task<(int, int, int)> ReadSessionCountAsync(DateTime dateTime);

    Task FixData();
}