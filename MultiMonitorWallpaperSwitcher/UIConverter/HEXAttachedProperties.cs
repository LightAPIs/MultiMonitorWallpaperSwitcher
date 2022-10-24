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
    public class HexStringAttachedProperties
    {
        public static readonly DependencyProperty IsHexStringProperty = DependencyProperty.RegisterAttached("IsHexString", typeof(bool), typeof(TextBox), new PropertyMetadata(false, (d, e) =>
        {
            if (d is TextBox tb)
            {
                //? IsHexString 属性指定为 true 时，则禁用输入法系统(如：IME)输入
                tb.SetValue(InputMethod.IsInputMethodEnabledProperty, !(bool)e.NewValue);
                tb.PreviewTextInput -= HexInput;
                if ((bool)e.NewValue)
                {
                    tb.PreviewTextInput += HexInput;
                }
            }
        }));

        private static void HexInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9a-fA-F]+").IsMatch(e.Text);
        }

        public static bool GetIsHexString(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHexStringProperty);
        }

        public static void SetIsHexString(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHexStringProperty, value);
        }
    }
}
