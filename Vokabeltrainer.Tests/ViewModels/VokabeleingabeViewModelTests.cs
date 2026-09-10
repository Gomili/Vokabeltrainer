using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.ViewModels;
using Vokabeltrainer.Contracts.Services;

namespace Vokabeltrainer.Tests.ViewModels;

[TestClass]
public sealed class VokabeleingabeViewModelTests
{
    [TestMethod]
    public async Task AddNewVokabelAsync_MitPlanung_VerwendetGewaehltesFreigabedatum()
    {
        var dataService = new DataServiceFuerTests();
        var viewModel = new VokabeleingabeViewModel(dataService, new LernspracheServiceFuerTests())
        {
            FreigabedatumFestlegen = true,
            FreigabedatumNeueVokabeln = new DateTimeOffset(2026, 10, 15, 0, 0, 0, TimeSpan.Zero)
        };

        await viewModel.AddNewVokabelAsync("Baum", "tree");

        Assert.AreEqual(
            new DateTime(2026, 10, 15),
            dataService.GespeicherteVokabeln.Single().Freigabedatum);
    }

    [TestMethod]
    public async Task AddNewVokabelAsync_OhnePlanung_VerwendetEingabetagAlsFreigabedatum()
    {
        var dataService = new DataServiceFuerTests();
        var viewModel = new VokabeleingabeViewModel(dataService, new LernspracheServiceFuerTests())
        {
            FreigabedatumFestlegen = false,
            FreigabedatumNeueVokabeln = DateTimeOffset.Now.AddDays(14)
        };

        await viewModel.AddNewVokabelAsync("Haus", "house");

        Assert.AreEqual(
            DateTime.Today,
            dataService.GespeicherteVokabeln.Single().Freigabedatum);
    }

    [TestMethod]
    public async Task AddNewVokabelAsync_MitLateinischerAuswahl_SpeichertLatein()
    {
        var dataService = new DataServiceFuerTests();
        var viewModel = new VokabeleingabeViewModel(
            dataService,
            new LernspracheServiceFuerTests(Lernsprache.Latein));

        await viewModel.AddNewVokabelAsync("Baum", "arbor");

        Assert.AreEqual(Lernsprache.Latein, dataService.GespeicherteVokabeln.Single().Sprache);
        Assert.AreEqual("Latein", viewModel.FremdsprachenBezeichnung);
    }

    private sealed class LernspracheServiceFuerTests(Lernsprache sprache = Lernsprache.Englisch)
        : ILernspracheService
    {
        public event Action<Lernsprache>? SpracheGeaendert;

        public Lernsprache AktuelleSprache { get; private set; } = sprache;

        public Task InitializeAsync() => Task.CompletedTask;

        public Task SetzeSpracheAsync(Lernsprache neueSprache)
        {
            AktuelleSprache = neueSprache;
            SpracheGeaendert?.Invoke(neueSprache);
            return Task.CompletedTask;
        }
    }

    private sealed class DataServiceFuerTests : IDataService
    {
        public List<Vokabel> GespeicherteVokabeln { get; } = [];

        public Task<List<Vokabel>> ReadAllVokabelAsync(Lernsprache sprache) =>
            Task.FromResult(GespeicherteVokabeln.Where(vokabel => vokabel.Sprache == sprache).ToList());

        public Task<List<Vokabel>> ReadFreigegebeneVokabelnAsync(DateTime stichtag, Lernsprache sprache) =>
            Task.FromResult(GespeicherteVokabeln
                .Where(vokabel => vokabel.Sprache == sprache && vokabel.Freigabedatum.Date <= stichtag.Date)
                .ToList());

        public Task<Vokabel?> ReadVokabelAsync(Guid id) =>
            Task.FromResult(GespeicherteVokabeln.FirstOrDefault(vokabel => vokabel.Id == id));

        public Task<bool> SaveVokabelAsync(Vokabel content)
        {
            GespeicherteVokabeln.Add(content);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteVokabelAsync(Vokabel content) => Task.FromResult(true);

        public Task<List<Session>> ReadAllSessionAsync(Lernsprache sprache) => Task.FromResult(new List<Session>());

        public Task<Session?> ReadSessionAsync(Guid id) => Task.FromResult<Session?>(null);

        public Task<bool> SaveSessionAsync(Session content) => Task.FromResult(true);

        public Task<bool> DeleteSessionAsync(Session content) => Task.FromResult(true);

        public Task SaveFromImport(Vokabel? oldvokabel, Vokabel vokabel) => Task.CompletedTask;

        public Task<(int, int, int)> ReadSessionCountAsync(DateTime dateTime, Lernsprache sprache) =>
            Task.FromResult((0, 0, 0));

        public Task SaveIsChangedAsync(Vokabel? oldVokabel, Vokabel newvokabel) => Task.CompletedTask;

        public Task FixData() => Task.CompletedTask;

        public Task<int> GetAnzahlPriorisierterVokabelnAsync(Lernsprache sprache) =>
            Task.FromResult(GespeicherteVokabeln.Count(vokabel => vokabel.Sprache == sprache && vokabel.IsMarked));
    }
}
