using MyNCMusic.Helper;
using MyNCMusic.Model;
using MyNCMusic.Services;
using Newtonsoft.Json;
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
    public sealed partial class Recommendation : Page
    {
        RecommendRoot recommendRoot;
        int searchType = 1;//搜索类型
        public Recommendation()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            WaitToLoad();
        }

        async void WaitToLoad()
        {
            recommendRoot=await PlaylistService.GetcommendatoryList();
            if(recommendRoot!=null)
                AdaptiveGridView_recommendList.ItemsSource = recommendRoot.recommend;
        }

        //点击推荐歌单
        private async void AdaptiveGridViewControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressBar_loadRecomendList.Visibility = Visibility.Visible;
            Recommend recommend = e.ClickedItem as Recommend;
            if(recommend!=null)
            {
                PlayListDetailRoot playListDetailRoot=await Task.Run(()=>PlaylistService.GetPlaylistDetail(recommend.id));
                if (playListDetailRoot == null || playListDetailRoot.playlist.trackIds.Count == 0)
                {
                    ProgressBar_loadRecomendList.Visibility = Visibility.Collapsed;
                    return;
                }
                string ids = "";
                for(int i=0;i< playListDetailRoot.playlist.trackIds.Count;i++)
                {
                    if (i != 0)
                        ids += ",";
                    ids += playListDetailRoot.playlist.trackIds[i].id;
                }
                MusicDetailRoot musicDetailRoot= await Task.Run(() => SongService.GetMusicDetail_Post(ids));
                if (musicDetailRoot == null)
                {
                    ProgressBar_loadRecomendList.Visibility = Visibility.Collapsed;
                    return;
                }
                //Frame frame = new Frame();
                //PlayListDetai playListDetai = new PlayListDetai(playListDetailRoot, musicDetailRoot);
                //frame.Content = playListDetai;
                ProgressBar_loadRecomendList.Visibility = Visibility.Collapsed;
                List<Object> list = new List<object>();
                list.Add(playListDetailRoot);
                list.Add(musicDetailRoot);
                Frame.Navigate(typeof(PlayListDetai), list);
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            if(pivotItem.Tag.ToString()=="1")//日推
            {
                if (ListBox_recommendMusic.ItemsSource != null)
                    return;
                //string result = await MyClassManager.HttpClientGet(MyClassManager.apiUri + @"/recommend/songs");
                ProgressBar_loadRecomendSongs.Visibility = Visibility.Visible;
                string result = await Task.Run(()=>Http.Get(ConfigService.ApiUri + @"/recommend/songs"));
                if (result == null || result.Equals(""))
                {
                    ProgressBar_loadRecomendSongs.Visibility = Visibility.Collapsed;
                    return;
                }
                RecommendMusicsRoot recommendMusics = JsonConvert.DeserializeObject<RecommendMusicsRoot>(result);
                if (recommendMusics == null)
                {
                    ProgressBar_loadRecomendSongs.Visibility = Visibility.Collapsed;
                    return;
                }
                //判断是否为喜欢歌曲
                if (MainPage.favoriteSongsRoot != null)
                {
                    foreach (var temp in recommendMusics.data.dailySongs)
                    {
                        if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.Id)) != 0)
                            temp.isFavorite = true;
                    }
                }
                ProgressBar_loadRecomendSongs.Visibility = Visibility.Collapsed;
                ListBox_recommendMusic.ItemsSource = recommendMusics.data.dailySongs;
            }
            else if(pivotItem.Tag.ToString() == "2")//随机喜欢的50首
            {
                if(MainPage.favoriteSongsRoot!=null&&MainPage.favoriteSongsRoot.ids.Count!=0&& MainPage.favoriteSongsRoot.songs==null)
                {
                    MainPage.favoriteSongsRoot.songs = new List<SongsItem>();
                    string ids = "";
                    //for (int i = 0; i < MainPage.favoriteSongsRoot.ids.Count; i++)
                    //{
                    //    if (i != 0)
                    //        ids += ",";
                    //    ids += MainPage.favoriteSongsRoot.ids[i];
                    //}
                    List<long> idsTemp = new List<long>( MainPage.favoriteSongsRoot.ids);
                    //随机取50条数据
                    int totalCount = 50; //需要取得数据的数量
                    Random random = new Random();
                    while (idsTemp.Count > totalCount) //判断数据总数是否大于需要取得数据的数量
                    {
                        int k = random.Next(0, idsTemp.Count); //取得大于等于0且小于数据总数的一个随机数
                        idsTemp.RemoveAt(k); //根据随机数移除一条数据
                    }
                    for (int i = 0; i < idsTemp.Count; i++)
                    {
                        if (i != 0)
                            ids += ",";
                        ids += idsTemp[i];
                    }
                    ProgressBar_loadRandomFavoriteSongs.Visibility = Visibility.Visible;
                    MusicDetailRoot musicDetailRoot = await Task.Run(()=>SongService.GetMusicDetail_Post(ids));
                    if (musicDetailRoot == null)
                    {
                        ProgressBar_loadRandomFavoriteSongs.Visibility = Visibility.Collapsed;
                        return;
                    }
                    foreach (var temp in musicDetailRoot.songs)
                        temp.isFavorite = true;
                    ProgressBar_loadRandomFavoriteSongs.Visibility = Visibility.Collapsed;
                    ListBox_myFavoriteSongs.ItemsSource = musicDetailRoot.songs;
                    MainPage.favoriteSongsRoot.songs = musicDetailRoot.songs.ToList();
                }
            }
        }

        /// <summary>
        /// 双击日推歌曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListBox_recommendMusic_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            if (songsItem == null)
                return;
            
            PlayingService.PlayingListId = songsItem.al.id;
            await PlayingService.ChangePlayingSong(songsItem.Id, ListBox_recommendMusic.ItemsSource as List<SongsItem>, songsItem);
        }
        /// <summary>
        /// 双击我喜欢的歌曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListBox_myFavoriteSongs_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            if (songsItem == null)
                return;
            //SongUrlRoot songUrlRoot = SongService.GetMusicUrl(songsItem.Id);
            //if (songUrlRoot == null)
            //    return;
            ////添加播放列表
            //if (ListBox_myFavoriteSongs.ItemsSource == null)
            //    return;
            //if (MainPage.PlayingListId != 2)//已在播放此歌单，仅修改播放歌曲，否则，重置播放列表及历史记录
            //{
            //    (Application.Current as App).myMainPage.currentPlayList.Clear();
            //    foreach (var temp in ListBox_myFavoriteSongs.ItemsSource as List<SongsItem>)
            //        (Application.Current as App).myMainPage.currentPlayList.Add(temp);
            //    (Application.Current as App).myMainPage.playHistoryIndex.Clear();
            //    //MainPage.PlayingListId = 2;
            //}
            ////修改mainpage以触发修改正在播放的音乐
            //(Application.Current as App).myMainPage.ChnagePlayingSong(songsItem, songUrlRoot,-2);
            PlayingService.PlayingListId = songsItem.al.id;
            await PlayingService.ChangePlayingSong(songsItem.Id, ListBox_myFavoriteSongs.ItemsSource as List<SongsItem>, songsItem);
        }


        private void AutoSuggestBox_favorite_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text == ""|| MainPage.favoriteSongsRoot==null)
                ListBox_myFavoriteSongs.ItemsSource = MainPage.favoriteSongsRoot.songs;
            else
            {
                var list = MainPage.favoriteSongsRoot.songs.FindAll(p => p.Name.Contains(sender.Text));
                if(list!=null&&list.Count!=0)
                {
                    ListBox_myFavoriteSongs.ItemsSource = list;
                }
            }
        }

        /// <summary>
        /// 播放全部我喜欢的歌曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_playFavoriteSongs_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_myFavoriteSongs.ItemsSource == null)
                return;
            //(Application.Current as App).myMainPage.currentPlayList.Clear();
            //foreach (var temp in ListBox_myFavoriteSongs.ItemsSource as List<SongsItem>)
            //    (Application.Current as App).myMainPage.currentPlayList.Add(temp);
            //(Application.Current as App).myMainPage.playHistoryIndex.Clear();
            //MainPage.PlayingListId = 2;
            //(Application.Current as App).myMainPage.PlayNextSongs(-1);
            
            var songs = ListBox_myFavoriteSongs.ItemsSource as List<SongsItem>;
            PlayingService.PlayingListId = songs.First().al.id;
            await PlayingService.ChangePlayingSong(songs.First().Id, songs, songs.First()) ;
        }

        /// <summary>
        /// 播放全部日推歌曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_playDailySongs_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox_recommendMusic.ItemsSource == null)
                return;
            var songs = ListBox_recommendMusic.ItemsSource as List<SongsItem>;
            PlayingService.PlayingListId = songs.First().al.id;
            await PlayingService.ChangePlayingSong(songs.First().Id, songs, songs.First());
        }

        private void AutoSuggestBox_search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text == "")
                return;
            
            string keyword = sender.Text;//避免线程冲突
            IntoSearch(keyword);
        }

        async void IntoSearch(string keyword)
        {
            searchType = int.Parse(((PivotItem)Pivot_search.SelectedItem).Tag.ToString());
            SearchRoot searchRoot = await Task.Run(() => SearchService.SearchClound(keyword, searchType));
            if (searchRoot == null)
                return;
            switch (searchType)
            {
                case 1:
                    {
                        //判断是否为喜欢歌曲
                        if (MainPage.favoriteSongsRoot != null)
                        {
                            foreach (var temp in searchRoot.result.songs)
                            {
                                if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(temp.Id)) != 0)
                                    temp.isFavorite = true;
                            }
                        }
                        ListBox_searchSong.ItemsSource = searchRoot.result.songs;
                    }
                    break;
                case 10:
                    {
                        ListBox_searchAlbum.ItemsSource = searchRoot.result.albums;
                    }
                    break;
                case 100:
                    {
                        ListBox_searchArtist.ItemsSource = searchRoot.result.artists;
                    }
                    break;
                case 1000:
                    {
                        AdaptiveGridView_searchPlaylist.ItemsSource = searchRoot.result.playlists;
                    }
                    break;
            }
        }

        private async void ListBox_searchSong_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            if (songsItem == null)
                return;
            
            PlayingService.PlayingListId = songsItem.al.id;
            await PlayingService.ChangePlayingSong(songsItem.Id, ListBox_searchSong.ItemsSource as List<SongsItem>, songsItem);
        }

        private async void ListBox_searchArtist_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(artist.id));
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }

        private async void ListBox_searchAlbum_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Album album = ((ListBox)sender).SelectedItem as Album;
            if (album == null)
                return;
            if (album.id == PlayingService.PlayingListId)
                Frame.Navigate(typeof(AlbumDetail));
            else
            {
                ProgressBar_loadAlbum.Visibility = Visibility.Visible;
                AlbumRoot albumRoot = await Task.Run(() => AlbumService.GetAlbum(album.id));
                if (albumRoot == null)
                    return;
                Frame.Navigate(typeof(AlbumDetail), albumRoot);
                ProgressBar_loadAlbum.Visibility = Visibility.Collapsed;
            }
        }

        private async void AdaptiveGridView_searchPlaylist_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressBar_loadPlaylist.Visibility = Visibility.Visible;
            PlaylistItem playlistItem = e.ClickedItem as PlaylistItem;
            if (playlistItem == null)
                return;
            List<Object> list = await Task.Run(() => prepareToNavigeteToPlaylitDetail(playlistItem.id));
            if (list != null)
                Frame.Navigate(typeof(PlayListDetai), list);
            ProgressBar_loadPlaylist.Visibility = Visibility.Collapsed;
        }

        List<Object> prepareToNavigeteToPlaylitDetail(long id)
        {
            PlayListDetailRoot playListDetailRoot = PlaylistService.GetPlaylistDetail(id);
            if (playListDetailRoot == null || playListDetailRoot.playlist.trackIds.Count == 0)
                return null;
            string ids = "";
            for (int i = 0; i < playListDetailRoot.playlist.trackIds.Count; i++)
            {
                if (i != 0)
                    ids += ",";
                ids += playListDetailRoot.playlist.trackIds[i].id;
            }
            MusicDetailRoot musicDetailRoot = SongService.GetMusicDetail_Post(ids);
            if (musicDetailRoot == null)
                return null;
            List<Object> list = new List<object>();
            list.Add(playListDetailRoot);
            list.Add(musicDetailRoot);
            return list;
        }

        private void Pivot_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string keyword = AutoSuggestBox_search.Text;
            IntoSearch(keyword);
        }



        private async void Button_artists_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SongsItem songsItem = button.DataContext as SongsItem;
            if(songsItem.ar.Count==1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(songsItem.ar.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
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


        private async void Button_album_Click(object sender, RoutedEventArgs e)
        {
            SongsItem songsItem = ((Button)sender).DataContext as SongsItem;
            if (songsItem == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            AlbumRoot albumRoot= await Task.Run(() => AlbumService.GetAlbum(songsItem.al.id));
            if (albumRoot == null)
            {
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                return;
            }
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            Frame.Navigate(typeof(AlbumDetail), albumRoot);
        }

        private async void ListBox_artists_SelectionChanged_Artist(object sender, SelectionChangedEventArgs e)
        {
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            if (artist == null)
                return;
            ProgressBar_loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(artist.id));
            ProgressBar_loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }

        private async void Button_artists_Click_Artist(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            List<Artist> artists = ((Album)button.DataContext).artists as List<Artist>;
            if (artists.Count == 1)
            {
                ProgressBar_loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(artists.First().id));
                ProgressBar_loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }
    }
}
