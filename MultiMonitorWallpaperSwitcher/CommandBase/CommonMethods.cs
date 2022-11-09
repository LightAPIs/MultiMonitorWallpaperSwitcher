using MultiMonitorWallpaperSwitcher.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiMonitorWallpaperSwitcher.CommandBase
{
    public class CommonMethods
    {
        /// <summary>
        /// 显示主窗口
        /// </summary>
        public static void ShowWindowMethod()
        {
            bool isCheck = false;
            if (!Glob.AutoUpdateChecked && UserProfile.GetAutoCheckAtStartup())
            {
                isCheck = true;
                Glob.AutoUpdateChecked = true;
            }

            if (App.Current.MainWindow == null)
            {
                App.Current.MainWindow = new MainWindow(isCheck);
                App.Current.MainWindow.Show();
            }
            else
            {
                bool hadMain = false;
                foreach (Window win in App.Current.Windows)
                {
                    if (win is MainWindow)
                    {
                        hadMain = true;
                        win.Show();
                        win.WindowState = WindowState.Normal;
                        win.Activate();
                    }
                }

                if (!hadMain)
                {
                    App.Current.MainWindow = new MainWindow(isCheck);
                    App.Current.MainWindow.Show();
                }
            }
        }
    }
}
