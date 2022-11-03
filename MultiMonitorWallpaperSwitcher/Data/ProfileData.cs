using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LanguageResources;
using MultiMonitorWallpaperSwitcher.CommandBase;
using MultiMonitorWallpaperSwitcher.Profile;
using MultiMonitorWallpaperSwitcher.Wallpaper;
using MultiMonitorWallpaperSwitcher.Update;
using MultiMonitorWallpaperSwitcher.RegistryMgr;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace MultiMonitorWallpaperSwitcher.Data
{
    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    internal class ShellLink
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    internal interface IShellLink
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    public class ProfileData : INotifyPropertyChanged
    {
        private readonly string pVersion;
        private string pLatestVersion;
        private bool pIsUpdate;
        private bool pIsShowVersionText;
        private string pVersionTextColor;
        private string pVersionText;
        private string pCheckButtonTip;
        private string pCheckButtonKind;
        private bool pIsChecking;
        private string pLangId;
        private string pWallpaperPosition;
        private Stretch pStretchMode;
        private bool pSwitchOnStartup;
        private bool pShowWindowAtStartup;
        private bool pAutoCheckAtStartup;
        private bool pRunAtSystemStart;
        private DoubleClickTrayCommandEnum pDoubleClickTray;
        private DesktopBackgroundModeEnum pDesktopBackgroundMode;
        private uint pDesktopBackgroundColor;
        private bool pDisableQualityReduction;

        /// <summary>
        /// 版本格式
        /// </summary>
        private static readonly int[] Standard = new int[3] { 8, 4, 2 };

        public ProfileData(bool autoCheckUpdate = false)
        {
            Version? ver = Application.ResourceAssembly.GetName().Version;
            if (ver != null)
            {
                pVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}";
            }
            else
            {
                pVersion = "0.0.1";
            }
            pLatestVersion = UserProfile.GetLatestVersion();
            pIsUpdate = CompareVer(pVersion, pLatestVersion);
            pIsShowVersionText = pIsUpdate;
            pVersionTextColor = pIsUpdate ? "Red" : "Blue";
            pVersionText = pIsUpdate ? Resource.HadUpdate + "v" + pLatestVersion : Resource.IsLatest;
            pCheckButtonTip = pIsUpdate ? Resource.GoUpgrade : Resource.CheckUpdate;
            pCheckButtonKind = pIsUpdate ? "ArrowUpBoldCircle" : "RefreshCircle";
            pIsChecking = false;

            pLangId = UserProfile.GetLangId();
            pWallpaperPosition = UserProfile.GetWallpaperPosition();
            //? 当前系统壁纸的实际显示方式可能与设置值不一定相同，但是该 StretchMode 值只在卡片上的预览时使用，并且一旦经过程序设定壁纸后值就会相等，所以以设置值为准
            pStretchMode = WallpaperProc.GetCardImageStretchModeByValue(pWallpaperPosition);
            pSwitchOnStartup = UserProfile.GetSwitchOnStartup();
            pShowWindowAtStartup = UserProfile.GetShowWindowAtStartup();
            pAutoCheckAtStartup = UserProfile.GetAutoCheckAtStartup();
            pRunAtSystemStart = UserProfile.GetRunAtSystemStart();
            pDoubleClickTray = UserProfile.GetDoubleClickTray();
            pDesktopBackgroundMode = UserProfile.GetDesktopBackgroundMode();
            pDesktopBackgroundColor = UserProfile.GetDesktopBackgroundColor();
            pDisableQualityReduction = UserProfile.GetDisableQualityReduction();

            if (autoCheckUpdate)
            {
                pIsChecking = true;
                IsShowVersionText = true;
                VersionText = Resource.Checking;
                UpdateCheck uCheck = new UpdateCheck();
                uCheck.HasUpdated += HasUpdated;
                Task task = Task.Run(() => { uCheck.UpdateChecking(); });
            }
        }

        public string LangId
        {
            get { return pLangId; }
            set
            {
                if (LangId != value)
                {
                    pLangId = value;
                    UserProfile.SetLangId(pLangId);
                    Notify(nameof(LangId));
                    if (pLangId != "system")
                    {
                        LanguageResources.Resource.Culture = new System.Globalization.CultureInfo(pLangId);
                    }
                    else
                    {
                        LanguageResources.Resource.Culture = new System.Globalization.CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
                    }
                }
            }
        }

        public string WallpaperPosition
        {
            get { return pWallpaperPosition; }
            set
            {
                if (pWallpaperPosition != value)
                {
                    pWallpaperPosition = value;
                    UserProfile.SetWallpaperPosition(pWallpaperPosition);
                    Notify(nameof(WallpaperPosition));
                    WallpaperProc.SetWallpaperPosition();
                    StretchMode = WallpaperProc.GetCardImageStretchModeByValue(pWallpaperPosition);
                }
            }
        }

        /// <summary>
        /// 卡片内图片的 Stretch 模式
        /// </summary>
        public Stretch StretchMode
        {
            get { return pStretchMode; }
            set
            {
                if (pStretchMode != value)
                {
                    pStretchMode = value;
                    Notify(nameof(StretchMode));
                }
            }
        }

        public bool SwitchOnStartup
        {
            get { return pSwitchOnStartup; }
            set
            {
                if (pSwitchOnStartup != value)
                {
                    pSwitchOnStartup = value;
                    UserProfile.SetSwitchOnStartup(pSwitchOnStartup);
                    Notify(nameof(SwitchOnStartup));
                }
            }
        }

        public bool ShowWindowAtStartup
        {
            get { return pShowWindowAtStartup; }
            set
            {
                if (pShowWindowAtStartup != value)
                {
                    pShowWindowAtStartup = value;
                    UserProfile.SetShowWindowAtStartup(pShowWindowAtStartup);
                    Notify(nameof(ShowWindowAtStartup));
                }
            }
        }

        public bool AutoCheckAtStartup
        {
            get { return pAutoCheckAtStartup; }
            set
            {
                if (pAutoCheckAtStartup != value)
                {
                    pAutoCheckAtStartup = value;
                    UserProfile.SetAutoCheckAtStartup(pAutoCheckAtStartup);
                    Notify(nameof(AutoCheckAtStartup));
                }
            }
        }

        public bool RunAtSystemStart
        {
            get { return pRunAtSystemStart; }
            set
            {
                if (pRunAtSystemStart != value)
                {
                    pRunAtSystemStart = value;
                    UserProfile.SetRunAtSystemStart(pRunAtSystemStart);
                    Notify(nameof(RunAtSystemStart));

                    StartupRegistry regHandler = new StartupRegistry("Multi-Monitor Wallpaper Switcher");
                    if (pRunAtSystemStart)
                    {
                        ProcessModule? pModule = Process.GetCurrentProcess().MainModule;
                        if (pModule != null)
                        {
                            string? programPath = pModule.FileName;
                            if (!string.IsNullOrEmpty(programPath))
                            {
                                regHandler.SetStartup(programPath);
                            }
                        }
                    }
                    else
                    {
                        regHandler.RemoveStartup();
                    }
                }
            }
        }

        public string DoubleClickTray
        {
            get
            {
                return ((int)pDoubleClickTray).ToString();
            }
            set
            {
                DoubleClickTrayCommandEnum v = (DoubleClickTrayCommandEnum)(int.Parse(value));
                if (pDoubleClickTray != v)
                {
                    pDoubleClickTray = v;
                    UserProfile.SetDoubleClickTray(pDoubleClickTray);
                    Notify(nameof(DoubleClickTray));
                }
            }
        }

        public string DesktopBackgroundMode
        {
            get
            {
                return ((int)pDesktopBackgroundMode).ToString();
            }
            set
            {
                DesktopBackgroundModeEnum v = (DesktopBackgroundModeEnum)(int.Parse(value));
                if (pDesktopBackgroundMode != v)
                {
                    pDesktopBackgroundMode = v;
                    UserProfile.SetDesktopBackgroundMode(pDesktopBackgroundMode);
                    switch (pDesktopBackgroundMode)
                    {
                        case DesktopBackgroundModeEnum.Black:
                            DesktopBackgroundColor = "000000";
                            break;
                        case DesktopBackgroundModeEnum.White:
                            DesktopBackgroundColor = "FFFFFF";
                            break;
                        case DesktopBackgroundModeEnum.DarkGray:
                            DesktopBackgroundColor = "2C2C2C";
                            break;
                        case DesktopBackgroundModeEnum.LightGray:
                            DesktopBackgroundColor = "CCCCCC";
                            break;
                        case DesktopBackgroundModeEnum.Red:
                            DesktopBackgroundColor = "FF0000";
                            break;
                        case DesktopBackgroundModeEnum.Green:
                            DesktopBackgroundColor = "00FF00";
                            break;
                        case DesktopBackgroundModeEnum.Blue:
                            DesktopBackgroundColor = "0000FF";
                            break;
                        case DesktopBackgroundModeEnum.Yellow:
                            DesktopBackgroundColor = "FFFF00";
                            break;
                        case DesktopBackgroundModeEnum.Cyan:
                            DesktopBackgroundColor = "00FFFF";
                            break;
                        case DesktopBackgroundModeEnum.Purple:
                            DesktopBackgroundColor = "FF00FF";
                            break;
                    }
                    Notify(nameof(DesktopBackgroundMode));
                }
            }
        }

        public string DesktopBackgroundColor
        {
            get
            {
                return pDesktopBackgroundColor.ToString("X6");
            }
            set
            {
                string val = value;
                //! 处理简写格式
                if (value.Length == 3)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var c in value)
                    {
                        sb.Append(c.ToString() + c.ToString());
                    }
                    val = sb.ToString();
                }

                uint.TryParse(val, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out uint v);
                if (pDesktopBackgroundColor != v)
                {
                    pDesktopBackgroundColor = v;
                    UserProfile.SetDesktopBackgroundColor(pDesktopBackgroundColor);
                    Notify(nameof(DesktopBackgroundColor));
                    WallpaperProc.SetDesktopBackgroundColor();
                }
            }
        }

        public bool DisableQualityReduction
        {
            get
            {
                return pDisableQualityReduction;
            }
            set
            {
                if (pDisableQualityReduction != value)
                {
                    pDisableQualityReduction = value;
                    UserProfile.SetDisableQualityReduction(pDisableQualityReduction);
                    Notify(nameof(DisableQualityReduction));

                    QualityRegistry regHandler = new QualityRegistry();
                    if (pDisableQualityReduction)
                    {
                        regHandler.SetQuality();
                    }
                    else
                    {
                        regHandler.RemoveQuality();
                    }
                }
            }
        }

        /// <summary>
        /// 当前版本值
        /// </summary>
        public string Version
        {
            get
            {
                return "v" + pVersion;
            }
        }

        /// <summary>
        /// 是否存在更新版本
        /// </summary>
        public bool IsUpdate
        {
            get { return pIsUpdate; }
            set
            {
                if (pIsUpdate != value)
                {
                    pIsUpdate = value;
                    Notify(nameof(IsUpdate));
                }
            }
        }

        public bool IsShowVersionText
        {
            get { return pIsShowVersionText; }
            set
            {
                if (pIsShowVersionText != value)
                {
                    pIsShowVersionText = value;
                    Notify(nameof(IsShowVersionText));
                }
            }
        }

        public string VersionTextColor
        {
            get { return pVersionTextColor; }
            set
            {
                if (pVersionTextColor != value)
                {
                    pVersionTextColor = value;
                    Notify(nameof(VersionTextColor));
                }
            }
        }

        public string VersionText
        {
            get { return pVersionText; }
            set
            {
                if (pVersionText != value)
                {
                    pVersionText = value;
                    Notify(nameof(VersionText));
                }
            }
        }

        public string CheckButtonTip
        {
            get { return pCheckButtonTip; }
            set
            {
                if (pCheckButtonTip != value)
                {
                    pCheckButtonTip = value;
                    Notify(nameof(CheckButtonTip));
                }
            }
        }

        public string CheckButtonKind
        {
            get { return pCheckButtonKind; }
            set
            {
                if (pCheckButtonKind != value)
                {
                    pCheckButtonKind = value;
                    Notify(nameof(CheckButtonKind));
                }
            }
        }

        /// <summary>
        /// From: https://stackoverflow.com/a/14632782
        /// </summary>
        public ICommand CreateShortcut
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        string shortcutFile = desktopPath + @"\Multi-Monitor Wallpaper Switcher.lnk";
                        if (!File.Exists(shortcutFile))
                        {
                            ProcessModule? pModule = Process.GetCurrentProcess().MainModule;
                            if (pModule != null)
                            {
                                string? programPath = pModule.FileName;
                                if (!string.IsNullOrEmpty(programPath))
                                {
                                    IShellLink link = (IShellLink)new ShellLink();
                                    link.SetDescription("Multi-Monitor Wallpaper Switcher");
                                    link.SetPath(programPath);

                                    IPersistFile file = (IPersistFile)link;
                                    file.Save(shortcutFile, false);
                                }
                            }
                        }
                    }
                };
            }
        }

        public ICommand OpenUrl
        {
            get
            {
                return new ContextCommand
                {
                    CommandAction = paramater =>
                    {
                        if (paramater != null)
                        {
                            string url = paramater.ToString()!;
                            if (!string.IsNullOrEmpty(url))
                            {
                                Process.Start("explorer.exe", url);
                            }
                        }
                    }
                };
            }
        }

        public ICommand CheckUpdate
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () =>
                    {
                        if (!pIsChecking)
                        {
                            if (pIsUpdate)
                            {
                                Process.Start("explorer.exe", "https://github.com/LightAPIs/MultiMonitorWallpaperSwitcher/releases/latest");
                            }
                            else
                            {
                                pIsChecking = true;
                                IsShowVersionText = true;
                                VersionText = Resource.Checking;
                                UpdateCheck uCheck = new UpdateCheck();
                                uCheck.HasUpdated += HasUpdated;
                                Task task = Task.Run(() => { uCheck.UpdateChecking(); });
                            }
                        }
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

        private void HasUpdated(object sender, bool isError, string version)
        {
            pLatestVersion = version;
            IsUpdate = CompareVer(pVersion, pLatestVersion);
            VersionTextColor = IsUpdate ? "Red" : "Blue";
            VersionText = IsUpdate ? Resource.HadUpdate + "v" + pLatestVersion : (isError ? Resource.NetworkError : Resource.IsLatest);
            CheckButtonTip = IsUpdate ? Resource.GoUpgrade : Resource.CheckUpdate;
            CheckButtonKind = IsUpdate ? "ArrowUpBoldCircle" : "RefreshCircle";

            pIsChecking = false;
            (sender as UpdateCheck)!.HasUpdated -= HasUpdated;
        }

        /// <summary>
        /// 获取各个小版本号
        /// </summary>
        /// <param name="ver"></param>
        /// <returns>3 位版本号数组</returns>
        private uint[] SpliteVer(string ver)
        {
            var sp = ver.Split('.');
            if (sp.Length < 3)
            {
                return new uint[3] { 1, 0, 0 };
            }
            var sv = new uint[3];
            uint.TryParse(sp[0], out sv[0]);
            uint.TryParse(sp[1], out sv[1]);
            uint.TryParse(sp[2], out sv[2]);
            return sv;
        }

        /// <summary>
        /// 比较版本是否存在更新
        /// </summary>
        /// <param name="vL">本地版本</param>
        /// <param name="vN">网络版本</param>
        /// <returns>若 vN 大于 vL 则返回 true</returns>
        private bool CompareVer(string vL, string vN)
        {
            var vLoc = SpliteVer(vL);
            var vNet = SpliteVer(vN);
            int compare = 0;
            for (int index = 0; index < 3; index++)
            {
                if (vNet[index] > vLoc[index])
                {
                    compare += Standard[index];
                }
                else if (vNet[index] < vLoc[index])
                {
                    compare -= Standard[index];
                }
            }
            return compare > 0;
        }
    }
}
