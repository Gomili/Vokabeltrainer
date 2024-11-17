using System.Text.Json;
using Vokabeltrainer.Core.Contracts.Services;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.Services;

public class ImportExportService : IImportExportService
{
    public Task<string> ExportAsync(List<Vokabel> vokabeln)
    {
        string json = JsonSerializer.Serialize(vokabeln, new JsonSerializerOptions() { WriteIndented = true });
        return Task.FromResult(json);
    }

    public Task<List<Vokabel>> ImportAsync(string content)
    {
        List<Vokabel> vokabeln = JsonSerializer.Deserialize<List<Vokabel>>(content);
        return Task.FromResult(vokabeln);
    }
}