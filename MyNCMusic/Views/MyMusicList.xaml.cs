using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MyMusicList : Page
    {
        public MyMusicList()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MyMusicList_Loaded;
        }

        private async void MyMusicList_Loaded(object sender, RoutedEventArgs e)
        {
            Controls.WaitingPopup.Show();
            MyPlaylistRoot myPlaylistRoot = await Task.Run(() => PlaylistService.GetMyPlaylist());
            if(myPlaylistRoot != null&& myPlaylistRoot.Playlist!=null)
            {
                CreatedPlaylist.ItemsSource = myPlaylistRoot.Playlist.Where(p => !p.Subscribed).ToList();
                SubPlaylist.ItemsSource = myPlaylistRoot.Playlist.Where(p => p.Subscribed).ToList();
            }
            Controls.WaitingPopup.Hide();
        }

        private void CreatedPlaylist_OnChangedPlaylist(PlaylistItem playlistItem)
        {
            NavigateService.NavigateToPlaylistAsync(playlistItem.Id);
        }
    }
}
