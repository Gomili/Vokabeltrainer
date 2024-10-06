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
    
    public async Task<List<Vokabel>> ReadAllVokabelAsync()
    {
        await using var context = new VokabelDataContext();
        return await context.Vokabeln.ToListAsync();
    }

    public async Task<Vokabel?> ReadVokabelAsync(Guid id)
    {
        await using var context = new VokabelDataContext();
        return await context.Vokabeln.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> SaveVokabelAsync(Vokabel content)
    {
        await using var context = new VokabelDataContext();
        
        if (content.Id == Guid.Empty)
        {
            content.Id = Guid.NewGuid();
            context.Add(content);
        }
        else
        {
            var org = await ReadVokabelAsync(content.Id);
            if (org is not null)
            {
                org.Deutsch = content.Deutsch;
                org.Englisch = content.Englisch;
                org.Zaehler = content.Zaehler;
                context.Update(org);
            }
        }

        int r = await context.SaveChangesAsync();
        if (r > 0) return true;
        
        return false;
    }

    public async Task<bool> DeleteVokabelAsync(Vokabel content)
    {
        await using var context = new VokabelDataContext();
        
        var org = await ReadVokabelAsync(content.Id);
        
        if (org is not null)
            context.Remove(org);
        
        int r = await context.SaveChangesAsync();
        if (r > 0) return true;
        
        return false;
    }

    public Task<List<Session>> ReadAllSessionAsync() => throw new NotImplementedException();

    public Task<Session> ReadSessionAsync(Guid id) => throw new NotImplementedException();

    public Task<bool> SaveSessionAsync(Session content) => throw new NotImplementedException();

    public Task<bool> DeleteSessionAsync(Session content) => throw new NotImplementedException();
}