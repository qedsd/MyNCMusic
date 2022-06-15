using MyNCMusic.Models;
using Prism.Commands;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace MyNCMusic.Controls
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public sealed partial class PlaylistList : UserControl
    {
        public List<PlaylistItem> PlaylistItems { get; set; }
        public PlaylistItem SelectedPlaylist { get; set; }
        public PlaylistList()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(List<PlaylistItem>), typeof(PlaylistList), new PropertyMetadata(null, new PropertyChangedCallback(ItemsSourcePropertyChanged)));
        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlaylistList)d).AdaptiveGridView.ItemsSource = (List<PlaylistItem>)e.NewValue;
        }
        public List<PlaylistItem> ItemsSource
        {
            get => (List<PlaylistItem>)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(PlaylistItem), typeof(PlaylistList), new PropertyMetadata(null, new PropertyChangedCallback(SelectedItemPropertyChanged)));
        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlaylistList)d).AdaptiveGridView.SelectedItem = (PlaylistItem)e.NewValue;
        }
        public PlaylistItem SelectedItem
        {
            get => (PlaylistItem)GetValue(SelectedItemProperty);
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedItem = e.ClickedItem as PlaylistItem;
            OnChangedPlaylist?.Invoke(SelectedItem);
        }

        public delegate void ChangedPlaylistDelegate(PlaylistItem playlistItem);
        public event ChangedPlaylistDelegate OnChangedPlaylist;

        public static readonly DependencyProperty ItemClickCommandProperty =
            DependencyProperty.Register("ItemClickCommand", typeof(DelegateCommand), typeof(PlaylistList), new PropertyMetadata(null, new PropertyChangedCallback(ItemClickCommandPropertyChanged)));
        private static void ItemClickCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlaylistList)d).AdaptiveGridView.ItemClickCommand = (DelegateCommand)e.NewValue;
        }
        /// <summary>
        /// 会在触发SelectedItem前触发，慎用,ItemClick同理
        /// </summary>
        public DelegateCommand ItemClickCommand
        {
            get => (DelegateCommand)GetValue(ItemClickCommandProperty);
            set
            {
                SetValue(ItemClickCommandProperty, value);
            }
        }
    }
}
