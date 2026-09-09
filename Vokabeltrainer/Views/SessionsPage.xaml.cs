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
        : this(App.GetService<SessionsViewModel>())
    {
    }

    public SessionsPage(SessionsViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
    }
}
