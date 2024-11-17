using Windows.Storage;
using Windows.Storage.Pickers;

namespace Vokabeltrainer.Helpers;

internal static class FileDialogHelper
{
    internal static async Task<string> PickAFileAsync()
    {
        // Create a file picker
        var openPicker = new FileOpenPicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.MainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        openPicker.CommitButtonText = "Auswählen";
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add("*");
        openPicker.FileTypeFilter.Add(".json");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            return file.Path;
        }

        return "";
    }
    
    internal static async Task SaveFileAsync(string content)
    {
        FileSavePicker savePicker = new FileSavePicker();
        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        savePicker.FileTypeChoices.Add("Json", new List<string>() { ".json" });
        savePicker.SuggestedFileName = "Vokabeln.json";

        // Open the picker for the user to pick a file
        StorageFile file= await savePicker.PickSaveFileAsync();
        if (file != null)
            await FileIO.WriteTextAsync(file, content);
    }
}