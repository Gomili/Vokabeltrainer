using Microsoft.EntityFrameworkCore;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Core.VokabelContext;

namespace Vokabeltrainer.Core.Services;

public class DataService : IDataService
{

    public DataService()
    {
        using var context = new VokabelDataContext();
        context.Database.Migrate();
    }
    
    public async Task<List<Vokabel>> ReadAllAsync()
    {
        await using var context = new VokabelDataContext();
        return await context.Vokabeln.ToListAsync();
    }

    public async Task<Vokabel?> ReadAsync(Guid id)
    {
        await using var context = new VokabelDataContext();
        return await context.Vokabeln.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task SaveAsync(Vokabel content)
    {
        await using var context = new VokabelDataContext();
        if (content.Id == Guid.Empty)
        {
            content.Id = Guid.NewGuid();
            context.Vokabeln.Add(content);
        }
        else
        {
            context.Vokabeln.Update(content);
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Vokabel content)
    {
        await using var context = new VokabelDataContext();
        context.Vokabeln.Remove(content);
        await context.SaveChangesAsync();
    }
}