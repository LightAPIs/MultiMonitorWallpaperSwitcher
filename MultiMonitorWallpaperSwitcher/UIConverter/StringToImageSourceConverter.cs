using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MultiMonitorWallpaperSwitcher.UIConverter
{
    public class StringToImageSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;
            if (!string.IsNullOrEmpty(path))
            {
                return GetImage(path);
            }
            else
            {
                return null;
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private BitmapImage? GetImage(string imagePath)
        {
            try
            {
                BitmapImage bi = new BitmapImage();
                if (File.Exists(imagePath))
                {
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                    {
                        bi.StreamSource = ms;
                        bi.DecodePixelHeight = 120;
                        bi.EndInit();
                        bi.Freeze();
                    }
                }
                return bi;
            }
            catch
            {
                return null;
            }
        }
    }
}
