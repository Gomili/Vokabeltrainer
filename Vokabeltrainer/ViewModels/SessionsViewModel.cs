using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Contracts.Services;

namespace Vokabeltrainer.ViewModels;

public partial class SessionsViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private ObservableCollection<Session> _displayListe = new();

    public int AnzahlSitzungen { get; }

    public int GeuebteVokabeln { get; }

    public int Trefferquote { get; }
    
    public SessionsViewModel(IDataService dataService, ILernspracheService lernspracheService)
    {
        _dataService = dataService;
        
        List<Session> sourceListe = _dataService
            .ReadAllSessionAsync(lernspracheService.AktuelleSprache)
            .GetAwaiter()
            .GetResult();

        AnzahlSitzungen = sourceListe.Count;
        GeuebteVokabeln = sourceListe.Sum(sitzung => sitzung.Anzahl);
        var richtigeAntworten = sourceListe.Sum(sitzung => sitzung.Richtige);

        // Warum: Bei einem leeren Verlauf muss die Quote definiert bleiben. Die
        // kaufmännische Rundung liefert zudem eine für Benutzer nachvollziehbare Ganzzahl.
        Trefferquote = GeuebteVokabeln == 0
            ? 0
            : (int)Math.Round(richtigeAntworten * 100d / GeuebteVokabeln, MidpointRounding.AwayFromZero);

        foreach (Session session in sourceListe.OrderByDescending(x => x.StartTime))
            DisplayListe.Add(session);
    }
}
