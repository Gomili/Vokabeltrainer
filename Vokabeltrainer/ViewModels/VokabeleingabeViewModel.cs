using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Core.Services;
using Vokabeltrainer.Core.VokabelContext;

namespace Vokabeltrainer.ViewModels;

public partial class VokabeleingabeViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private string _textDeutsch = string.Empty;
    [ObservableProperty] private string _textEnglisch = string.Empty;
    [ObservableProperty] private string _zaehler = string.Empty;
    [ObservableProperty] private ObservableCollection<Vokabel> _displayListe = new() ;

    private List<Vokabel> _sourceListe;
    
    public VokabeleingabeViewModel(IDataService dataService)
    {
        _dataService = dataService;
        
        _sourceListe = _dataService.ReadAllAsync().GetAwaiter().GetResult();

        foreach (Vokabel vokabel in _sourceListe)
            DisplayListe.Add(vokabel);
    }

    
}
