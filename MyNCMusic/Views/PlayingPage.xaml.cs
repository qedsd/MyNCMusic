using MyNCMusic.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        //int currentPivotState = 0;//0歌词 1评论 2相似
        //bool isLoadedComment = false;
        //public static SolidColorBrush mainSolidColorBrush;
        List<LyricStr> lyricStrs;
        static Object locker;
        public PlayingPage()
        {
            lyricStrs = new List<LyricStr>();
            //mainSolidColorBrush = MainPage.mainSolidColorBrush;
            locker = new object();
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
            if(_songsItem==null)//仅第一次加载改页面才主动调用LoadLayout更新界面，其他时候都是由mainpage控制
                LoadLayout();
        }
        public async void LoadLayout()
        {
            ProgressBar_loadLyric.Visibility = Visibility.Visible;
            _albumRoot = (Application.Current as App).myMainPage._album;
            _songsItem = MainPage.songsItem;
            Image_album.Source = (Application.Current as App).myMainPage._AlbumBitmapImage;
            Button_albumName.Content = _albumRoot.album.name;
            string name = "";
            //for(int i=0;i< _albumRoot.album.artists.Count;i++)
            //{
            //    if (i != 0)
            //        name += "/";
            //    name += _albumRoot.album.artists[i].name;
            //}
            for (int i = 0; i < _songsItem.ar.Count; i++)
            {
                if (i != 0)
                    name += "/";
                name += _songsItem.ar[i].name;
            }
            ListBox_artists.ItemsSource = _songsItem.ar;
            Button_artistName.Content = name;
            TextBlock_songName.Text = _songsItem.name;
            LyricRoot lyricRoot = await Task.Run(() => MyClassManager.GetLyric(_songsItem.id));
            if (lyricRoot == null)
            {
                ListBox_lyric.ItemsSource = null;
                return;
            }
            lyricStrs = MyClassManager.GetLyricStrs(lyricRoot);
            if(lyricStrs != null)
            {
                if(lyricStrs.Count==0)
                {
                    DateTime dateTime = DateTime.Now;
                    lyricStrs.Add(new LyricStr() { DateTime = dateTime, Original = "纯音乐，无歌词" });
                }
                else
                    ListBox_lyric.ScrollIntoView(lyricStrs.First());
                ListBox_lyric.ItemsSource = lyricStrs;
            }
            //if (currentPivotState == 1)
            //{
            //    UpDateComment();
            //    ListBox_simiSongs.ItemsSource = null;
            //}
            //else if (currentPivotState == 2)
            //{
            //    UpDateSimiSongs();
            //    ListBox_HotComment.ItemsSource = null;
            //    ListBox_allComment.ItemsSource = null;
            //    TextBlock_commentCount.Text = "";
            //}
            UpDateComment();
            UpDateSimiSongs();
            ProgressBar_loadLyric.Visibility = Visibility.Collapsed;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            //if(pivotItem.Tag.ToString() == "0")
            //    currentPivotState = 0;
            //else if (pivotItem.Tag.ToString() == "1")
            //{
            //    currentPivotState = 1;
            //    if (ListBox_HotComment.ItemsSource != null)
            //        return;
            //    UpDateComment();
            //}
            //else if(pivotItem.Tag.ToString() == "2")
            //{
            //    currentPivotState = 2;
            //    if(ListBox_simiSongs.ItemsSource==null)
            //        UpDateSimiSongs();
            //}
        }

        async void UpDateComment()
        {
            ProgressBar_loadComment.Visibility = Visibility.Visible;
            CommentRoot commentRoot = await Task.Run(() => MyClassManager.GetSongsComment(_songsItem.id));
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
            //ListBox_HotComment.ScrollIntoView(commentRoot.hotComments.First());
            ScrollViewer_comment.ChangeView(null, 0, null);
            ProgressBar_loadComment.Visibility = Visibility.Collapsed;
        }

        async void UpDateSimiSongs()
        {
            ProgressBar_loadSimiSongs.Visibility = Visibility.Visible;
            SimiSongsRoot simiSongsRoot =await Task.Run(()=> MyClassManager.GetSimiSongs(_songsItem.id));
            if (simiSongsRoot == null)
            {
                ProgressBar_loadSimiSongs.Visibility = Visibility.Collapsed;
                ListBox_simiSongs.ItemsSource = null;
                return;
            }
            //判断是否为喜欢歌曲
            if (MainPage.favoriteSongsRoot != null)
            {
                foreach (var temp in simiSongsRoot.songs)
                {
                    if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.id)) != 0)
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
            MusicDetailRoot musicDetailRoot=await Task.Run(()=>MyClassManager.GetMusicDetail(songsItem.id.ToString()));
            if (musicDetailRoot == null || musicDetailRoot.songs == null)
            {
                ProgressBar_loadSimiSongs.Visibility = Visibility.Collapsed;
                return;
            }
            
            SongUrlRoot songUrlRoot = await Task.Run(() => MyClassManager.GetMusicUrl(songsItem.id));
            if (songUrlRoot == null)
            {
                ProgressBar_loadSimiSongs.Visibility = Visibility.Collapsed;
                return;
            }
            //修改播放列表
            if (MainPage.PlayingListId != _songsItem.id+3)//已在播放此歌单，仅修改播放歌曲，否则，重置播放列表及历史记录
            {
                (Application.Current as App).myMainPage.currentPlayList.Clear();
                foreach (var temp in ListBox_simiSongs.ItemsSource as List<SongsItem>)
                    (Application.Current as App).myMainPage.currentPlayList.Add(temp);
                (Application.Current as App).myMainPage.playHistoryIndex.Clear();
                MainPage.PlayingListId = _songsItem.id + 3;
            }
            //修改mainpage以触发修改正在播放的音乐
            (Application.Current as App).myMainPage.ChnagePlayingSong(musicDetailRoot.songs.First(), songUrlRoot);
            ProgressBar_loadSimiSongs.Visibility = Visibility.Collapsed;
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
            Frame.Navigate(typeof(AlbumDetail), _albumRoot);
        }

        private async void Button_artistName_Click(object sender, RoutedEventArgs e)
        {
            if(_songsItem.ar.Count==1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => MyClassManager.GetArtistBaseDetail(_songsItem.ar.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        private async void ListBox_artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArItem arItem = ((ListBox)sender).SelectedItem as ArItem;
            if (arItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => MyClassManager.GetArtistBaseDetail(arItem.id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }
    }
}
