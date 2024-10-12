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
                org.IsMarked = content.IsMarked;
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

    public async Task<List<Session>> ReadAllSessionAsync()
    {
        await using var context = new VokabelDataContext();
        return await context.Sessions.ToListAsync();
    }

    public async Task<Session?> ReadSessionAsync(Guid id)
    {
        await using var context = new VokabelDataContext();
        return await context.Sessions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> SaveSessionAsync(Session content)
    {
        await using var context = new VokabelDataContext();
        
        if (content.Id == Guid.Empty)
        {
            content.Id = Guid.NewGuid();
            context.Add(content);
        }
        else
        {
            var org = await ReadSessionAsync(content.Id);
            if (org is not null)
            {
                org.Anzahl = content.Anzahl;
                org.Falsche = content.Falsche;
                org.Richtige = content.Richtige;
                org.StopTime = content.StopTime;
                org.StartTime = content.StartTime;
                org.Zeit = content.Zeit;
                context.Update(org);
            }
        }

        int r = await context.SaveChangesAsync();
        if (r > 0) return true;
        
        return false;
    }

    public async Task<bool> DeleteSessionAsync(Session content)
    {
        await using var context = new VokabelDataContext();
        
        var org = await ReadSessionAsync(content.Id);
        
        if (org is not null)
            context.Remove(org);
        
        int r = await context.SaveChangesAsync();
        if (r > 0) return true;
        
        return false;
    }

    public async Task<(int, int, int)> ReadSessionCountAsync(DateTime dateTime)
    {
        int anzahl = 0, falsche = 0, richtige = 0;
        
        await using var context = new VokabelDataContext();
        var data = await context.Sessions.Where(x => x.StartTime >= dateTime).ToListAsync();

        foreach (Session session in data)
        {
            anzahl += session.Anzahl;
            falsche += session.Falsche;
            richtige += session.Richtige;
        }

        return (anzahl, richtige, falsche);
    }

    public async Task FixData()
    {
        await using var context = new VokabelDataContext();
        var data = await context.Sessions.ToListAsync();
        foreach (Session session in data)
        {
            if (session.Richtige + session.Falsche != session.Anzahl)
            {
                session.Anzahl = session.Richtige + session.Falsche;
                context.Update(session);
            }
        }
        await context.SaveChangesAsync();
    }
}