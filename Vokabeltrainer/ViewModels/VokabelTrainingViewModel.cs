using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Vokabeltrainer.Core.Contracts.Services;

namespace Vokabeltrainer.ViewModels;

public partial class VokabelTrainingViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    [ObservableProperty] private string _laufzeit = string.Empty;
    [ObservableProperty] private string _richtige = "0";
    [ObservableProperty] private string _falsche = "0";
    [ObservableProperty] private string _anzahl = "0";
    
    private readonly DispatcherTimer _timer = new ();
    private DateTime _startTime;
    
    public VokabelTrainingViewModel(IDataService dataService)
    {
        _dataService = dataService;
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick -= Timer_Tick;
        _timer.Tick += Timer_Tick;
    }

    [RelayCommand]
    private void Start()
    {
        _timer.Start();
        _startTime = DateTime.Now;
    }

    [RelayCommand]
    private void Stop()
    {
        _timer.Stop();
    }
    
    private void Timer_Tick(object sender, object e)
    {
        Laufzeit = (DateTime.Now - _startTime).ToString(@"hh\:mm\:ss");
    }
}
