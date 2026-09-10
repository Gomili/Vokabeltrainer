using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Services;

public sealed class LernspracheService : ILernspracheService
{
    private const string EinstellungsSchluessel = "Lernsprache";
    private readonly ILocalSettingsService _localSettingsService;

    public Lernsprache AktuelleSprache { get; private set; }

    public LernspracheService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
        AktuelleSprache = _localSettingsService
            .ReadSettingAsync<Lernsprache?>(EinstellungsSchluessel)
            .GetAwaiter()
            .GetResult() ?? Lernsprache.Englisch;
    }

    public async Task SetzeSpracheAsync(Lernsprache sprache)
    {
        AktuelleSprache = sprache;
        await _localSettingsService.SaveSettingAsync(EinstellungsSchluessel, sprache);
    }
}
