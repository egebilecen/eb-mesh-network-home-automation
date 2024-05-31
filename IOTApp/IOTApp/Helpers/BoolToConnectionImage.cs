using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace IOTApp.Helpers
{
    public class BoolToConnectionImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool _value = (bool)value;

            if(_value) return "conn.png";
            else       return "no_conn.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _value = (string)value;

            if(_value == "conn.png") 
                return true;
            else 
                return false;
        }
    }
}
