using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Navigation;

using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Views;

namespace Vokabeltrainer.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IDisposable
{
    private bool _istFreigegeben;
    private readonly ILernspracheService _lernspracheService;
    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

    [ObservableProperty]
    private string _lernspracheBezeichnung;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public ShellViewModel(
        INavigationService navigationService,
        INavigationViewService navigationViewService,
        ILernspracheService lernspracheService)
    {
        _lernspracheService = lernspracheService;
        _lernspracheBezeichnung = ErmittleSprachtext(lernspracheService.AktuelleSprache);
        _lernspracheService.SpracheGeaendert += OnSpracheGeaendert;
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

    private void OnSpracheGeaendert(Lernsprache sprache)
    {
        LernspracheBezeichnung = ErmittleSprachtext(sprache);
    }

    private static string ErmittleSprachtext(Lernsprache sprache) =>
        $"Sprache: {(sprache == Lernsprache.Latein ? "Latein" : "Englisch")}";

    public void Dispose()
    {
        if (_istFreigegeben)
        {
            return;
        }

        _istFreigegeben = true;
        _lernspracheService.SpracheGeaendert -= OnSpracheGeaendert;
        NavigationService.Navigated -= OnNavigated;
        NavigationViewService.UnregisterEvents();
        GC.SuppressFinalize(this);
    }
}
