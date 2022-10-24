using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageResources;

namespace MultiMonitorWallpaperSwitcher.Data
{
    public class LanguageOptions : ObservableCollection<CategroryInfo>
    {
        public LanguageOptions()
        {
            this.Add(new CategroryInfo() { Name = Resource.System, Value = "system" });
            this.Add(new CategroryInfo() { Name = "English", Value = "en" });
            this.Add(new CategroryInfo() { Name = "简体中文", Value = "zh-CN" });
        }
    }
}
