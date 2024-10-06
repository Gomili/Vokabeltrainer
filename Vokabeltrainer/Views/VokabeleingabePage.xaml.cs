using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Vokabeltrainer.ContentDialog;
using Vokabeltrainer.ViewModels;

namespace Vokabeltrainer.Views;

public sealed partial class VokabeleingabePage : Page
{
    public VokabeleingabeViewModel ViewModel
    {
        get;
    }

    public VokabeleingabePage()
    {
        ViewModel = App.GetService<VokabeleingabeViewModel>();
        InitializeComponent();
    }

    private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog();
        VokabelDialog dialogView = new VokabelDialog();
        
        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Vokabel Eingabe:";
        dialog.PrimaryButtonText = "Speichern";
        dialog.SecondaryButtonText = "Nicht speichern";
        dialog.CloseButtonText = "Abbrechen";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = dialogView;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.AddNewVokabelAsync(dialogView.Deutsch, dialogView.Englisch);
        }
    }
}
