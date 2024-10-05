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
        InitializeComponent();
    }
}
