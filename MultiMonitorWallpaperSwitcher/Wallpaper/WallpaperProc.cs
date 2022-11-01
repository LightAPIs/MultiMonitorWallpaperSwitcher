using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MultiMonitorWallpaperSwitcher.Config;
using MultiMonitorWallpaperSwitcher.Monitor;
using MultiMonitorWallpaperSwitcher.Profile;

namespace MultiMonitorWallpaperSwitcher.Wallpaper
{
    public class WallpaperProc
    {
        /// <summary>
        /// 支持设置为壁纸的图片文件格式字典
        /// </summary>
        private static readonly Dictionary<string, int> ExtDic = new Dictionary<string, int>()
        {
            { ".png", 1 },
            { ".jpg", 1 },
            { ".jpeg", 1 },
            { ".gif", 1 },
            { ".bmp", 1 },
            { ".tif", 1 },
            { ".dib", 1 },
            { ".jfif", 1 },
            { ".jpe", 1 },
            { ".wdp", 1 }
        };

        /// <summary>
        /// 根据设备 ID 获取当前桌面壁纸图片路径
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static string GetPathOfDesktopWallpaper(string deviceId)
        {
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
            return wallpaper.GetWallpaper(deviceId);
        }

        /// <summary>
        /// 为显示器设定指定图片为壁纸
        /// - 方法内部已自动更新数据库
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="path"></param>
        public static void SetWallpaper(string deviceId, string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
                wallpaper.SetWallpaper(deviceId, path);
                Glob.Conf.UpdateMonitorWallpaperPath(deviceId, path);
            }
        }

        /// <summary>
        /// 通过文件夹为显示器设定壁纸
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="folder"></param>
        /// <returns>返回设定的图片路径</returns>
        public static string SetWallpaperByFolder(string deviceId, string folder)
        {
            List<string> list = new List<string>() { folder };
            string imageFile = GetRandomImageFromPathList(list);
            SetWallpaper(deviceId, imageFile);
            return imageFile;
        }

        private static string GetRandomImageFromPathList(List<string> dirs)
        {
            List<string> files = GetImageFilesFromPathList(dirs);
            if (files.Count > 0)
            {
                return files.PickRandom();
            }
            else
            {
                return string.Empty;
            }
        }

        private static List<string> GetImageFilesFromPathList(List<string> dirs)
        {
            List<string> files = new List<string>();
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        string ext = file.Extension.ToLower();
                        if (ExtDic.ContainsKey(ext))
                        {
                            files.Add(file.FullName);
                        }
                    }

                    List<string> oDirs = new List<DirectoryInfo>(dirInfo.GetDirectories()).ConvertAll<string>(x => x.FullName);
                    foreach (string subFile in GetImageFilesFromPathList(oDirs))
                    {
                        files.Add(subFile);
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// 转换字符串为具体路径
        /// </summary>
        /// <param name="str">待转换字符串</param>
        /// <returns>未启用时返回空字符串</returns>
        private static string ConvertPathString(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string[] cond = str.Split('*');
                if (cond.Length == 2)
                {
                    if (!string.IsNullOrEmpty(cond[1]))
                    {
                        int.TryParse(cond[1], out int keyIndex);
                        if (keyIndex == 1)
                        {
                            return cond[0];
                        }
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 转换颜色值
        /// 由于系统使用颜色字节顺序不同，即为 0x00bbggrr，故需要转换
        /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/gdi/colorref"/>
        /// </summary>
        /// <param name="color">0x00rrggbb 颜色值</param>
        /// <returns></returns>
        private static uint ConvertColorValue(uint color)
        {
            byte[] bytes = BitConverter.GetBytes(color);
            byte[] trans = new byte[bytes.Length];
            trans[3] = bytes[3];
            trans[2] = bytes[0];
            trans[1] = bytes[1];
            trans[0] = bytes[2];
            return BitConverter.ToUInt32(trans, 0);
        }

        /// <summary>
        /// 获取目录下的图片数量
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static uint GetImagesCountFromPath(string dir)
        {
            uint count = 0;
            if (Directory.Exists(dir))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    string ext = file.Extension.ToLower();
                    if (ExtDic.ContainsKey(ext))
                    {
                        count++;
                    }
                }

                List<string> oDirs = new List<DirectoryInfo>(dirInfo.GetDirectories()).ConvertAll<string>(x => x.FullName);
                foreach (string oDir in oDirs)
                {
                    count += GetImagesCountFromPath(oDir);
                }
            }
            return count;
        }

        /// <summary>
        /// 设置随机壁纸
        /// </summary>
        /// <param name="mRow"></param>
        /// <returns>返回所设置的壁纸图片路径</returns>
        public static string SetRondomWallpaper(ConfigDataSet.MonitorRow mRow)
        {
            string deviceId = mRow["device_id"].ToString()!;
            string folderList = mRow["folder_list"].ToString()!;
            return SetRondomWallpaper(deviceId, folderList);
        }

        /// <summary>
        /// 设置单张随机壁纸
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="folderList"></param>
        /// <returns>返回所设置的壁纸图片路径</returns>
        public static string SetRondomWallpaper(string deviceId, string folderList)
        {
            string[] list = folderList.Split("|");
            List<string> useList = new List<string>();
            foreach (var item in list)
            {
                string path = ConvertPathString(item);
                if (!string.IsNullOrEmpty(path))
                {
                    useList.Add(path);
                }
            }
            string imageFile = GetRandomImageFromPathList(useList);
            SetWallpaper(deviceId, imageFile);
            return imageFile;
        }

        /// <summary>
        /// 设置桌面背影色
        /// - 已在方法内部读取用户配置并判定
        /// </summary>
        /// <returns></returns>
        public static void SetDesktopBackgroundColor()
        {
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
            uint cur = wallpaper.GetBackgroundColor();
            uint userValue = ConvertColorValue(UserProfile.GetDesktopBackgroundColor());
            if (cur != userValue)
            {
                wallpaper.SetBackgroundColor(userValue);
            }
        }

        /// <summary>
        /// 设置壁纸的显示方式
        /// - 已在方法内部读取用户配置并判定
        /// </summary>
        public static void SetWallpaperPosition()
        {
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
            string cur = Enum.GetName(typeof(DesktopWallpaperPosition), wallpaper.GetPosition())!;
            string userValue = UserProfile.GetWallpaperPosition();
            if (cur != userValue)
            {
                wallpaper.SetPosition((DesktopWallpaperPosition)Enum.Parse(typeof(DesktopWallpaperPosition), userValue));
            }
        }

        /// <summary>
        /// 设置用户所配置的桌面环境
        /// - 设置桌面背景色和壁纸的显示方式
        /// - 已在方法内部读取用户配置并判定
        /// </summary>
        public static void SetUserEnvironmentValue()
        {
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
            uint curColor = wallpaper.GetBackgroundColor();
            uint userColor = ConvertColorValue(UserProfile.GetDesktopBackgroundColor());
            if (curColor != userColor)
            {
                wallpaper.SetBackgroundColor(userColor);
            }
            string curPos = Enum.GetName(typeof(DesktopWallpaperPosition), wallpaper.GetPosition())!;
            string userPos = UserProfile.GetWallpaperPosition();
            if (curPos != userPos)
            {
                wallpaper.SetPosition((DesktopWallpaperPosition)Enum.Parse(typeof(DesktopWallpaperPosition), userPos));
            }
        }

        /// <summary>
        /// 通过系统值获取主窗口中卡片里图片的显示方式
        /// </summary>
        /// <returns></returns>
        public static Stretch GetCardImageStretchModeByLocal()
        {
            var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
            var position = wallpaper.GetPosition();
            return GetCardImageStretchModeByValue(position);
        }

        /// <summary>
        /// 通过指定字符串值获取主窗口中卡片里图片的显示方式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Stretch GetCardImageStretchModeByValue(string value)
        {
            DesktopWallpaperPosition position = (DesktopWallpaperPosition)Enum.Parse(typeof(DesktopWallpaperPosition), value);
            return GetCardImageStretchModeByValue(position);
        }

        /// <summary>
        /// 通过指定值获取主窗口中卡片里图片的显示方式
        /// <see cref="https://learn.microsoft.com/zh-cn/dotnet/api/system.windows.media.stretch?view=windowsdesktop-6.0"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Stretch GetCardImageStretchModeByValue(DesktopWallpaperPosition value)
        {
            return value switch
            {
                DesktopWallpaperPosition.Fit => Stretch.Uniform,
                DesktopWallpaperPosition.Stretch => Stretch.Fill,
                DesktopWallpaperPosition.Tile or DesktopWallpaperPosition.Center or DesktopWallpaperPosition.Span => Stretch.None,
                _ => Stretch.UniformToFill,
            };
        }
    }

    public static class Extensions
    {
        private static Random rnd = new Random();
        public static T PickRandom<T>(this IList<T> source)
        {
            int randIndex = rnd.Next(source.Count);
            return source[randIndex];
        }
    }
}
