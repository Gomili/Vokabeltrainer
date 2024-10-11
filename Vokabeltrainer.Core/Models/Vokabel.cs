using System.ComponentModel.DataAnnotations;
using Vokabeltrainer.Core.Contracts.Services;

namespace Vokabeltrainer.Core.Models;

public class Vokabel : IEntity
{
    public Guid Id { get; set; }
    [MaxLength(256)]public string Deutsch { get; set; }
    [MaxLength(256)]public string Englisch { get; set; }
    public int Zaehler { get; set; }
    public bool IsMarked { get; set; }
}