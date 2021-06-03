using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Playback;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Security.Cryptography;
using MyNCMusic.Services;
using System.Collections.ObjectModel;

namespace MyNCMusic.Model
{
    //推荐歌单
    public class Creator
    {
        public string remarkName { get; set; }
        public bool mutual { get; set; }
        public int vipType { get; set; }
        public long userId { get; set; }
        public string detailDescription { get; set; }
        public string defaultAvatar { get; set; }
        //public string expertTags { get; set; }
        public long avatarImgId { get; set; }
        public long backgroundImgId { get; set; }
        public int province { get; set; }
        public string avatarImgIdStr { get; set; }
        public string backgroundImgIdStr { get; set; }
        public int djStatus { get; set; }
        public bool followed { get; set; }
        public string backgroundUrl { get; set; }
        public int accountStatus { get; set; }
        public int gender { get; set; }
        public string avatarUrl { get; set; }
        public int authStatus { get; set; }
        public int userType { get; set; }
        public string nickname { get; set; }
        public long birthday { get; set; }
        public int city { get; set; }
        public string description { get; set; }
        public string signature { get; set; }
        public int authority { get; set; }
    }
    public class Recommend
    {
        public long id { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public string copywriter { get; set; }
        public string picUrl { get; set; }
        public long playcount { get; set; }
        public long createTime { get; set; }
        public Creator creator { get; set; }
        public int trackCount { get; set; }
        public long userId { get; set; }
        public string alg { get; set; }
    }
    public class RecommendRoot
    {
        public int code { get; set; }
        public bool featureFirst { get; set; }
        public bool haveRcmdSongs { get; set; }
        public List<Recommend> recommend { get; set; }
    }
    public class H
    {
        public int br { get; set; }
        public long fid { get; set; }
        public int size { get; set; }
        public string vd { get; set; }
    }
    public class M
    {
        public int br { get; set; }
        public long fid { get; set; }
        public int size { get; set; }
        public string vd { get; set; }
    }
    public class L
    {
        public int br { get; set; }
        public long fid { get; set; }
        public int size { get; set; }
        public string vd { get; set; }
    }
    public class Privilege
    {
        public int id { get; set; }
        public int fee { get; set; }
        public int payed { get; set; }
        public int st { get; set; }
        public int pl { get; set; }
        public int dl { get; set; }
        public int sp { get; set; }
        public int cp { get; set; }
        public int subp { get; set; }
        public string cs { get; set; }
        public int maxbr { get; set; }
        public int fl { get; set; }
        public string toast { get; set; }
        public int flag { get; set; }
        public string preSell { get; set; }

        
        public int playMaxbr { get; set; }
        public int downloadMaxbr { get; set; }
        public List<ChargeInfoListItem> chargeInfoList { get; set; }
    }
    public class RecommendReasonsItem
    {
        public int songId { get; set; }
        public string reason { get; set; }
    }
    public class Data
    {
        public List<SongsItem> dailySongs { get; set; }
        public List<string> orderSongs { get; set; }
        public List<RecommendReasonsItem> recommendReasons { get; set; }
    }
    public class RecommendMusicsRoot
    {
        public int code { get; set; }
        public Data data { get; set; }
    }

    //搜索
    public class Artist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string picUrl { get; set; }
        public List<string> @alias { get; set; }
        public int albumSize { get; set; }
        public long picId { get; set; }
        public string img1v1Url { get; set; }
        public long img1v1 { get; set; }
        public string trans { get; set; }

        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string followed { get; set; }
        public int musicSize { get; set; }
        public string briefDesc { get; set; }
        public string picId_str { get; set; }
        public string img1v1Id_str { get; set; }

        public string info { get; set; }
        public int mvSize { get; set; }
    }
    public class Album
    {
        public long id { get; set; }
        public string name { get; set; }
        public Artist artist { get; set; }
        public long publishTime { get; set; }
        public int size { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public long picId { get; set; }
        public int mark { get; set; }

        public List<string> songs { get; set; }
        public string paid { get; set; }
        public string onSale { get; set; }
        public string tags { get; set; }
        public string commentThreadId { get; set; }
        public string description { get; set; }
        public List<string> alias { get; set; }
        public List<Artist> artists { get; set; }
        public string briefDesc { get; set; }
        public string company { get; set; }
        public string picUrl { get; set; }
        public long pic { get; set; }
        public string subType { get; set; }
        public int companyId { get; set; }
        public string blurPicUrl { get; set; }
        public string type { get; set; }
        public string picId_str { get; set; }
        public Info info { get; set; }
        //public bool liked { get; set; }
        //public int likedCount { get; set; }
    }
    public class SongsItem : PlayingSongBaseObject
    {
        //public new long Id { get; set; }
        //public new string Name { get; set; }
        public List<Artist> artists { get; set; }
        public Album album { get; set; }
        public int duration { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public List<string> alias { get; set; }
        public int rtype { get; set; }
        public int ftype { get; set; }
        public int mvid { get; set; }
        public int fee { get; set; }
        public string rUrl { get; set; }
        public long mark { get; set; }

        //歌曲详细增加
        public int pst { get; set; }
        public int t { get; set; }
        public List<Artist> ar { get; set; }
        public List<string> alia { get; set; }
        public int pop { get; set; }
        public int st { get; set; }
        public string rt { get; set; }
        public int v { get; set; }
        public string crbt { get; set; }
        public string cf { get; set; }
        public Album al { get; set; }
        public int dt { get; set; }
        public H h { get; set; }
        public M m { get; set; }
        public L l { get; set; }
        public string a { get; set; }
        public string cd { get; set; }
        public int no { get; set; }
        public string rtUrl { get; set; }
        public List<string> rtUrls { get; set; }
        public int djId { get; set; }
        public int copyright { get; set; }
        public int s_id { get; set; }
        public int originCoverType { get; set; }
        public int single { get; set; }
        //public string noCopyrightRcmd { get; set; }
        public int mv { get; set; }
        public string rurl { get; set; }
        public int mst { get; set; }
        public int cp { get; set; }
        public long publishTime { get; set; }

        public Privilege privilege { get; set; }

        //是否为喜欢歌曲
        public bool _isFavorite=false;
        public bool isFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;
                NotifyPropertyChanged("isFavorite");
            }
        }
        

        
        public string eq { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;
        //public void NotifyPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}


    }
    public class SearchResult
    {
        //歌曲
        public List<SongsItem> songs { get; set; }
        public string hasMore { get; set; }
        public int songCount { get; set; }

        //歌单
        public List<PlaylistItem> playlists { get; set; }
        public int playlistCount { get; set; }

        //歌手
        public int artistCount { get; set; }
        public List<Artist> artists { get; set; }

        //专辑
        public List<Album> albums { get; set; }
        public int albumCount { get; set; }
    }
    public class SearchRoot
    {
        public SearchResult result { get; set; }
        public int code { get; set; }
    }

    //歌单详细
    public class SubscribersItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string defaultAvatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string followed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int accountStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int city { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string detailDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long avatarImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long backgroundImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mutual { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expertTags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string experts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int djStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarkName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarImgId_str { get; set; }
    }

    public class TracksItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pst { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int t { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Artist> ar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> alia { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pop { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int st { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int v { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string crbt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Album al { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public H h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public M m { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public L l { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string a { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rtUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ftype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> rtUrls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int djId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int copyright { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int s_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long mark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int originCoverType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string noCopyrightRcmd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int rtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rurl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int mst { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int mv { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long publishTime { get; set; }
    }

    public class TrackIdsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int v { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string alg { get; set; }
    }

    public class PlaylistItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SubscribersItem> subscribers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subscribed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Creator creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TracksItem> tracks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TrackIdsItem> trackIds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updateFrequency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long backgroundCoverId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundCoverUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long titleImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string titleImageUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string englishTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string opRecommend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int adType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trackNumberUpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int subscribedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cloudTrackCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int privacy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trackUpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int trackCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string highQuality { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long updateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long coverImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string newImported { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int specialType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string commentThreadId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string coverImgUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long playCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> tags { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ordered { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long shareCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string coverImgId_str { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long commentCount { get; set; }



        public string artists { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string anonimous { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int totalDuration { get; set; }
    }

    public class PrivilegesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int payed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int st { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int subp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int maxbr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string toast { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string preSell { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int playMaxbr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int downloadMaxbr { get; set; }
    }

    public class PlayListDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string relatedVideos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PlaylistItem playlist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string urls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PrivilegesItem> privileges { get; set; }
    }

    //歌曲详细
    public class ChargeInfoListItem
    {
        public int rate { get; set; }
        public string chargeUrl { get; set; }
        public string chargeMessage { get; set; }
        public int chargeType { get; set; }
    }
    public class MusicDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SongsItem> songs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PrivilegesItem> privileges { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }


    //歌曲url
    public class SongDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int br { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string md5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 21.3.2 4.0.8版本api报错，由原本int改为double，不知道这玩意用来干嘛的
        /// </summary>
        public double gain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int payed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string canExtend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string freeTrialInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string encodeType { get; set; }
    }
    public class SongUrlRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SongDataItem> data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //专辑详细
    public class ResourceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string imgUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string encodedId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string webUrl { get; set; }
    }
    public class CommentThread
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResourceInfo resourceInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int commentCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int likedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int shareCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int hotCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string latestLikedUsers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceOwnerId { get; set; }
        /// <summary>
        /// 神的游戏
        /// </summary>
        public string resourceTitle { get; set; }
    }
    public class Info
    {
        /// <summary>
        /// 
        /// </summary>
        public CommentThread commentThread { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string latestLikedUsers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string comments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int commentCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int likedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int shareCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string threadId { get; set; }
    }
    public class AlbumRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SongsItem> songs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Album album { get; set; }
    }

    //喜欢歌曲
    public class FavoriteSongsRoot
    {
        public List<long> ids { get; set; }
        public long checkPoint { get; set; }
        public int code { get; set; }
        public List<SongsItem> songs { get; set; }
    }


    public class MyPlaylistRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string more { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PlaylistItem> playlist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //收藏的专辑

    public class CADataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public long subTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Artist> artists { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long picId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string picUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> transNames { get; set; }
    }

    public class MyCollectionfAlbumRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CADataItem> data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hasMore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cover { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int paidCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //收藏的歌手
    public class MyCollectionfArtistRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Artist> data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hasMore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //歌手详情

    public class ArtistBaseDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public Artist artist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SongsItem> hotSongs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string more { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //歌手专辑
    public class ArtistAllAlbumRoot
    {
        public Artist artist { get; set; }
        public List<Album> hotAlbums { get; set; }
        public string more { get; set; }
        public int code { get; set; }
    }

    //歌词
    public class TransUser
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int demand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userid { get; set; }
        /// <summary>
        /// 只是安渚
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long uptime { get; set; }
    }

    public class Lrc
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class Klyric
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class Tlyric
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class LyricRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string sgc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TransUser transUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Lrc lrc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Klyric klyric { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Tlyric tlyric { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    public class LyricStr
    {
        public string Original { get; set; }
        public string Tran { get; set; }
        public DateTime DateTime { get; set; }
    }

    //评论
    public class Associator
    {
        /// <summary>
        /// 
        /// </summary>
        public int vipCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rights { get; set; }
    }

    public class VipRights
    {
        /// <summary>
        /// 
        /// </summary>
        public Associator associator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string musicPackage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int redVipAnnualCount { get; set; }
    }

    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        public string locationInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liveInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int anonym { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string experts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VipRights vipRights { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userType { get; set; }
        /// <summary>
        /// 蛋蛋是圆D
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarkName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string expertTags { get; set; }
    }

    public class CommentsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public User user { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public List<string> beReplied { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string pendantData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string showFloorComment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long commentId { get; set; }
        public string content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int likedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expressionUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int commentLocationType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long parentCommentId { get; set; }

        public string repliedMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liked { get; set; }
    }

    public class CommentRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string isMusician { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cnum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> topComments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string moreHot { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CommentsItem> hotComments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string commentBanner { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CommentsItem> comments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string more { get; set; }
    }

    //相似歌曲
    public class SimiSongsRoot
    {
        public List<SongsItem> songs { get; set; }
        public int code { get; set; }
    }

    //登录
    public class Account
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int whitelistAuthority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string salt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int tokenVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ban { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int baoyueVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int donateVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long viptypeVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string anonimousUser { get; set; }
    }

    public class Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int accountStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long avatarImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long city { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long backgroundImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string defaultAvatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int djStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mutual { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarkName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expertTags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string followed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string detailDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long authority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long followeds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long follows { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long eventCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int playlistCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int playlistBeSubscribedCount { get; set; }
    }

    public class LoginRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int loginType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Account account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Profile profile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cookie { get; set; }
    }

    public class LoginStatus
    {
        public LoginRoot Data;
    }




}
