using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.ViewModels;

public partial class VokabeleingabeViewModel : ObservableRecipient
{
    [ObservableProperty] private string _textDeutsch = string.Empty;
    [ObservableProperty] private string _textEnglisch = string.Empty;
    [ObservableProperty] private string _zaehler = string.Empty;
    [ObservableProperty] private ObservableCollection<Vokabel> _displayListe = new() ;

    private List<Vokabel> _sourceListe = [];
    
    public VokabeleingabeViewModel()
    {
        _sourceListe.Add(new Vokabel(){Deutsch = "Baum",Englisch = "tree",Zaehler = 100});
        _sourceListe.Add(new Vokabel(){Deutsch = "Tisch",Englisch = "table",Zaehler = 100});
        _sourceListe.Add(new Vokabel(){Deutsch = "reisen",Englisch = "to travel",Zaehler = 100});

        foreach (Vokabel vokabel in _sourceListe)
            DisplayListe.Add(vokabel);
    }
}
