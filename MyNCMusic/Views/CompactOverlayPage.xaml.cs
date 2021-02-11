using MyNCMusic.Model;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

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
            PlayingService.MediaTimelineController.StateChanged += MediaTimelineController_StateChanged;
            (Application.Current as App).myMainPage.OnIsOrNotFavoriteChanged += MyMainPage_OnIsOrNotFavoriteChanged;
        }

        

        private async void MediaTimelineController_StateChanged(Windows.Media.MediaTimelineController sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (sender.State == MediaTimelineControllerState.Running)
                {
                    SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
                }
                else
                {
                    SymbolIcon_stopOrPlay.Symbol = Symbol.Play;
                }
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            UpdateLayout();
            Window.Current.SetTitleBar(MyTitleBar);
        }
        public void UpdateLayout_lyric(List<LyricStr> list)
        {
            TextBlock_1_o.Text = list[0].Original ?? "";
            TextBlock_1_t.Text = list[0].Tran ?? "";
            TextBlock_2_o.Text = list[1].Original ?? "";
            TextBlock_2_t.Text = list[1].Tran ?? "";
            TextBlock_3_o.Text = list[2].Original ?? "";
            TextBlock_3_t.Text = list[2].Tran ?? "";
        }
        public new void UpdateLayout()
        {
            TextBlock_name.Text = PlayingService.IsPlayingSong? PlayingService.PlayingSong.Name: PlayingService.PlayingRadio.Name;
            TextBlock_1_o.Text = "";
            TextBlock_1_t.Text = "";
            TextBlock_2_o.Text = "";
            TextBlock_2_t.Text = "";
            TextBlock_3_o.Text = "";
            TextBlock_3_t.Text = "";
            Grid_background.Background = (Application.Current as App).myMainPage.mainImageBrush;
        }

        private async void Button_standardMode_Click(object sender, RoutedEventArgs e)
        {
            if(await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default))
                Frame.GoBack();
        }

        

        private void Button_IsOrNotFavorite_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).myMainPage.Button_isOrNotFavorite_Click(new object(), new RoutedEventArgs());
        }

        private void MyMainPage_OnIsOrNotFavoriteChanged()
        {
            if (!PlayingService.IsPlayingSong)
                return;
            if (PlayingService.PlayingSong.isFavorite)
            {
                TextBlock_IsOrnotFavorite.Text = "\xE00B";
            }
            else
            {
                TextBlock_IsOrnotFavorite.Text = "\xE006";
            }
        }

        private void Button_Previous_Click(object sender, RoutedEventArgs e)
        {
            if (PlayingService.IsPlayingSong)
                PlayingService.PlayLastSongs();
            else
                PlayingService.PlayLastRadio();
            MyMainPage_OnIsOrNotFavoriteChanged();
        }

        MediaPlayer _mediaPlayer = (Application.Current as App).myMainPage._mediaPlayer;

        private void Button_StopOrPlay_Click(object sender, RoutedEventArgs e)
        {
            var state = _mediaPlayer.PlaybackSession.PlaybackState;
            if (state == MediaPlaybackState.Playing)//改为暂停
            {
                PlayingService.MediaTimelineController.Pause();
            }
            else if (state == MediaPlaybackState.Paused)//改为播放
            {
                PlayingService.MediaTimelineController.Resume();
            }
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            if (PlayingService.IsPlayingSong)
                PlayingService.PlayNextSongs();
            else
                PlayingService.PlayNextRadio();
            MyMainPage_OnIsOrNotFavoriteChanged();
        }


        private void Grid_PlayController_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            StackPanel_ControlButton.Visibility = Visibility.Collapsed;
        }

        private void Grid_PlayController_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            StackPanel_ControlButton.Visibility = Visibility.Visible;
            MyMainPage_OnIsOrNotFavoriteChanged();
            var state = _mediaPlayer.PlaybackSession.PlaybackState;
            if (state == MediaPlaybackState.Playing)
            {
                SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
            }
            else if (state == MediaPlaybackState.Paused)
            {
                SymbolIcon_stopOrPlay.Symbol = Symbol.Play;
            }
        }
    }
}
