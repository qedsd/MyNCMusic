using MyNCMusic.Services;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyNCMusic
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Instance;
        public MainPage()
        {
            this.InitializeComponent();

            //设置标题栏
            var tiWtleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            tiWtleBar.BackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonBackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonForegroundColor = Colors.White;
            tiWtleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonHoverBackgroundColor = Colors.LightGray;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(MyTitleBar);
            MainFrame.Navigate(typeof(Views.NavigationPage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(MyTitleBar);
        }
    }
}
