using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlayingPage : Page
    {
        bool isLoad=false;
        private ViewModel.PlayingViewModel PlayingViewModel;
        public PlayingPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            PlayingViewModel = new ViewModel.PlayingViewModel();
            DataContext = PlayingViewModel;
            ViewModel.PlayingViewModel.OnUpdatePlayingLyric += PlayingViewModel_OnUpdatePlayingLyric;
        }

        private async void PlayingViewModel_OnUpdatePlayingLyric(int index)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
             {
                 if (ListBox_lyric.Items.Count != 0)
                 {
                     int showItemCount = (int)(ListBox_lyric.ActualHeight / 50);
                     ListBox_lyric.SelectedIndex = index;
                     if (index < ListBox_lyric.Items.Count - showItemCount / 2)
                     {
                         ListBox_lyric.ScrollIntoView(ListBox_lyric.Items[index + showItemCount / 2]);
                     }
                 }
             });
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void Button_compactOverlayback_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).myMainPage.IntoCompactOverlayMode();
        }

        private void Button_FullScreenMode_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isInFullScreenMode = view.IsFullScreenMode;
            if (isInFullScreenMode)
            {
                view.ExitFullScreenMode();
                TextBlock_FullScreenModeTip.Text = "全屏";
                FontIcon_FullScreenMode.Glyph = "\xE740";
            }
            else
            {
                view.TryEnterFullScreenMode();
                TextBlock_FullScreenModeTip.Text = "退出全屏";
                FontIcon_FullScreenMode.Glyph = "\xE73F";
            }
        }


        private void ListBox_Comment_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(TextBlock))
            {
                OtherHelper.CopyTextToClipboard((e.OriginalSource as TextBlock).Text);
            }
            else if(e.OriginalSource.GetType() == typeof(Windows.UI.Xaml.Shapes.Rectangle))
            {
                OtherHelper.CopyTextToClipboard(((e.OriginalSource as Windows.UI.Xaml.Shapes.Rectangle).DataContext as CommentItem).Content);
            }
        }

        private void ListBox_Artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayingService.IsPlayingSong)
            {
                Artist arItem = ((ListBox)sender).SelectedItem as Artist;
                if (arItem != null)
                {
                    PlayingViewModel.CheckArtistCommand.Execute(arItem);
                }
            }
        }

        private void MusicList_OnChangedSong(MusicItem musicItem)
        {
            PlayingViewModel.PlaySimiMusic(musicItem);
        }

        private void MusicList_OnChangedArtist(ArtistBaseDetailRoot artist)
        {
            NavigateService.NavigateToArtistAsync(artist);
        }

        private void MusicList_OnChangedAlbum(AlbumRoot album)
        {
            NavigateService.NavigateToAlbumAsync(album);
        }
    }
}
