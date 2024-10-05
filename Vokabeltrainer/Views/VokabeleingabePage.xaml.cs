using Microsoft.UI.Xaml.Controls;

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
}
