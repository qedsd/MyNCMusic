using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    /// <summary>
    /// 歌单创建者
    /// </summary>
    public class Creator
    {
        public string RemarkName { get; set; }
        public bool Mutual { get; set; }
        public int VipType { get; set; }
        public long UserId { get; set; }
        public string DetailDescription { get; set; }
        public string DefaultAvatar { get; set; }
        //public string expertTags { get; set; }
        public long AvatarImgId { get; set; }
        public long BackgroundImgId { get; set; }
        public int Province { get; set; }
        public string AvatarImgIdStr { get; set; }
        public string BackgroundImgIdStr { get; set; }
        public int DjStatus { get; set; }
        public bool Followed { get; set; }
        public string BackgroundUrl { get; set; }
        public int AccountStatus { get; set; }
        public int Gender { get; set; }
        public string AvatarUrl { get; set; }
        public int AuthStatus { get; set; }
        public int UserType { get; set; }
        public string Nickname { get; set; }
        public long Birthday { get; set; }
        public int City { get; set; }
        public string Description { get; set; }
        public string Signature { get; set; }
        public int Authority { get; set; }
    }
}
