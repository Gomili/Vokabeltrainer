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
        var viewModel = new VokabeleingabeViewModel(
            dataService,
            new LernspracheServiceFuerTests(),
            new BenutzerprofilServiceFuerTests())
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
        var viewModel = new VokabeleingabeViewModel(
            dataService,
            new LernspracheServiceFuerTests(),
            new BenutzerprofilServiceFuerTests())
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
            new LernspracheServiceFuerTests(Lernsprache.Latein),
            new BenutzerprofilServiceFuerTests());

        await viewModel.AddNewVokabelAsync("Baum", "arbor");

        Assert.AreEqual(Lernsprache.Latein, dataService.GespeicherteVokabeln.Single().Sprache);
        Assert.AreEqual("Latein", viewModel.FremdsprachenBezeichnung);
    }

    [TestMethod]
    public async Task AddNewVokabelAsync_MitAktiverBelohnung_VergibtEinenWortfunken()
    {
        var profil = new BenutzerprofilServiceFuerTests();
        var viewModel = new VokabeleingabeViewModel(
            new DataServiceFuerTests(),
            new LernspracheServiceFuerTests(),
            profil);

        await viewModel.AddNewVokabelAsync("Sonne", "sun");

        Assert.AreEqual(1, profil.Wortfunken);
    }

    [TestMethod]
    public async Task AddNewVokabelAsync_MitDeaktivierterBelohnung_VergibtKeineWortfunken()
    {
        var profil = new BenutzerprofilServiceFuerTests { BelohneNeueVokabeln = false };
        var viewModel = new VokabeleingabeViewModel(
            new DataServiceFuerTests(),
            new LernspracheServiceFuerTests(),
            profil);

        await viewModel.AddNewVokabelAsync("Mond", "moon");

        Assert.AreEqual(0, profil.Wortfunken);
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

    private sealed class BenutzerprofilServiceFuerTests : IBenutzerprofilService
    {
        public event Action? ProfilGeaendert;

        public string Name { get; private set; } = string.Empty;

        public int Wortfunken { get; private set; }

        public bool BelohneNeueVokabeln { get; set; } = true;

        public Task InitializeAsync() => Task.CompletedTask;

        public Task SetzeNameAsync(string name)
        {
            Name = name;
            ProfilGeaendert?.Invoke();
            return Task.CompletedTask;
        }

        public Task SetzeBelohnungFuerNeueVokabelnAsync(bool aktiviert)
        {
            BelohneNeueVokabeln = aktiviert;
            ProfilGeaendert?.Invoke();
            return Task.CompletedTask;
        }

        public Task FuegeWortfunkenHinzuAsync(int anzahl = 1)
        {
            Wortfunken += anzahl;
            ProfilGeaendert?.Invoke();
            return Task.CompletedTask;
        }

        public Task SetzeWortfunkenZurueckAsync()
        {
            Wortfunken = 0;
            ProfilGeaendert?.Invoke();
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
