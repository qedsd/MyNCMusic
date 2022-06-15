using MyNCMusic.Models;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.ViewManagement;
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
    public sealed partial class CompactOverlayPage : Page
    {
        
        public CompactOverlayPage()
        {
            (Application.Current as App).compactOverlayPage = this;
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Window.Current.SetTitleBar(MyTitleBar);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下
            ViewModel.CompactOverlayViewModel compactOverlayViewModel = new ViewModel.CompactOverlayViewModel();
            DataContext = compactOverlayViewModel;
            Window.Current.SetTitleBar(MyTitleBar);
        }

        private async void Button_StandardMode_Click(object sender, RoutedEventArgs e)
        {
            if(await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default))
                Frame.GoBack();
        }

        private void Grid_PlayController_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            StackPanel_ControlButton.Visibility = Visibility.Collapsed;
        }

        private void Grid_PlayController_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StackPanel_ControlButton.Visibility = Visibility.Visible;
        }
    }
}
