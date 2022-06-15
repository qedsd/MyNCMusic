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
    public sealed partial class ArtistList : UserControl
    {
        public ArtistList()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<Models.Artist>), typeof(ArtistList), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourcePropertyChanged)));
        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ArtistList)d).ListBox_Artist.ItemsSource = (List<Models.Artist>)e.NewValue;
        }
        public List<Models.Artist> ItemsSource
        {
            get => (List<Models.Artist>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        public delegate void ChangedArtistDelegate(ArtistBaseDetailRoot artist);
        public event ChangedArtistDelegate OnChangedArtist;
        private async void ListBox_Artist_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            ArtistBaseDetailRoot artistBaseDetailRoot = await ArtistService.GetArtistBaseDetailAsync(artist.Id);
            if (artistBaseDetailRoot != null)
            {
                OnChangedArtist?.Invoke(artistBaseDetailRoot);
            }
        }
    }
}
