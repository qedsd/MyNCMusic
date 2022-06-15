using MyNCMusic.Models;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyNCMusic.Controls
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public sealed partial class AlbumList : UserControl
    {
        public AlbumList()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<Models.Album>), typeof(AlbumList), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourcePropertyChanged)));
        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AlbumList)d).ListBox_Album.ItemsSource = (List<Models.Album>)e.NewValue;
        }
        public List<Models.Album> ItemsSource
        {
            get => (List<Models.Album>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public delegate void ChangedAlbumDelegate(AlbumRoot albumRoot);
        public event ChangedAlbumDelegate OnChangedAlbum;
        private bool IsHandledTapped = false;
        private async void ListBox_Album_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(IsHandledTapped)
            {
                IsHandledTapped = false;
                return;
            }
            var album = ((ListBox)sender).SelectedItem as Album;
            if(album != null)
            {
                Controls.WaitingPopup.Show();
                AlbumRoot albumRoot = await AlbumService.GetAlbumAsync(album.Id);
                Controls.WaitingPopup.Hide();
                if (albumRoot != null)
                {
                    OnChangedAlbum?.Invoke(albumRoot);
                }
            }
        }

        public delegate void ChangedArtistDelegate(ArtistBaseDetailRoot artist);
        public event ChangedArtistDelegate OnChangedArtist;

        private async void ListBox_Artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            if (artist != null)
            {
                ((ListBox)sender).SelectedIndex = -1;
                Controls.WaitingPopup.Show();
                ArtistBaseDetailRoot artistBaseDetailRoot = await ArtistService.GetArtistBaseDetailAsync(artist.Id);
                Controls.WaitingPopup.Hide();
                if (artistBaseDetailRoot != null)
                {
                    OnChangedArtist?.Invoke(artistBaseDetailRoot);
                }
            }
        }

        private async void Button_Artists_Click(object sender, RoutedEventArgs e)
        {
            IsHandledTapped = true;
            Controls.WaitingPopup.Show();
            ArtistBaseDetailRoot artistBaseDetailRoot = await ArtistService.GetArtistBaseDetailAsync(((sender as Button).DataContext as Album).Artists.First().Id);
            Controls.WaitingPopup.Hide();
            OnChangedArtist?.Invoke(artistBaseDetailRoot);
        }
        /// <summary>
        /// 多个歌手不处理，仅屏蔽Tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Artists_Click2(object sender, RoutedEventArgs e)
        {
            IsHandledTapped = true;
        }
    }
}
