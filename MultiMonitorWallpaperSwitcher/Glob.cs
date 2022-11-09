using System;
using System.IO;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.TaskScheduler;
using MultiMonitorWallpaperSwitcher.KeyMgr;

namespace MultiMonitorWallpaperSwitcher
{
    public class Glob
    {
        public static string RootName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MultiMonitorWallpaperSwitcher");
        public static ConfigDB Conf = new ConfigDB("config");
        public static ExecutionTime ExcutionInfo = new ExecutionTime();
        public static bool AutoUpdateChecked = false;
        public static IntPtr HWND;
        public static NWindow? NWin;
    }
}
