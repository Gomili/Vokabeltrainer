using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Vokabeltrainer.ContentDialog;
using Vokabeltrainer.Core.Models;
using Vokabeltrainer.ViewModels;

namespace Vokabeltrainer.Views;

public sealed partial class VokabeleingabePage : Page
{
    public VokabeleingabeViewModel ViewModel
    {
        get;
    }

    public VokabeleingabePage()
        : this(App.GetService<VokabeleingabeViewModel>())
    {
    }

    public VokabeleingabePage(VokabeleingabeViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
    }

    private async void NeueVokabel_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog();
        var dialogView = new VokabelDialog(
            ViewModel.FremdsprachenBezeichnung,
            ViewModel.FremdsprachenPlatzhalter);
        
        // Warum: Desktop-ContentDialogs besitzen keinen eigenen XAML-Baum und müssen
        // deshalb explizit an die aktuelle Seite gebunden werden.
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Neue Vokabel";
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

    private async void VokabelListe_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView vokabelListe)
        {
            await ViewModel.SelectionChangedCommand.ExecuteAsync(vokabelListe.SelectedItem);
        }
    }

    private async void Priorisierung_Click(object sender, RoutedEventArgs e)
    {
        // Warum: Der dünne UI-Adapter übergibt nur den angeklickten Datensatz. Die fachliche
        // Änderung und das Speichern bleiben dadurch unabhängig von der Oberfläche testbar.
        if (sender is CheckBox { Tag: Vokabel vokabel })
        {
            await ViewModel.SwitchMarkedCommand.ExecuteAsync(vokabel);
        }
    }
}
