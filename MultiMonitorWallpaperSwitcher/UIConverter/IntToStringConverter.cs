using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultiMonitorWallpaperSwitcher.UIConverter
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = (int)value;
            return val.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = (string)value;
            int val;
            if (!int.TryParse(text, out val))
            {
                return 0;
            }
            else if (val > 10080)
            { //? 10080min = 60min * 24hour * 7day
                return 10080;
            }
            return val;
        }
    }
}
