using Microsoft.UI.Xaml.Controls;
using Vokabeltrainer.ViewModels;

namespace Vokabeltrainer.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
        : this(App.GetService<SettingsViewModel>())
    {
    }

    public SettingsPage(SettingsViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
    }

    private async void WortfunkenZuruecksetzen_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            Title = "Wortfunken zurücksetzen?",
            Content = "Alle gesammelten Wortfunken werden auf 0 gesetzt. Dieser Schritt kann nicht rückgängig gemacht werden.",
            PrimaryButtonText = "Zurücksetzen",
            CloseButtonText = "Abbrechen",
            DefaultButton = Microsoft.UI.Xaml.Controls.ContentDialogButton.Close,
            XamlRoot = XamlRoot
        };

        if (await dialog.ShowAsync() == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
        {
            await ViewModel.WortfunkenZuruecksetzenCommand.ExecuteAsync(null);
        }
    }
}
