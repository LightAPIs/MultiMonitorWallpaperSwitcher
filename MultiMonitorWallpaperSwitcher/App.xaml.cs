using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MultiMonitorWallpaperSwitcher.Monitor;
using MultiMonitorWallpaperSwitcher.Profile;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.TaskScheduler;
using MultiMonitorWallpaperSwitcher.Wallpaper;
using FluentScheduler;
using System.Diagnostics;
using MultiMonitorWallpaperSwitcher.KeyMgr;

namespace MultiMonitorWallpaperSwitcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon? taskbarIcon;
        private Mutex? mutex;

        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            UserProfile.LoadProfile();
            SetLanguageDictionary();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //* 禁止启动多个实例
            mutex = new Mutex(true, Process.GetCurrentProcess().ProcessName);
            if (mutex.WaitOne(0, false))
            {
                taskbarIcon = (TaskbarIcon)FindResource("Taskbar");
                base.OnStartup(e);
            }
            else
            {
                this.Shutdown();
            }

            JobManager.Initialize();

            var monitorItems = MonitorProc.GetMonitorItems();
            if (monitorItems.Count > 0)
            {
                ConfigDataSet.MonitorDataTable monitorData = Glob.Conf.GetAllMonitor();

                //* 启动时是否自动切换壁纸
                bool switchOnStautup = UserProfile.GetSwitchOnStartup();

                DateTime now = DateTime.Now;
                foreach (var item in monitorItems)
                {
                    var mData = ConfigDataSet.GetMonitorRowFromDeviceId(monitorData, item.DeviceId);
                    if (mData != null)
                    {
                        int intervalTime = (int)mData["interval_time"];
                        Glob.ExcutionInfo.SetTime(mData["device_id"].ToString()!, now, intervalTime);
                        if (switchOnStautup && intervalTime > 0)
                        {
                            WallpaperProc.SetRondomWallpaper(mData);
                        }
                    }
                    else
                    {
                        Glob.ExcutionInfo.SetTime(item.DeviceId, now, 0);
                    }
                }
                TaskProc.SetNextTask();
            }

            //* 注册热键消息处理
            Glob.NWin = new NWindow();

            //* 注册热键
            HotKey showHotKey = UserProfile.GetShowWindowHotKey();
            if (showHotKey.GetKeys() != "None")
            {
                HotKeyManager.RegisterSystemHotKey(HotKeyManager.HotKeySet.ShowWindow, showHotKey, Glob.HWND);
            }
            HotKey switchHotKey = UserProfile.GetSwitchWallpaperHotKey();
            if (switchHotKey.GetKeys() != "None")
            {
                HotKeyManager.RegisterSystemHotKey(HotKeyManager.HotKeySet.SwitchWallpaper, switchHotKey, Glob.HWND);
            }

            //* 启动时自动检查更新
            bool autoCheck = UserProfile.GetAutoCheckAtStartup();
            if (!autoCheck)
            {
                Glob.AutoUpdateChecked = true;
            }

            //* 启动时显示主窗口
            if (UserProfile.GetShowWindowAtStartup())
            {
                if (App.Current.MainWindow == null)
                {
                    App.Current.MainWindow = new MainWindow(autoCheck);
                    App.Current.MainWindow.Show();
                    Glob.AutoUpdateChecked = true;
                }
            }
        }

        /// <summary>
        /// 初始化软件语言
        /// </summary>
        private void SetLanguageDictionary()
        {
            switch (UserProfile.GetLangId())
            {
                case "en":
                    LanguageResources.Resource.Culture = new System.Globalization.CultureInfo("en");
                    break;
                case "zh-Hans":
                    LanguageResources.Resource.Culture = new System.Globalization.CultureInfo("zh-Hans");
                    break;
            }
            //* 否则自动读取系统语言
        }
    }
}
