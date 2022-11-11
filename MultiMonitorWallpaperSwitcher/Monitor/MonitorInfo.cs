using System;
using System.Runtime.InteropServices;

namespace MultiMonitorWallpaperSwitcher.Monitor
{
    /// <summary>
    /// 矩形结构，定义矩形左上角和右下角的坐标。
    /// <remarks>
    /// 按照贯例，矩形的右边缘和下边缘通常被认为是独立的，即坐标为 (Right, Bottom) 的像素位于矩形之外。
    /// </remarks>
    /// <see cref="https://learn.microsoft.com/en-us/previous-versions/dd162897(v=vs.85)"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        /// <summary>
        /// 左边缘，矩形左上角的 X 坐标
        /// </summary>
        public int Left;
        /// <summary>
        /// 上边缘，矩形左上角的 Y 坐标
        /// </summary>
        public int Top;
        /// <summary>
        /// 右边缘，矩形右下角的 X 坐标
        /// </summary>
        public int Right;
        /// <summary>
        /// 下边缘，矩形右下角的 Y 坐标
        /// </summary>
        public int Bottom;
    }

    /// <summary>
    /// 此枚举用于设置和获取幻灯片放映选项。
    /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-idesktopwallpaper-getslideshowoptions"/>
    /// </summary> 
    public enum DesktopSlideshowOptions
    {
        /// <summary>
        /// 设置后，按随机顺序放映幻灯片
        /// </summary>
        ShuffleImages = 0x01,
    }

    /// <summary>
    /// 此枚举来指示幻灯片的当前状态。
    /// 在 GetStatus 方法中使用。
    /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-idesktopwallpaper-getstatus"/>
    /// </summary>
    public enum DesktopSlideshowState
    {
        Enabled = 0x01,
        Slideshow = 0x02,
        DisabledByRemoteSession = 0x04,
    }

    /// <summary>
    /// 此枚举指示幻灯片放映应前进的方向。
    /// 在 AdvanceSlideshow 方法中使用。
    /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-idesktopwallpaper-advanceslideshow"/>
    /// </summary>
    public enum DesktopSlideshowDirection
    {
        Forward = 0,
        Backward = 1,
    }

    /// <summary>
    /// 此枚举指示桌面壁纸的显示方式(包括运行幻灯片时的显示方式)。
    /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/ne-shobjidl_core-desktop_wallpaper_position"/>
    /// </summary>
    public enum DesktopWallpaperPosition
    {
        Center = 0,
        Tile = 1,
        Stretch = 2,
        Fit = 3,
        Fill = 4,
        Span = 5,
    }

    /// <summary>
    /// 提供用于管理桌面壁纸的方法。
    /// <see cref="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nn-shobjidl_core-idesktopwallpaper"/>
    /// </summary>
    [ComImport, Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDesktopWallpaper
    {
        void SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.LPWStr)] string wallpaper);
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

        /// <summary>
        /// 获取显示器的唯一 ID
        /// </summary>
        /// <param name="monitorIndex">显示器设备列表中的设备索引</param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetMonitorDevicePathAt(uint monitorIndex);
        /// <summary>
        /// 获取显示器数目
        /// </summary>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.U4)]
        uint GetMonitorDevicePathCount();

        [return: MarshalAs(UnmanagedType.Struct)]
        Rect GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

        void SetBackgroundColor([MarshalAs(UnmanagedType.U4)] uint color);
        [return: MarshalAs(UnmanagedType.U4)]
        uint GetBackgroundColor();

        void SetPosition([MarshalAs(UnmanagedType.I4)] DesktopWallpaperPosition position);
        [return: MarshalAs(UnmanagedType.I4)]
        DesktopWallpaperPosition GetPosition();

        void SetSlideshow(IntPtr items);
        IntPtr GetSlideshow();

        /// <summary>
        /// 幻灯片放映设置
        /// </summary>
        /// <param name="options">置 0 禁用随机</param>
        /// <param name="slideshowTick">图像转换之间的时间量(以 ms 为单位)</param>
        void SetSlideshowOptions(DesktopSlideshowOptions options, uint slideshowTick);
        [PreserveSig]
        uint GetSlideshowOptions(out DesktopSlideshowOptions options, out uint slideshowTick);

        void AdvanceSlideshow([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.I4)] DesktopSlideshowDirection direction);

        DesktopSlideshowDirection GetStatus();

        bool Enable();
    }

    /// <summary>
    /// DesktopWallpaper 组件类
    /// </summary>
    [ComImport, Guid("C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD")]
    public class DesktopWallpaperClass { }
}
