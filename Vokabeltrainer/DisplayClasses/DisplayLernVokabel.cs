using System.ComponentModel;
using System.Runtime.CompilerServices;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.DisplayClasses;

public class DisplayLernVokabel : INotifyPropertyChanged
{
    private string _englisch = "";
    private string _richtig = "";
    private string _englischRichtig = "";
    public Vokabel Vokabel { get; set; }
    public string Deutsch { get; set; }

    public string Englisch
    {
        get => _englisch;
        set => SetField(ref _englisch, value);
    }

    public string Richtig
    {
        get => _richtig;
        set => SetField(ref _richtig, value);
    }

    public string EnglischRichtig
    {
        get => _englischRichtig;
        set => SetField(ref _englischRichtig, value);
    }

    public DisplayLernVokabel(Vokabel vokabel)
    {
        Vokabel = vokabel;
        Richtig = "O";
        Deutsch = Vokabel.Deutsch;
        EnglischRichtig = string.Empty;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}