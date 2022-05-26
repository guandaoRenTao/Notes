using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Text.RegularExpressions;
using Telerik.XamarinForms.RichTextEditor;
namespace Notes.Helpers
{
    class HTMLToStr : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
            string result = "";
            Regex regex = new Regex(@"<img.*>");
            Regex regex1 = new Regex(@"<.*?>");
            result = regex.Replace((string)value, " [изображение] ");
            result = regex1.Replace(result, " ");
            return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value;
    }
}
}
