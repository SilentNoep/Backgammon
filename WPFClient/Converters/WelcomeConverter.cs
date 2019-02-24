using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFClient.Converters
{
    public class WelcomeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string lol = "";
            string myValue = "";
            if (value != null)
            {
                lol = "Welcome ";
                myValue = (string)value;

            }
            string nelol = lol + myValue;
            return nelol;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
