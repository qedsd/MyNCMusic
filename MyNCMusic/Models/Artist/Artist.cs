using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    /// <summary>
    /// 歌手
    /// </summary>
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PicUrl { get; set; }
        public List<string> @Alias { get; set; }
        public int AlbumSize { get; set; }
        public long PicId { get; set; }
        public string Img1v1Url { get; set; }
        public long Img1v1 { get; set; }
        public string Trans { get; set; }

        public long Img1v1Id { get; set; }
        public int TopicPerson { get; set; }
        public string Followed { get; set; }
        public int MusicSize { get; set; }
        public string BriefDesc { get; set; }
        public string PicId_str { get; set; }
        public string Img1v1Id_str { get; set; }

        public string Info { get; set; }
        public int MvSize { get; set; }
    }
}
