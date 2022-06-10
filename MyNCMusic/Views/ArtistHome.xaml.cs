using Microsoft.Toolkit.Uwp.UI.Controls;
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
    public sealed partial class ArtistHome : Page
    {
        ArtistBaseDetailRoot artistBaseDetailRoot;
        //public static SolidColorBrush mainSolidColorBrush_static;
        public SolidColorBrush mainSolidColorBrush;
        public ArtistHome()
        {
           // mainSolidColorBrush = MainPage.mainSolidColorBrush;
            //mainSolidColorBrush_static = MainPage.mainSolidColorBrush;
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            ArtistBaseDetailRoot artistBaseDetailRoot_temp = e.Parameter as ArtistBaseDetailRoot;
            if (artistBaseDetailRoot_temp == null)
                return;
            if (artistBaseDetailRoot!=null&&artistBaseDetailRoot_temp.Artist.Id == artistBaseDetailRoot.Artist.Id)
                return;
            artistBaseDetailRoot = artistBaseDetailRoot_temp;
            LoadLayout();
        }

        void LoadLayout()
        {
            Image_artist.Source = new BitmapImage(new Uri(artistBaseDetailRoot.Artist.Img1v1Url));
            TextBlock_artitName.Text = artistBaseDetailRoot.Artist.Name;
            string othersName = "";
            foreach(var temp in artistBaseDetailRoot.Artist.Alias)
            {
                othersName += temp;
                othersName += "; ";
            }
            othersName += artistBaseDetailRoot.Artist.Trans;
            TextBlock_othersName.Text = othersName;
            //判断是否为喜欢歌曲
            if (MainPage.favoriteSongsRoot != null)
            {
                foreach (var temp in artistBaseDetailRoot.HotSongs)
                {
                    if (MainPage.favoriteSongsRoot.Ids.Find(p => p.Equals(temp.Id)) != 0)
                        temp.IsFavorite = true;
                }
            }
            ListBox_hotSongs.ItemsSource = artistBaseDetailRoot.HotSongs;
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void AdaptiveGridViewControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            Album album = e.ClickedItem as Album;
            if (album.Id == PlayingService.PlayingListId)
                Frame.Navigate(typeof(AlbumDetail));
            else
            {
                ProgressBar_loadAlbum.Visibility = Visibility.Visible;
                AlbumRoot albumRoot = await Task.Run(() => AlbumService.GetAlbum(album.Id));
                if (albumRoot == null)
                    return;
                Frame.Navigate(typeof(AlbumDetail), albumRoot);
                ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
            }
        }

        private async void ListBox_hotSongs_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            MusicItem songsItem = listBox.SelectedItem as MusicItem;
            if (songsItem == null)
                return;
            
            PlayingService.PlayingListId= songsItem.Al.Id;
            await PlayingService.ChangePlayingSong(songsItem.Id, artistBaseDetailRoot.HotSongs, songsItem);
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            if (pivotItem.Tag.ToString() == "1")
            {
                if (artistBaseDetailRoot == null|| AdaptiveGridView_albums.ItemsSource!=null)
                    return;
                ArtistAllAlbumRoot artistAllAlbumRoot=await Task.Run(() => AlbumService.GetArtistAllAlbums(artistBaseDetailRoot.Artist.Id));
                if(artistAllAlbumRoot!=null)
                {
                    AdaptiveGridView_albums.ItemsSource = artistAllAlbumRoot.HotAlbums;
                }
            }
        }

        private async void Button_allSongs_Click(object sender, RoutedEventArgs e)
        {
            PlayingService.PlayingListId= artistBaseDetailRoot.HotSongs.First().Al.Id;
            await PlayingService.ChangePlayingSong(artistBaseDetailRoot.HotSongs.First().Id, artistBaseDetailRoot.HotSongs, artistBaseDetailRoot.HotSongs.First());
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

        private async void Button_album_Click(object sender, RoutedEventArgs e)
        {
            MusicItem songsItem = ((Button)sender).DataContext as MusicItem;
            if (songsItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            AlbumRoot albumRoot = await Task.Run(() => AlbumService.GetAlbum(songsItem.Al.Id));
            if (albumRoot == null)
            {
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                return;
            }
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            Frame.Navigate(typeof(AlbumDetail), albumRoot);
        }
    }
}
