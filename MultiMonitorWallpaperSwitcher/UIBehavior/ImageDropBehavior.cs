using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using MultiMonitorWallpaperSwitcher.Wallpaper;
using MultiMonitorWallpaperSwitcher.TaskScheduler;

namespace MultiMonitorWallpaperSwitcher.UIBehavior
{
    public class ImageDropBehavior : Behavior<FrameworkElement>
    {
        public string ID
        {
            get { return (string)GetValue(IDProperty); }
            set
            {
                SetValue(IDProperty, value);
            }
        }

        public int Time
        {
            get { return (int)GetValue(TimeProperty); }
            set
            {
                SetValue(TimeProperty, value);
            }
        }

        public string Wallpaper
        {
            get { return (string)GetValue(WallpaperProperty); }
            set
            {
                SetValue(WallpaperProperty, value);
            }
        }

        public static readonly DependencyProperty IDProperty = DependencyProperty.Register("ID", typeof(string), typeof(ImageDropBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(int), typeof(ImageDropBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty WallpaperProperty = DependencyProperty.Register("Wallpaper", typeof(string), typeof(ImageDropBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewDragEnter += AssociatedObject_PreviewDragEnter;
            AssociatedObject.PreviewDrop += AssociatedObject_PreviewDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewDrop -= AssociatedObject_PreviewDrop;
            AssociatedObject.PreviewDragEnter -= AssociatedObject_PreviewDragEnter;
        }

        private void AssociatedObject_PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void AssociatedObject_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetData(DataFormats.FileDrop) is Array arr)
                {
                    string? imageFile = arr.GetValue(0)!.ToString();
                    if (!string.IsNullOrEmpty(imageFile) && File.Exists(imageFile))
                    {
                        FileInfo file = new FileInfo(imageFile);
                        string ext = file.Extension.ToLower();
                        if (WallpaperProc.ExtDic.ContainsKey(ext))
                        {
                            TaskProc.ExecuteOneTaskByImage(ID, Time, imageFile);
                            Wallpaper = imageFile;
                        }
                    }
                }
            }
            catch { }
        }
    }
}
