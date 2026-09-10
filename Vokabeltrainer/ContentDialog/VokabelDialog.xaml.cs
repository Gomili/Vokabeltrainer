using Microsoft.UI.Xaml.Controls;

namespace Vokabeltrainer.ContentDialog;

public sealed partial class VokabelDialog : Page
{
    public string Deutsch { get; set; } = string.Empty;

    public string Englisch { get; set; } = string.Empty;

    public string FremdsprachenBezeichnung { get; }
    public string FremdsprachenPlatzhalter { get; }

    public VokabelDialog(string fremdsprachenBezeichnung, string fremdsprachenPlatzhalter)
    {
        FremdsprachenBezeichnung = fremdsprachenBezeichnung;
        FremdsprachenPlatzhalter = fremdsprachenPlatzhalter;
        InitializeComponent();
    }
}
