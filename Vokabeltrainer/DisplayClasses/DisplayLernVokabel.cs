using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.DisplayClasses;

public class DisplayLernVokabel : INotifyPropertyChanged
{
    private string _englisch = "";
    private string _richtig = "";
    private string _englischRichtig = "";
    private bool _speakButtonEnabled;
    public Vokabel Vokabel { get; set; }
    public string Deutsch { get; set; }

    public IRelayCommand<DisplayLernVokabel> SpeakCommand { get; set; }
    
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

    public bool SpeakButtonEnabled
    {
        get => _speakButtonEnabled;
        set => SetField(ref _speakButtonEnabled, value);
    }

    public DisplayLernVokabel(Vokabel vokabel, Action<DisplayLernVokabel> speakAction)
    {
        Vokabel = vokabel;
        Richtig = "";
        Deutsch = Vokabel.Deutsch;
        
        if (Vokabel.Zaehler == 100)
            EnglischRichtig = Vokabel.Englisch;
        else
            EnglischRichtig = string.Empty;
        
        SpeakCommand = new RelayCommand<DisplayLernVokabel>(speakAction);
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