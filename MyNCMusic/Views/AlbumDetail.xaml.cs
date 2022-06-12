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
            Image_album.Source = new BitmapImage(new Uri(albumRoot.Album.PicUrl));
            TextBlock_albumName.Text = albumRoot.Album.Name;
            Button_ar.Content = ArtistService.GetArNames_ArtistsItem(albumRoot.Album.Artists);
            TextBlock_comentCount.Text = albumRoot.Album.Info.CommentCount.ToString();
            TextBlock_des.Text = albumRoot.Album.Description == null ? "" : albumRoot.Album.Description;
            TextBlock_songsCount.Text = albumRoot.Songs.Count.ToString();
            
            //判断是否为喜欢歌曲
            if (MainPage.favoriteSongsRoot != null)
            {
                foreach (var temp in albumRoot.Songs)
                {
                    if (MainPage.favoriteSongsRoot.Ids.Find(p => p.Equals(temp.Id)) != 0)
                        temp.IsFavorite = true;
                }
            }
            ListBox_musicDetail.ItemsSource = albumRoot.Songs;
            //TextBlock_subCount.Text = playListDetailRoot.playlist.subscribedCount.ToString();
            //TextBlock_comentCount.Text = playListDetailRoot.playlist.commentCount.ToString();
            //TextBlock_songsCount.Text = musicDetailRootSource.songs.Count.ToString();
            //ListBox_musicDetail.ItemsSource = musicDetailRootSource.songs;
        }

        private async void Button_playAll_Click(object sender, RoutedEventArgs e)
        {
            PlayingService.PlayingListId = albumRoot.Album.Id;
            await PlayingService.ChangePlayingSong(albumRoot.Songs.First().Id, albumRoot.Songs);
        }

        private async void ListBox_musicDetail_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            MusicItem songsItem = listBox.SelectedItem as MusicItem;
            if (songsItem == null)
                return;
            PlayingService.PlayingListId = albumRoot.Album.Id;
            await PlayingService.ChangePlayingSong(songsItem.Id, albumRoot.Songs, songsItem);
        }

        private void AutoSuggestBox_search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text == "")
                ListBox_musicDetail.ItemsSource = albumRoot.Songs;
            else
            {
                var list = albumRoot.Songs.FindAll(p => p.Name.Contains(sender.Text));
                if (list != null && list.Count != 0)
                {
                    ListBox_musicDetail.ItemsSource = list;
                }
            }
        }

        private async void Button_artists_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MusicItem songsItem = button.DataContext as MusicItem;
            if (songsItem.Ar.Count == 1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(songsItem.Ar.First().Id));
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
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(arItem.Id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }

        private async void Button_comment_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar_loading.Visibility = Visibility.Visible;
            CommentRoot commentRoot = await CommentService.GetAlbumCommentAsync(albumRoot.Album.Id);
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (commentRoot == null)
                return;
            Frame.Navigate(typeof(Comment), commentRoot);
        }

        private async void Button_ar_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(albumRoot.Album.Artists.First().Id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }
    }
}
