using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    /// <summary>
    /// 推荐歌单结果类
    /// </summary>
    public class RecommendListRoot
    {
        public int Code { get; set; }
        public bool FeatureFirst { get; set; }
        public bool HaveRcmdSongs { get; set; }
        public List<RecommendList> Recommend { get; set; }
    }
}
