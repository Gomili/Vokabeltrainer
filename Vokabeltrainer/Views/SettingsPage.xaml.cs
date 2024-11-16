using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
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
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        
    }
    
    // private async Task<string> PickAFileAsync(string filename)
    // {
    //     // Create a file picker
    //     var openPicker = new FileOpenPicker();
    //
    //     // See the sample code below for how to make the window accessible from the App class.
    //     var window = App.MainWindow;
    //
    //     // Retrieve the window handle (HWND) of the current WinUI 3 window.
    //     var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
    //
    //     // Initialize the file picker with the window handle (HWND).
    //     WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
    //
    //     // Set options for your file picker
    //     openPicker.ViewMode = PickerViewMode.Thumbnail;
    //     openPicker.FileTypeFilter.Add("*");
    //     openPicker.FileTypeFilter.Add(".csv");
    //
    //     // Open the picker for the user to pick a file
    //     var file = await openPicker.PickSingleFileAsync();
    //     if (file != null)
    //     {
    //         return file.Path;
    //     }
    //
    //     return "";
    // }
}
