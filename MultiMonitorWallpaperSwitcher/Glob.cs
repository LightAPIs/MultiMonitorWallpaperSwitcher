using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.TaskScheduler;
using System.IO;

namespace MultiMonitorWallpaperSwitcher
{
    public class Glob
    {
        public static string RootName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MultiMonitorWallpaperSwitcher");
        public static ConfigDB Conf = new ConfigDB("config");
        public static ExecutionTime ExcutionInfo = new ExecutionTime();
        public static bool AutoUpdateChecked = false;
    }
}
