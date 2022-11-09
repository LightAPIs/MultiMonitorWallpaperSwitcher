using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiMonitorWallpaperSwitcher.KeyMgr
{
    public class HotKeyHelper
    {
        /// <summary>
        /// 热键消息
        /// </summary>
        public const int WM_HOTKEY = 0x0312;

        /// <summary>
        /// 注册全局热键
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        /// <param name="fsModifiers"></param>
        /// <param name="vK"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, Keys vK);

        /// <summary>
        /// 取消全局热键
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 添加全局原子
        /// <see cref="https://www.pinvoke.net/default.aspx/kernel32.globaladdatom"/>
        /// </summary>
        /// <param name="lpStr"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern ushort GlobalAddAtom(string lpString);

        /// <summary>
        /// 搜索全局原子
        /// <see cref="https://pinvoke.net/default.aspx/kernel32/GlobalFindAtom.html"/>
        /// </summary>
        /// <param name="lpString"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern ushort GlobalFindAtom(string lpString);

        /// <summary>
        /// 删除全局原子
        /// <see cref="https://pinvoke.net/default.aspx/kernel32/GlobalDeleteAtom.html"/>
        /// </summary>
        /// <param name="nAtom"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern ushort GlobalDeleteAtom(ushort nAtom);
    }
}
