using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class MusicItem:MusicBase
    {
        public List<Artist> Artists { get; set; }
        public Album Album { get; set; }
        public int Duration { get; set; }
        public int CopyrightId { get; set; }
        public int Status { get; set; }
        public List<string> Alias { get; set; }
        public int Rtype { get; set; }
        public int Ftype { get; set; }
        public int Mvid { get; set; }
        public int Fee { get; set; }
        public string RUrl { get; set; }
        public long Mark { get; set; }

        //歌曲详细增加
        public int Pst { get; set; }
        public int T { get; set; }
        public List<Artist> Ar { get; set; }
        public List<string> Alia { get; set; }
        public int Pop { get; set; }
        public int St { get; set; }
        public string Rt { get; set; }
        public int V { get; set; }
        public string Crbt { get; set; }
        public string Cf { get; set; }
        public Album Al { get; set; }
        public int Dt { get; set; }
        public string A { get; set; }
        public string Cd { get; set; }
        public int No { get; set; }
        public string RtUrl { get; set; }
        public List<string> RtUrls { get; set; }
        public int DjId { get; set; }
        public int Copyright { get; set; }
        public int S_id { get; set; }
        public int OriginCoverType { get; set; }
        public int Single { get; set; }
        //public string noCopyrightRcmd { get; set; }
        public int Mv { get; set; }
        public string Rurl { get; set; }
        public int Mst { get; set; }
        public int Cp { get; set; }
        public long PublishTime { get; set; }

        public Privilege Privilege { get; set; }

        //是否为喜欢歌曲
        public bool IsFavorite { get; set; } = false;
        public string Eq { get; set; }
    }
}
