using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Models
{
    /// <summary>
    /// 歌单具体信息
    /// </summary>
    public class PlaylistItem
    {
        public List<SubscribersItem> Subscribers { get; set; }
        public bool Subscribed { get; set; }
        public Creator Creator { get; set; }
        
        public List<TracksItem> Tracks { get; set; }
        
        public List<TrackIdsItem> TrackIds { get; set; }
        
        public string UpdateFrequency { get; set; }
        
        public long BackgroundCoverId { get; set; }
        
        public string BackgroundCoverUrl { get; set; }
        
        public long TitleImage { get; set; }
        
        public string TitleImageUrl { get; set; }
        
        public string EnglishTitle { get; set; }
        
        public string OpRecommend { get; set; }
        
        public int AdType { get; set; }
        
        public long TrackNumberUpdateTime { get; set; }
        
        public int SubscribedCount { get; set; }
        
        public int CloudTrackCount { get; set; }
        
        public long UserId { get; set; }
        
        public int Privacy { get; set; }
        
        public long TrackUpdateTime { get; set; }
        
        public int TrackCount { get; set; }
        
        public long CreateTime { get; set; }
        
        public string HighQuality { get; set; }
        
        public long UpdateTime { get; set; }
        
        public long CoverImgId { get; set; }
        
        public string NewImported { get; set; }
        
        public int SpecialType { get; set; }
        
        public string CommentThreadId { get; set; }
        
        public string CoverImgUrl { get; set; }
        
        public long PlayCount { get; set; }
        
        public List<string> Tags { get; set; }
        
        public string Description { get; set; }
        
        public string Ordered { get; set; }
        
        public int Status { get; set; }
        
        public string Name { get; set; }
        
        public long Id { get; set; }
        
        public long ShareCount { get; set; }
       
        public string CoverImgId_str { get; set; }
        
        public long CommentCount { get; set; }

        public string Artists { get; set; }
        
        public string Anonimous { get; set; }
        
        public int TotalDuration { get; set; }
    }
}
