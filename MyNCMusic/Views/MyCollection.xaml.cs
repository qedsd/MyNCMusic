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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyCollection : Page
    {
        //public static SolidColorBrush mainSolidColorBrush;
        public MyCollection()
        {
            //mainSolidColorBrush = MainPage.mainSolidColorBrush;
            this.InitializeComponent();
            
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MyCollection_Loaded;
        }

        private async void MyCollection_Loaded(object sender, RoutedEventArgs e)
        {
            if (ListBox_album.ItemsSource != null)
                return;
            ProgressBar_loadAlbum.Visibility = Visibility.Visible;
            MyCollectionfAlbumRoot myPlaylistRoot = await Task.Run(() => MyClassManager.GetMyCollectionOfAlbum());
            if (myPlaylistRoot == null)
            {
                ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
                return;
            }
            ListBox_album.ItemsSource = myPlaylistRoot.data;
            ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
        }

        private async void ListBox_album_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CADataItem cADataItem = ((ListBox)sender).SelectedItem as CADataItem;
            if (cADataItem == null)
                return;
            if (cADataItem.id == MainPage.PlayingListId)
                Frame.Navigate(typeof(AlbumDetail));
            else
            {
                ProgressBar_loadAlbum.Visibility = Visibility.Visible;
                AlbumRoot albumRoot = await Task.Run(() => MyClassManager.GetMAlbum(cADataItem.id));
                if (albumRoot == null)
                {
                    ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
                    return;
                }
                Frame.Navigate(typeof(AlbumDetail), albumRoot);
                ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            if (pivotItem.Tag.ToString() == "1")
            {
                if (ListBox_artist.ItemsSource != null)
                    return;
                ProgressBar_loadArtist.Visibility = Visibility.Visible;
                MyCollectionfArtistRoot myCollectionfArtistRoot=await Task.Run(() => MyClassManager.GetMyCollectionOfArtist());
                if (myCollectionfArtistRoot == null)
                {
                    ProgressBar_loadArtist.Visibility = Visibility.Collapsed;
                    return;
                }
                ListBox_artist.ItemsSource = myCollectionfArtistRoot.data;
                ProgressBar_loadArtist.Visibility = Visibility.Collapsed;
            }
        }

        private async void ListBox_artist_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ProgressBar_loadArtist.Visibility = Visibility.Visible;
            ListBox listBox = (ListBox)sender;
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            ArtistBaseDetailRoot artistBaseDetailRoot=await Task.Run(() => MyClassManager.GetArtistBaseDetail(artist.id));
            ProgressBar_loadArtist.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }

        private async void Button_artists_Click_Artist(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            List<Artist> artists = ((CADataItem)button.DataContext).artists as List<Artist>;
            if (artists.Count == 1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => MyClassManager.GetArtistBaseDetail(artists.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        private async void ListBox_artists_SelectionChanged_Artist(object sender, SelectionChangedEventArgs e)
        {
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            if (artist == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => MyClassManager.GetArtistBaseDetail(artist.id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }
    }
}
