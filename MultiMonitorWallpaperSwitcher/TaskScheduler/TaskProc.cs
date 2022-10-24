using FluentScheduler;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.Monitor;
using MultiMonitorWallpaperSwitcher.Profile;
using MultiMonitorWallpaperSwitcher.Wallpaper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorWallpaperSwitcher.TaskScheduler
{
    public class TaskProc
    {
        /// <summary>
        /// 设置下一个定时任务
        /// - 方法内部已处理暂停时的情况
        /// </summary>
        public static void SetNextTask()
        {
            bool pauseState = UserProfile.GetPauseAutoSwitch();
            if (!pauseState)
            {
                var nextTaskInfo = Glob.ExcutionInfo.GetNextTaskInfo();
                if (nextTaskInfo.Time != 0)
                {
                    MonitorJob mj = new MonitorJob(nextTaskInfo.DeviceItems);
                    JobManager.AddJob(mj, s => s.ToRunOnceIn(nextTaskInfo.Time).Minutes());
                }
            }
        }

        /// <summary>
        /// 停止所有任务
        /// </summary>
        public static void StopAllTasks()
        {
            JobManager.RemoveAllJobs();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns>设备 ID 与图片路径对字典</returns>
        public static Dictionary<string, string> ExecuteTask()
        {
            StopAllTasks();
            Dictionary<string, string> result = new Dictionary<string, string>();
            var monitorItems = MonitorProc.GetMonitorItems();
            if (monitorItems.Count > 0)
            {
                ConfigDataSet.MonitorDataTable monitorData = Glob.Conf.GetAllMonitor();

                DateTime now = DateTime.Now;
                foreach ( var item in monitorItems )
                {
                    var mData = ConfigDataSet.GetMonitorRowFromDeviceId(monitorData, item.DeviceId);
                    if (mData != null)
                    {
                        int intervalTime = (int)mData["interval_time"];
                        Glob.ExcutionInfo.SetTime(mData["device_id"].ToString()!, now, intervalTime);
                        if (intervalTime > 0)
                        {
                            string path = WallpaperProc.SetRondomWallpaper(mData);
                            if (result.ContainsKey(item.DeviceId))
                            {
                                result[item.DeviceId] = path;
                            }
                            else
                            {
                                result.Add(item.DeviceId, path);
                            }
                        }
                    }
                    else
                    {
                        Glob.ExcutionInfo.SetTime(item.DeviceId, now, 0);
                    }
                }
            }
            SetNextTask();
            return result;
        }

        /// <summary>
        /// 根据设备 ID 列表执行任务
        /// - 无返回值
        /// </summary>
        /// <param name="items">设备 ID 列表</param>
        public static void ExecuteTask(List<string> items)
        {
            StopAllTasks();
            var monitorItems = MonitorProc.GetMonitorItems();
            if (monitorItems.Count > 0)
            {
                ConfigDataSet.MonitorDataTable monitorData = Glob.Conf.GetAllMonitor();
                WallpaperProc.SetUserEnvironmentValue();
                DateTime now = DateTime.Now;
                foreach (var item in monitorItems)
                {
                    if (items.Contains(item.DeviceId))
                    {
                        var mData = ConfigDataSet.GetMonitorRowFromDeviceId(monitorData, item.DeviceId);
                        if (mData != null)
                        {
                            Glob.ExcutionInfo.SetTime(mData["device_id"].ToString()!, now, (int)mData["interval_time"]);
                            WallpaperProc.SetRondomWallpaper(mData);
                        }
                        else
                        {
                            Glob.ExcutionInfo.SetTime(item.DeviceId, now, 0);
                        }
                    }
                }
            }
            SetNextTask();
        }

        /// <summary>
        /// 执行单个指定任务
        /// - 方法内部不会再去读取数据库内容
        /// </summary>
        /// <param name="deviceId">设备 ID</param>
        /// <param name="intervalTime"></param>
        /// <param name="folderList"></param>
        /// <returns>返回所设置的壁纸路径</returns>
        public static string ExecuteOneTask(string deviceId, int intervalTime, string folderList)
        {
            StopAllTasks();
            string imageFile = string.Empty;
            var monitorItems = MonitorProc.GetMonitorItems();
            if (monitorItems.Count > 0)
            {
                WallpaperProc.SetUserEnvironmentValue();
                DateTime now = DateTime.Now;
                foreach (var item in monitorItems)
                {
                    if (deviceId == item.DeviceId)
                    {
                        Glob.ExcutionInfo.SetTime(deviceId, now, intervalTime);
                        imageFile = WallpaperProc.SetRondomWallpaper(deviceId, folderList);
                    }
                }
            }
            SetNextTask();
            return imageFile;
        }

        /// <summary>
        /// 通过指定图片路径执行单个任务
        /// </summary>
        /// <param name="deviceId">设备 ID</param>
        /// <param name="intervalTime"></param>
        /// <param name="imageFile"></param>
        public static void ExecuteOneTaskByImage(string deviceId, int intervalTime, string imageFile)
        {
            StopAllTasks();
            if (!string.IsNullOrEmpty(imageFile) && File.Exists(imageFile))
            {
                var monitorItems = MonitorProc.GetMonitorItems();
                if (monitorItems.Count > 0 )
                {
                    WallpaperProc.SetUserEnvironmentValue();
                    DateTime now = DateTime.Now;
                    foreach (var item in monitorItems)
                    {
                        if (deviceId == item.DeviceId)
                        {
                            Glob.ExcutionInfo.SetTime(deviceId, now, intervalTime);
                            WallpaperProc.SetWallpaper(deviceId, imageFile);
                        }
                    }
                }
            }
            SetNextTask();
        }
    }
}
