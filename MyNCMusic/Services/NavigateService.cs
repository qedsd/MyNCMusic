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
        /// 调转到歌单详情界面
        /// </summary>
        /// <param name="id"></param>
        public static async void NavigateToPlaylistAsync(long id, bool isMain = false)
        {
            Controls.WaitingPopup.Show();
            PlaylistNavItem playlistNavItem = await PlaylistNavItem.CreateAsync(id);
            Controls.WaitingPopup.Hide();
            if(playlistNavItem != null)
            {
                Home.Instance.NavigateTo(typeof(Views.PlayListDetai), playlistNavItem, isMain);
            }
        }

        public static void NavigateToArtistAsync(ArtistBaseDetailRoot artistBaseDetailRoot, bool isMain = false)
        {
            Home.Instance.NavigateTo(typeof(ArtistHome), artistBaseDetailRoot);
        }

        public static void NavigateToAlbumAsync(AlbumRoot albumRoot, bool isMain = false)
        {
            Home.Instance.NavigateTo(typeof(AlbumDetail), albumRoot, isMain);
        }

        public static void NavigateToComment(CommentRoot commentRoot, bool isMain = false)
        {
            Home.Instance.NavigateTo(typeof(Comment), commentRoot, isMain);
        }

        public static async void NavigateToCompactOverlayMode()
        {
            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(340, 160);
            if (await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions))
                MainPage.Instance.Frame.Navigate(typeof(CompactOverlayPage));
        }
    }
}
