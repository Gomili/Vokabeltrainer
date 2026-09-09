using Microsoft.UI.Xaml.Data;

namespace Vokabeltrainer.Converter;

public class DateTimeToTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
        if (value is DateTime dateTime)
        {
            if (string.Equals(parameter as string, "DatumZeit", StringComparison.Ordinal))
            {
                // Warum: Im Verlauf muss neben der Uhrzeit auch der Kalendertag sichtbar
                // bleiben, damit gleichzeitige Uhrzeiten verschiedener Tage unterscheidbar sind.
                return dateTime.ToString("dd.MM.yyyy · HH:mm");
            }

            return dateTime.ToString("HH:mm:ss");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
