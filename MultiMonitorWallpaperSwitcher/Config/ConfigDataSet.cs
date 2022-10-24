using System;

namespace MultiMonitorWallpaperSwitcher.Config
{
    public partial class ConfigDataSet
    {
        public static MonitorRow? GetMonitorRowFromDeviceId(MonitorDataTable monitorData, string deviceId)
        {
            foreach (MonitorRow row in monitorData)
            {
                if (row["device_id"].ToString() == deviceId)
                {
                    return row;
                }
            }
            return null;
        }
    }
}