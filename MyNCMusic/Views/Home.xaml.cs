using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home : Page
    {
        public static Home Instance;
        public Home()
        {
            Instance = this;
            Loaded += Home_Loaded;
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ViewModel.HomeViewModel();
        }

        public bool NavigateToPlayingPage()
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(PlayingPage));
            }
            if (Frame.CanGoBack)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void NavigateTo([In] Type sourcePageType, [In] object parameter, [In] NavigationTransitionInfo infoOverride)
        {
            Frame_main.Navigate(sourcePageType, parameter, infoOverride);
        }
        public void NavigateTo([In] Type sourcePageType, [In] object parameter)
        {
            Frame_main.Navigate(sourcePageType, parameter);
        }
        public void NavigateTo([In] Type sourcePageType)
        {
            Frame_main.Navigate(sourcePageType);
        }
    }
}
