using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Snoop.Client
{
    public class BooleanToVisibilityValueConverter : IValueConverter
    {
        public bool IsInverted { get; set; }
        public Visibility VisibilityWhenFalse { get; set; } = Visibility.Hidden;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MaybeInvert(value is bool v && v) switch
            {
                true => Visibility.Visible,
                _ => VisibilityWhenFalse
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return MaybeInvert(
                value switch
                {
                    Visibility.Visible => true,
                    _ => false
                });
        }

        private bool MaybeInvert(bool value)
        {
            return IsInverted switch
            {
                true => !value,
                _ => value
            };
        }
    }
}
