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
    public sealed partial class MyMusicList : Page
    {
        List<PlaylistItem> playlistItems_created;
        List<PlaylistItem> playlistItems_subscribed;
        public MyMusicList()
        {
            this.InitializeComponent();
            playlistItems_created = new List<PlaylistItem>();
            playlistItems_subscribed = new List<PlaylistItem>();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MyMusicList_Loaded;
        }

        private async void MyMusicList_Loaded(object sender, RoutedEventArgs e)
        {
            MyPlaylistRoot myPlaylistRoot = await Task.Run(() => MyClassManager.GetMyPlaylist());
            foreach(var temp in myPlaylistRoot.playlist)
            {
                if (temp.subscribed == "true")
                    playlistItems_subscribed.Add(temp);
                else
                    playlistItems_created.Add(temp);
            }
            myPlaylistRoot = null;
            AdaptiveGridView_createdByMe.ItemsSource = playlistItems_created;
            AdaptiveGridView_subscribed.ItemsSource = playlistItems_subscribed;
        }

        private async void AdaptiveGridView_createdByMe_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressBar_loadPlaylistDetail.Visibility = Visibility.Visible;
            PlaylistItem playlistItem = e.ClickedItem as PlaylistItem;
            if (playlistItem == null)
                return;
            List<Object> list=await Task.Run(() => prepareToNavigeteToPlaylitDetail(playlistItem.id));
            if(list!=null)
                Frame.Navigate(typeof(PlayListDetai), list);
            ProgressBar_loadPlaylistDetail.Visibility = Visibility.Collapsed;
        }

        private async void AdaptiveGridView_subscribed_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressBar_loadPlaylistDetail.Visibility = Visibility.Visible;
            PlaylistItem playlistItem = e.ClickedItem as PlaylistItem;
            if (playlistItem == null)
                return;
            List<Object> list = await Task.Run(() => prepareToNavigeteToPlaylitDetail(playlistItem.id));
            if (list != null)
                Frame.Navigate(typeof(PlayListDetai), list);
            ProgressBar_loadPlaylistDetail.Visibility = Visibility.Collapsed;
        }

        List<Object> prepareToNavigeteToPlaylitDetail(long id)
        {
            PlayListDetailRoot playListDetailRoot = MyClassManager.GetPlayListDetail(id);
            if (playListDetailRoot == null || playListDetailRoot.playlist.trackIds.Count == 0)
                return null;
            string ids = "";
            for (int i = 0; i < playListDetailRoot.playlist.trackIds.Count; i++)
            {
                if (i != 0)
                    ids += ",";
                ids += playListDetailRoot.playlist.trackIds[i].id;
            }
            MusicDetailRoot musicDetailRoot = MyClassManager.GetMusicDetail_post(ids);
            if (musicDetailRoot == null)
                return null;
            List<Object> list = new List<object>();
            list.Add(playListDetailRoot);
            list.Add(musicDetailRoot);
            return list;
        }
    }
}
