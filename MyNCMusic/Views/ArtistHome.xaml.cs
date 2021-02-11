using Microsoft.Toolkit.Uwp.UI.Controls;
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
            if (artistBaseDetailRoot!=null&&artistBaseDetailRoot_temp.artist.id == artistBaseDetailRoot.artist.id)
                return;
            artistBaseDetailRoot = artistBaseDetailRoot_temp;
            LoadLayout();
        }

        void LoadLayout()
        {
            Image_artist.Source = new BitmapImage(new Uri(artistBaseDetailRoot.artist.img1v1Url));
            TextBlock_artitName.Text = artistBaseDetailRoot.artist.name;
            string othersName = "";
            foreach(var temp in artistBaseDetailRoot.artist.alias)
            {
                othersName += temp;
                othersName += "; ";
            }
            othersName += artistBaseDetailRoot.artist.trans;
            TextBlock_othersName.Text = othersName;
            //判断是否为喜欢歌曲
            if (MainPage.favoriteSongsRoot != null)
            {
                foreach (var temp in artistBaseDetailRoot.hotSongs)
                {
                    if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.Id)) != 0)
                        temp.isFavorite = true;
                }
            }
            ListBox_hotSongs.ItemsSource = artistBaseDetailRoot.hotSongs;
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void AdaptiveGridViewControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            Album album = e.ClickedItem as Album;
            if (album.id == PlayingService.PlayingListId)
                Frame.Navigate(typeof(AlbumDetail));
            else
            {
                ProgressBar_loadAlbum.Visibility = Visibility.Visible;
                AlbumRoot albumRoot = await Task.Run(() => AlbumService.GetAlbum(album.id));
                if (albumRoot == null)
                    return;
                Frame.Navigate(typeof(AlbumDetail), albumRoot);
                ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
            }
        }

        private async void ListBox_hotSongs_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            if (songsItem == null)
                return;
            
            PlayingService.PlayingListId= songsItem.al.id;
            await PlayingService.ChangePlayingSong(songsItem.Id, artistBaseDetailRoot.hotSongs, songsItem);
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            if (pivotItem.Tag.ToString() == "1")
            {
                if (artistBaseDetailRoot == null|| AdaptiveGridView_albums.ItemsSource!=null)
                    return;
                ArtistAllAlbumRoot artistAllAlbumRoot=await Task.Run(() => AlbumService.GetArtistAllAlbums(artistBaseDetailRoot.artist.id));
                if(artistAllAlbumRoot!=null)
                {
                    AdaptiveGridView_albums.ItemsSource = artistAllAlbumRoot.hotAlbums;
                }
            }
        }

        private async void Button_allSongs_Click(object sender, RoutedEventArgs e)
        {
            PlayingService.PlayingListId= artistBaseDetailRoot.hotSongs.First().al.id;
            await PlayingService.ChangePlayingSong(artistBaseDetailRoot.hotSongs.First().Id, artistBaseDetailRoot.hotSongs, artistBaseDetailRoot.hotSongs.First());
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

        private async void Button_album_Click(object sender, RoutedEventArgs e)
        {
            SongsItem songsItem = ((Button)sender).DataContext as SongsItem;
            if (songsItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            AlbumRoot albumRoot = await Task.Run(() => AlbumService.GetAlbum(songsItem.al.id));
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
