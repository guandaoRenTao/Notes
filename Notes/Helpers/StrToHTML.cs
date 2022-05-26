using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Essentials;
using Telerik.XamarinForms.RichTextEditor;
namespace Notes.Helpers
{
    class StrToHTML : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
            string htmlSource = "";
            if (value != null)
                htmlSource = (string)value;
            if ((String.IsNullOrEmpty(htmlSource)))
                htmlSource = "<p><span style='color: #666;'> </span></p>";
            return RichTextSource.FromString(htmlSource);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (string)value;
    }
}
}
