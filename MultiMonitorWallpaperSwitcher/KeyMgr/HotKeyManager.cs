using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiMonitorWallpaperSwitcher.CommandBase;
using MultiMonitorWallpaperSwitcher.TaskScheduler;

namespace MultiMonitorWallpaperSwitcher.KeyMgr
{
    public class HotKeyManager
    {
        public static HotKeyNameSet HotKeySet = new HotKeyNameSet();
        public static Dictionary<string, int> HotKeyDic = new Dictionary<string, int>();

        /// <summary>
        /// 注册系统热键
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hotKey"></param>
        /// <param name="hWnd"></param>
        public static void RegisterSystemHotKey(string name, HotKey hotKey, IntPtr hWnd)
        {
            UnregisterSystemHotKey(name, hWnd);

            ushort nAtom = HotKeyHelper.GlobalAddAtom(name);
            HotKeyHelper.RegisterHotKey(hWnd, nAtom, hotKey.TransModifiers(), hotKey.TransKeysEnum());
            HotKeyDic.Add(name, nAtom);
        }

        /// <summary>
        /// 取消系统热键
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hWnd"></param>
        public static void UnregisterSystemHotKey(string name, IntPtr hWnd)
        {
            if (HotKeyDic.ContainsKey(name))
            {
                HotKeyHelper.UnregisterHotKey(hWnd, HotKeyDic[name]);
                HotKeyDic.Remove(name);
            }

            ushort oAtom = HotKeyHelper.GlobalFindAtom(name);
            if (oAtom != 0)
            {
                HotKeyHelper.GlobalDeleteAtom(oAtom);
            }
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="name"></param>
        public static void Execute(string name)
        {
            switch (name)
            {
                case "ShowWindow":
                    CommonMethods.ShowWindowMethod();
                    break;
                case "SwitchWallpaper":
                    TaskProc.ExecuteTask();
                    break;
            }
        }
    }

    public readonly struct HotKeyNameSet
    {
        public readonly string ShowWindow = "ShowWindow";
        public readonly string SwitchWallpaper = "SwitchWallpaper";

        public HotKeyNameSet() { }
    }
}
