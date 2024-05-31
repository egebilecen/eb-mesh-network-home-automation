using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace IOTApp.Helpers
{
    public class BoolToOnOffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool _value = (bool)value;

            if(_value) return "Açık";
            else       return "Kapalı";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _value = (string)value;

            if(_value == "Açık") 
                return true;
            else 
                return false;
        }
    }
}
