using MyNCMusic.Model;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlayListDetai : Page
    {
        MusicDetailRoot musicDetailRootSource;//具体每一首歌
        PlayListDetailRoot playListDetailRoot;//歌单信息
        long playListDetailID = 0;
        public PlayListDetai()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            (Application.Current as App).PlayListDetai = this;
            ////设置歌单信息

            ////添加歌曲
            //musicDetailRootSource.songs.Clear();
            //foreach(var temp in musicDetailRoot.songs)
            //{
            //    musicDetailRootSource.songs.Add(temp);
            //}
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            var list = (List<Object>)e.Parameter;
            playListDetailRoot = list.First() as PlayListDetailRoot;
            musicDetailRootSource = list.Last() as MusicDetailRoot;
            //if (playListDetailRoot.playlist.id == MainPage.PlayingListId)
            //    return;
            if (playListDetailRoot.playlist.id == playListDetailID)
                return;
            playListDetailID = playListDetailRoot.playlist.id;
            LoadLayout();
        }
        void LoadLayout()
        {
            Image_playlist.Source = new BitmapImage(new Uri(playListDetailRoot.playlist.coverImgUrl));
            TextBlock_listName.Text = playListDetailRoot.playlist.name;
            Button_ar.Content = playListDetailRoot.playlist.creator.nickname;
            TextBlock_des.Text = playListDetailRoot.playlist.description==null?"": playListDetailRoot.playlist.description;
            TextBlock_subCount.Text = playListDetailRoot.playlist.subscribedCount.ToString();
            TextBlock_comentCount.Text = playListDetailRoot.playlist.commentCount.ToString();
            TextBlock_songsCount.Text = musicDetailRootSource.songs.Count.ToString();
            //判断是否为喜欢歌曲
            if (MainPage.favoriteSongsRoot != null)
            {
                foreach (var temp in musicDetailRootSource.songs)
                {
                    if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.Id)) != 0)
                        temp.isFavorite = true;
                }
            }
            //是否已收藏
            if (playListDetailRoot.playlist.subscribed == "true")
            {
                TextBlock_subIcon.Text = "\xF89A";
                TextBlock_subscribe.Text = "已收藏";
            }
            else
            {
                TextBlock_subIcon.Text = "\xECCD";
                TextBlock_subscribe.Text = "收藏";
            }
            ListBox_musicDetail.ItemsSource = musicDetailRootSource.songs;
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        /// <summary>
        /// 双击某一首歌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListBox_musicDetail_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            PlayingService.PlayingListId= playListDetailRoot.playlist.id;
            await PlayingService.ChangePlayingSong(songsItem.Id, musicDetailRootSource.songs, songsItem);
            
        }
        /// <summary>
        /// 播放全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_playAll_Click(object sender, RoutedEventArgs e)
        {
            PlayingService.PlayingListId = playListDetailRoot.playlist.id;
            await PlayingService.ChangePlayingSong(musicDetailRootSource.songs.First().Id, musicDetailRootSource.songs, musicDetailRootSource.songs.First());
        }

        private void AutoSuggestBox_search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text == "")
                ListBox_musicDetail.ItemsSource = musicDetailRootSource.songs;
            else
            {
                var list = musicDetailRootSource.songs.Where(p => p.Name.Contains(sender.Text));
                if (list != null && list.Count() != 0)
                {
                    ListBox_musicDetail.ItemsSource = list;
                }
            }
        }

        private async void Button_aritis_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SongsItem songsItem = button.DataContext as SongsItem;
            if (songsItem.ar.Count == 1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(songsItem.ar.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        private async void Button_album_Click(object sender, RoutedEventArgs e)
        {
            SongsItem songsItem = ((Button)sender).DataContext as SongsItem;
            if (songsItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            AlbumRoot albumRoot = await Task.Run(() => AlbumService.GetAlbum(songsItem.al.id));
            if (albumRoot == null)
            {
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                return;
            }
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            Frame.Navigate(typeof(AlbumDetail), albumRoot);
        }

        private async void ListBox_artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Artist arItem = ((ListBox)sender).SelectedItem as Artist;
            if (arItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(arItem.id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }

        private async void Button_comment_Click(object sender, RoutedEventArgs e)
        {
            //var dialog = new ContentDialog();
            //Comment comment = new Comment(playListDetailRoot.playlist.id);
            //dialog.Content = comment;
            //await dialog.ShowAsync();
            //ProgressBar_loadingComment.Visibility = Visibility.Visible;
            //CommentRoot commentRoot = await Task.Run(() => MyClassManager.GetPlayListComment(playListDetailRoot.playlist.id));
            //ProgressBar_loadingComment.Visibility = Visibility.Collapsed;
            //ListBox_HotComment.ItemsSource = commentRoot.hotComments == null ? null : commentRoot.hotComments;
            //ListBox_allComment.ItemsSource = commentRoot.topComments == null ? null : commentRoot.comments;
            ProgressBar_loading.Visibility = Visibility.Visible;
            CommentRoot commentRoot = await Task.Run(() => CommentService.GetPlaylistComment(playListDetailRoot.playlist.id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (commentRoot == null)
                return;
            Frame.Navigate(typeof(Comment), commentRoot);
        }


        private async void Button_sub_Click(object sender, RoutedEventArgs e)
        {
            if (playListDetailRoot.playlist.subscribed == "true")
            {
                if(await Task.Run(()=>PlaylistService.SubOrCancelPlayList(playListDetailRoot.playlist.id,2)))
                {
                    NotifyPopup notifyPopup = new NotifyPopup("已取消收藏", "\xE224", Colors.MediumSeaGreen);
                    notifyPopup.Show();
                    TextBlock_subIcon.Text = "\xECCD";
                    TextBlock_subscribe.Text = "收藏";
                }
                else
                {
                    NotifyPopup notifyPopup = new NotifyPopup("操作失败", "\xE171");
                    notifyPopup.Show();
                }
            }
            else
            {
                if (await Task.Run(() => PlaylistService.SubOrCancelPlayList(playListDetailRoot.playlist.id, 1)))
                {
                    NotifyPopup notifyPopup = new NotifyPopup("已添加收藏", "\xE082", Colors.MediumSeaGreen);
                    notifyPopup.Show();
                    TextBlock_subIcon.Text = "\xF89A";
                    TextBlock_subscribe.Text = "已收藏";
                }
                else
                {
                    NotifyPopup notifyPopup = new NotifyPopup("操作失败", "\xE171");
                    notifyPopup.Show();
                }
            }
        }
    }
}
