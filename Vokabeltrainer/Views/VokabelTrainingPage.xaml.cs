using Microsoft.UI.Xaml.Controls;

using Vokabeltrainer.ViewModels;

namespace Vokabeltrainer.Views;

public sealed partial class VokabelTrainingPage : Page
{
    public VokabelTrainingViewModel ViewModel
    {
        get;
    }

    public VokabelTrainingPage()
        : this(App.GetService<VokabelTrainingViewModel>())
    {
    }

    public VokabelTrainingPage(VokabelTrainingViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        ViewModel.Message = Message;
        InitializeComponent();
        Unloaded += VokabelTrainingPage_Unloaded;
    }

    private async void Message(string message)
    {
        var messageDialog = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Information",
            Content = message,
            CloseButtonText = "OK"
        };

        await messageDialog.ShowAsync();
    }

    private void VokabelTrainingPage_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Unloaded -= VokabelTrainingPage_Unloaded;
        ViewModel.Dispose();
    }
}
