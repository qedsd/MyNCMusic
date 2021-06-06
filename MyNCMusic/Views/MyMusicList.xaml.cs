using MyNCMusic.Model;
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
                if(myPlaylistRoot==null|| myPlaylistRoot.playlist==null)
                {
                    NotifyPopup notifyPopup = new NotifyPopup("获取失败");
                    notifyPopup.Show();
                    return;
                }
                foreach (var temp in myPlaylistRoot.playlist)
                {
                    if (temp.subscribed == "true")
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
            PlayListDetailRoot playListDetailRoot = PlaylistService.GetPlaylistDetail(id);
            if (playListDetailRoot == null || playListDetailRoot.playlist.trackIds.Count == 0)
                return null;
            string ids = "";
            //for (int i = 0; i < playListDetailRoot.playlist.trackIds.Count; i++)
            //for (int i = 0; i < 1000; i++)
            //{
            //    if (i != 0)
            //        ids += ",";
            //    ids += playListDetailRoot.playlist.trackIds[i].id;
            //}
            MusicDetailRoot musicDetailRoot = new MusicDetailRoot();
            musicDetailRoot.songs = new List<SongsItem>();
            musicDetailRoot.privileges = new List<PrivilegesItem>();
            for (int i=0;i< playListDetailRoot.playlist.trackIds.Count;i+=1000)//最高单次1000个
            {
                ids = "";
                int j = i;
                if((i+1000)> playListDetailRoot.playlist.trackIds.Count)//剩下的不足1000
                {
                    for (; j < playListDetailRoot.playlist.trackIds.Count; j++)
                    {
                        if (j%1000 != 0)
                            ids += ",";
                        ids += playListDetailRoot.playlist.trackIds[j].id;
                    }
                }
                else//剩下的超过1000
                {
                    for (; j < i + 1000; j++)
                    {
                        if (j%1000 != 0)
                            ids += ",";
                        ids += playListDetailRoot.playlist.trackIds[j].id;
                    }
                }
                MusicDetailRoot musicDetailRootTemp = SongService.GetMusicDetail_Post(ids);
                if(musicDetailRootTemp!=null&&musicDetailRootTemp.songs!=null&& musicDetailRootTemp.privileges!=null)
                {
                    foreach(var temp in musicDetailRootTemp.songs)
                    {
                        musicDetailRoot.songs.Add(temp);
                    }
                    foreach (var temp in musicDetailRootTemp.privileges)
                    {
                        musicDetailRoot.privileges.Add(temp);
                    }
                    musicDetailRoot.code = musicDetailRootTemp.code;
                }
            }
            List<Object> list = new List<object>();
            list.Add(playListDetailRoot);
            list.Add(musicDetailRoot);
            return list;
        }
    }
}
