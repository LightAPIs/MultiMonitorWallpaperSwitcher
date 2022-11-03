using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace MultiMonitorWallpaperSwitcher.RegistryMgr
{
    public class StartupRegistry : RegistryHandler
    {
        private readonly string programName;
        public StartupRegistry(string name) : base(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run")
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
            return SetValue(programName, path);
        }

        /// <summary>
        /// 取消自启动
        /// </summary>
        /// <returns></returns>
        public void RemoveStartup()
        {
            RemoveValue(programName);
        }
    }
}
