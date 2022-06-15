using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        public static NavigationPage Instance { get; private set; }
        public NavigationPage()
        {
            Instance = this;
            this.InitializeComponent();
            MainFrame.Navigate(typeof(Home));
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = new ViewModel.NavigationViewModel();
        }
        public Frame HomeFrame
        {
            get => MainFrame;
        }
        public void NavigateTo([In] Type sourcePageType, [In] object parameter, [In] NavigationTransitionInfo infoOverride)
        {
            MainFrame.Navigate(sourcePageType, parameter, infoOverride);
        }
        public void NavigateTo([In] Type sourcePageType, [In] object parameter)
        {
            MainFrame.Navigate(sourcePageType, parameter);
        }
        public void NavigateTo([In] Type sourcePageType)
        {
            MainFrame.Navigate(sourcePageType);
        }
    }
}
