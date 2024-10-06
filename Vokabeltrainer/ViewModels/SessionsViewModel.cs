using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.ViewModels;

public partial class SessionsViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private ObservableCollection<Session> _displayListe = new();
    
    public SessionsViewModel(IDataService dataService)
    {
        _dataService = dataService;
        
        List<Session> sourceListe = _dataService.ReadAllSessionAsync().GetAwaiter().GetResult();

        foreach (Session session in sourceListe.OrderByDescending(x => x.StartTime))
            DisplayListe.Add(session);
    }
}
