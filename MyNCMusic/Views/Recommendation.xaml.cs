using MyNCMusic.Helper;
using MyNCMusic.Models;
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
        public static Recommendation Instance { get;private set; }
        private ViewModel.RecommendationViewModel RecommendationViewModel;
        public Recommendation()
        {
            Instance = this;
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            RecommendationViewModel = new ViewModel.RecommendationViewModel();
            DataContext = RecommendationViewModel;
        }

        /// <summary>
        /// 播放全部我喜欢的歌曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PlayFavoriteSongs_Click(object sender, RoutedEventArgs e)
        {
            RecommendationViewModel.PlayRandomFavoriteMusic();
        }

        /// <summary>
        /// 播放全部日推歌曲
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_PlayDailySongs_Click(object sender, RoutedEventArgs e)
        {
            RecommendationViewModel.PlayRecommendMusic();
        }

        private void AutoSuggestBox_Search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            RecommendationViewModel.Search();
        }

        private void RecommendMusic_OnChangedSong(MusicItem musicItem)
        {
            RecommendationViewModel.PlayRecommendMusic(musicItem);
        }
        /// <summary>
        /// 日推歌曲、随机喜欢、搜索歌曲共用点击歌手事件
        /// </summary>
        /// <param name="artist"></param>
        private void MusicList_OnChangedArtist(ArtistBaseDetailRoot artist)
        {
            NavigateService.NavigateToArtistAsync(artist);
        }
        /// <summary>
        /// 日推歌曲、随机喜欢、搜索歌曲共用点击专辑事件
        /// </summary>
        /// <param name="album"></param>
        private void MusicList_OnChangedAlbum(AlbumRoot album)
        {
            NavigateService.NavigateToAlbumAsync(album);
        }

        private void SearchPlaylistList_OnChangedPlaylist(PlaylistItem playlistItem)
        {
            NavigateService.NavigateToPlaylistAsync(playlistItem.Id);
        }

        private void RecommendPlaylist_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RecommendList;
            if (item != null)
            {
                NavigateService.NavigateToPlaylistAsync(item.Id);
            }
        }

        private void FavoriteSongs_OnChangedSong(MusicItem musicItem)
        {
            RecommendationViewModel.PlayRandomFavoriteMusic(musicItem);
        }

        private void Search_OnChangedSong(MusicItem musicItem)
        {
            RecommendationViewModel.PlaySearchMusic(musicItem);
        }
    }
}
