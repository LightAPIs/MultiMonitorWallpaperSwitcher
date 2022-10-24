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
    public class BackgroundColorOptions : ObservableCollection<CategroryInfo>
    {
        public BackgroundColorOptions()
        {
            this.Add(new CategroryInfo() { Name = Resource.Black, Value = ((int)DesktopBackgroundModeEnum.Black).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.DarkGray, Value = ((int)DesktopBackgroundModeEnum.DarkGray).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.LightGray, Value = ((int)DesktopBackgroundModeEnum.LightGray).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.White, Value = ((int)DesktopBackgroundModeEnum.White).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.Red, Value = ((int)DesktopBackgroundModeEnum.Red).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.Green, Value = ((int)DesktopBackgroundModeEnum.Green).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.Blue, Value = ((int)DesktopBackgroundModeEnum.Blue).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.Yellow, Value = ((int)DesktopBackgroundModeEnum.Yellow).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.Cyan, Value = ((int)DesktopBackgroundModeEnum.Cyan).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.Purple, Value = ((int)DesktopBackgroundModeEnum.Purple).ToString() });
            this.Add(new CategroryInfo() { Name = Resource.Custom, Value = ((int)DesktopBackgroundModeEnum.Custom).ToString() });
        }
    }
}
