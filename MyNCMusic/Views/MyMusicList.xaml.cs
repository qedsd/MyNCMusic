using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        ObservableCollection<PlaylistItem> playlistItems_created = PlayingService.PlaylistItems_Created;
        ObservableCollection<PlaylistItem> playlistItems_subscribed = PlayingService.PlaylistItems_Subscribed;
        public MyMusicList()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MyMusicList_Loaded;
        }

        private async void MyMusicList_Loaded(object sender, RoutedEventArgs e)
        {
            if (playlistItems_created == null && playlistItems_subscribed == null)
            {
                playlistItems_created = new ObservableCollection<PlaylistItem>();
                playlistItems_subscribed = new ObservableCollection<PlaylistItem>();
                MyPlaylistRoot myPlaylistRoot = await Task.Run(() => PlaylistService.GetMyPlaylist());
                if(myPlaylistRoot==null|| myPlaylistRoot.Playlist==null)
                {
                    NotifyPopup notifyPopup = new NotifyPopup("获取失败");
                    notifyPopup.Show();
                    return;
                }
                foreach (var temp in myPlaylistRoot.Playlist)
                {
                    if (temp.Subscribed)
                        playlistItems_subscribed.Add(temp);
                    else
                        playlistItems_created.Add(temp);
                }
                myPlaylistRoot = null;
            }
            AdaptiveGridView_createdByMe.ItemsSource = playlistItems_created;
            AdaptiveGridView_subscribed.ItemsSource = playlistItems_subscribed;
        }

        private async void AdaptiveGridView_createdByMe_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressBar_loadPlaylistDetail.Visibility = Visibility.Visible;
            PlaylistItem playlistItem = e.ClickedItem as PlaylistItem;
            if (playlistItem == null)
                return;
            List<Object> list=await Task.Run(() => prepareToNavigeteToPlaylitDetail(playlistItem.Id));
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
            List<Object> list = await Task.Run(() => prepareToNavigeteToPlaylitDetail(playlistItem.Id));
            if (list != null)
                Frame.Navigate(typeof(PlayListDetai), list);
            ProgressBar_loadPlaylistDetail.Visibility = Visibility.Collapsed;
        }

        List<Object> prepareToNavigeteToPlaylitDetail(long id)
        {
            PlayListDetailRoot playListDetailRoot = PlaylistService.GetPlaylistDetail(id);
            if (playListDetailRoot == null || playListDetailRoot.Playlist.TrackIds.Count == 0)
                return null;
            string ids = "";
            MusicDetailRoot musicDetailRoot = new MusicDetailRoot();
            musicDetailRoot.Songs = new List<MusicItem>();
            musicDetailRoot.Privileges = new List<PrivilegesItem>();
            for (int i=0;i< playListDetailRoot.Playlist.TrackIds.Count;i+=1000)//最高单次1000个
            {
                ids = "";
                int j = i;
                if((i+1000)> playListDetailRoot.Playlist.TrackIds.Count)//剩下的不足1000
                {
                    for (; j < playListDetailRoot.Playlist.TrackIds.Count; j++)
                    {
                        if (j%1000 != 0)
                            ids += ",";
                        ids += playListDetailRoot.Playlist.TrackIds[j].Id;
                    }
                }
                else//剩下的超过1000
                {
                    for (; j < i + 1000; j++)
                    {
                        if (j%1000 != 0)
                            ids += ",";
                        ids += playListDetailRoot.Playlist.TrackIds[j].Id;
                    }
                }
                MusicDetailRoot musicDetailRootTemp = SongService.GetMusicDetail_Post(ids);
                if(musicDetailRootTemp!=null&&musicDetailRootTemp.Songs!=null&& musicDetailRootTemp.Privileges!=null)
                {
                    foreach(var temp in musicDetailRootTemp.Songs)
                    {
                        musicDetailRoot.Songs.Add(temp);
                    }
                    foreach (var temp in musicDetailRootTemp.Privileges)
                    {
                        musicDetailRoot.Privileges.Add(temp);
                    }
                    musicDetailRoot.Code = musicDetailRootTemp.Code;
                }
            }
            List<Object> list = new List<object>();
            list.Add(playListDetailRoot);
            list.Add(musicDetailRoot);
            return list;
        }
    }
}
