using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Essentials;
using Telerik.XamarinForms.RichTextEditor;
namespace Notes.Helpers
{
    class HeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int c = Regex.Matches((string)value, "</p>").Count;
            c += Regex.Matches((string)value, "</br>").Count;
            return c*40;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value;
        }
    }
}
