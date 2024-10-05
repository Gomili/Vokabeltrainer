using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.Contracts.Services;

public interface IDataService
{
    Task<List<Vokabel>> ReadAllAsync();

    Task<Vokabel?> ReadAsync(Guid id);
    
    Task SaveAsync(Vokabel content);

    Task DeleteAsync(Vokabel content);
}