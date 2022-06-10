using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace MyNCMusic.Controls
{
    public sealed partial class WaitingPopup : UserControl
    {
        private Popup Popup;
        public WaitingPopup()
        {
            this.InitializeComponent();
            Popup = new Popup();
            Popup.Child = this;
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
            Window.Current.SizeChanged += Current_SizeChanged;
        }
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.Width = e.Size.Width;
            this.Height = e.Size.Height;
        }
        private static WaitingPopup Instance;
        public static void Show()
        {
            Instance = new WaitingPopup();
            Instance.ProgressRing.IsActive = true;
            Instance.Popup.IsOpen = true;
        }
        public static void Hide()
        {
            if (Instance != null)
            {
                Instance.ProgressRing.IsActive = false;
                Instance.Popup.IsOpen = false;
                Instance = null;
            }
        }
    }
}
