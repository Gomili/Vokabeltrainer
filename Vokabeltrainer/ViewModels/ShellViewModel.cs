using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;

using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Views;

namespace Vokabeltrainer.ViewModels;

public partial class ShellViewModel : ObservableRecipient, IDisposable
{
    private bool _istFreigegeben;
    private readonly ILernspracheService _lernspracheService;
    private readonly IBenutzerprofilService _benutzerprofilService;
    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

    [ObservableProperty]
    private string _lernspracheBezeichnung;

    [ObservableProperty]
    private string _benutzernameBezeichnung;

    [ObservableProperty]
    private string _wortfunkenBezeichnung;

    [ObservableProperty]
    private Visibility _wortfunkenSichtbarkeit;

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
        ILernspracheService lernspracheService,
        IBenutzerprofilService benutzerprofilService)
    {
        _lernspracheService = lernspracheService;
        _benutzerprofilService = benutzerprofilService;
        _lernspracheBezeichnung = ErmittleSprachtext(lernspracheService.AktuelleSprache);
        _benutzernameBezeichnung = ErmittleNamenstext();
        _wortfunkenBezeichnung = ErmittleWortfunkentext();
        _wortfunkenSichtbarkeit = ErmittleWortfunkenSichtbarkeit();
        _lernspracheService.SpracheGeaendert += OnSpracheGeaendert;
        _benutzerprofilService.ProfilGeaendert += OnProfilGeaendert;
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

    private void OnProfilGeaendert()
    {
        BenutzernameBezeichnung = ErmittleNamenstext();
        WortfunkenBezeichnung = ErmittleWortfunkentext();
        WortfunkenSichtbarkeit = ErmittleWortfunkenSichtbarkeit();
    }

    private string ErmittleNamenstext() => string.IsNullOrWhiteSpace(_benutzerprofilService.Name)
        ? "Persönlicher Wortschatz"
        : $"Hallo, {_benutzerprofilService.Name}!";

    private string ErmittleWortfunkentext() => $"✨ {_benutzerprofilService.Wortfunken} Wortfunken";

    private Visibility ErmittleWortfunkenSichtbarkeit() => _benutzerprofilService.BelohnungssystemAktiv
        ? Visibility.Visible
        : Visibility.Collapsed;

    public void Dispose()
    {
        if (_istFreigegeben)
        {
            return;
        }

        _istFreigegeben = true;
        _lernspracheService.SpracheGeaendert -= OnSpracheGeaendert;
        _benutzerprofilService.ProfilGeaendert -= OnProfilGeaendert;
        NavigationService.Navigated -= OnNavigated;
        NavigationViewService.UnregisterEvents();
        GC.SuppressFinalize(this);
    }
}
