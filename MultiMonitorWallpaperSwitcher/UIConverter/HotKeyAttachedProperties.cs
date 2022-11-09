using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultiMonitorWallpaperSwitcher.UIConverter
{
    public class HotKeyAttachedProperties
    {
        public static readonly DependencyProperty HotKeyProperty = DependencyProperty.RegisterAttached("HotKey", typeof(bool), typeof(TextBox), new PropertyMetadata(false, (d, e) =>
        {
            if (d is TextBox tb)
            {
                //? HotKey 属性指定为 true 时，则禁用输入法系统(如：IME)输入
                tb.SetValue(InputMethod.IsInputMethodEnabledProperty, !(bool)e.NewValue);
                tb.PreviewKeyDown -= HotKeyPressInput;
                if ((bool)e.NewValue)
                {
                    tb.PreviewKeyDown += HotKeyPressInput;
                }
            }
        }));

        private static void HotKeyPressInput(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && textBox.IsEnabled)
            {
                string res = "";
                if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) || e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt))
                {
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        res += "Ctrl+";
                    }

                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Alt))
                    {
                        res += "Alt+";
                    }

                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        res += "Shift+";
                    }

                    if ((e.Key >= Key.F1 && e.Key <= Key.F12) || (e.Key >= Key.A && e.Key <= Key.Z))
                    {
                        res += e.Key.ToString();
                    }
                    else if (e.Key >= Key.D0 && e.Key <= Key.D9)
                    {
                        res += e.Key.ToString().Replace("D", "");
                    }
                    else
                    {
                        res = "None";
                    }
                }
                else
                {
                    res = "None";
                }
                textBox.Text = res;
                e.Handled = true;
            }
        }

        public static bool GetHotKey(DependencyObject? obj)
        {
            if (obj != null)
            {
                return (bool)obj.GetValue(HotKeyProperty);
            }
            return false;
        }

        public static void SetHotKey(DependencyObject obj, bool value)
        {
            obj.SetValue(HotKeyProperty, value);
        }
    }
}
