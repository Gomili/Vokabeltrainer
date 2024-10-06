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
}