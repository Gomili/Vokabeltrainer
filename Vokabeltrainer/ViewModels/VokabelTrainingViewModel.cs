using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.DisplayClasses;

namespace Vokabeltrainer.ViewModels;

public partial class VokabelTrainingViewModel : ObservableRecipient, IDisposable
{
    private readonly MediaPlayer _mediaPlayer;
    private SpeechSynthesisStream? _speechStream;
    private bool _istFreigegeben;
    
    private readonly IDataService _dataService;
    private readonly IBenutzerprofilService _benutzerprofilService;
    private readonly Lernsprache _lernsprache;
    private readonly VoiceInformation? _sprachstimme;
    [ObservableProperty] private string _laufzeit = string.Empty;
    [ObservableProperty] private int _richtige = 0;
    [ObservableProperty] private int _falsche = 0;
    [ObservableProperty] private int _anzahl = 0;
    [ObservableProperty] private int _anzahlLernVokabeln = 10;
    [ObservableProperty] ObservableCollection<DisplayLernVokabel> _lernliste = [];
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TrainingsstatusText))]
    [NotifyPropertyChangedFor(nameof(TrainingsstatusHinweis))]
    private bool _running = true;
    [ObservableProperty] private int _anzahlPrioVokabeln = 0;
    [ObservableProperty] private int _gesammtRichtige = 0;
    [ObservableProperty] private int _gesammtFalsche = 0;
    [ObservableProperty] private int _gesammtAnzahl = 0;
    [ObservableProperty] private bool _nurNeueVokabeln = false;
    
    private readonly DispatcherTimer _timer = new ();
    private DateTime _startTime;
    private List<Vokabel> _vokabelListe = [];

    public Action<string> Message { get; set; } = _ => { };

    public string TrainingsstatusText => Running ? "Bereit für eine neue Runde" : "Training läuft";

    public string TrainingsstatusHinweis => Running
        ? "Stelle dein Training zusammen und beginne, wenn du bereit bist."
        : "Trage deine Übersetzungen ein und werte danach die Runde aus.";
    public string AudioTooltip => _sprachstimme is null
        ? $"Keine Stimme für {FremdsprachenBezeichnung} installiert"
        : $"{(_lernsprache == Lernsprache.Latein ? "Lateinische" : "Englische")} Lösung anhören";
    public string FremdsprachenBezeichnung => _lernsprache == Lernsprache.Latein ? "Latein" : "Englisch";
    
    public VokabelTrainingViewModel(
        IDataService dataService,
        ILernspracheService lernspracheService,
        IBenutzerprofilService benutzerprofilService)
    {
        _dataService = dataService;
        _benutzerprofilService = benutzerprofilService;
        _lernsprache = lernspracheService.AktuelleSprache;
        string sprachcode = _lernsprache == Lernsprache.Latein ? "la" : "en";
        _sprachstimme = SpeechSynthesizer.AllVoices
            .FirstOrDefault(voice => voice.Language.StartsWith(sprachcode, StringComparison.OrdinalIgnoreCase));
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick -= Timer_Tick;
        _timer.Tick += Timer_Tick;
        
        _mediaPlayer = new MediaPlayer();

        // Warum: Die Bereinigung muss vor der Trainingsabfrage abgeschlossen sein, damit
        // keine noch unvollständigen Datumswerte in die Freigabeentscheidung gelangen.
        _dataService.FixData().GetAwaiter().GetResult();
        
        _vokabelListe = _dataService.ReadFreigegebeneVokabelnAsync(DateTime.Today, _lernsprache).GetAwaiter().GetResult();
        
        (GesammtAnzahl, GesammtRichtige, GesammtFalsche) = _dataService.ReadSessionCountAsync(DateTime.Today, _lernsprache).GetAwaiter().GetResult();
    }

    private async Task<(int,int)> PruefenAsync()
    {
        int richtige = 0;
        int falsche = 0;
        if (Lernliste.Count > 0)
        {
            foreach (DisplayLernVokabel lernVokabel in Lernliste)
            {
                lernVokabel.SpeakButtonEnabled = _sprachstimme is not null;
                lernVokabel.EnglischRichtig = lernVokabel.Vokabel.Englisch;
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
                    falsche++;
                }
            }
        }

        await _benutzerprofilService.FuegeWortfunkenHinzuAsync(richtige);
        return (richtige, falsche);
    }
    
    [RelayCommand]
    private void Start()
    {
        if (AnzahlLernVokabeln > 0)
        {
            if (_vokabelListe.Count > AnzahlLernVokabeln)
            {
                Anzahl = AnzahlLernVokabeln;
                Lernliste = ErstelleLernListe(_vokabelListe, AnzahlPrioVokabeln, NurNeueVokabeln);
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

    private ObservableCollection<DisplayLernVokabel> ErstelleLernListe(List<Vokabel> vokabelListe, int anzahlprio, bool nurNeueVokabeln = false)
    {
        ObservableCollection<DisplayLernVokabel> ausgabeListe = [];
        List<Vokabel> lernListe = [];
        List<Vokabel> prioList = vokabelListe.Where(x => x.IsMarked).ToList();
        List<Vokabel> prioFoundList = [];

        if (nurNeueVokabeln)
        {
            for (int i = 0; i < AnzahlLernVokabeln; i++)
            {
                Vokabel? vokabel = WähleZufälligeVokabel(prioList, prioFoundList);
                if (vokabel != null) prioFoundList.Add(vokabel);
            }
        }
        
        for (int i = 0; i < anzahlprio; i++)
        {
            Vokabel? vokabel = WähleZufälligeVokabel(prioList, prioFoundList);
            if (vokabel != null) prioFoundList.Add(vokabel);    
        }

        for (int i = 0; i < AnzahlLernVokabeln - prioFoundList.Count; i++)
        {
            Vokabel? vokabel = null;

            vokabel = WähleZufälligeVokabel(vokabelListe, lernListe);
            if (vokabel is not null)
                lernListe.Add(vokabel);
        }

        foreach (Vokabel vokabel in prioFoundList)
        {
            InsertInList(lernListe,vokabel);
        }        
        
        foreach (Vokabel vokabel in lernListe)
        {
            var lernVokabel = new DisplayLernVokabel(vokabel, SpeakText)
            {
                SpeakButtonEnabled = _sprachstimme is not null && vokabel.Zaehler == 100
            };
            ausgabeListe.Add(lernVokabel);
        }
        
        return ausgabeListe;
    }

    private void InsertInList(List<Vokabel> list, Vokabel vokabel)
    {
        Random random = new Random();
        int insert = random.Next(list.Count);
        if (insert == list.Count) list.Add(vokabel);
        else list.Insert(insert, vokabel);
    }
    
    private Vokabel? WähleZufälligeVokabel(List<Vokabel> vokabelListe, List<Vokabel> foundListe)
    {
        Random random = new Random();
        
        if (!vokabelListe.Any()) return null;

        int x = 0;
        do
        {
            int w1 = random.Next(0, vokabelListe.Count);
            Vokabel vokabel = vokabelListe[w1];
            
            int w2 = random.Next(0, 100);
            if (w2 < vokabel.Zaehler)
            {
                if (!foundListe.Any(x => x.Id == vokabel.Id))
                    return vokabel;
            }

            x++;
        } while (x < 20);
        
        return null;
    }
    
    private void SpeakText(DisplayLernVokabel displayLernVokabel)
    {
        SpeakText(displayLernVokabel.Vokabel.Englisch);
    }
    
    private async void SpeakText(string text)
    {
        try
        {
            using var synth = new SpeechSynthesizer();

            if (_sprachstimme is null) return;
            synth.Voice = _sprachstimme;

            // Erstellen eines SpeechSynthesisStream aus dem Text
            var neuerStream = await synth.SynthesizeTextToStreamAsync(text);

            if (_istFreigegeben)
            {
                neuerStream.Dispose();
                return;
            }

            // Warum: Der MediaPlayer liest den Stream zeitversetzt. Deshalb bleibt der
            // aktuelle Stream bis zur nächsten Ausgabe bzw. bis Dispose im Besitz des ViewModels.
            _speechStream?.Dispose();
            _speechStream = neuerStream;
            _mediaPlayer.Source = MediaSource.CreateFromStream(_speechStream, _speechStream.ContentType);
            _mediaPlayer.Play();
        }
        catch (Exception ex)
        {
            // Fehlerbehandlung
            Debug.WriteLine("Fehler bei der Sprachsynthese: " + ex.Message);
        }
    }
    
    [RelayCommand]
    private async Task StopSync()
    {
        _timer.Stop();
        (Richtige, Falsche) = await PruefenAsync();
        var session = new Session(Anzahl, Richtige, Falsche, _startTime, DateTime.Now)
        {
            Sprache = _lernsprache
        };
        await _dataService.SaveSessionAsync(session);
        (GesammtAnzahl, GesammtRichtige, GesammtFalsche) = await _dataService.ReadSessionCountAsync(DateTime.Today, _lernsprache);
        Anzahl = AnzahlLernVokabeln;
        Falsche = 0;
        Richtige = 0;
        Laufzeit = "00:00:00";
        Running = true;
    }
    
    private void Timer_Tick(object? sender, object e)
    {
        Laufzeit = (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss");
    }

    public void Dispose()
    {
        if (_istFreigegeben)
        {
            return;
        }

        _istFreigegeben = true;
        _timer.Stop();
        _timer.Tick -= Timer_Tick;
        _mediaPlayer.Source = null;
        _speechStream?.Dispose();
        _mediaPlayer.Dispose();
        GC.SuppressFinalize(this);
    }
}
