using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.ViewModels;

namespace Vokabeltrainer.Tests.ViewModels;

[TestClass]
public sealed class VokabeleingabeViewModelTests
{
    [TestMethod]
    public async Task AddNewVokabelAsync_MitPlanung_VerwendetGewaehltesFreigabedatum()
    {
        var dataService = new DataServiceFuerTests();
        var viewModel = new VokabeleingabeViewModel(dataService)
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
        var viewModel = new VokabeleingabeViewModel(dataService)
        {
            FreigabedatumFestlegen = false,
            FreigabedatumNeueVokabeln = DateTimeOffset.Now.AddDays(14)
        };

        await viewModel.AddNewVokabelAsync("Haus", "house");

        Assert.AreEqual(
            DateTime.Today,
            dataService.GespeicherteVokabeln.Single().Freigabedatum);
    }

    private sealed class DataServiceFuerTests : IDataService
    {
        public List<Vokabel> GespeicherteVokabeln { get; } = [];

        public Task<List<Vokabel>> ReadAllVokabelAsync() =>
            Task.FromResult(GespeicherteVokabeln.ToList());

        public Task<List<Vokabel>> ReadFreigegebeneVokabelnAsync(DateTime stichtag) =>
            Task.FromResult(GespeicherteVokabeln
                .Where(vokabel => vokabel.Freigabedatum.Date <= stichtag.Date)
                .ToList());

        public Task<Vokabel?> ReadVokabelAsync(Guid id) =>
            Task.FromResult(GespeicherteVokabeln.FirstOrDefault(vokabel => vokabel.Id == id));

        public Task<bool> SaveVokabelAsync(Vokabel content)
        {
            GespeicherteVokabeln.Add(content);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteVokabelAsync(Vokabel content) => Task.FromResult(true);

        public Task<List<Session>> ReadAllSessionAsync() => Task.FromResult(new List<Session>());

        public Task<Session?> ReadSessionAsync(Guid id) => Task.FromResult<Session?>(null);

        public Task<bool> SaveSessionAsync(Session content) => Task.FromResult(true);

        public Task<bool> DeleteSessionAsync(Session content) => Task.FromResult(true);

        public Task SaveFromImport(Vokabel? oldvokabel, Vokabel vokabel) => Task.CompletedTask;

        public Task<(int, int, int)> ReadSessionCountAsync(DateTime dateTime) =>
            Task.FromResult((0, 0, 0));

        public Task SaveIsChangedAsync(Vokabel? oldVokabel, Vokabel newvokabel) => Task.CompletedTask;

        public Task FixData() => Task.CompletedTask;

        public Task<int> GetAnzahlPriorisierterVokabelnAsync() =>
            Task.FromResult(GespeicherteVokabeln.Count(vokabel => vokabel.IsMarked));
    }
}
