using System;

namespace MultiMonitorWallpaperSwitcher.Monitor
{
    public class MonitorItem
    {
        public string DeviceId { get; private set; }
        public Rect PositionRect { get; private set; }
        public uint PelsWidth { get; private set; }
        public uint PelsHeight { get; private set; }

        public MonitorItem(string deviceId, Rect rect)
        {
            DeviceId = deviceId;
            PositionRect = rect;
            PelsWidth = (uint)Math.Abs(rect.Right - rect.Left);
            PelsHeight = (uint)Math.Abs(rect.Bottom - rect.Top);
        }
    }
}
