using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.Contracts.Services;

public interface IDataService
{
    Task<List<Vokabel>> ReadAllAsync();

    Task<Vokabel?> ReadAsync(Guid id);
    
    Task<bool> SaveAsync(Vokabel content);

    Task<bool> DeleteAsync(Vokabel content);
}