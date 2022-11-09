using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiMonitorWallpaperSwitcher.KeyMgr
{
    public class NWindow : NativeWindow, IDisposable
    {
        public NWindow()
        {
            this.CreateHandle(new CreateParams());
            Glob.HWND = this.Handle;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == HotKeyHelper.WM_HOTKEY)
            {
                int sid = m.WParam.ToInt32();
                foreach (var item in HotKeyManager.HotKeyDic)
                {
                    if (item.Value == sid)
                    {
                        HotKeyManager.Execute(item.Key);
                    }
                }
            }
        }

        public void Dispose()
        {
            this.DestroyHandle();
        }
    }
}
