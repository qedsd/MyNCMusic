using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    /// <summary>
    /// 专辑
    /// </summary>
    public class Album
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Artist Artist { get; set; }
        public long PublishTime { get; set; }
        public int Size { get; set; }
        public int CopyrightId { get; set; }
        public int Status { get; set; }
        public long PicId { get; set; }
        public int Mark { get; set; }

        public List<string> Songs { get; set; }
        public string Paid { get; set; }
        public string OnSale { get; set; }
        public string Tags { get; set; }
        public string CommentThreadId { get; set; }
        public string Description { get; set; }
        public List<string> Alias { get; set; }
        public List<Artist> Artists { get; set; }
        public string BriefDesc { get; set; }
        public string Company { get; set; }
        public string PicUrl { get; set; }
        public long Pic { get; set; }
        public string SubType { get; set; }
        public int CompanyId { get; set; }
        public string BlurPicUrl { get; set; }
        public string Type { get; set; }
        public string PicId_str { get; set; }
        public AlbumInfo Info { get; set; }
    }
}
