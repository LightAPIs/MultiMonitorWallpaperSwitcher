using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MultiMonitorWallpaperSwitcher.Data;
using MultiMonitorWallpaperSwitcher.KeyMgr;
using MultiMonitorWallpaperSwitcher.Profile;

namespace MultiMonitorWallpaperSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MonitorDataItems? monitorDataItems;
        private ProfileData? profileData;

        public MainWindow(bool checkUpdate = false)
        {
            InitializeComponent();
            profileData = new ProfileData(checkUpdate);
            WindowTabControl.DataContext = profileData;
            WindowViewModel wModel = new WindowViewModel(
                () =>
                {
                    monitorDataItems?.RefreshContent();
                },
                () =>
                {
                    monitorDataItems?.SwitchWallpaperSet();
                });
            MonitorButtonSet.DataContext = wModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            monitorDataItems = new MonitorDataItems(MonitorListBox.ActualWidth, MonitorListBox.ActualHeight - MonitorGroupBox.Padding.Bottom);
            MonitorListBox.ItemsSource = monitorDataItems;
        }

        private void MonitorScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            args.RoutedEvent = UIElement.MouseWheelEvent;
            args.Source = sender;
            MonitorScrollViewer.RaiseEvent(args);
        }

        private void ProfileScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            args.RoutedEvent = UIElement.MouseWheelEvent;
            args.Source = sender;
            ProfileScrollViewer.RaiseEvent(args);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (profileData != null)
            {   //? 处理在编辑热键时关闭窗口的情况
                if (profileData.CanSetShowWindowHotKey && profileData.ShowWindowHotKey != "None")
                {
                    HotKeyManager.RegisterSystemHotKey(HotKeyManager.HotKeySet.ShowWindow, UserProfile.GetShowWindowHotKey(), Glob.HWND);
                }
                if (profileData.CanSetSwitchWallpaperHotKey && profileData.SwitchWallpaperHotKey != "None")
                {
                    HotKeyManager.RegisterSystemHotKey(HotKeyManager.HotKeySet.SwitchWallpaper, UserProfile.GetSwitchWallpaperHotKey(), Glob.HWND);
                }
            }

            WindowTabControl.DataContext = null;
            MonitorListBox.ItemsSource = null;
            profileData = null;
            monitorDataItems = null;
        }
    }
}
