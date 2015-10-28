using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EdlinSoftware.MemoryCards.UI.ModelViews;

namespace EdlinSoftware.MemoryCards.UI.Views.Support.Converters
{
    public class CardStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (CardState) value;

            switch (state)
            {
                case CardState.Hidden:
                    return new SolidColorBrush(Colors.Red);
                case CardState.TemporarilyOpened:
                case CardState.Opened:
                    return new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri((string)parameter)),
                        Viewbox = new Rect(new Point(0,0), new Size(1,1)),
                        Stretch = Stretch.Fill
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}