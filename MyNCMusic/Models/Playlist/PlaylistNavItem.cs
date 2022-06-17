using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    /// <summary>
    /// 传给歌单界面的实体类
    /// </summary>
    public class PlaylistNavItem
    {
        /// <summary>
        /// 歌单信息
        /// </summary>
        public PlaylistItem PlaylistInfo { get; set; }
        /// <summary>
        /// 具体每一首歌
        /// </summary>
        public List<MusicItem> Songs { get; set; } = new List<MusicItem>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"><歌单id/param>
        /// <returns></returns>
        public static async Task<PlaylistNavItem> CreateAsync(long id)
        {
            PlayListDetailRoot playListDetailRoot = await PlaylistService.GetPlaylistDetailAsync(id);
            if (playListDetailRoot == null || playListDetailRoot.Playlist.TrackIds.Count == 0)
                return null;
            StringBuilder stringBuilder = new StringBuilder();
            PlaylistNavItem playlistNavItem = new PlaylistNavItem();
            playlistNavItem.PlaylistInfo = playListDetailRoot.Playlist;
            for (int i = 0; i < playListDetailRoot.Playlist.TrackIds.Count; i += 1000)//最高单次1000个
            {
                int j = i;
                if ((i + 1000) > playListDetailRoot.Playlist.TrackIds.Count)//剩下的不足1000
                {
                    for (; j < playListDetailRoot.Playlist.TrackIds.Count; j++)
                    {
                        if (j % 1000 != 0)
                            stringBuilder.Append(",");
                        stringBuilder.Append(playListDetailRoot.Playlist.TrackIds[j].Id);
                    }
                }
                else//剩下的超过1000
                {
                    for (; j < i + 1000; j++)
                    {
                        if (j % 1000 != 0)
                            stringBuilder.Append(",");
                        stringBuilder.Append(playListDetailRoot.Playlist.TrackIds[j].Id);
                    }
                }
                MusicDetailRoot musicDetailRootTemp = await SongService.GetMusicDetail_PostAsync(stringBuilder.ToString());
                if (musicDetailRootTemp != null && musicDetailRootTemp.Songs != null)
                {
                    playlistNavItem.Songs.AddRange(musicDetailRootTemp.Songs);
                }
            }
            return playlistNavItem;
        }
    }
}
