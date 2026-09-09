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
}
