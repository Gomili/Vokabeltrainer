using Microsoft.EntityFrameworkCore;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Migrations;
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

    public async Task SaveFromImport(Vokabel? oldvokabel, Vokabel vokabel)
    {
        await using var context = new VokabelDataContext();
        
        if (oldvokabel is null)
        {
            Vokabel foundvokabel = new();
            foundvokabel.Id = vokabel.Id;
            foundvokabel.Cdt = vokabel.Cdt;
            foundvokabel.Mdt = vokabel.Mdt;
            foundvokabel.Deutsch = vokabel.Deutsch;
            foundvokabel.Englisch = vokabel.Englisch;
            foundvokabel.IsMarked = false;
            foundvokabel.Zaehler = 100;
            context.Add(foundvokabel);
        }
        else
        {
            oldvokabel.Cdt = vokabel.Cdt;
            oldvokabel.Mdt = vokabel.Mdt;
            oldvokabel.Deutsch = vokabel.Deutsch;
            oldvokabel.Englisch = vokabel.Englisch;
            context.Update(oldvokabel);
        }
        int i = await context.SaveChangesAsync();
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

    public async Task SaveIsChangedAsync(Vokabel? oldVokabel, Vokabel newvokabel)
    {
        if (oldVokabel is null) 
            await SaveFromImport(oldVokabel, newvokabel);
        else
            if (oldVokabel.Englisch != newvokabel.Englisch || oldVokabel.Deutsch != newvokabel.Deutsch || oldVokabel.Mdt <= newvokabel.Mdt)
                await SaveFromImport(oldVokabel, newvokabel);
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
        
        var vokabeln = await context.Vokabeln.ToListAsync();
        foreach (Vokabel vokabel in vokabeln)
        {
            if (vokabel.Cdt == DateTime.MinValue)
                vokabel.Cdt = DateTime.Now;
            
            if (vokabel.Mdt == DateTime.MinValue)
                vokabel.Mdt = DateTime.Now;
            
            context.Update(vokabel);
        }
        
        await context.SaveChangesAsync();
    }

    public async Task<int> GetAnzahlPriorisierterVokabelnAsync()
    {
        await using var context = new VokabelDataContext();

        // Warum: Die Datenbank kann die Markierungen zählen, ohne dafür den vollständigen
        // Wortschatz inklusive aller Texte in den Arbeitsspeicher laden zu müssen.
        return await context.Vokabeln.CountAsync(vokabel => vokabel.IsMarked);
    }
}
