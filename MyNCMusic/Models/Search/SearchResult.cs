using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class SearchResult
    {
        //歌曲
        public List<MusicItem> Songs { get; set; }
        public string HasMore { get; set; }
        public int SongCount { get; set; }

        //歌单
        public List<PlaylistItem> Playlists { get; set; }
        public int PlaylistCount { get; set; }

        //歌手
        public int ArtistCount { get; set; }
        public List<Artist> Artists { get; set; }

        //专辑
        public List<Album> Albums { get; set; }
        public int AlbumCount { get; set; }
    }
}
