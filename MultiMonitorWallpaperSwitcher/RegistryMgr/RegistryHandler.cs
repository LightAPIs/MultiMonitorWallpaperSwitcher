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
        protected string RootKeyPath;
        public RegistryHandler(string rootKeyPath)
        {
            RootKeyPath = rootKeyPath;
        }

        protected bool SetValue(string name, object value, RegistryValueKind valueKind = RegistryValueKind.String)
        {
            if (string.IsNullOrEmpty(RootKeyPath))
            {
                return false;
            }

            try
            {
                RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(RootKeyPath, true);
                if (regkey != null)
                {
                    regkey.SetValue(name, value, valueKind);
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

        protected void RemoveValue(string keyName)
        {
            try
            {
                RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(RootKeyPath, true);
                if (regkey != null)
                {
                    string[] subNames = regkey.GetValueNames();
                    foreach (string readName in subNames)
                    {
                        if (readName == keyName)
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
