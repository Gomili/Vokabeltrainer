using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.UIA3;

namespace Vokabeltrainer.Tests.Oberflaeche;

[TestClass]
[DoNotParallelize]
public sealed class EinstellungenEndToEndTests
{
    private static readonly TimeSpan MaximaleWartezeit = TimeSpan.FromSeconds(10);

    [TestMethod]
    [TestCategory("EndToEnd")]
    public void Einstellungen_WechseltSichtbarZwischenHellUndDunkel()
    {
        string anwendungspfad = Path.Combine(AppContext.BaseDirectory, "Vokabeltrainer.exe");
        Assert.IsTrue(File.Exists(anwendungspfad), $"Die Anwendung wurde nicht gefunden: {anwendungspfad}");

        using var anwendung = Application.Launch(anwendungspfad);
        using var automation = new UIA3Automation();

        string? urspruenglichesFarbschema = null;
        Window? fenster = null;

        try
        {
            fenster = WarteAufElement(
                () => anwendung.GetMainWindow(automation),
                "Das Hauptfenster wurde nicht geöffnet. Bitte eine bereits laufende Instanz vor dem Test schließen.");

            AutomationElement navigationEinstellungen = WarteAufElement(
                () => fenster.FindFirstDescendant(
                    bedingung => bedingung.ByAutomationId("NavigationEinstellungen")),
                "Der Navigationseintrag für die Einstellungen wurde nicht gefunden.");

            // Warum: NavigationViewItem stellt kein Invoke-Pattern bereit und eine reine
            // UIA-Auswahl löst ItemInvoked nicht aus. Der Klick erfolgt erst nach Abschluss
            // des WinUI-Layouts, damit die wechselnde Seitenleistenbreite kein falsches Ziel erzeugt.
            fenster.SetForeground();
            navigationEinstellungen
                .WaitUntilClickable(MaximaleWartezeit)
                .Click();

            RadioButton hell = WarteAufElement(
                () => fenster.FindFirstDescendant(
                        bedingung => bedingung.ByAutomationId("FarbschemaHell"))
                    ?.AsRadioButton(),
                "Die Auswahl für das helle Farbschema wurde nicht gefunden.");

            RadioButton dunkel = WarteAufElement(
                () => fenster.FindFirstDescendant(
                        bedingung => bedingung.ByAutomationId("FarbschemaDunkel"))
                    ?.AsRadioButton(),
                "Die Auswahl für das dunkle Farbschema wurde nicht gefunden.");

            RadioButton system = WarteAufElement(
                () => fenster.FindFirstDescendant(
                        bedingung => bedingung.ByAutomationId("FarbschemaSystem"))
                    ?.AsRadioButton(),
                "Die Auswahl für das Systemfarbschema wurde nicht gefunden.");

            urspruenglichesFarbschema = new[] { hell, dunkel, system }
                .FirstOrDefault(IstAusgewaehlt)
                ?.AutomationId;

            WaehleFarbschema(fenster, "FarbschemaHell", "Das helle Farbschema wurde nicht ausgewählt.");
            WaehleFarbschema(fenster, "FarbschemaDunkel", "Das dunkle Farbschema wurde nicht ausgewählt.");
        }
        finally
        {
            // Warum: Ein Oberflächentest darf keine Benutzereinstellung zurücklassen.
            // Deshalb wird das vor dem Test aktive Farbschema vor dem Schließen wiederhergestellt.
            if (urspruenglichesFarbschema is not null && fenster is not null && !anwendung.HasExited)
            {
                WaehleFarbschema(
                    fenster,
                    urspruenglichesFarbschema,
                    "Das ursprüngliche Farbschema konnte nicht wiederhergestellt werden.");
            }

            if (!anwendung.HasExited)
            {
                anwendung.Close();
            }
        }
    }

    private static T WarteAufElement<T>(Func<T?> elementLesen, string fehlermeldung)
        where T : class
    {
        var stoppuhr = Stopwatch.StartNew();

        while (stoppuhr.Elapsed < MaximaleWartezeit)
        {
            T? element = elementLesen();
            if (element is not null)
            {
                return element;
            }

            Thread.Sleep(100);
        }

        Assert.Fail(fehlermeldung);
        throw new InvalidOperationException(fehlermeldung);
    }

    private static void WarteBis(Func<bool> bedingung, string fehlermeldung)
    {
        var stoppuhr = Stopwatch.StartNew();

        while (stoppuhr.Elapsed < MaximaleWartezeit)
        {
            if (bedingung())
            {
                return;
            }

            Thread.Sleep(100);
        }

        Assert.Fail(fehlermeldung);
    }

    private static bool IstAusgewaehlt(RadioButton radioButton)
    {
        return radioButton.Patterns.SelectionItem.Pattern.IsSelected.Value;
    }

    private static void WaehleFarbschema(Window fenster, string automationId, string fehlermeldung)
    {
        RadioButton radioButton = WarteAufElement(
            () => fenster.FindFirstDescendant(
                    bedingung => bedingung.ByAutomationId(automationId))
                ?.AsRadioButton(),
            fehlermeldung);

        radioButton.Patterns.SelectionItem.Pattern.Select();

        // Warum: Beim Theme-Wechsel kann WinUI den Automation-Peer ersetzen. Für die
        // Prüfung wird deshalb bewusst ein frisches Element aus dem UIA-Baum gelesen.
        WarteBis(
            () =>
            {
                RadioButton? aktuellerRadioButton = fenster.FindFirstDescendant(
                        bedingung => bedingung.ByAutomationId(automationId))
                    ?.AsRadioButton();

                return aktuellerRadioButton is not null && IstAusgewaehlt(aktuellerRadioButton);
            },
            fehlermeldung);
    }
}
