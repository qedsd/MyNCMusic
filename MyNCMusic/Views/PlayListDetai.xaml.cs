using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class PlayListDetai : Page
    {
        public PlayListDetai()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        private ViewModel.PlaylistDetailViewModel ViewModel;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PlaylistNavItem playlistNavItem = e.Parameter as PlaylistNavItem;
            ViewModel = new ViewModel.PlaylistDetailViewModel(playlistNavItem.PlaylistInfo, playlistNavItem.Songs);
            DataContext = ViewModel;
        }

        private void MusicList_OnChangedSong(MusicItem musicItem)
        {
            ViewModel.PlayCommand.Execute(musicItem);
        }

        private void MusicList_OnChangedArtist(ArtistBaseDetailRoot artist)
        {
            NavigateService.NavigateToArtistAsync(artist);
        }

        private void MusicList_OnChangedAlbum(AlbumRoot album)
        {
            NavigateService.NavigateToAlbumAsync(album);
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
