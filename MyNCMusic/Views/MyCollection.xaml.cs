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
using Windows.UI.Xaml.Navigation;

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyCollection : Page
    {
        public MyCollection()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MyCollection_Loaded;
        }

        private async void MyCollection_Loaded(object sender, RoutedEventArgs e)
        {
            if (AlbumList.ItemsSource != null)
                return;
            Controls.WaitingPopup.Show();
            MyCollectionfAlbumRoot myPlaylistRoot = await AlbumService.GetMyCollectionOfAlbumAsync();
            Controls.WaitingPopup.Hide();
            if (myPlaylistRoot != null)
            {
                AlbumList.ItemsSource = myPlaylistRoot.Data;
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((Pivot)sender).SelectedIndex == 1&& ArtistList.ItemsSource==null)
            {
                Controls.WaitingPopup.Show();
                MyCollectionfArtistRoot myCollectionfArtistRoot = await ArtistService.GetMyCollectionOfArtistAsync();
                Controls.WaitingPopup.Hide();
                if (myCollectionfArtistRoot != null)
                {
                    ArtistList.ItemsSource = myCollectionfArtistRoot.Data;
                }
            }
        }
        private void AlbumList_OnChangedAlbum(AlbumRoot album)
        {
            NavigateService.NavigateToAlbumAsync(album);
        }

        private void AlbumList_OnChangedArtist(ArtistBaseDetailRoot artist)
        {
            NavigateService.NavigateToArtistAsync(artist);
        }

        private void ArtistList_OnChangedArtist(ArtistBaseDetailRoot artist)
        {
            NavigateService.NavigateToArtistAsync(artist);
        }
    }
}
