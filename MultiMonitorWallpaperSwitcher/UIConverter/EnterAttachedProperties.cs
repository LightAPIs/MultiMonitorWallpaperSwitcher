using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MultiMonitorWallpaperSwitcher.UIConverter
{
    public class EnterAttachedProperties
    {
        public static readonly DependencyProperty EnterPressProperty = DependencyProperty.RegisterAttached("EnterPress", typeof(bool), typeof(TextBox), new PropertyMetadata(false, (d, e) =>
        {
            if (d is TextBox tb)
            {
                tb.PreviewKeyDown -= EnterPressInput;
                if ((bool)e.NewValue)
                {
                    tb.PreviewKeyDown += EnterPressInput;
                }
            }
        }));

        private static void EnterPressInput(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateSource(e.Source);
            }
        }

        private static void UpdateSource(object source)
        {
            if (!GetEnterPress(source as DependencyObject))
            {
                return;
            }

            if (source is not UIElement elt)
            {
                return;
            }
            DependencyProperty prop = TextBox.TextProperty;

            BindingExpression binding = BindingOperations.GetBindingExpression(elt, prop);
            binding?.UpdateSource();
        }

        public static bool GetEnterPress(DependencyObject? obj)
        {
            if (obj != null)
            {
                return (bool)obj.GetValue(EnterPressProperty);
            }
            return false;
        }

        public static void SetEnterPress(DependencyObject obj, bool value)
        {
            obj.SetValue(EnterPressProperty, value);
        }
    }
}
