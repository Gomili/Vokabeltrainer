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
    {
        ViewModel = App.GetService<VokabelTrainingViewModel>();
        ViewModel.Message = Message;
        InitializeComponent();
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

        var result = await messageDialog.ShowAsync();
    }
}
