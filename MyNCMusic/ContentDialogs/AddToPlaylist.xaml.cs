using MyNCMusic.Controls;
using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace MyNCMusic.ContentDialogs
{
    public sealed partial class AddToPlaylist : ContentDialog
    {
        public AddToPlaylist()
        {
            this.InitializeComponent();
            Loaded += MyPlaylist_Loaded;
        }

        private void MyPlaylist_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPlaylist();
        }
        private async void LoadPlaylist()
        {
            WaitingPopup.Show();
            MyPlaylistRoot myPlaylistRoot = await PlaylistService.GetMyPlaylistAsync();
            WaitingPopup.Hide();
            ListBox_CreatedPlaylist.ItemsSource = myPlaylistRoot.Playlist.Where(p => !p.Subscribed);
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Closed += AddToPlaylist_Closed;
            args.Cancel = true;
            this.Hide();
        }

        private async void AddToPlaylist_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            Closed -= AddToPlaylist_Closed;
            NewPlaylist newPlaylist = new NewPlaylist();
            await newPlaylist.ShowAsync();
            await this.ShowAsync();
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private async void ListBox_CreatedPlaylist_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var playlist = ((ListBox)sender).SelectedItem as PlaylistItem;
            if (await PlaylistService.AddToPlaylistAsync(playlist.Id, PlayingService.PlayingSong.Id))
            {
                NotifyPopup.ShowSuccess("已添加到歌单");
                playlist.CoverImgUrl = PlayingService.PlayingAlbum.Album.PicUrl;
                this.CloseButtonCommand?.Execute(null);
            }
            else
            {
                NotifyPopup.ShowError("添加失败");
            }
        }

        private void Button_CloseAddToPlaylistDialog_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private async void Button_AddNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            NewPlaylist newPlaylist = new NewPlaylist();
            if(await newPlaylist.ShowAsync() == ContentDialogResult.Primary)
            {
                LoadPlaylist();
            }
        }
    }
}
