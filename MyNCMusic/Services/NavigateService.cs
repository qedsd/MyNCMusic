using MyNCMusic.Models;
using MyNCMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace MyNCMusic.Services
{
    public static class NavigateService
    {
        /// <summary>
        /// 歌单详情
        /// </summary>
        /// <param name="id"></param>
        public static async void NavigateToPlaylistAsync(long id)
        {
            Controls.WaitingPopup.Show();
            PlaylistNavItem playlistNavItem = await PlaylistNavItem.CreateAsync(id);
            Controls.WaitingPopup.Hide();
            if(playlistNavItem != null)
            {
                if(IsInPlayingPage)
                {
                    NavigationPage.Instance.NavigateTo(typeof(Views.PlayListDetai), playlistNavItem);
                }
                else
                {
                    Home.Instance.NavigateTo(typeof(Views.PlayListDetai), playlistNavItem);
                }
            }
        }

        public static void NavigateToArtistAsync(ArtistBaseDetailRoot artistBaseDetailRoot)
        {
            if(IsInPlayingPage)
            {
                NavigationPage.Instance.NavigateTo(typeof(ArtistHome), artistBaseDetailRoot);
            }
            else
            {
                Home.Instance.NavigateTo(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        public static void NavigateToAlbumAsync(AlbumRoot albumRoot)
        {
            if (IsInPlayingPage)
            {
                NavigationPage.Instance.NavigateTo(typeof(AlbumDetail), albumRoot);
            }
            else
            {
                Home.Instance.NavigateTo(typeof(AlbumDetail), albumRoot);
            }
        }

        public static void NavigateToComment(CommentRoot commentRoot)
        {
            if (IsInPlayingPage)
            {
                NavigationPage.Instance.NavigateTo(typeof(CommentPage), commentRoot);
            }
            else
            {
                Home.Instance.NavigateTo(typeof(CommentPage), commentRoot);
            }
        }

        public static async void NavigateToCompactOverlayMode()
        {
            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(340, 160);
            if (await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions))
            {
                NavigationPage.Instance.Frame.Navigate(typeof(CompactOverlayPage));
            }
        }
        private static bool IsInPlayingPage = false;
        public static void TryNavigateToPlayingPage()
        {
            IsInPlayingPage = Home.Instance.NavigateToPlayingPage();
        }

        public static void NavigateToRadioDetail(List<RadioSongItem> radioDetail)
        {
            if (IsInPlayingPage)
            {
                NavigationPage.Instance.NavigateTo(typeof(RadioDetailPage), radioDetail);
            }
            else
            {
                Home.Instance.NavigateTo(typeof(RadioDetailPage), radioDetail);
            }
        }
    }
}
