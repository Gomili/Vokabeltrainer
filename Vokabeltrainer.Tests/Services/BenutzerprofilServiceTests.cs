using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Services;

namespace Vokabeltrainer.Tests.Services;

[TestClass]
public sealed class BenutzerprofilServiceTests
{
    [TestMethod]
    public async Task InitializeAsync_OhneEinstellungen_VerwendetSinnvolleStandardwerte()
    {
        var service = new BenutzerprofilService(new LocalSettingsServiceFuerTests());

        await service.InitializeAsync();

        Assert.AreEqual(string.Empty, service.Name);
        Assert.AreEqual(0, service.Wortfunken);
        Assert.IsTrue(service.BelohneNeueVokabeln);
    }

    [TestMethod]
    public async Task Profilwerte_WerdenGespeichertUndZurueckgesetzt()
    {
        var einstellungen = new LocalSettingsServiceFuerTests();
        var service = new BenutzerprofilService(einstellungen);
        await service.InitializeAsync();

        await service.SetzeNameAsync("  Mia  ");
        await service.SetzeBelohnungFuerNeueVokabelnAsync(false);
        await service.FuegeWortfunkenHinzuAsync(3);

        Assert.AreEqual("Mia", service.Name);
        Assert.AreEqual(3, service.Wortfunken);
        Assert.IsFalse(service.BelohneNeueVokabeln);

        var neuGeladenerService = new BenutzerprofilService(einstellungen);
        await neuGeladenerService.InitializeAsync();

        Assert.AreEqual("Mia", neuGeladenerService.Name);
        Assert.AreEqual(3, neuGeladenerService.Wortfunken);
        Assert.IsFalse(neuGeladenerService.BelohneNeueVokabeln);

        await service.SetzeWortfunkenZurueckAsync();

        Assert.AreEqual(0, service.Wortfunken);
    }

    private sealed class LocalSettingsServiceFuerTests : ILocalSettingsService
    {
        private readonly Dictionary<string, object?> _werte = [];

        public Task<T?> ReadSettingAsync<T>(string key)
        {
            if (_werte.TryGetValue(key, out object? wert))
            {
                return Task.FromResult((T?)wert);
            }

            return Task.FromResult(default(T));
        }

        public Task SaveSettingAsync<T>(string key, T value)
        {
            _werte[key] = value;
            return Task.CompletedTask;
        }
    }
}
