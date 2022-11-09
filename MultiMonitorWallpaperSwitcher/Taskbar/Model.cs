using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MultiMonitorWallpaperSwitcher.CommandBase;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.KeyMgr;
using MultiMonitorWallpaperSwitcher.Monitor;
using MultiMonitorWallpaperSwitcher.Profile;
using MultiMonitorWallpaperSwitcher.TaskScheduler;

namespace MultiMonitorWallpaperSwitcher.Taskbar
{
    public class NotifyIconViewModel : INotifyPropertyChanged
    {
        private bool nPauseState;
        private string nIconFile;

        public NotifyIconViewModel()
        {
            nPauseState = UserProfile.GetPauseAutoSwitch();
            if (nPauseState)
            {
                nIconFile = "/MMWS_Pause.ico";
            }
            else
            {
                nIconFile = "/MMWS.ico";
            }
        }

        public bool PauseState
        {
            get { return nPauseState; }
            set
            {
                if (nPauseState != value)
                {
                    nPauseState = value;
                    UserProfile.SetPauseAutoSwitch(nPauseState);
                    if (nPauseState)
                    {
                        IconFile = "/MMWS_Pause.ico";
                        TaskProc.StopAllTasks();
                    }
                    else
                    {
                        IconFile = "/MMWS.ico";
                        var monitorItems = MonitorProc.GetMonitorItems();
                        if (monitorItems.Count > 0)
                        {
                            ConfigDataSet.MonitorDataTable monitorData = Glob.Conf.GetAllMonitor();

                            DateTime now = DateTime.Now;
                            foreach (var item in monitorItems)
                            {
                                var mData = ConfigDataSet.GetMonitorRowFromDeviceId(monitorData, item.DeviceId);
                                if (mData != null)
                                {
                                    int intervalTime = (int)mData["interval_time"];
                                    Glob.ExcutionInfo.SetTime(mData["device_id"].ToString()!, now, intervalTime);
                                }
                                else
                                {
                                    Glob.ExcutionInfo.SetTime(item.DeviceId, now, 0);
                                }
                            }
                            TaskProc.SetNextTask();
                        }
                    }
                    Notify(nameof(PauseState));
                }
            }
        }

        public string IconFile
        {
            get { return nIconFile; }
            set
            {
                if (nIconFile != value)
                {
                    nIconFile = value;
                    Notify(nameof(IconFile));
                }
            }
        }

        public ICommand ClickCustomCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        DoubleClickTrayCommandEnum res = UserProfile.GetClickTray();
                        switch (res)
                        {
                            case DoubleClickTrayCommandEnum.SwitchWallpaper:
                                SwitchNextWallpaperSet.Execute(this);
                                break;
                            case DoubleClickTrayCommandEnum.PauseAuto:
                                ChangePauseState.Execute(this);
                                break;
                            case DoubleClickTrayCommandEnum.ShowWindow:
                            default:
                                ShowWindowCommand.Execute(this);
                                break;
                        }
                    }
                };
            }
        }

        public ICommand DoubleCustomCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        DoubleClickTrayCommandEnum res = UserProfile.GetDoubleClickTray();
                        switch (res)
                        {
                            case DoubleClickTrayCommandEnum.ShowWindow:
                                ShowWindowCommand.Execute(this);
                                break;
                            case DoubleClickTrayCommandEnum.PauseAuto:
                                ChangePauseState.Execute(this);
                                break;
                            case DoubleClickTrayCommandEnum.SwitchWallpaper:
                            default:
                                SwitchNextWallpaperSet.Execute(this);
                                break;
                        }
                    }
                };
            }
        }

        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        CommonMethods.ShowWindowMethod();
                    }
                };
            }
        }

        public ICommand SwitchNextWallpaperSet
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        TaskProc.ExecuteTask();
                    }
                };
            }
        }

        /// <summary>
        /// 调整暂停状态
        /// - 该命令只会绑定在双击托盘图标操作上
        /// </summary>
        public ICommand ChangePauseState
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        PauseState = !PauseState;
                    }
                };
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        Glob.Conf.CloseDatabase();
                        foreach (var item in HotKeyManager.HotKeyDic)
                        {
                            HotKeyManager.UnregisterSystemHotKey(item.Key, Glob.HWND);
                        }
                        Glob.NWin?.Dispose();
                        App.Current.Shutdown();
                    }
                };
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Notify(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
