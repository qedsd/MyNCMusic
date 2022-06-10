using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    public class RecommendList
    {
        public long Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Copywriter { get; set; }
        public string PicUrl { get; set; }
        public long Playcount { get; set; }
        public long CreateTime { get; set; }
        public Creator Creator { get; set; }
        public int TrackCount { get; set; }
        public long UserId { get; set; }
        public string Alg { get; set; }
    }
}
