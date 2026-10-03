using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using Vokabeltrainer.Core.Models;

namespace Vokabeltrainer.DisplayClasses;

public class DisplayLernVokabel : INotifyPropertyChanged
{
    private string _antwort = "";
    private string _richtig = "";
    private string _loesung = "";
    private bool _speakButtonEnabled;
    public Vokabel Vokabel { get; set; }
    public string Vorgabe { get; }
    public string ErwarteteAntwort { get; }

    public IRelayCommand<DisplayLernVokabel> SpeakCommand { get; set; }
    
    public string Antwort
    {
        get => _antwort;
        set => SetField(ref _antwort, value);
    }

    public string Richtig
    {
        get => _richtig;
        set => SetField(ref _richtig, value);
    }

    public string Loesung
    {
        get => _loesung;
        set => SetField(ref _loesung, value);
    }

    public bool SpeakButtonEnabled
    {
        get => _speakButtonEnabled;
        set => SetField(ref _speakButtonEnabled, value);
    }

    public DisplayLernVokabel(
        Vokabel vokabel,
        Lernsprache lernsprache,
        Action<DisplayLernVokabel> speakAction)
    {
        Vokabel = vokabel;
        Richtig = "";
        Vorgabe = lernsprache == Lernsprache.Latein ? Vokabel.Englisch : Vokabel.Deutsch;
        ErwarteteAntwort = lernsprache == Lernsprache.Latein ? Vokabel.Deutsch : Vokabel.Englisch;
        
        if (Vokabel.Zaehler == 100)
            Loesung = ErwarteteAntwort;
        else
            Loesung = string.Empty;
        
        SpeakCommand = new RelayCommand<DisplayLernVokabel>(displayLernVokabel =>
        {
            if (displayLernVokabel is not null)
            {
                speakAction(displayLernVokabel);
            }
        });
    }

    public bool IstAntwortRichtig() =>
        Antwort.Trim() == ErwarteteAntwort.Trim();
    
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
