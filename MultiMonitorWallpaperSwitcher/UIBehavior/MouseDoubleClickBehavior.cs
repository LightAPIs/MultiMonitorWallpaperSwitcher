using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MultiMonitorWallpaperSwitcher.UIBehavior
{
    public class MouseDoubleClickBehavior : Behavior<FrameworkElement>
    {
        private uint count = 0;

        public string PathValue
        {
            get { return (string)GetValue(PathValueProperty); }
            set
            {
                SetValue(PathValueProperty, value);
            }
        }

        public static readonly DependencyProperty PathValueProperty = DependencyProperty.Register("PathValue", typeof(string), typeof(MouseDoubleClickBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            count++;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 350);
            timer.Tick += (s, el) => { timer.IsEnabled = false; count = 0; };
            timer.IsEnabled = true;
            if (count >= 2)
            {
                timer.IsEnabled = false;
                count = 0;
                OpenPathValue();
            }
        }

        private void OpenPathValue()
        {
            if (!string.IsNullOrEmpty(PathValue))
            {
                if (File.Exists(PathValue) || Directory.Exists(PathValue))
                {
                    try
                    {
                        Process.Start("explorer.exe", PathValue);
                    }
                    catch { }
                }
            }
        }
    }
}
