using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EdlinSoftware.MemoryCards.UI.Views.Support.Converters
{
    public class ProgressBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeLeft = (int) value;

            return timeLeft > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}