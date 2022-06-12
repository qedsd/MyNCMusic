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
    public sealed partial class NewPlaylist : ContentDialog
    {
        public NewPlaylist()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PlayListDetailRoot playListDetailRoot = await PlaylistService.AddNewPlaylistAsync((bool)CheckBox_Privacy.IsChecked, TextBox_PlaylistName.Text);
            if (playListDetailRoot.Code != 200)
            {
                NotifyPopup.ShowError("创建失败");
                args.Cancel = true;
            }
            else
            {
                NotifyPopup.ShowSuccess("创建成功");
                //PlayingService.PlaylistItems_Created.Insert(1, playListDetailRoot.Playlist);
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void TextBox_PlaylistName_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (sender.Text.Length > 20)
            {
                sender.Text = sender.Text.Substring(0, 20);
            }
        }
    }
}
