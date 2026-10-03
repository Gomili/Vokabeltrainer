using Vokabeltrainer.Core.Models;
using Vokabeltrainer.DisplayClasses;

namespace Vokabeltrainer.Tests.DisplayClasses;

[TestClass]
public sealed class DisplayLernVokabelTests
{
    private static readonly Vokabel TestVokabel = new()
    {
        Deutsch = "Baum",
        Englisch = "arbor"
    };

    [TestMethod]
    public void Latein_GibtLateinVorUndPrueftDeutscheAntwort()
    {
        var lernVokabel = new DisplayLernVokabel(TestVokabel, Lernsprache.Latein, _ => { })
        {
            Antwort = "Baum"
        };

        Assert.AreEqual("arbor", lernVokabel.Vorgabe);
        Assert.AreEqual("Baum", lernVokabel.ErwarteteAntwort);
        Assert.IsTrue(lernVokabel.IstAntwortRichtig());
    }

    [TestMethod]
    public void Englisch_GibtDeutschVorUndPrueftEnglischeAntwort()
    {
        var lernVokabel = new DisplayLernVokabel(TestVokabel, Lernsprache.Englisch, _ => { })
        {
            Antwort = "arbor"
        };

        Assert.AreEqual("Baum", lernVokabel.Vorgabe);
        Assert.AreEqual("arbor", lernVokabel.ErwarteteAntwort);
        Assert.IsTrue(lernVokabel.IstAntwortRichtig());
    }
}
