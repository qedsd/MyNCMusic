using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Model
{
    /// <summary>
    /// 获取订阅、创建的所有节目入口类
    /// </summary>
    public class DjRadio:JsonBaseObject
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DjRadiosItem> DjRadios { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HasMore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SubCount { get; set; }
        public long Time { get; set; }
    }
    /// <summary>
    /// DJ
    /// </summary>
    public class Dj
    {
        /// <summary>
        /// 
        /// </summary>
        public string DefaultAvatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AuthStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Followed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AccountStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// 异元骇客
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DetailDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long AvatarImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long BackgroundImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BackgroundUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Authority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Mutual { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ExpertTags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Experts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DjStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int VipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RemarkName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AuthenticationTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AvatarDetail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Anchor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BackgroundImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AvatarImgIdStr { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DjRadiosItem
    {
        /// <summary>
        /// 
        /// </summary>
        public Dj Dj { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SecondCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Buyed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int OriginalPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DiscountPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PurchaseCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LastProgramName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Videos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Finished { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UnderShelf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LiveInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PlayCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Privacy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProgramCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long PicId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SubCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long LastProgramCreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int RadioFeeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long LastProgramId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FeeScope { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Rcmdtext { get; set; }
    }

    /// <summary>
    /// 获取节目信息入口类
    /// </summary>
    public class RadioPrograms: JsonBaseObject
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<RadioSongItem> Programs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string More { get; set; }
    }

    /// <summary>
    /// 节目详细
    /// </summary>
    public class RadioSongItem
    {
        //public int LikedCount { get; set; }
        //public int CommentCount { get; set; }
        //public int ListenerCount { get; set; }
        //public long CreateTime { get; set; }
        //public string CoverUrl { get; set; }
        /// <summary>
        /// 最终具体每个节目
        /// </summary>
        public MainSong MainSong { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Songs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dj Dj { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BlurCoverUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DjRadiosItem Radio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Buyed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProgramDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string H5Links { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CanReward { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AuditStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VideoInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LiveInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Alg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DsPlayStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AuditDisPlayStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SecondCategoryName { get; set; }
        /// <summary>
        /// 什么是Visual Studio Code(VSC), 它和Visual Studio的关系
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CoverUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Titbits { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SerialNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Privacy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int FeeScope { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BdAuditStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ListenerCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SubscribedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Channels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SmallLanguageAuditStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProgramFeeType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MainTrackId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CommentThreadId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PubStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Reward { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TrackCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TitbitImages { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsPublish { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SecondCategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ScheduledPublishTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long CreateTime { get; set; }
        /// <summary>
        /// Visual Studio Code - 吕鹏
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ShareCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Subscribed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LikedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CommentCount { get; set; }
    }

    /// <summary>
    /// 节目每一期详细
    /// 需以ID通过song获取播放地址
    /// </summary>
    public class MainSong:PlayingSongBaseObject
    {
        /// <summary>
        /// 
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long CopyrightId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Disc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Artist> Artists { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Album Album { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Starred { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Popularity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int StarredNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PlayedNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int DayPlays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int HearTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ringtone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Crbt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Audition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CopyFrom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CommentThreadId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RtUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Ftype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> RtUrls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Copyright { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TransName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Mark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NoCopyrightRcmd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Rtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Rurl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Mvid { get; set; }
        /// <summary>
        /// 无效
        /// </summary>
        public string Mp3Url { get; set; }
    }

}
