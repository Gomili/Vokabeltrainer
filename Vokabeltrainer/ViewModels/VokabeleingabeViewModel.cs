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
    
    public VokabeleingabeViewModel()
    {
    }
}
