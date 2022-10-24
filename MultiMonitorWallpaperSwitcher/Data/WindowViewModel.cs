using MultiMonitorWallpaperSwitcher.CommandBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MultiMonitorWallpaperSwitcher.Data
{
    public class WindowViewModel
    {
        private readonly Action refreshAction;
        private readonly Action switchAction;

        public WindowViewModel(Action refreshAction, Action switchAction)
        {
            this.refreshAction = refreshAction;
            this.switchAction = switchAction;
        }

        public ICommand RefreshCards
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = refreshAction,
                };
            }
        }

        public ICommand SwitchNextWallpapers
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = switchAction,
                };
            }
        }
    }
}
