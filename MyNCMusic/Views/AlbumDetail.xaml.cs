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
    public sealed partial class AlbumDetail : Page
    {
        //public static SolidColorBrush mainSolidColorBrush;
        AlbumRoot albumRoot;
        public AlbumDetail()
        {
            //mainSolidColorBrush = MainPage.mainSolidColorBrush;
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            if (e.Parameter == null)
                return;
            albumRoot = (AlbumRoot)e.Parameter;
            //if (albumRoot == null)
            //    return;
            LoadLayout();
        }

        void LoadLayout()
        {
            Image_album.Source = new BitmapImage(new Uri(albumRoot.album.picUrl));
            TextBlock_albumName.Text = albumRoot.album.name;
            Button_ar.Content = ArtistService.GetArNames_ArtistsItem(albumRoot.album.artists);
            TextBlock_comentCount.Text = albumRoot.album.info.commentCount.ToString();
            TextBlock_des.Text = albumRoot.album.description == null ? "" : albumRoot.album.description;
            TextBlock_songsCount.Text = albumRoot.songs.Count.ToString();
            
            //判断是否为喜欢歌曲
            if (MainPage.favoriteSongsRoot != null)
            {
                foreach (var temp in albumRoot.songs)
                {
                    if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.Id)) != 0)
                        temp.isFavorite = true;
                }
            }
            ListBox_musicDetail.ItemsSource = albumRoot.songs;
            //TextBlock_subCount.Text = playListDetailRoot.playlist.subscribedCount.ToString();
            //TextBlock_comentCount.Text = playListDetailRoot.playlist.commentCount.ToString();
            //TextBlock_songsCount.Text = musicDetailRootSource.songs.Count.ToString();
            //ListBox_musicDetail.ItemsSource = musicDetailRootSource.songs;
        }

        private async void Button_playAll_Click(object sender, RoutedEventArgs e)
        {
            PlayingService.PlayingListId = albumRoot.album.id;
            await PlayingService.ChangePlayingSong(albumRoot.songs.First().Id, albumRoot.songs);
        }

        private async void ListBox_musicDetail_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            if (songsItem == null)
                return;
            //SongUrlRoot songUrlRoot = SongService.GetMusicUrl(songsItem.Id);
            //if (songUrlRoot == null)
            //    return;

            ////修改播放列表
            //if (MainPage.PlayingListId != albumRoot.album.id)//已在播放此歌单，仅修改播放歌曲，否则，重置播放列表及历史记录
            //{
            //    (Application.Current as App).myMainPage.currentPlayList.Clear();
            //    foreach (var temp in albumRoot.songs)
            //        (Application.Current as App).myMainPage.currentPlayList.Add(temp);
            //    (Application.Current as App).myMainPage.playHistoryIndex.Clear();
            //    //MainPage.PlayingListId = albumRoot.album.id;
            //}
            ////修改mainpage以触发修改正在播放的音乐
            //(Application.Current as App).myMainPage.ChnagePlayingSong(songsItem, songUrlRoot, albumRoot.album.id);
            PlayingService.PlayingListId = albumRoot.album.id;
            await PlayingService.ChangePlayingSong(songsItem.Id, albumRoot.songs, songsItem);
        }

        private void AutoSuggestBox_search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text == "")
                ListBox_musicDetail.ItemsSource = albumRoot.songs;
            else
            {
                var list = albumRoot.songs.FindAll(p => p.Name.Contains(sender.Text));
                if (list != null && list.Count != 0)
                {
                    ListBox_musicDetail.ItemsSource = list;
                }
            }
        }

        private async void Button_artists_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SongsItem songsItem = button.DataContext as SongsItem;
            if (songsItem.ar.Count == 1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(songsItem.ar.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        private async void ListBox_artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Artist arItem = ((ListBox)sender).SelectedItem as Artist;
            if (arItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(arItem.id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }

        private async void Button_comment_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar_loading.Visibility = Visibility.Visible;
            CommentRoot commentRoot = await Task.Run(() => CommentService.GetAlbumComment(albumRoot.album.id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (commentRoot == null)
                return;
            Frame.Navigate(typeof(Comment), commentRoot);
        }

        private async void Button_ar_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(albumRoot.album.artists.First().id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }
    }
}
