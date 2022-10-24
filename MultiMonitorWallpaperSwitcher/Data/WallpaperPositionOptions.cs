using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageResources;

namespace MultiMonitorWallpaperSwitcher.Data
{
    public class WallpaperPositionOptions : ObservableCollection<CategroryInfo>
    {
        public WallpaperPositionOptions()
        {
            this.Add(new CategroryInfo() { Name = Resource.Fill, Value = "Fill" });
            this.Add(new CategroryInfo() { Name = Resource.Fit, Value = "Fit" });
            this.Add(new CategroryInfo() { Name = Resource.Stretch, Value = "Stretch" });
            this.Add(new CategroryInfo() { Name = Resource.Tile, Value = "Tile" });
            this.Add(new CategroryInfo() { Name = Resource.Center, Value = "Center" });
            this.Add(new CategroryInfo() { Name = Resource.Span, Value = "Span" });
        }
    }
}
