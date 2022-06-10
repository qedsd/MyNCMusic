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
    public sealed partial class MusicList : UserControl
    {
        public MusicList()
        {
            this.InitializeComponent();
            PlayingService.OnPlayingChanged += PlayingService_OnPlayingChanged;
        }

        private void PlayingService_OnPlayingChanged(long id, string url)
        {
            if(ItemsSource!=null)
            {
                foreach(var item in ItemsSource)
                {
                    if(item.Id == id)
                    {
                        item.IsPlaying = true;
                    }
                    else
                    {
                        item.IsPlaying = false;
                    }
                }
            }
        }

        public delegate void ChangedSongDelegate(MusicItem musicItem);
        public event ChangedSongDelegate OnChangedSong;
        private void ListBox_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            OnChangedSong?.Invoke(((ListBox)sender).SelectedItem as MusicItem);
        }

        public delegate void ChangedAlbumDelegate(AlbumRoot album);
        public event ChangedAlbumDelegate OnChangedAlbum;
        private async void Button_Album_Click(object sender, RoutedEventArgs e)
        {
            AlbumRoot albumRoot = await AlbumService.GetAlbumAsync((((sender as Button).DataContext as MusicItem).Al.Id));
            OnChangedAlbum?.Invoke(albumRoot);
        }

        public delegate void ChangedArtistDelegate(ArtistBaseDetailRoot artist);
        public event ChangedArtistDelegate OnChangedArtist;
        private async void ListBox_Artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            if (artist != null)
            {
                ArtistBaseDetailRoot artistBaseDetailRoot = await ArtistService.GetArtistBaseDetailAsync(artist.Id);
                OnChangedArtist?.Invoke(artistBaseDetailRoot);
            }
        }

        private async void Button_Artists_Click(object sender, RoutedEventArgs e)
        {
            ArtistBaseDetailRoot artistBaseDetailRoot = await ArtistService.GetArtistBaseDetailAsync(((sender as Button).DataContext as MusicItem).Ar.First().Id);
            OnChangedArtist?.Invoke(artistBaseDetailRoot);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<Models.MusicItem>), typeof(MusicList), new PropertyMetadata(null, new PropertyChangedCallback(DataCountPropertyChanged)));
        private static void DataCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null)
            {
                (e.NewValue as List<Models.MusicItem>).ForEach(p =>
                {
                    p.IsFavorite = PlayingService.FavoriteMusics.Contains(p.Id);
                    p.IsPlaying = p.Id == PlayingService.PlayingSongId;
                });
            }
            ((MusicList)d).ListBox_MusicItems.ItemsSource = (List<Models.MusicItem>)e.NewValue;
        }
        public List<Models.MusicItem> ItemsSource
        {
            get => (List<Models.MusicItem>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
    }
}
