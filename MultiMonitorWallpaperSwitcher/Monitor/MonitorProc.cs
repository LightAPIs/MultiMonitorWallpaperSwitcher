using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorWallpaperSwitcher.Monitor
{
    public class MonitorProc
    {
        public static List<MonitorItem> GetMonitorItems()
        {
            var list = new List<MonitorItem>();
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
            for (uint i = 0; i < wallpaper.GetMonitorDevicePathCount(); i++)
            {
                var monitorId = wallpaper.GetMonitorDevicePathAt(i);
                var rect = wallpaper.GetMonitorRECT(monitorId);
                list.Add(new MonitorItem(monitorId, rect));
            }
            return list;
        }
    }
}
