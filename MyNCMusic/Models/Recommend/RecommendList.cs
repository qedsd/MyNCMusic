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
        /// <summary>
        /// 仅当非自动创建的歌单时才使用
        /// 可通过Creator.UserType判断？
        /// 如私人雷达这类歌单，此处的图片将是一个固定的图，需要获取歌单详细来取封面图
        /// PlaylistItem.CoverImgUrl
        /// </summary>
        public string PicUrl { get; set; }
        public long Playcount { get; set; }
        public long CreateTime { get; set; }
        public Creator Creator { get; set; }
        public int TrackCount { get; set; }
        public long UserId { get; set; }
        public string Alg { get; set; }
    }
}
