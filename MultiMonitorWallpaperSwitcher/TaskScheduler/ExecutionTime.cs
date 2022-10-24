using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorWallpaperSwitcher.TaskScheduler
{
    public class ExecutionTime
    {
        private readonly Dictionary<string, TimeRecord> _timeDictionary;
        private readonly NextTask _nextTask;

        private struct TimeRecord
        {
            public DateTime LastTime { get; set; }
            public int IntervalTime { get; set; }
        }

        public ExecutionTime()
        {
            _timeDictionary = new Dictionary<string, TimeRecord>();
            _nextTask = new NextTask();
        }

        public void SetTime(string key, DateTime lastTime, int intervalTime)
        {
            TimeRecord tr = new TimeRecord
            {
                LastTime = lastTime,
                IntervalTime = intervalTime
            };

            if (_timeDictionary.ContainsKey(key))
            {
                _timeDictionary[key] = tr;
            }
            else
            {
                _timeDictionary.Add(key, tr);
            }
        }

        public NextTask GetNextTaskInfo()
        {
            _nextTask.Time = 1;
            _nextTask.DeviceItems.Clear();

            DateTime now = DateTime.Now;
            Dictionary<string, TimeSpan> spanDic = new();
            TimeSpan minTs = TimeSpan.Zero;
            foreach (var timeItem in _timeDictionary)
            {
                DateTime dt = timeItem.Value.LastTime.AddMinutes(timeItem.Value.IntervalTime);
                if (DateTime.Compare(dt, now) > 0)
                {
                    TimeSpan ts = dt - now;
                    spanDic.Add(timeItem.Key, ts);
                    if (minTs.Equals(TimeSpan.Zero))
                    {
                        minTs = ts;
                    }
                    else if (ts < minTs)
                    {
                        minTs = ts;
                    }
                }
                else if (timeItem.Value.IntervalTime > 0)
                {   //? 如果出现这种情况，那可能是任务被漏执行或者显示器被移除
                    _nextTask.DeviceItems.Add(timeItem.Key);
                }
            }

            if (minTs.Equals(TimeSpan.Zero))
            {
                _nextTask.Time = 0;
            }
            else
            {
                _nextTask.Time = (int)Math.Round(minTs.TotalMinutes);
                if (_nextTask.Time < 1)
                {
                    _nextTask.Time = 1;
                }
            }
            foreach (var span in spanDic)
            {
                TimeSpan sub = span.Value.Subtract(minTs).Duration();
                if (sub.TotalMinutes < 1)
                {
                    _nextTask.DeviceItems.Add(span.Key);
                }
            }

            return _nextTask;
        }
    }

    public class NextTask
    {
        public int Time { get; set; } = 1;
        public List<string> DeviceItems { get; set; } = new List<string>();
    }
}
