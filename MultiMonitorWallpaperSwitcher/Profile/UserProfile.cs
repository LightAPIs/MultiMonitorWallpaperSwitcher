﻿using MultiMonitorWallpaperSwitcher.KeyMgr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MultiMonitorWallpaperSwitcher.Profile
{
    public class UserProfile
    {
        /// <summary>
        /// 配置的信息
        /// </summary>
        public static Dictionary<string, string> Options = new Dictionary<string, string>
        {
            // 语言 ID
            { "LangId", "system" },
            // 壁纸的显示方式
            { "WallpaperPosition", "Fill" },
            // 程序启动时是否自动切换壁纸
            { "SwitchOnStartup", "False" },
            // 程序启动时是否显示主窗口
            { "ShowWindowAtStartup", "True" },
            // 暂停自动切换功能
            { "PauseAutoSwitch", "False" },
            // 单击托盘图标执行的命令 - 默认打开主窗口
            { "ClickTray", "0" },
            // 双击托盘图标执行的命令 - 默认切换下一组壁纸
            { "DoubleClickTray", "1" },
            // 双击显示器卡片执行的操作
            { "DoubleClickCard", "0" },
            // 双击文件夹执行的操作
            { "DoubleClickFolder", "0" },
            // 桌面背景色 - 预设值
            { "DesktopBackgroundMode", "0" },
            // 桌面背景色 - 自定义
            { "DesktopBackgroundColor", "0" },
            // 最新版本号
            { "LatestVersion", "0.0.1" },
            // 自动检查更新
            { "AutoCheckAtStartup", "False" },
            // 开机自启动
            { "RunAtSystemStart", "False" },
            // 禁用桌面壁纸 JPEG 质量降低 - 仅 Windows 10 以上有效
            { "DisableQualityReduction", "False" },
            // 显示主窗口热键
            { "ShowWindowHotKey", "None" },
            // 切换下一组壁纸热键
            { "SwitchWallpaperHotKey", "None" },
            // 是否启用日志记录
            { "EnableLogging", "False" }
        };

        /// <summary>
        /// 配置文件的路径
        /// </summary>
        public static string FilePath = Path.Combine(Glob.RootName, "profile.xml");

        /// <summary>
        /// 从文件中加载配置
        /// - 启动时调用
        /// </summary>
        /// <returns></returns>
        public static bool LoadProfile()
        {
            if (!File.Exists(FilePath))
            {
                return false;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlElement root = xmlDoc.DocumentElement!;
            List<string> keys = new List<string>(Options.Keys);
            foreach (string key in keys)
            {
                XmlNode? itemNode = root.SelectSingleNode(key);
                if (itemNode != null)
                {
                    Options[key] = itemNode.InnerText;
                }
            }
            return true;
        }

        public static bool SaveProfile()
        {
            try
            {
                XmlDocument xmlDoc = CreateXMLDocument();
                xmlDoc.Save(FilePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool SetValue(string key, string value)
        {
            if (Options.ContainsKey(key))
            {
                Options[key] = value;
                return SaveProfile();
            }
            return false;
        }

        public static string GetValue(string key, string defaultValue = "")
        {
            if (Options.ContainsKey(key))
            {
                return Options[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// 设置语言 ID
        /// </summary>
        /// <param name="langId"></param>
        /// <returns></returns>
        public static bool SetLangId(string langId)
        {
            return SetValue("LangId", langId);
        }

        /// <summary>
        /// 设置壁纸的显式方式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetWallpaperPosition(string value)
        {
            return SetValue("WallpaperPosition", value);
        }

        /// <summary>
        /// 设置启动时是否自动切换壁纸
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetSwitchOnStartup(bool value)
        {
            return SetValue("SwitchOnStartup", value.ToString());
        }

        /// <summary>
        /// 设置启动时是否显示主窗口
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetShowWindowAtStartup(bool value)
        {
            return SetValue("ShowWindowAtStartup", value.ToString());
        }

        /// <summary>
        /// 设置是否暂停自动切换功能
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPauseAutoSwitch(bool value)
        {
            return SetValue("PauseAutoSwitch", value.ToString());
        }

        /// <summary>
        /// 设置单击托盘图标的操作命令
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetClickTray(DoubleClickTrayCommandEnum value)
        {
            return SetValue("ClickTray", ((int)value).ToString());
        }

        /// <summary>
        /// 设置双击托盘图标的操作命令
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDoubleClickTray(DoubleClickTrayCommandEnum value)
        {
            return SetValue("DoubleClickTray", ((int)value).ToString());
        }

        /// <summary>
        /// 设置双击卡片的操作命令
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDoubleClickCard(DoubleClickCardCommandEnum value)
        {
            return SetValue("DoubleClickCard", ((int)value).ToString());
        }

        /// <summary>
        /// 设置双击文件夹的操作命令
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDoubleClickFolder(DoubleClickFolderCommandEnum value)
        {
            return SetValue("DoubleClickFolder", ((int)value).ToString());
        }

        /// <summary>
        /// 设置桌面背影色模式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDesktopBackgroundMode(DesktopBackgroundModeEnum value)
        {
            return SetValue("DesktopBackgroundMode", ((int)value).ToString());
        }

        /// <summary>
        /// 设置桌面背影色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool SetDesktopBackgroundColor(uint color)
        {
            return SetValue("DesktopBackgroundColor", color.ToString());
        }

        /// <summary>
        /// 设置最新版本号
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool SetLatestVersion(string version)
        {
            return SetValue("LatestVersion", version);
        }

        /// <summary>
        /// 设置启动时自动检查更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetAutoCheckAtStartup(bool value)
        {
            return SetValue("AutoCheckAtStartup", value.ToString());
        }

        /// <summary>
        /// 设置是否开机自启动
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetRunAtSystemStart(bool value)
        {
            return SetValue("RunAtSystemStart", value.ToString());
        }

        /// <summary>
        /// 设置是否禁用桌面壁纸质量降低
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetDisableQualityReduction(bool value)
        {
            return SetValue("DisableQualityReduction", value.ToString());
        }

        /// <summary>
        /// 设置显示主窗口热键
        /// </summary>
        /// <param name="hotKey"></param>
        /// <returns></returns>
        public static bool SetShowWindowHotKey(HotKey hotKey)
        {
            return SetValue("ShowWindowHotKey", hotKey.GetKeys());
        }

        /// <summary>
        /// 设置切换下一组壁纸热键
        /// </summary>
        /// <param name="hotKey"></param>
        /// <returns></returns>
        public static bool SetSwitchWallpaperHotKey(HotKey hotKey)
        {
            return SetValue("SwitchWallpaperHotKey", hotKey.GetKeys());
        }

        /// <summary>
        /// 设置是否启用日志记录
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetEnableLogging(bool value)
        {
            return SetValue("EnableLogging", value.ToString());
        }

        /// <summary>
        /// 获取语言 ID
        /// </summary>
        /// <returns></returns>
        public static string GetLangId()
        {
            return GetValue("LangId", "system");
        }

        /// <summary>
        /// 获取壁纸的显示方式
        /// </summary>
        /// <returns></returns>
        public static string GetWallpaperPosition()
        {
            return GetValue("WallpaperPosition", "Fill");
        }

        /// <summary>
        /// 获取启动时是否自动切换壁纸
        /// </summary>
        /// <returns></returns>
        public static bool GetSwitchOnStartup()
        {
            string value = GetValue("SwitchOnStartup", "False");
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取启动时是否显示主窗口
        /// </summary>
        /// <returns></returns>
        public static bool GetShowWindowAtStartup()
        {
            string value = GetValue("ShowWindowAtStartup", "True");
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (FormatException)
            {
                return true;
            }
        }

        /// <summary>
        /// 获取是否暂停自动切换功能
        /// </summary>
        /// <returns></returns>
        public static bool GetPauseAutoSwitch()
        {
            string value = GetValue("PauseAutoSwitch", "False");
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取单击托盘图标的操作命令
        /// </summary>
        /// <returns></returns>
        public static DoubleClickTrayCommandEnum GetClickTray()
        {
            string value = GetValue("ClickTray", "0");
            int.TryParse(value, out int index);
            if (Enum.IsDefined(typeof(DoubleClickTrayCommandEnum), index))
            {
                return (DoubleClickTrayCommandEnum)index;
            }
            else
            {
                return DoubleClickTrayCommandEnum.ShowWindow;
            }
        }

        /// <summary>
        /// 获取双击托盘图标的操作命令
        /// </summary>
        /// <returns></returns>
        public static DoubleClickTrayCommandEnum GetDoubleClickTray()
        {
            string value = GetValue("DoubleClickTray", "1");
            int.TryParse(value, out int index);
            if (Enum.IsDefined(typeof(DoubleClickTrayCommandEnum), index))
            {
                return (DoubleClickTrayCommandEnum)index;
            }
            else
            {
                return DoubleClickTrayCommandEnum.SwitchWallpaper;
            }
        }

        /// <summary>
        /// 获取双击卡片的操作命令
        /// </summary>
        /// <returns></returns>
        public static DoubleClickCardCommandEnum GetDoubleClickCard()
        {
            string value = GetValue("DoubleClickCard", "0");
            int.TryParse(value, out int index);
            if (Enum.IsDefined(typeof(DoubleClickCardCommandEnum), index))
            {
                return (DoubleClickCardCommandEnum)index;
            }
            else
            {
                return DoubleClickCardCommandEnum.ViewCurrentImage;
            }
        }

        /// <summary>
        /// 获取双击文件夹的操作命令
        /// </summary>
        /// <returns></returns>
        public static DoubleClickFolderCommandEnum GetDoubleClickFolder()
        {
            string value = GetValue("DoubleClickFolder", "0");
            int.TryParse(value, out int index);
            if (Enum.IsDefined(typeof(DoubleClickFolderCommandEnum), index))
            {
                return (DoubleClickFolderCommandEnum)index;
            }
            else
            {
                return DoubleClickFolderCommandEnum.OpenFolder;
            }
        }

        /// <summary>
        /// 获取桌面背影色模式
        /// </summary>
        /// <returns></returns>
        public static DesktopBackgroundModeEnum GetDesktopBackgroundMode()
        {
            string value = GetValue("DesktopBackgroundMode", "0");
            int.TryParse(value, out int index);
            if (Enum.IsDefined(typeof(DesktopBackgroundModeEnum), index))
            {
                return (DesktopBackgroundModeEnum)index;
            }
            else
            {
                return DesktopBackgroundModeEnum.Black;
            }
        }

        /// <summary>
        /// 获取桌面背影色
        /// </summary>
        /// <returns></returns>
        public static uint GetDesktopBackgroundColor()
        {
            string value = GetValue("DesktopBackgroundColor", "0");
            uint.TryParse(value, out uint index);
            return index;
        }

        /// <summary>
        /// 获取最新版本号
        /// </summary>
        /// <returns></returns>
        public static string GetLatestVersion()
        {
            return GetValue("LatestVersion", "0.0.1");
        }

        /// <summary>
        /// 获取启动时自动检查更新
        /// </summary>
        /// <returns></returns>
        public static bool GetAutoCheckAtStartup()
        {
            string value = GetValue("AutoCheckAtStartup", "False");
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取是否开机自启动
        /// </summary>
        /// <returns></returns>
        public static bool GetRunAtSystemStart()
        {
            string value = GetValue("RunAtSystemStart", "False");
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取是否禁用桌面壁纸质量降低
        /// </summary>
        /// <returns></returns>
        public static bool GetDisableQualityReduction()
        {
            string value = GetValue("DisableQualityReduction", "False");
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取显示主窗口热键
        /// </summary>
        /// <returns></returns>
        public static HotKey GetShowWindowHotKey()
        {
            string value = GetValue("ShowWindowHotKey", "None");
            HotKey hotKey = new HotKey(value);
            return hotKey;
        }

        /// <summary>
        /// 获取切换下一组壁纸热键
        /// </summary>
        /// <returns></returns>
        public static HotKey GetSwitchWallpaperHotKey()
        {
            string value = GetValue("SwitchWallpaperHotKey", "None");
            HotKey hotKey = new HotKey(value);
            return hotKey;
        }

        /// <summary>
        /// 获取是否启用日志记录
        /// </summary>
        /// <returns></returns>
        public static bool GetEnableLogging()
        {
            string value = GetValue("EnableLogging", "False");
            try
            {
                return Convert.ToBoolean(value);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static XmlDocument CreateXMLDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration xmlDeclar = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.AppendChild(xmlDeclar);

            XmlElement xmlEle = xmlDoc.CreateElement("", "profile", "");
            xmlDoc.AppendChild(xmlEle);

            XmlNode root = xmlDoc.SelectSingleNode("profile")!;
            List<string> keys = new List<string>(Options.Keys);
            foreach (string key in keys)
            {
                XmlElement xe = xmlDoc.CreateElement(key);
                xe.InnerText = Options[key];
                root.AppendChild(xe);
            }

            return xmlDoc;
        }
    }

    public enum DoubleClickTrayCommandEnum
    {
        ShowWindow = 0,
        SwitchWallpaper = 1,
        PauseAuto = 2
    }

    public enum DesktopBackgroundModeEnum
    {
        Black = 0,
        Custom = 1,
        White = 2,
        DarkGray = 3,
        LightGray = 4,
        Red = 5,
        Green = 6,
        Blue = 7,
        Yellow = 8,
        Cyan = 9,
        Purple = 10,
    }

    public enum DoubleClickCardCommandEnum
    {
        ViewCurrentImage = 0,
        SwitchNextWallpaper = 1,
        SetSpecifiedImage = 2,
        RefreshCurrentCard = 3,
        ViewSettingImage = 4,
        ViewLastSettingImage = 5,
        OpenDirectoryOfCurrentImage = 6,
        OpenDirectoryOfSettingImage = 7,
        OpenDirectoryOfLastSettingImage = 8
    }

    public enum DoubleClickFolderCommandEnum
    {
        OpenFolder = 0,
        SetWallpaperByFolder = 1
    }
}
