using MyNCMusic.Helper;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public string Version;
        public SettingPage()
        {
            Version = String.Format("{0}.{1}.{2}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = new ViewModel.SettingViewModel();
            Unloaded += SettingPage_Unloaded;
        }

        private void SettingPage_Unloaded(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModel.SettingViewModel).TryCancelQr();
        }
    }
}
