using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Services;

public sealed class LernspracheService : ILernspracheService
{
    private const string EinstellungsSchluessel = "Lernsprache";
    private readonly ILocalSettingsService _localSettingsService;

    public Lernsprache AktuelleSprache { get; private set; }
    public event Action<Lernsprache>? SpracheGeaendert;

    public LernspracheService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
        AktuelleSprache = Lernsprache.Englisch;
    }

    public async Task InitializeAsync()
    {
        AktuelleSprache = await _localSettingsService
            .ReadSettingAsync<Lernsprache?>(EinstellungsSchluessel)
            .ConfigureAwait(false) ?? Lernsprache.Englisch;
    }

    public async Task SetzeSpracheAsync(Lernsprache sprache)
    {
        AktuelleSprache = sprache;
        SpracheGeaendert?.Invoke(sprache);
        await _localSettingsService.SaveSettingAsync(EinstellungsSchluessel, sprache);
    }
}
