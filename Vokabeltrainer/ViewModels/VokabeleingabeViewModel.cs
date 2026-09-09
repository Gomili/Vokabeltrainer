using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.DisplayClasses;
using Vokabeltrainer.Helpers;

namespace Vokabeltrainer.ViewModels;

public partial class VokabeleingabeViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private string _textDeutsch = string.Empty;
    [ObservableProperty] private string _textEnglisch = string.Empty;
    [ObservableProperty] private ObservableCollection<Vokabel> _displayListe = new();
    public ObservableCollection<VokabelTagesgruppe> TagesGruppen { get; } = new();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HatAusgewaehlteVokabel))]
    private Vokabel? _selectedVokabel;
    [ObservableProperty] private int _anzahlVokabeln;
    [ObservableProperty] private int _anzahlPriorisierteVokabeln;
    [ObservableProperty] private string _suchText = string.Empty;

    public bool HatAusgewaehlteVokabel => SelectedVokabel is not null;

    public VokabeleingabeViewModel(IDataService dataService)
    {
        _dataService = dataService;
        
        List<Vokabel> sourceListe = _dataService.ReadAllVokabelAsync().GetAwaiter().GetResult();

        AnzahlVokabeln = sourceListe.Count;
        AnzahlPriorisierteVokabeln = _dataService.GetAnzahlPriorisierterVokabelnAsync().GetAwaiter().GetResult();
        
        foreach (Vokabel vokabel in sourceListe)
            DisplayListe.Add(vokabel);

        AktualisiereTagesgruppen();
    }

    [RelayCommand]
    private Task SelectionChangedAsync(Vokabel? selectedVokabel)
    {
        if (selectedVokabel != null)
        {
            TextDeutsch = selectedVokabel.Deutsch;
            TextEnglisch = selectedVokabel.Englisch;
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SwitchMarkedAsync(Vokabel? vokabel)
    {
        if (vokabel is not null)
        {
            // Warum: Die CheckBox zeigt den Zustand nur an. Das ViewModel übernimmt die Änderung,
            // damit recycelte Listenelemente keinen ungeprüften UI-Standardwert ins Fachmodell schreiben.
            vokabel.IsMarked = !vokabel.IsMarked;
            if (await _dataService.SaveVokabelAsync(vokabel))
            {
                AnzahlPriorisierteVokabeln = await _dataService.GetAnzahlPriorisierterVokabelnAsync();
            }
        }
    }
    
    [RelayCommand]
    private async Task StartSucheAsync(string text)
    {
        List<Vokabel> searchResults = await _dataService.ReadAllVokabelAsync();
        DisplayListe.Clear();
        
        if (!string.IsNullOrWhiteSpace(text))
        {
            foreach (Vokabel vokabel in searchResults.Where(x => x.Deutsch.ToLower().Contains(text.ToLower()) || x.Englisch.ToLower().Contains(text.ToLower())))
            {
                DisplayListe.Add(vokabel);
            }    
        }
        else
        {
            foreach (Vokabel vokabel in searchResults)
            {
                DisplayListe.Add(vokabel);
            }    
        }

        AktualisiereTagesgruppen();
    }

    partial void OnSuchTextChanged(string value)
    {
        // Warum: Die Suche hängt am ViewModel statt an einem XAML-Event. Dadurch ist
        // dasselbe Verhalten ohne gerenderte Oberfläche automatisiert prüfbar.
        StartSucheCommand.Execute(value);
    }
    
    public async Task AddNewVokabelAsync(string deutsch, string englisch)
    {
        if (string.IsNullOrWhiteSpace(deutsch) || string.IsNullOrWhiteSpace(englisch)) return;
        
        Vokabel vokabel = new Vokabel { Deutsch = deutsch, Englisch = englisch, Zaehler = 100, IsMarked = true };
        if (await _dataService.SaveVokabelAsync(vokabel))
        {
            TextDeutsch = string.Empty;
            TextEnglisch = string.Empty;
            
            ListHelper.AddListEntry(DisplayListe, vokabel);
            AnzahlVokabeln++;
            AnzahlPriorisierteVokabeln = await _dataService.GetAnzahlPriorisierterVokabelnAsync();
            AktualisiereTagesgruppen();
        }
    }
    
    [RelayCommand]
    private async Task VokabelSpeichernAsync()
    {
        if (SelectedVokabel is null) return;
        SelectedVokabel.Deutsch = TextDeutsch;
        SelectedVokabel.Englisch = TextEnglisch;
        if (await _dataService.SaveVokabelAsync(SelectedVokabel))
        {
            ListHelper.UpdateListEntry(DisplayListe, SelectedVokabel);
            AktualisiereTagesgruppen();
        }
    }

    [RelayCommand]
    private async Task VokabelLoeschenAsync()
    {
        if (SelectedVokabel is null) return;
        if (await _dataService.DeleteVokabelAsync(SelectedVokabel))
        {
            ListHelper.DeleteListEntry(DisplayListe, SelectedVokabel);
            AnzahlVokabeln--;
            AnzahlPriorisierteVokabeln = await _dataService.GetAnzahlPriorisierterVokabelnAsync();
            SelectedVokabel = null;
            TextDeutsch = string.Empty;
            TextEnglisch = string.Empty;
            AktualisiereTagesgruppen();
        }
    }

    private void AktualisiereTagesgruppen()
    {
        HashSet<DateTime> zuvorAufgeklappteTage = TagesGruppen
            .Where(gruppe => gruppe.IstAufgeklappt)
            .Select(gruppe => gruppe.Lerntag)
            .ToHashSet();
        HashSet<DateTime> zuvorBekannteTage = TagesGruppen
            .Select(gruppe => gruppe.Lerntag)
            .ToHashSet();

        var gruppen = DisplayListe
            .GroupBy(vokabel => vokabel.Cdt.Date)
            .OrderByDescending(gruppe => gruppe.Key)
            .ToList();

        TagesGruppen.Clear();

        for (int index = 0; index < gruppen.Count; index++)
        {
            var gruppe = gruppen[index];

            // Warum: Beim ersten Laden soll die aktuelle Lektion sofort sichtbar sein. Spätere
            // Aktualisierungen bewahren die Auswahl des Benutzers und öffnen einen neu hinzugekommenen Tag.
            bool istAufgeklappt = zuvorAufgeklappteTage.Contains(gruppe.Key)
                                  || index == 0 && (zuvorBekannteTage.Count == 0 || !zuvorBekannteTage.Contains(gruppe.Key));

            TagesGruppen.Add(new VokabelTagesgruppe(
                gruppe.Key,
                gruppe.OrderBy(vokabel => vokabel.Deutsch, StringComparer.CurrentCultureIgnoreCase),
                istAufgeklappt));
        }
    }
}
