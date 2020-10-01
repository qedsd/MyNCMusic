using Microsoft.Toolkit.Uwp.UI.Controls;
using MyNCMusic.Model;
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
                    if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.id)) != 0)
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
            if (album.id == MainPage.PlayingListId)
                Frame.Navigate(typeof(AlbumDetail));
            else
            {
                ProgressBar_loadAlbum.Visibility = Visibility.Visible;
                AlbumRoot albumRoot = await Task.Run(() => MyClassManager.GetMAlbum(album.id));
                if (albumRoot == null)
                    return;
                Frame.Navigate(typeof(AlbumDetail), albumRoot);
                ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
            }
        }

        private void ListBox_hotSongs_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            if (songsItem == null)
                return;
            SongUrlRoot songUrlRoot = MyClassManager.GetMusicUrl(songsItem.id);
            if (songUrlRoot == null)
                return;

            //修改播放列表
            if (MainPage.PlayingListId != artistBaseDetailRoot.artist.id)//已在播放此歌单，仅修改播放歌曲，否则，重置播放列表及历史记录
            {
                (Application.Current as App).myMainPage.currentPlayList.Clear();
                foreach (var temp in artistBaseDetailRoot.hotSongs)
                    (Application.Current as App).myMainPage.currentPlayList.Add(temp);
                (Application.Current as App).myMainPage.playHistoryIndex.Clear();
                MainPage.PlayingListId = artistBaseDetailRoot.artist.id;
            }
            //修改mainpage以触发修改正在播放的音乐
            (Application.Current as App).myMainPage.ChnagePlayingSong(songsItem, songUrlRoot);
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            if (pivotItem.Tag.ToString() == "1")
            {
                if (artistBaseDetailRoot == null|| AdaptiveGridView_albums.ItemsSource!=null)
                    return;
                ArtistAllAlbumRoot artistAllAlbumRoot=await Task.Run(() => MyClassManager.GetArtistAllAlbums(artistBaseDetailRoot.artist.id));
                if(artistAllAlbumRoot!=null)
                {
                    AdaptiveGridView_albums.ItemsSource = artistAllAlbumRoot.hotAlbums;
                }
            }
        }

        private void Button_allSongs_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).myMainPage.currentPlayList.Clear();
            foreach (var temp in artistBaseDetailRoot.hotSongs)
                (Application.Current as App).myMainPage.currentPlayList.Add(temp);
            (Application.Current as App).myMainPage.playHistoryIndex.Clear();
            MainPage.PlayingListId = artistBaseDetailRoot.artist.id;
            (Application.Current as App).myMainPage.PlayNextSongs(-1);
        }

        private async void Button_artists_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SongsItem songsItem = button.DataContext as SongsItem;
            if (songsItem.ar.Count == 1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => MyClassManager.GetArtistBaseDetail(songsItem.ar.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        private async void ListBox_artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArItem arItem = ((ListBox)sender).SelectedItem as ArItem;
            if (arItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => MyClassManager.GetArtistBaseDetail(arItem.id));
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
            AlbumRoot albumRoot = await Task.Run(() => MyClassManager.GetMAlbum(songsItem.al.id));
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
