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
    public class DoubleClickCardOptions : ObservableCollection<CategroryInfo>
    {
        public DoubleClickCardOptions()
        {
            this.Add(new CategroryInfo() { Name = Resource.ViewCurrentImage, Value = ((int)DoubleClickCardCommandEnum.ViewCurrentImage).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.SwitchNextWallpaper, Value = ((int)DoubleClickCardCommandEnum.SwitchNextWallpaper).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.SetSpecifiedImage, Value = ((int)DoubleClickCardCommandEnum.SetSpecifiedImage).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.RefreshCurrentCard, Value = ((int)DoubleClickCardCommandEnum.RefreshCurrentCard).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.ViewSettingImage, Value = ((int)DoubleClickCardCommandEnum.ViewSettingImage).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.ViewLastSettingImage, Value = ((int)DoubleClickCardCommandEnum.ViewLastSettingImage).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.OpenDirectoryOfCurrentImage, Value = ((int)DoubleClickCardCommandEnum.OpenDirectoryOfCurrentImage).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.OpenDirectoryOfSettingImage, Value = ((int)DoubleClickCardCommandEnum.OpenDirectoryOfSettingImage).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.OpenDirectoryOfLastSettingImage, Value = ((int)DoubleClickCardCommandEnum.OpenDirectoryOfLastSettingImage).ToString() });
        }
    }
}
