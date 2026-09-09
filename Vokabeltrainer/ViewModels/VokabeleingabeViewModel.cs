using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Helpers;

namespace Vokabeltrainer.ViewModels;

public partial class VokabeleingabeViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private string _textDeutsch = string.Empty;
    [ObservableProperty] private string _textEnglisch = string.Empty;
    [ObservableProperty] private ObservableCollection<Vokabel> _displayListe = new();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HatAusgewaehlteVokabel))]
    private Vokabel? _selectedVokabel;
    [ObservableProperty] private int _anzahlVokabeln;
    [ObservableProperty] private string _suchText = string.Empty;

    public bool HatAusgewaehlteVokabel => SelectedVokabel is not null;

    public VokabeleingabeViewModel(IDataService dataService)
    {
        _dataService = dataService;
        
        List<Vokabel> sourceListe = _dataService.ReadAllVokabelAsync().GetAwaiter().GetResult();

        AnzahlVokabeln = sourceListe.Count;
        
        foreach (Vokabel vokabel in sourceListe)
            DisplayListe.Add(vokabel);
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
            await _dataService.SaveVokabelAsync(vokabel);
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
            SelectedVokabel = null;
            TextDeutsch = string.Empty;
            TextEnglisch = string.Empty;
        }
    }
}
