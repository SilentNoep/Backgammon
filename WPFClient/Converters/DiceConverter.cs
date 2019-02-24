using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFClient.Converters
{
    public class DiceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string myPath = @"/Assets/";

            if (value is int)
            {
                switch ((int)value)
                {
                    case 0:
                        return myPath + "quest.jpg";
                    case 1:
                        return myPath + "1.png";
                    case 2:
                        return myPath + "2.png";
                    case 3:
                        return myPath + "3.png";
                    case 4:
                        return myPath + "4.png";
                    case 5:
                        return myPath + "5.png";
                    case 6:
                        return myPath + "6.png";
                }
            }
            else if (value is bool)
            {
                if ((bool)value)
                    return 0.5;
                else
                    return 1;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
