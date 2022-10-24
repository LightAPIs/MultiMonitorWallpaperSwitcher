using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageResources;
using MultiMonitorWallpaperSwitcher.Profile;

namespace MultiMonitorWallpaperSwitcher.Data
{
    public class DoubleClickTrayOptions : ObservableCollection<CategroryInfo>
    {
        public DoubleClickTrayOptions()
        {
            this.Add(new CategroryInfo() { Name = Resource.ShowWindow, Value = ((int)DoubleClickTrayCommandEnum.ShowWindow).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.SwitchWallpaperSet, Value = ((int)DoubleClickTrayCommandEnum.SwitchWallpaper).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.PauseAutoSwitch, Value = ((int)DoubleClickTrayCommandEnum.PauseAuto).ToString() });
        }
    }
}
