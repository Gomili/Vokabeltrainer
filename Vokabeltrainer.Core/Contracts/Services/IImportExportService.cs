using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.Core.Contracts.Services;

public interface IImportExportService
{
    Task<string> ExportAsync(List<Vokabel> vokabeln);
    Task<List<Vokabel>> ImportAsync(string content);
}