using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Vokabeltrainer.Converter;

public class StringToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string stringValue)
        {
            if (stringValue == "") return new SolidColorBrush(Colors.Transparent);
            if (stringValue == "👍") return new SolidColorBrush(Colors.Green);
            if (stringValue == "👎") return new SolidColorBrush(Colors.Red);
        }
        
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}