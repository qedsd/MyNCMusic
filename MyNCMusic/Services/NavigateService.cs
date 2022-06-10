using MyNCMusic.Models;
using MyNCMusic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Services
{
    public static class NavigateService
    {
        public static void GoBack()
        {
            Views.Home.Instance.Frame.GoBack();
        }
        /// <summary>
        /// 调转到歌单详情界面
        /// </summary>
        /// <param name="id"></param>
        public static async void NavigateToPlaylistAsync(long id)
        {
            Controls.WaitingPopup.Show();
            PlaylistNavItem playlistNavItem = await PlaylistNavItem.CreateAsync(id);
            Controls.WaitingPopup.Hide();
            if(playlistNavItem != null)
            {
                Home.Instance.NavigateTo(typeof(Views.PlayListDetai), playlistNavItem);
            }
        }

        public static void NavigateToArtistAsync(ArtistBaseDetailRoot artistBaseDetailRoot)
        {
            Home.Instance.NavigateTo(typeof(ArtistHome), artistBaseDetailRoot);
        }

        public static void NavigateToAlbumAsync(AlbumRoot albumRoot)
        {
            Home.Instance.NavigateTo(typeof(AlbumDetail), albumRoot);
        }

        public static void NavigateToComment(CommentRoot commentRoot)
        {
            Home.Instance.NavigateTo(typeof(Comment), commentRoot);
        }
    }
}
