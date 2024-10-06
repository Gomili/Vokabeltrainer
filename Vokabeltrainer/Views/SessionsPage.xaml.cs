using Microsoft.UI.Xaml.Controls;

using Vokabeltrainer.ViewModels;

namespace Vokabeltrainer.Views;

public sealed partial class SessionsPage : Page
{
    public SessionsViewModel ViewModel
    {
        get;
    }

    public SessionsPage()
    {
        ViewModel = App.GetService<SessionsViewModel>();
        InitializeComponent();
    }
}
