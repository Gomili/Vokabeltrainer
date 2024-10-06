using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.DisplayClasses;

namespace Vokabeltrainer.ViewModels;

public partial class VokabelTrainingViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private string _laufzeit = string.Empty;
    [ObservableProperty] private int _richtige = 0;
    [ObservableProperty] private int _falsche = 0;
    [ObservableProperty] private int _anzahl = 0;
    [ObservableProperty] private int _anzahlLernVokabeln = 0;
    [ObservableProperty] ObservableCollection<DisplayLernVokabel> _lernliste = [];
    
    private readonly DispatcherTimer _timer = new ();
    private DateTime _startTime;
    private List<Vokabel> _vokabelListe = [];

    public Action<string> Message { get; set; }
    
    public VokabelTrainingViewModel(IDataService dataService)
    {
        _dataService = dataService;
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick -= Timer_Tick;
        _timer.Tick += Timer_Tick;
        
        _vokabelListe = _dataService.ReadAllVokabelAsync().GetAwaiter().GetResult();
    }

    private (int,int) Pruefen()
    {
        int richtige = 0;
        int falsche = 0;
        if (Lernliste.Count > 0)
        {
            foreach (DisplayLernVokabel lernVokabel in Lernliste)
            {
                if (lernVokabel.Englisch.Trim() == lernVokabel.Vokabel.Englisch.Trim())
                {
                    lernVokabel.Richtig = "👍";
                    richtige++;
                }
                else
                {
                    lernVokabel.Richtig = "👎";
                    lernVokabel.EnglischRichtig = lernVokabel.Vokabel.Englisch;
                    falsche++;
                }
            }
        }
        return (richtige, falsche);
    }
    
    [RelayCommand]
    private void Start()
    {
        if (AnzahlLernVokabeln > 0)
        {
            if (_vokabelListe.Count > AnzahlLernVokabeln)
            {
                Lernliste = ErstelleLernListe(_vokabelListe);
                _timer.Start();
                _startTime = DateTime.Now;
            }
            else
            {
                Message("Für die eingestellte Anzahl sind zu wenig Vokabeln gespeichert!");
            }
        }
    }

    private ObservableCollection<DisplayLernVokabel> ErstelleLernListe(List<Vokabel> vokabelListe)
    {
        ObservableCollection<DisplayLernVokabel> lernListe = [];

        for (int i = 0; i < AnzahlLernVokabeln; i++)
        {
            Vokabel? vokabel = WähleZufälligeVokabel(vokabelListe);
            if (vokabel != null)
                lernListe.Add(new DisplayLernVokabel(vokabel));
        }
        
        return lernListe;
    }

    private Vokabel? WähleZufälligeVokabel(List<Vokabel> vokabelListe)
    {
        Random random1 = new Random();
        Random random2 = new Random();
        int maxLoop = 10;
        do
        {
            maxLoop--;
            foreach (Vokabel vokabel in vokabelListe)
            {
                int w1 = random1.Next(0, 100);
                if (w1 > 20) continue;
                
                int w2 = random2.Next(0, 100);
                if (w2 < vokabel.Zaehler) return vokabel;
            }
        } while (maxLoop > 0);
        
        return null;
    }

    [RelayCommand]
    private void Stop()
    {
        _timer.Stop();
        (Richtige, Falsche) = Pruefen();
        _dataService.SaveSessionAsync(new Session(Anzahl, Richtige, Falsche, _startTime, DateTime.Now));
        Anzahl = AnzahlLernVokabeln;
        Falsche = 0;
        Richtige = 0;
        Laufzeit = "00:00:00";
    }
    
    private void Timer_Tick(object sender, object e)
    {
        Laufzeit = (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss");
    }
}
