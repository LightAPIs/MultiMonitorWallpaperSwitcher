using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MultiMonitorWallpaperSwitcher.RegistryMgr
{
    public class RegistryHandler
    {
        private readonly string StartupRootKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private string programName;
        public RegistryHandler(string name)
        {
            programName = name;
        }

        /// <summary>
        /// 设置自启动
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SetStartup(string path)
        {
            try
            {
                // 在启动路径下写入键值对
                RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(StartupRootKeyPath, true);
                if (regkey != null)
                {
                    regkey.SetValue(programName, path);
                    regkey.Close();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 取消自启动
        /// </summary>
        /// <returns></returns>
        public void RemoveStartup()
        {
            try
            {
                // 删除启动路径下指定键值
                RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(StartupRootKeyPath, true);
                if (regkey != null)
                {
                    string[] subNames = regkey.GetValueNames();
                    foreach (string readName in subNames)
                    {
                        if (readName == programName)
                        {
                            regkey.DeleteValue(readName);
                            regkey.Close();
                        }
                    }
                }

            }
            catch { }
        }
    }
}
