using Vokabeltrainer.Contracts.Services;

namespace Vokabeltrainer.Services;

public sealed class BenutzerprofilService : IBenutzerprofilService
{
    private const string NameSchluessel = "Benutzername";
    private const string WortfunkenSchluessel = "Wortfunken";
    private const string BelohnungssystemAktivSchluessel = "BelohnungssystemAktiv";
    private const string BelohnungNeueVokabelnSchluessel = "BelohneNeueVokabeln";

    private readonly ILocalSettingsService _localSettingsService;
    private readonly SemaphoreSlim _zugriff = new(1, 1);
    private bool _istInitialisiert;

    public event Action? ProfilGeaendert;

    public string Name { get; private set; } = string.Empty;

    public int Wortfunken { get; private set; }

    public bool BelohnungssystemAktiv { get; private set; } = true;

    public bool BelohneNeueVokabeln { get; private set; } = true;

    public BenutzerprofilService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        await _zugriff.WaitAsync();
        try
        {
            if (_istInitialisiert)
            {
                return;
            }

            Name = (await _localSettingsService.ReadSettingAsync<string>(NameSchluessel) ?? string.Empty).Trim();
            Wortfunken = Math.Max(0, await _localSettingsService.ReadSettingAsync<int?>(WortfunkenSchluessel) ?? 0);
            BelohnungssystemAktiv = await _localSettingsService
                .ReadSettingAsync<bool?>(BelohnungssystemAktivSchluessel) ?? true;
            BelohneNeueVokabeln = await _localSettingsService
                .ReadSettingAsync<bool?>(BelohnungNeueVokabelnSchluessel) ?? true;
            _istInitialisiert = true;
        }
        finally
        {
            _zugriff.Release();
        }
    }

    public async Task SetzeNameAsync(string name)
    {
        Name = (name ?? string.Empty).Trim();
        await _localSettingsService.SaveSettingAsync(NameSchluessel, Name);
        ProfilGeaendert?.Invoke();
    }

    public async Task SetzeBelohnungssystemAktivAsync(bool aktiviert)
    {
        BelohnungssystemAktiv = aktiviert;
        await _localSettingsService.SaveSettingAsync(BelohnungssystemAktivSchluessel, aktiviert);
        ProfilGeaendert?.Invoke();
    }

    public async Task SetzeBelohnungFuerNeueVokabelnAsync(bool aktiviert)
    {
        BelohneNeueVokabeln = aktiviert;
        await _localSettingsService.SaveSettingAsync(BelohnungNeueVokabelnSchluessel, aktiviert);
        ProfilGeaendert?.Invoke();
    }

    public async Task FuegeWortfunkenHinzuAsync(int anzahl = 1)
    {
        if (anzahl <= 0 || !BelohnungssystemAktiv)
        {
            return;
        }

        await _zugriff.WaitAsync();
        try
        {
            Wortfunken = checked(Wortfunken + anzahl);
            await _localSettingsService.SaveSettingAsync(WortfunkenSchluessel, Wortfunken);
        }
        finally
        {
            _zugriff.Release();
        }

        ProfilGeaendert?.Invoke();
    }

    public async Task SetzeWortfunkenZurueckAsync()
    {
        await _zugriff.WaitAsync();
        try
        {
            Wortfunken = 0;
            await _localSettingsService.SaveSettingAsync(WortfunkenSchluessel, Wortfunken);
        }
        finally
        {
            _zugriff.Release();
        }

        ProfilGeaendert?.Invoke();
    }
}
