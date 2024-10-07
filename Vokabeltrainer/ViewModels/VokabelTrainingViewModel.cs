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
    [ObservableProperty] private int _anzahlLernVokabeln = 5;
    [ObservableProperty] ObservableCollection<DisplayLernVokabel> _lernliste = [];
    [ObservableProperty] private bool _running = true;
    
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

    private async Task<(int,int)> PruefenAsync()
    {
        int richtige = 0;
        int falsche = 0;
        if (Lernliste.Count > 0)
        {
            foreach (DisplayLernVokabel lernVokabel in Lernliste)
            {
                if (lernVokabel.Englisch.Trim() == lernVokabel.Vokabel.Englisch.Trim())
                {
                    if (lernVokabel.Vokabel.Zaehler > 10)
                    {
                        lernVokabel.Vokabel.Zaehler -= 10;
                        await _dataService.SaveVokabelAsync(lernVokabel.Vokabel);
                    }
                    
                    lernVokabel.Richtig = "👍";
                    richtige++;
                }
                else
                {
                    if (lernVokabel.Vokabel.Zaehler < 100)
                    {
                        lernVokabel.Vokabel.Zaehler += 10;
                        await _dataService.SaveVokabelAsync(lernVokabel.Vokabel);
                    }
                    
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
                Running = false;
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
            Vokabel? vokabel = null;
            int x = 0;
            
            do
            {
                vokabel = WähleZufälligeVokabel(vokabelListe);
                if (vokabel is not null && !lernListe.Any(x => x.Vokabel.Id == vokabel.Id)) break;
                x++;
            } while (x < 5);
            
            if (vokabel is not null)
            {
                if (!lernListe.Any(x => x.Vokabel.Id == vokabel.Id))
                    lernListe.Add(new DisplayLernVokabel(vokabel));
            }
        }
        
        return lernListe;
    }

    private Vokabel? WähleZufälligeVokabel(List<Vokabel> vokabelListe)
    {
        Random random1 = new Random();
        Random random2 = new Random();
        
        do
        {
            int w1 = random1.Next(0, vokabelListe.Count - 1);
            Vokabel vokabel = vokabelListe[w1];
            
            int w2 = random2.Next(0, 100);
            if (w2 < vokabel.Zaehler) return vokabel;    
        } while (true);
    }

    [RelayCommand]
    private async Task StopSync()
    {
        _timer.Stop();
        (Richtige, Falsche) = await PruefenAsync();
        await _dataService.SaveSessionAsync(new Session(Anzahl, Richtige, Falsche, _startTime, DateTime.Now));
        Anzahl = AnzahlLernVokabeln;
        Falsche = 0;
        Richtige = 0;
        Laufzeit = "00:00:00";
        Running = true;
    }
    
    private void Timer_Tick(object sender, object e)
    {
        Laufzeit = (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss");
    }
}
