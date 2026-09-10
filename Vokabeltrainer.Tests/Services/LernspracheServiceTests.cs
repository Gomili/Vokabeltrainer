using Vokabeltrainer.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.Services;

namespace Vokabeltrainer.Tests.Services;

[TestClass]
public sealed class LernspracheServiceTests
{
    [TestMethod]
    public void OhneGespeicherteEinstellung_VerwendetEnglisch()
    {
        var service = new LernspracheService(new LocalSettingsServiceFuerTests());

        Assert.AreEqual(Lernsprache.Englisch, service.AktuelleSprache);
    }

    [TestMethod]
    public async Task SetzeSpracheAsync_SpeichertLatein()
    {
        var einstellungen = new LocalSettingsServiceFuerTests();
        var service = new LernspracheService(einstellungen);

        await service.SetzeSpracheAsync(Lernsprache.Latein);

        Assert.AreEqual(Lernsprache.Latein, service.AktuelleSprache);
        Assert.AreEqual(Lernsprache.Latein, einstellungen.GespeicherteSprache);
    }

    private sealed class LocalSettingsServiceFuerTests : ILocalSettingsService
    {
        public Lernsprache? GespeicherteSprache { get; private set; }

        public Task<T?> ReadSettingAsync<T>(string key)
        {
            object? wert = GespeicherteSprache;
            return Task.FromResult((T?)wert);
        }

        public Task SaveSettingAsync<T>(string key, T value)
        {
            GespeicherteSprache = (Lernsprache?)(object?)value;
            return Task.CompletedTask;
        }
    }
}
