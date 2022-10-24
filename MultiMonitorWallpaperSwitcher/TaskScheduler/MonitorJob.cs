using FluentScheduler;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.Monitor;
using MultiMonitorWallpaperSwitcher.Wallpaper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MultiMonitorWallpaperSwitcher.TaskScheduler
{
    public class MonitorJob : IJob
    {
        private readonly List<string> deviceItems;
        public MonitorJob(List<string> items)
        {
            deviceItems = items;
        }
        public void Execute()
        {
            TaskProc.ExecuteTask(deviceItems);
        }
    }
}
