using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorWallpaperSwitcher.RegistryMgr
{
    public class QualityRegistry : RegistryHandler
    {
        private readonly string KeyName = "JPEGImportQuality";
        public QualityRegistry() : base(@"Control Panel\Desktop")
        {
        }

        public bool SetQuality()
        {
            return SetValue(KeyName, 100, RegistryValueKind.DWord);
        }

        public void RemoveQuality()
        {
            RemoveValue(KeyName);
        }
    }
}
