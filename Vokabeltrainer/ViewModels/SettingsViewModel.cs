using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Helpers;
using Windows.ApplicationModel;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IImportExportService _importExportService;
    private readonly IDataService _dataService;
    private readonly ILernspracheService _lernspracheService;
    private readonly IBenutzerprofilService _benutzerprofilService;

    [ObservableProperty] private ElementTheme _elementTheme;
    [ObservableProperty] private string _versionDescription;
    [ObservableProperty] private int _progressVal;
    [ObservableProperty] private int _progressMax;
    [ObservableProperty] private Visibility _progressBarVisibility = Visibility.Collapsed;
    [ObservableProperty] private int _markierte = 0;
    [ObservableProperty] private Lernsprache _ausgewaehlteLernsprache;
    [ObservableProperty] private string _benutzername;
    [ObservableProperty] private int _wortfunken;
    [ObservableProperty] private bool _belohneNeueVokabeln;

    public IReadOnlyList<Lernsprache> Lernsprachen { get; } =
        [Lernsprache.Englisch, Lernsprache.Latein];
    
    public ICommand SwitchThemeCommand { get; }

    [RelayCommand]
    private async Task LoeschenAsync()
    {
        List<Vokabel> vokabeln = await _dataService.ReadAllVokabelAsync(AusgewaehlteLernsprache);

        foreach (Vokabel vokabel in vokabeln.Where(x => x.IsMarked))
        {
            vokabel.IsMarked = false;
            await _dataService.SaveVokabelAsync(vokabel);
        }
        
        Markierte = await _dataService.GetAnzahlPriorisierterVokabelnAsync(AusgewaehlteLernsprache);
    }
    
    [RelayCommand]
    private async Task ExportAsync()
    {
        List<Vokabel> vokabeln = await _dataService.ReadAllVokabelAsync(AusgewaehlteLernsprache);
        string exportText = await _importExportService.ExportAsync(vokabeln);
        await FileDialogHelper.SaveFileAsync(exportText);
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        var a = await FileDialogHelper.PickAFileAsync();
        string data = await File.ReadAllTextAsync(a);
        if (!string.IsNullOrWhiteSpace(data))
        {
            List<Vokabel> vokabeln = await _importExportService.ImportAsync(data);
            ProgressMax = vokabeln.Count;
            ProgressVal = 0;
            ProgressBarVisibility = Visibility.Visible;
            await Task.Delay(200);
            foreach (Vokabel vokabel in vokabeln)
            {
                vokabel.Sprache = AusgewaehlteLernsprache;
                var oldVokabel = await _dataService.ReadVokabelAsync(vokabel.Id);
                if (oldVokabel is not null && oldVokabel.Sprache != AusgewaehlteLernsprache)
                {
                    vokabel.Id = Guid.NewGuid();
                    oldVokabel = null;
                }
                await _dataService.SaveIsChangedAsync(oldVokabel, vokabel);
                ProgressVal++;
            }

            ProgressVal = ProgressMax;
            await Task.Delay(200);
            ProgressBarVisibility = Visibility.Collapsed;
        }
    }

    [RelayCommand]
    private async Task BenutzernameSpeichernAsync()
    {
        Benutzername = Benutzername.Trim();
        await _benutzerprofilService.SetzeNameAsync(Benutzername);
    }

    [RelayCommand]
    private async Task WortfunkenZuruecksetzenAsync()
    {
        await _benutzerprofilService.SetzeWortfunkenZurueckAsync();
        Wortfunken = _benutzerprofilService.Wortfunken;
    }
    
    public SettingsViewModel(
        IThemeSelectorService themeSelectorService,
        IImportExportService importExportService,
        IDataService dataService,
        ILernspracheService lernspracheService,
        IBenutzerprofilService benutzerprofilService)
    {
        _themeSelectorService = themeSelectorService;
        _importExportService = importExportService;
        _dataService = dataService;
        _lernspracheService = lernspracheService;
        _benutzerprofilService = benutzerprofilService;
        _elementTheme = _themeSelectorService.Theme;
        _ausgewaehlteLernsprache = _lernspracheService.AktuelleSprache;
        _benutzername = _benutzerprofilService.Name;
        _wortfunken = _benutzerprofilService.Wortfunken;
        _belohneNeueVokabeln = _benutzerprofilService.BelohneNeueVokabeln;
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });

        Markierte = _dataService.GetAnzahlPriorisierterVokabelnAsync(AusgewaehlteLernsprache).GetAwaiter().GetResult();
    }

    partial void OnAusgewaehlteLernspracheChanged(Lernsprache value)
    {
        _ = SpeichereLernspracheAsync(value);
    }

    partial void OnBelohneNeueVokabelnChanged(bool value)
    {
        _ = _benutzerprofilService.SetzeBelohnungFuerNeueVokabelnAsync(value);
    }

    private async Task SpeichereLernspracheAsync(Lernsprache sprache)
    {
        await _lernspracheService.SetzeSpracheAsync(sprache);
        Markierte = await _dataService.GetAnzahlPriorisierterVokabelnAsync(sprache);
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
