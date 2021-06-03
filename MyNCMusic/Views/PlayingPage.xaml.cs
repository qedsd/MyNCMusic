using MyNCMusic.Helper;
using MyNCMusic.Model;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
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
    public sealed partial class PlayingPage : Page
    {
        SongsItem _songsItem;
        AlbumRoot _albumRoot;
        List<LyricStr> lyricStrs;
        bool isLoad=false;
        public PlayingPage()
        {
            lyricStrs = new List<LyricStr>();
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            (Application.Current as App).playingPage = this;
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　　
            //List<Object> list= e.Parameter as List<Object>;
            //SongsItem songsItem_temp = list.First() as SongsItem;
            //AlbumRoot albumRoot_temp = list.Last() as AlbumRoot;
            //if (songsItem_temp == _songsItem)
            //    return;
            //_songsItem = songsItem_temp;
            //_albumRoot = albumRoot_temp;
            if(!isLoad)//仅第一次加载改页面才主动调用LoadLayout更新界面，其他时候都是由mainpage控制
                LoadLayout();
        }
        public void LoadLayout()
        {
            isLoad = true;
            ProgressBar_loadLyric.Visibility = Visibility.Visible;
            if (PlayingService.IsPlayingSong)
                UpDateSong();
            else
                UpDateRadio();
            ProgressBar_loadLyric.Visibility = Visibility.Collapsed;
        }

        void UpDateRadio()
        {
            Image_album.Source = PlayingService.PlayingAlbumBitmapImage;
            Button_albumName.Content = PlayingService.PlayingRadio.Radio.Name;
            
            ListBox_artists.ItemsSource = null;
            Button_artistName.Content = PlayingService.PlayingRadio.Dj.Nickname;
            TextBlock_songName.Text = PlayingService.PlayingRadio.Name;
            ListBox_lyric.ItemsSource = null;
            UpDateRadioComment(PlayingService.PlayingRadio.Id);
            ListBox_simiSongs.ItemsSource = null;//相似不可用

        }

        async void UpDateSong()
        {
            _albumRoot = PlayingService.PlayingAlbum;
            _songsItem = PlayingService.PlayingSong;
            Image_album.Source = PlayingService.PlayingAlbumBitmapImage;
            Button_albumName.Content = _albumRoot.album.name;
            string name = "";
            for (int i = 0; i < _songsItem.ar.Count; i++)
            {
                if (i != 0)
                    name += "/";
                name += _songsItem.ar[i].name;
            }
            ListBox_artists.ItemsSource = _songsItem.ar;
            Button_artistName.Content = name;
            TextBlock_songName.Text = _songsItem.Name;
            LyricRoot lyricRoot = await Task.Run(() => LyricService.GetLyric(_songsItem.Id));
            if (lyricRoot == null)
            {
                ListBox_lyric.ItemsSource = null;
                return;
            }
            lyricStrs = LyricService.GetLyricStrs(lyricRoot);
            if (lyricStrs != null)
            {
                if (lyricStrs.Count == 0)
                {
                    DateTime dateTime = DateTime.Now;
                    lyricStrs.Add(new LyricStr() { DateTime = dateTime, Original = "无歌词" });
                }
                ListBox_lyric.ItemsSource = lyricStrs;
                ListBox_lyric.ScrollIntoView(lyricStrs.First());
            }
            UpDateComment();
            UpDateSimiSongs();
        }

        async void UpDateComment()
        {
            ProgressBar_loadComment.Visibility = Visibility.Visible;
            CommentRoot commentRoot = await Task.Run(() => CommentService.GetSongsComment(_songsItem.Id));
            if (commentRoot == null)
            {
                ProgressBar_loadComment.Visibility = Visibility.Collapsed;
                ListBox_HotComment.ItemsSource = null;
                ListBox_allComment.ItemsSource = null;
                TextBlock_commentCount.Text = "";
                return;
            }
            ListBox_HotComment.ItemsSource = commentRoot.hotComments;
            ListBox_allComment.ItemsSource = commentRoot.comments;
            TextBlock_commentCount.Text = commentRoot.total.ToString();
            ScrollViewer_comment.ChangeView(null, 0, null);
            ProgressBar_loadComment.Visibility = Visibility.Collapsed;
        }

        async void UpDateRadioComment(long id)
        {
            ProgressBar_loadComment.Visibility = Visibility.Visible;
            CommentRoot commentRoot = await Task.Run(() => CommentService.GetRadioComment(id));
            if (commentRoot == null)
            {
                ProgressBar_loadComment.Visibility = Visibility.Collapsed;
                ListBox_HotComment.ItemsSource = null;
                ListBox_allComment.ItemsSource = null;
                TextBlock_commentCount.Text = "";
                return;
            }
            ListBox_HotComment.ItemsSource = commentRoot.hotComments;
            ListBox_allComment.ItemsSource = commentRoot.comments;
            TextBlock_commentCount.Text = commentRoot.total.ToString();
            ScrollViewer_comment.ChangeView(null, 0, null);
            ProgressBar_loadComment.Visibility = Visibility.Collapsed;
        }

        async void UpDateSimiSongs()
        {
            ProgressBar_loadSimiSongs.Visibility = Visibility.Visible;
            SimiSongsRoot simiSongsRoot =await Task.Run(()=> SongService.GetSimiSongs(_songsItem.Id));
            if (simiSongsRoot == null)
            {
                ProgressBar_loadSimiSongs.Visibility = Visibility.Collapsed;
                ListBox_simiSongs.ItemsSource = null;
                return;
            }
            //此处获取的专辑信息为album，需手动赋值给al，歌手信息为artistsm，需改为ar
            foreach (var temp in simiSongsRoot.songs)
            {
                temp.al = temp.album;
                temp.ar = temp.artists;
            }
            //判断是否为喜欢歌曲
            if (MainPage.favoriteSongsRoot != null)
            {
                foreach (var temp in simiSongsRoot.songs)
                {
                    if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.Id)) != 0)
                        temp.isFavorite = true;
                }
            }
            ListBox_simiSongs.ItemsSource = simiSongsRoot.songs;
            ProgressBar_loadSimiSongs.Visibility = Visibility.Collapsed;
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void ListBox_simiSongs_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            if (songsItem == null)
                return;
            ProgressBar_loadSimiSongs.Visibility = Visibility.Visible;
            PlayingService.PlayingListId = songsItem.al.id;
            await PlayingService.ChangePlayingSong(songsItem.Id, ListBox_simiSongs.ItemsSource as List<SongsItem>, songsItem);
        }

        /// <summary>
        /// _mediaTimelineController_PositionChanged 250ms间隔，无可避免歌词延迟
        /// </summary>
        /// <param name="totalMilliseconds"></param>
        public async void ChangeLyricPosition(double totalMilliseconds)
        {
            if (lyricStrs == null || lyricStrs.Count == 0)
                return;
            int index=await Task.Run(()=>SearchPerfectTime(totalMilliseconds));
            if (index < 0)
                return;
            //下一句歌词距当前时间大于100ms先不显示
            if (lyricStrs[index].DateTime.TimeOfDay.TotalMilliseconds - totalMilliseconds > 100)
                return;
            if (ListBox_lyric.Items.Count!=0)
            {
                int showItemCount = (int)(ListBox_lyric.ActualHeight / 50);
                ListBox_lyric.SelectedIndex = index;
                if (index < ListBox_lyric.Items.Count- showItemCount/2)
                    ListBox_lyric.ScrollIntoView(lyricStrs[index + showItemCount/2]);
                if ((Application.Current as App).compactOverlayPage != null)
                {
                    List<LyricStr> lsTemp = new List<LyricStr>();
                    //前一句
                    if (index == 0)
                        lsTemp.Add(new LyricStr());
                    else
                        lsTemp.Add(lyricStrs[index - 1]);
                    //当前
                    lsTemp.Add(lyricStrs[index]);
                    //后一句
                    if (index == lyricStrs.Count - 1)
                        lsTemp.Add(new LyricStr());
                    else
                        lsTemp.Add(lyricStrs[index + 1]);
                    (Application.Current as App).compactOverlayPage.UpdateLayout_lyric(lsTemp);
                }
            }



        }

        /// <summary>
        /// 从歌词lyricStrs中寻找下一句歌词
        /// 二分查找
        /// </summary>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        int SearchPerfectTime(double totalMilliseconds)
        {
            if (lyricStrs.Count == 0)
                return -1;
            List<double> ranges = new List<double>();
            int low = 0, high = lyricStrs.Count - 1, mid;
            double range = lyricStrs[lyricStrs.Count / 2].DateTime.TimeOfDay.TotalMilliseconds > totalMilliseconds ? lyricStrs[lyricStrs.Count / 2].DateTime.TimeOfDay.TotalMilliseconds - totalMilliseconds : totalMilliseconds - lyricStrs[lyricStrs.Count / 2].DateTime.TimeOfDay.TotalMilliseconds;
            while (low <= high) //当前区间存在元素时循环
            {
                mid = (low + high) / 2;
                if (low == high)
                {
                    return low;
                }
                double rangeTemp_mid = Range(lyricStrs[mid].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds);

                //range = rangeTemp_mid;
                //判断前后数据变化
                if (mid == 0)//即只剩下0和1位置
                {
                    if (Range(lyricStrs[mid].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds) > Range(lyricStrs[mid + 1].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds))
                    {
                        return mid + 1;
                    }
                    else
                    {
                        return mid;
                    }
                }
                else
                {
                    double rangeTemp_previous = Range(lyricStrs[mid - 1].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds);
                    double rangeTemp_next = Range(lyricStrs[mid + 1].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds);
                    if (rangeTemp_mid < rangeTemp_previous && rangeTemp_mid < rangeTemp_next)
                    {
                        return mid;
                    }
                    if (rangeTemp_previous > rangeTemp_next)
                        low = mid + 1;
                    else
                        high = mid - 1;
                }
            }
            return -1;
        }
        double Range(double d1,double d2)
        {
            return d1 > d2 ? d1 - d2 : d2 - d1;
        }

        private void Button_compactOverlayback_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).myMainPage.IntoCompactOverlayMode();
        }

        private void Button_albumName_Click(object sender, RoutedEventArgs e)
        {
            if(PlayingService.IsPlayingSong)
                Frame.Navigate(typeof(AlbumDetail), _albumRoot);
            else
            {
                Frame.Navigate(typeof(RadioDetail), PlayingService.PlayingRadioList);
            }
        }

        private async void Button_artistName_Click(object sender, RoutedEventArgs e)
        {
            if(PlayingService.IsPlayingSong&&_songsItem.ar.Count==1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(_songsItem.ar.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        private async void ListBox_artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayingService.IsPlayingSong)
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
        }

        private void Button_CloseAddToPlaylistDialog_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog_CreatedPlaylist.Hide();
        }

        private async void Button_AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (!PlayingService.IsPlayingSong)
                return;
            if(PlayingService.PlaylistItems_Created==null&& PlayingService.PlaylistItems_Subscribed==null)
            {
                PlayingService.PlaylistItems_Created = new ObservableCollection<PlaylistItem>();
                PlayingService.PlaylistItems_Subscribed = new ObservableCollection<PlaylistItem>();
                MyPlaylistRoot myPlaylistRoot = await Task.Run(() => PlaylistService.GetMyPlaylist());
                foreach (var temp in myPlaylistRoot.playlist)
                {
                    if (temp.subscribed == "true")
                        PlayingService.PlaylistItems_Subscribed.Add(temp);
                    else
                        PlayingService.PlaylistItems_Created.Add(temp);
                }
                myPlaylistRoot = null;
            }
            ListBox_CreatedPlaylist.ItemsSource = PlayingService.PlaylistItems_Created;
            await ContentDialog_CreatedPlaylist.ShowAsync();
        }

        private void ListBox_CreatedPlaylist_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var playlist = ((ListBox)sender).SelectedItem as PlaylistItem;
            if (PlaylistService.AddToPlaylist(playlist.id, _songsItem.Id))
            {
                NotifyPopup notifyPopup = new NotifyPopup("已添加到歌单", "\xF78C", Colors.MediumSeaGreen);
                notifyPopup.Show();
                playlist.coverImgUrl = _albumRoot.album.picUrl;
                ContentDialog_CreatedPlaylist.Hide();
            }
            else
            {
                NotifyPopup notifyPopup = new NotifyPopup("添加失败", "\xF78A");
                notifyPopup.Show();
            }
        }

        private void Button_AddNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog_CreatedPlaylist.Closed += ContentDialog_CreatedPlaylist_Closed;
            ContentDialog_CreatedPlaylist.Hide();
        }

        private async void ContentDialog_CreatedPlaylist_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            ContentDialog_CreatedPlaylist.Closed -= ContentDialog_CreatedPlaylist_Closed;
            await ContentDialog_AddNewPlaylist.ShowAsync();
        }

        private void Button_CancelNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog_AddNewPlaylist.Closed += ContentDialog_AddNewPlaylist_Closed;
            ContentDialog_AddNewPlaylist.Hide();
        }

        private async void ContentDialog_AddNewPlaylist_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            ContentDialog_AddNewPlaylist.Closed -= ContentDialog_AddNewPlaylist_Closed;
            await ContentDialog_CreatedPlaylist.ShowAsync();
        }

        private void Button_ConfirmNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            PlayListDetailRoot playListDetailRoot=PlaylistService.AddNewPlaylist((bool)CheckBox_Privacy.IsChecked, TextBox_PlaylistName.Text);
            if(playListDetailRoot.code!=200)
            {
                NotifyPopup notifyPopup = new NotifyPopup("创建失败", "\xF78A");
                notifyPopup.Show();
            }
            else
            {
                NotifyPopup notifyPopup = new NotifyPopup("创建成功", "\xF78C", Colors.MediumSeaGreen);
                notifyPopup.Show();
                PlayingService.PlaylistItems_Created.Insert(1, playListDetailRoot.playlist);
                ContentDialog_AddNewPlaylist.Closed += ContentDialog_AddNewPlaylist_Closed;
                ContentDialog_AddNewPlaylist.Hide();
            }
        }

        private void TextBox_PlaylistName_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if(sender.Text.Length>20)
            {
                sender.Text = sender.Text.Substring(0, 20);
            }
        }

        private void Button_FullScreenMode_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isInFullScreenMode = view.IsFullScreenMode;
            if (isInFullScreenMode)
            {
                view.ExitFullScreenMode();
                TextBlock_FullScreenModeTip.Text = "全屏";
                FontIcon_FullScreenMode.Glyph = "\xE740";
            }
            else
            {
                view.TryEnterFullScreenMode();
                TextBlock_FullScreenModeTip.Text = "退出全屏";
                FontIcon_FullScreenMode.Glyph = "\xE73F";
            }
        }


        private void ListBox_Comment_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(TextBlock))
            {
                OtherHelper.CopyTextToClipboard((e.OriginalSource as TextBlock).Text);
            }
            else if(e.OriginalSource.GetType() == typeof(Windows.UI.Xaml.Shapes.Rectangle))
            {
                OtherHelper.CopyTextToClipboard(((e.OriginalSource as Windows.UI.Xaml.Shapes.Rectangle).DataContext as CommentsItem).content);
            }
        }
    }
}
