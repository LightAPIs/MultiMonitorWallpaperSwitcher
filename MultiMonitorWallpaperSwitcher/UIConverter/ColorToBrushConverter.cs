using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiMonitorWallpaperSwitcher.UIConverter
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null)
            {
                uint.TryParse(value.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var color);
                byte[] bytes = BitConverter.GetBytes(color);
                // bytes 结果有 4 位，但不使用 alpha 通道，即不用 bytes[3]
                return new SolidColorBrush(Color.FromRgb(bytes[2], bytes[1], bytes[0]));
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
