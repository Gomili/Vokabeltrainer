using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Navigation;

using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Views;

namespace Vokabeltrainer.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IDisposable
{
    private bool _istFreigegeben;
    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            // Warum: Die Einstellungen sind ein regulärer Footer-Eintrag, damit sie
            // dieselbe Gestaltung und AutomationId wie die übrige Navigation besitzen.
            Selected = NavigationViewService.GetSelectedItem(e.SourcePageType);
            return;
        }

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }

    public void Dispose()
    {
        if (_istFreigegeben)
        {
            return;
        }

        _istFreigegeben = true;
        NavigationService.Navigated -= OnNavigated;
        NavigationViewService.UnregisterEvents();
        GC.SuppressFinalize(this);
    }
}
