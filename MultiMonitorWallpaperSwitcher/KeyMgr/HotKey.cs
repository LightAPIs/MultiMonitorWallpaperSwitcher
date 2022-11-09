using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiMonitorWallpaperSwitcher.KeyMgr
{
    public class HotKey
    {
        private static readonly Regex KeyReg = new Regex(@"^[A-Z0-9]$|^F[1-9][0-2]?$");
        private static readonly Regex NumReg = new Regex(@"^[0-9]$");
        private string keys = "None";

        [Flags]
        private enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WindowsKey = 8
        }

        public HotKey(string keys = "None")
        {
            SetKeys(keys);
        }

        private bool IsValid(string val)
        {
            bool valid = true;
            if (val == "None")
            {
                return true;
            }
            string[] splitsKeys = val.Split('+');
            foreach (string k in splitsKeys)
            {
                if (k == "Ctrl" || k == "Alt" || k == "Shift" || KeyReg.IsMatch(k))
                {
                    continue;
                }
                valid = false;
            }
            return valid;
        }

        public bool SetKeys(string keys)
        {
            if (IsValid(keys))
            {
                this.keys = keys;
                return true;
            }
            return false;
        }

        public string GetKeys()
        {
            return keys;
        }

        /// <summary>
        /// Trans Keys Enum
        /// 注意：
        /// Keys Enum(https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=windowsdesktop-6.0)
        /// 不同于 Key Enum(https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.key?view=windowsdesktop-6.0)
        /// </summary>
        /// <returns></returns>
        public Keys TransKeysEnum()
        {
            string[] splitsKeys = keys.Split('+');
            foreach (string k in splitsKeys)
            {
                if (KeyReg.IsMatch(k))
                {
                    if (NumReg.IsMatch(k))
                    {
                        return (Keys)Enum.Parse(typeof(Keys), "D" + k, true);
                    }
                    else
                    {
                        return (Keys)Enum.Parse(typeof(Keys), k, true);
                    }
                }
            }
            return Keys.None;
        }

        public int TransModifiers()
        {
            int res = (int)KeyModifiers.None;
            if (keys.Contains("Ctrl"))
            {
                res |= (int)KeyModifiers.Control;
            }
            if (keys.Contains("Shift"))
            {
                res |= (int)KeyModifiers.Shift;
            }
            if (keys.Contains("Alt"))
            {
                res |= (int)KeyModifiers.Alt;
            }
            return res;
        }
    }
}
