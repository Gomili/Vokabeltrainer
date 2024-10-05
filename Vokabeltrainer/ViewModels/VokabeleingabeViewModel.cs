using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Core.Services;
using Vokabeltrainer.Core.VokabelContext;
using Vokabeltrainer.Helpers;

namespace Vokabeltrainer.ViewModels;

public partial class VokabeleingabeViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private string _textDeutsch = string.Empty;
    [ObservableProperty] private string _textEnglisch = string.Empty;
    [ObservableProperty] private ObservableCollection<Vokabel> _displayListe = new() ;
    [ObservableProperty] private Vokabel? _selectedVokabel;
    [ObservableProperty] private int _anzahlVokabeln;

    public VokabeleingabeViewModel(IDataService dataService)
    {
        _dataService = dataService;
        
        List<Vokabel> sourceListe = _dataService.ReadAllAsync().GetAwaiter().GetResult();

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
    private async Task NeueVokabelAsync()
    {
        Vokabel vokabel = new Vokabel { Deutsch = TextDeutsch, Englisch = TextEnglisch, Zaehler = 100 };
        if (await _dataService.SaveAsync(vokabel))
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
        if (await _dataService.SaveAsync(SelectedVokabel))
        {
            ListHelper.UpdateListEntry(DisplayListe, SelectedVokabel);
        }
    }

    [RelayCommand]
    private async Task VokabelLoeschenAsync()
    {
        if (SelectedVokabel is null) return;
        if (await _dataService.DeleteAsync(SelectedVokabel))
        {
            ListHelper.DeleteListEntry(DisplayListe, SelectedVokabel);
            AnzahlVokabeln--;
        }
    }
}
