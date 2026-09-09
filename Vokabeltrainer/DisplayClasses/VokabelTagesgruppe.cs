using System.Collections.ObjectModel;
using System.Globalization;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.DisplayClasses;

/// <summary>
/// Fasst die Vokabeln einer Unterrichtseinheit an einem Kalendertag zusammen.
/// </summary>
public sealed class VokabelTagesgruppe
{
    private static readonly CultureInfo DeutscheKultur = CultureInfo.GetCultureInfo("de-DE");

    public VokabelTagesgruppe(DateTime lerntag, IEnumerable<Vokabel> vokabeln, bool istAufgeklappt)
    {
        Lerntag = lerntag.Date;
        Vokabeln = new ObservableCollection<Vokabel>(vokabeln);
        IstAufgeklappt = istAufgeklappt;
    }

    public DateTime Lerntag { get; }

    public ObservableCollection<Vokabel> Vokabeln { get; }

    public bool IstAufgeklappt { get; set; }

    public string DatumsText => Lerntag == DateTime.MinValue.Date
        ? "Datum unbekannt"
        : Lerntag.ToString("dddd, dd. MMMM yyyy", DeutscheKultur);

    public string EinordnungsText => Lerntag switch
    {
        _ when Lerntag == DateTime.Today => "Heute gelernt",
        _ when Lerntag == DateTime.Today.AddDays(-1) => "Gestern gelernt",
        _ when Lerntag == DateTime.MinValue.Date => "Älterer Eintrag ohne Lerntag",
        _ => "Lektion des Tages"
    };

    public string AnzahlText => Vokabeln.Count == 1
        ? "1 Vokabel"
        : $"{Vokabeln.Count} Vokabeln";

    public string AutomatisierungsId => Lerntag == DateTime.MinValue.Date
        ? "VokabelTag-Unbekannt"
        : $"VokabelTag-{Lerntag:yyyyMMdd}";

    public string ListenAutomatisierungsId => $"{AutomatisierungsId}-Liste";

    public string Automatisierungsname => $"{DatumsText}, {AnzahlText}";
}
