using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultiMonitorWallpaperSwitcher.UIConverter
{
    [ValueConversion(typeof(long), typeof(string))]
    public class LongToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not null)
            {
                long count = (long)value;
                return count.ToString();
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object paramater, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
