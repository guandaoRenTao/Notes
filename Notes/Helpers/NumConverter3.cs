using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Xamarin.Forms;
namespace Notes.Helpers
{
    class NumConverter3 : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)value > 3;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? 1 : 0;
    }
}
}
