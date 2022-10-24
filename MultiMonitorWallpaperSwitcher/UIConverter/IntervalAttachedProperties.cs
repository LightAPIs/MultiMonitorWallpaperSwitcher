using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultiMonitorWallpaperSwitcher.UIConverter
{
    public class IntervalAttachedProperties
    {
        public static readonly DependencyProperty IsOnlyNumberProperty = DependencyProperty.RegisterAttached("IsOnlyNumber", typeof(bool), typeof(TextBox), new PropertyMetadata(false, (d, e) =>
        {
            if (d is TextBox tb)
            {
                //? IsOnlyNumber 属性指定为 true 时，则禁用输入法系统(如：IME)输入
                tb.SetValue(InputMethod.IsInputMethodEnabledProperty, !(bool)e.NewValue);
                tb.PreviewTextInput -= NumInput;
                if ((bool)e.NewValue)
                {
                    tb.PreviewTextInput += NumInput;
                }
            }
        }));

        private static void NumInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        public static bool GetIsOnlyNumber(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsOnlyNumberProperty);
        }

        public static void SetIsOnlyNumber(DependencyObject obj, bool value)
        {
            obj.SetValue(IsOnlyNumberProperty, value);
        }
    }
}
