﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LanguageResources;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.Monitor;
using MultiMonitorWallpaperSwitcher.Wallpaper;
using MultiMonitorWallpaperSwitcher.CommandBase;
using MultiMonitorWallpaperSwitcher.TaskScheduler;
using MultiMonitorWallpaperSwitcher.Profile;

namespace MultiMonitorWallpaperSwitcher.Data
{
    public class MonitorDataItems : ObservableCollection<MonitorData>
    {
        private double scaleValue;
        private double startX;
        private double startY;
        private int maxWidth;
        private int maxHeight;
        private int xMin;
        private int yMin;
        private readonly double frameWidth;
        private readonly double frameHeight;

        public MonitorDataItems(double frameWidth, double frameHeight) : base()
        {
            // 读取本地显示器数据
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            var monitorItems = MonitorProc.GetMonitorItems();
            if (monitorItems.Count > 0)
            {
                Initialization(monitorItems);
            }
        }

        /// <summary>
        /// 刷新内容
        /// </summary>
        public void RefreshContent()
        {
            // 读取本地显示器数据
            var monitorItems = MonitorProc.GetMonitorItems();
            // 清空数据
            this.Clear();
            if (monitorItems.Count > 0)
            {
                Initialization(monitorItems);
            }
        }

        /// <summary>
        /// 切换壁纸
        /// </summary>
        public void SwitchWallpaperSet()
        {
            Dictionary<string, string> keyValuePairs = TaskProc.ExecuteTask();
            foreach (var item in this)
            {
                if (keyValuePairs.ContainsKey(item.DeviceId))
                {
                    item.WallpaperPath = keyValuePairs[item.DeviceId];
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="monitorItems"></param>
        private void Initialization(List<MonitorItem> monitorItems)
        {
            List<int> xList = new();
            List<int> yList = new();
            foreach (MonitorItem mi in monitorItems)
            {
                xList.Add(mi.PositionRect.Left);
                xList.Add(mi.PositionRect.Right);
                yList.Add(mi.PositionRect.Top);
                yList.Add(mi.PositionRect.Bottom);
            }

            xMin = xList.Min();
            yMin = yList.Min();
            maxWidth = xList.Max() - xMin;
            maxHeight = yList.Max() - yMin;
            if ((double)maxWidth / maxHeight >= frameWidth / frameHeight)
            {
                scaleValue = maxWidth / frameWidth;
                startX = 0;
                startY = (frameHeight - (maxHeight / scaleValue)) / 2;
            }
            else
            {
                scaleValue = maxHeight / frameHeight;
                startX = (frameWidth - (maxWidth / scaleValue)) / 2;
                startY = 0;
            }

            ConfigDataSet.MonitorDataTable monitorData = Glob.Conf.GetAllMonitor();

            foreach (MonitorItem mi in monitorItems)
            {
                double x = (mi.PositionRect.Left - xMin) / scaleValue + startX;
                double y = (mi.PositionRect.Top - yMin) / scaleValue + startY;
                double width = mi.PelsWidth / scaleValue;
                double height = mi.PelsHeight / scaleValue;
                string deviceId = mi.DeviceId;
                int intervalTime = 0;
                string folders = string.Empty;

                string wallpaperPath = WallpaperProc.GetPathOfDesktopWallpaper(mi.DeviceId);

                ConfigDataSet.MonitorRow? mRow = ConfigDataSet.GetMonitorRowFromDeviceId(monitorData, deviceId);
                if (mRow == null)
                { //! 不存在该显示器配置，写入新配置
                    Glob.Conf.SetMonitor(deviceId, intervalTime, folders);
                }
                else
                {
                    intervalTime = (int)mRow["interval_time"];
                    folders = mRow["folder_list"].ToString()!;
                }

                MonitorData md = new MonitorData(folders, intervalTime)
                {
                    DeviceId = deviceId,
                    PelsWidth = mi.PelsWidth,
                    PelsHeight = mi.PelsHeight,
                    PositionX = mi.PositionRect.Left,
                    PositionY = mi.PositionRect.Top,
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    WallpaperPath = wallpaperPath,
                };
                this.Add(md);
            }
        }
    }

    public class MonitorData : INotifyPropertyChanged
    {
        private double mX;
        private double mY;
        private double mWidth;
        private double mHeight;
        private uint mPelsWidth;
        private uint mPelsHeight;
        private int mPositionX;
        private int mPositionY;
        private string mDeviceId = string.Empty;
        private int mIntervalTime;
        private string mFolderList = string.Empty;
        private string mWallpaperPath = string.Empty;
        private string mWallpaperInfo = string.Empty;
        private long mFileLength = 0;
        private ImageInfo mImageInfo = new();
        private static readonly string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
        public AddCommand AddFolderCommand { get; }
        public ObservableCollection<FolderData> Folders { get; }

        public MonitorData(string folderList, int iTime)
        {
            mFolderList = folderList;
            mIntervalTime = iTime;
            AddFolderCommand = new AddCommand()
            {
                AddHandler = AddFolder
            };
            Folders = new ObservableCollection<FolderData>();
            string[] list = mFolderList.Split("|");
            foreach (string folder in list)
            {
                if (!string.IsNullOrEmpty(folder))
                {
                    FolderData fd = new FolderData(RemoveFolder, CheckFolder, SetWallpaperFromFolder);
                    fd.Parse(folder);
                    if (fd.Path.Length > 0)
                    {
                        Folders.Add(fd);
                    }
                }
            }
        }

        public double X
        {
            get { return mX; }
            set
            {
                if (mX != value)
                {
                    mX = value;
                    Notify(nameof(X));
                }
            }
        }

        public double Y
        {
            get { return mY; }
            set
            {
                if (mY != value)
                {
                    mY = value;
                    Notify(nameof(Y));
                }
            }
        }

        public double Width
        {
            get { return mWidth; }
            set
            {
                if (mWidth != value)
                {
                    mWidth = value;
                    Notify(nameof(Width));
                }
            }
        }

        public double Height
        {
            get { return mHeight; }
            set
            {
                if (mHeight != value)
                {
                    mHeight = value;
                    Notify(nameof(Height));
                }
            }
        }

        public uint PelsWidth
        {
            get { return mPelsWidth; }
            set
            {
                if (mPelsWidth != value)
                {
                    mPelsWidth = value;
                    Notify(nameof(PelsWidth));
                }
            }
        }

        public uint PelsHeight
        {
            get { return mPelsHeight; }
            set
            {
                if (mPelsHeight != value)
                {
                    mPelsHeight = value;
                    Notify(nameof(PelsHeight));
                }
            }
        }

        public int PositionX
        {
            get { return mPositionX; }
            set
            {
                if (mPositionX != value)
                {
                    mPositionX = value;
                    Notify(nameof(PositionX));
                }
            }
        }

        public int PositionY
        {
            get { return mPositionY; }
            set
            {
                if (mPositionY != value)
                {
                    mPositionY = value;
                    Notify(nameof(PositionY));
                }
            }
        }

        public string DeviceId
        {
            get { return mDeviceId; }
            set
            {
                if (mDeviceId != value)
                {
                    mDeviceId = value;
                    Notify(nameof(DeviceId));
                }
            }
        }

        public int IntervalTime
        {
            get { return mIntervalTime; }
            set
            {
                if (mIntervalTime != value)
                {
                    mIntervalTime = value;
                    Glob.Conf.UpdateMonitorIntervalTime(mDeviceId, mIntervalTime);
                    Notify(nameof(IntervalTime));
                    TaskProc.StopAllTasks();
                    Glob.ExcutionInfo.SetTime(mDeviceId, DateTime.Now, mIntervalTime);
                    TaskProc.SetNextTask();
                }
            }
        }

        public string WallpaperPath
        {
            get { return mWallpaperPath; }
            set
            {
                if (mWallpaperPath != value)
                {
                    mWallpaperPath = value;
                    //* 读取文件信息
                    mFileLength = GetFileLength(value);
                    mImageInfo = GetImageInfo(value);
                    WallpaperInfo = $"{Resource.FileLocation}: {mWallpaperPath}\n{Resource.Filesize}: {SizeConvert(mFileLength)}\n{Resource.ImageSize}: {mImageInfo.Width:F0}x{mImageInfo.Height:F0}";
                    Notify(nameof(WallpaperPath));
                }
            }
        }

        public string ResolutionText
        {
            get
            {
                return $"{mPelsWidth}x{mPelsHeight} ({mPositionX},{mPositionY})";
            }
        }

        public string WallpaperInfo
        {
            get { return mWallpaperInfo; }
            set
            {
                if (mWallpaperInfo != value)
                {
                    mWallpaperInfo = value;
                    Notify(nameof(WallpaperInfo));
                }
            }
        }

        /// <summary>
        /// 手动切换下一张壁纸
        /// </summary>
        public ICommand SwitchNextWallpaper
        {
            get
            {
                return new ContextCommand
                {
                    CanExecuteFunc = _paramater => mIntervalTime > 0,
                    CommandAction = _paramater =>
                    {
                        if (mIntervalTime > 0)
                        {
                            string imageFile = TaskProc.ExecuteOneTask(mDeviceId, mIntervalTime, mFolderList);
                            if (!string.IsNullOrEmpty(imageFile))
                            {
                                WallpaperPath = imageFile;
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// 设置为指定的图片
        /// </summary>
        public ICommand SetSpecifiedImage
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        OpenFileDialog ofd = new OpenFileDialog
                        {
                            Filter = Resource.Image + "|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.tif;*.dib;*.jfif;*.jpe;*.wdp",
                            Title = Resource.SelectImage,
                            RestoreDirectory = true,
                            CheckFileExists = true,
                        };
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            string imagePath = ofd.FileName;
                            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                            {
                                TaskProc.ExecuteOneTaskByImage(mDeviceId, mIntervalTime, imagePath);
                                WallpaperPath = imagePath;
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// 刷新当前卡片内容
        /// </summary>
        public ICommand RefreshCardContent
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        string wallpaperPath = WallpaperProc.GetPathOfDesktopWallpaper(mDeviceId);
                        if (!string.IsNullOrEmpty(wallpaperPath))
                        {
                            WallpaperPath = wallpaperPath;
                        }

                        //* 刷新目录中的图片数量
                        foreach (var item in Folders)
                        {
                            item.RefreshCount();
                        }
                    }
                };
            }
        }

        /// <summary>
        /// 使用默认图片浏览器打开当前卡片内的图片
        /// </summary>
        public ICommand ShowCurrentWallpaperImage
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        OpenImageFile(mWallpaperPath);
                    }
                };
            }
        }

        /// <summary>
        /// 使用系统文件管理器打开当前卡片内图片所在目录
        /// </summary>
        public ICommand ShowCurrentWallpaperDir
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        OpenImageDir(mWallpaperPath);
                    }
                };
            }
        }

        /// <summary>
        /// 使用默认图片浏览器打开记录中当前图片
        /// </summary>
        public ICommand ShowCurrentRecordImage
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        string path = Glob.Conf.GetMonitorCurPath(mDeviceId);
                        OpenImageFile(path);
                    }
                };
            }
        }

        /// <summary>
        /// 使用系统文件管理器打开记录中当前图片所在目录
        /// </summary>
        public ICommand ShowCurrentRecordDir
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        string path = Glob.Conf.GetMonitorCurPath(mDeviceId);
                        OpenImageDir(path);
                    }
                };
            }
        }

        /// <summary>
        /// 使用默认图片浏览器打开记录中上次图片
        /// </summary>
        public ICommand ShowLastRecordImage
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        string path = Glob.Conf.GetMonitorLastPath(mDeviceId);
                        OpenImageFile(path);
                    }
                };
            }
        }

        /// <summary>
        /// 使用系统文件管理器打打开记录中上次图片所在目录
        /// </summary>
        public ICommand ShowLastRecordDir
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        string path = Glob.Conf.GetMonitorLastPath(mDeviceId);
                        OpenImageDir(path);
                    }
                };
            }
        }

        public ICommand DoubleClickCard
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        DoubleClickCardCommandEnum res = UserProfile.GetDoubleClickCard();
                        switch (res)
                        {
                            case DoubleClickCardCommandEnum.SwitchNextWallpaper:
                                SwitchNextWallpaper.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.SetSpecifiedImage:
                                SetSpecifiedImage.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.RefreshCurrentCard:
                                RefreshCardContent.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.ViewSettingImage:
                                ShowCurrentRecordImage.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.ViewLastSettingImage:
                                ShowLastRecordImage.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.OpenDirectoryOfCurrentImage:
                                ShowCurrentWallpaperDir.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.OpenDirectoryOfSettingImage:
                                ShowCurrentRecordDir.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.OpenDirectoryOfLastSettingImage:
                                ShowLastRecordDir.Execute(this);
                                break;
                            case DoubleClickCardCommandEnum.ViewCurrentImage:
                            default:
                                ShowCurrentWallpaperImage.Execute(this);
                                break;
                        }
                    }
                };
            }
        }

        public void AddFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string outDir = dialog.SelectedPath;
                bool isExists = false;
                foreach (var item in Folders)
                {
                    if (item.Path == outDir || item.Path.IndexOf(outDir) > -1 || outDir.IndexOf(item.Path) > -1)
                    {
                        isExists = true;
                        break;
                    }
                }

                if (!isExists)
                {
                    mFolderList += (mFolderList.Length > 0 ? "|" : "") + outDir + "*1";
                    Glob.Conf.UpdateMonitorFolderList(mDeviceId, mFolderList);
                    Folders.Add(new FolderData(RemoveFolder, CheckFolder, SetWallpaperFromFolder)
                    {
                        Path = outDir,
                        Enabled = true,
                    });
                }
            }
        }

        private void RemoveFolder(object parameter)
        {
            string folder = parameter.ToString()!;
            string res = "";
            int removeIndex = -1;
            for (int i = 0; i < Folders.Count; i++)
            {
                if (Folders[i].Path == folder)
                {
                    removeIndex = i;
                }
                else
                {
                    res += (res.Length > 0 ? "|" : "") + Folders[i].ToString();
                }
            }

            if (removeIndex > -1)
            {
                Folders.RemoveAt(removeIndex);
                Glob.Conf.UpdateMonitorFolderList(mDeviceId, res);
                mFolderList = res;
            }
        }

        private void CheckFolder(object parameter)
        {
            string folder = parameter.ToString()!;
            string res = "";
            bool checkState = false;
            foreach (FolderData fd in Folders)
            {
                if (fd.Path == folder)
                {
                    checkState = true;
                }
                res += (res.Length > 0 ? "|" : "") + fd.ToString();
            }

            if (checkState)
            {
                Glob.Conf.UpdateMonitorFolderList(mDeviceId, res);
                mFolderList = res;
            }
        }

        private void SetWallpaperFromFolder(object parameter)
        {
            string folder = parameter.ToString()!;
            string imageFile = TaskProc.ExecuteOneTaskByFolder(mDeviceId, mIntervalTime, folder);
            if (!string.IsNullOrEmpty(imageFile))
            {
                WallpaperPath = imageFile;
            }
        }

        private void OpenImageFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    Process.Start("explorer.exe", "\"" + filePath + "\"");
                }
                catch (Exception) { }
            }
        }

        private void OpenImageDir(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                DirectoryInfo? dir = Directory.GetParent(filePath);
                if (dir != null)
                {
                    if (Directory.Exists(dir.FullName))
                    {
                        try
                        {
                            Process.Start("explorer.exe", "\"" + dir.FullName + "\"");
                        }
                        catch (Exception) { }
                    }
                }
            }
        }

        private long GetFileLength(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    return fileInfo.Length;
                }
                catch { }
            }
            return 0;
        }

        private ImageInfo GetImageInfo(string filePath)
        {
            ImageInfo info = new();
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    using (Stream ms = new MemoryStream(File.ReadAllBytes(filePath))) {
                        var decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnLoad);
                        var frame = decoder.Frames[0];
                        info.Width = frame.PixelWidth;
                        info.Height = frame.PixelHeight;
                    }
                }
                catch { }
            }
            return info;
        }

        private string SizeConvert(long bytes)
        {
            double cur = bytes;
            foreach (string unit in units)
            {
                if (cur / 1024.0 < 1)
                {
                    return cur.ToString("F2") + unit;
                }
                cur /= 1024.0;
            }
            return bytes.ToString() + units[0];
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

    public class FolderData : INotifyPropertyChanged
    {
        private string fPath = string.Empty;
        private bool fEnabled = false;
        private uint fCount = 0;
        public GenericCommand RemoveCommand { get; }
        public GenericCommand CheckCommand { get; }
        public GenericCommand SetWallpaperCommand { get; }

        public FolderData(GenericCommand.HandlerDelegate del, GenericCommand.HandlerDelegate check, GenericCommand.HandlerDelegate setWallpaper)
        {
            RemoveCommand = new GenericCommand()
            {
                Handler = del
            };
            CheckCommand = new GenericCommand()
            {
                Handler = check
            };
            SetWallpaperCommand = new GenericCommand()
            {
                Handler = setWallpaper
            };
        }

        public string Path
        {
            get { return fPath; }
            set
            {
                if (value != fPath)
                {
                    fPath = value;
                    Count = WallpaperProc.GetImagesCountFromPath(fPath);
                    Notify(nameof(Path));
                }
            }
        }

        public bool Enabled
        {
            get { return fEnabled; }
            set
            {
                if (value != fEnabled)
                {
                    fEnabled = value;
                    Notify(nameof(Enabled));
                }
            }
        }

        public uint Count
        {
            get { return fCount; }
            set
            {
                if (fCount != value)
                {
                    fCount = value;
                    Notify(nameof(Count));
                }
            }
        }

        public ICommand OpenFolder
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = _paramater =>
                    {
                        if (Directory.Exists(fPath))
                        {
                            try
                            {
                                Process.Start("explorer.exe", "\"" + fPath + "\"");
                            }
                            catch (Exception) { }
                        }
                    }
                };
            }
        }

        public ICommand DoubleClickFolder
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        DoubleClickFolderCommandEnum res = UserProfile.GetDoubleClickFolder();
                        switch (res)
                        {
                            case DoubleClickFolderCommandEnum.SetWallpaperByFolder:
                                SetWallpaperCommand.Execute(fPath);
                                break;
                            case DoubleClickFolderCommandEnum.OpenFolder:
                            default:
                                OpenFolder.Execute(this);
                                break;
                        }
                    }
                };
            }
        }

        public void RefreshCount()
        {
            Count = WallpaperProc.GetImagesCountFromPath(fPath);
        }

        public void Parse(string str)
        {
            string[] cond = str.Split('*');
            if (cond.Length == 2)
            {
                Path = cond[0];
                if (string.IsNullOrEmpty(cond[1]))
                {
                    Enabled = false;
                }
                else
                {
                    int.TryParse(cond[1], out int keyIndex);
                    if (keyIndex == 1)
                    {
                        Enabled = true;
                    }
                    else
                    {
                        Enabled = false;
                    }
                }
            }
            else
            {
                Path = "";
                Enabled = false;
            }
        }

        public override string ToString()
        {
            string key = fEnabled ? "1" : "0";
            return $"{fPath}*{key}";
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

    internal class ImageInfo
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
