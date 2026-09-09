using Microsoft.UI.Xaml;
using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Services;

namespace Vokabeltrainer.Tests.Services;

[TestClass]
public sealed class ThemeSelectorServiceTests
{
    [TestMethod]
    public async Task SetThemeAsync_WechseltZwischenHellUndDunkelUndSpeichertDieAuswahl()
    {
        var einstellungsspeicher = new EinstellungsspeicherFuerTests();
        var service = new ThemeSelectorServiceFuerTests(einstellungsspeicher);

        await service.SetThemeAsync(ElementTheme.Light);

        Assert.AreEqual(ElementTheme.Light, service.Theme);
        Assert.AreEqual("Light", einstellungsspeicher.GespeichertesFarbschema);

        await service.SetThemeAsync(ElementTheme.Dark);

        Assert.AreEqual(ElementTheme.Dark, service.Theme);
        Assert.AreEqual("Dark", einstellungsspeicher.GespeichertesFarbschema);
        CollectionAssert.AreEqual(
            new[] { ElementTheme.Light, ElementTheme.Dark },
            service.AngewendeteFarbschemata);
    }

    private sealed class ThemeSelectorServiceFuerTests(ILocalSettingsService einstellungsspeicher)
        : ThemeSelectorService(einstellungsspeicher)
    {
        public List<ElementTheme> AngewendeteFarbschemata { get; } = [];

        public override Task SetRequestedThemeAsync()
        {
            // Warum: Der Test prüft die Umschaltlogik unabhängig vom echten App-Fenster.
            // Dadurch bleibt er reproduzierbar und verändert nicht die sichtbare Oberfläche.
            AngewendeteFarbschemata.Add(Theme);
            return Task.CompletedTask;
        }
    }

    private sealed class EinstellungsspeicherFuerTests : ILocalSettingsService
    {
        private readonly Dictionary<string, object?> _werte = [];

        public string? GespeichertesFarbschema =>
            _werte.TryGetValue("AppBackgroundRequestedTheme", out object? wert)
                ? wert as string
                : null;

        public Task<T?> ReadSettingAsync<T>(string key)
        {
            return Task.FromResult(
                _werte.TryGetValue(key, out object? wert) && wert is T typisierterWert
                    ? typisierterWert
                    : default);
        }

        public Task SaveSettingAsync<T>(string key, T value)
        {
            _werte[key] = value;
            return Task.CompletedTask;
        }
    }
}
