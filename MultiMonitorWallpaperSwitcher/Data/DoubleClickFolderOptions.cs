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
    public class DoubleClickFolderOptions : ObservableCollection<CategroryInfo>
    {
        public DoubleClickFolderOptions()
        {
            this.Add(new CategroryInfo() { Name = Resource.OpenFolder, Value = ((int)DoubleClickFolderCommandEnum.OpenFolder).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.SetWallpaperByFolder, Value = ((int)DoubleClickFolderCommandEnum.SetWallpaperByFolder).ToString() });
        }
    }
}
