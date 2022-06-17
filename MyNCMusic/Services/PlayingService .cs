using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheGuideToTheNewEden.Helper;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.Services
{
    public static class PlayingService
    {
        /// <summary>
        /// 当前播放的歌曲列表
        /// </summary>
        public static List<MusicItem> PlayingSongList { get; set; }

        /// <summary>
        /// 当前播放的电台列表
        /// </summary>
        public static List<RadioSongItem> PlayingRadioList { get; set; }

        /// <summary>
        /// 正在播放的歌曲的ID
        /// </summary>
        public static long PlayingSongId;

        /// <summary>
        /// 正在播放的电台节目的ID
        /// </summary>
        public static long PlayingRadioId;

        /// <summary>
        /// 当前播放歌曲
        /// </summary>
        public static MusicItem PlayingSong { get; set; }

        /// <summary>
        /// 当前播放电台
        /// </summary>
        public static RadioSongItem PlayingRadio { get; set; }

        /// <summary>
        /// 当前播放的歌曲的专辑
        /// </summary>
        public static AlbumRoot PlayingAlbum;

        /// <summary>
        /// 当前播放歌曲URL
        /// </summary>
        public static SongUrlRoot PlayingUrlRoot;

        /// <summary>
        /// 播放过的歌曲的ID
        /// </summary>
        public static List<long> PlayedSongId;

        /// <summary>
        /// 播放过的电台的ID
        /// </summary>
        public static List<long> PlayedRadioId;

        /// <summary>
        /// 当前播放的歌单/专辑ID
        /// </summary>
        public static long PlayingListId{get;set;}

        /// <summary>
        /// 当前播放歌曲专辑Image
        /// </summary>
        public static BitmapImage PlayingAlbumBitmapImage;

        public static bool IsPlayingSong;

        public static double Volume = 0.5;

        /// <summary>
        /// 更改播放歌曲入口函数
        /// </summary>
        /// <param name="playingSongId"></param>
        /// <param name="songsItems"></param>
        /// <param name="songsItem"></param>
        /// <returns></returns>
        public static async Task<bool> ChangePlayingSongAsync(long playingSongId,long playingListId, List<MusicItem> songsItems, MusicItem songsItem)
        {
            if(IsPlayingSong && MediaTimelineController != null && MediaTimelineController.Position.TotalSeconds!=0)
            {
                long sourceId = PlayingListId;
                if (sourceId <= 0)//日推、搜索等没有明确播放来源的，都按自身专辑记录
                {
                    sourceId = PlayingAlbum.Album.Id;
                }
                _ = SongService.MarkPlayDurationAsync(PlayingSong.Id, sourceId, (long)MediaTimelineController.Position.TotalSeconds);
            }
            if(PlayingListId != playingListId)//不同播放列表
            {
                PlayingSongList.Clear();
                PlayedSongId.Clear();
                PlayingListToBaseObject(songsItems);
                PlayingListId = playingListId;
                PlayingSongList = songsItems;
            }
            else
            {
                foreach (var item in PlayingList)
                {
                    if (item.Id == playingSongId)
                    {
                        item.IsPlaying = true;
                    }
                    else
                    {
                        item.IsPlaying = false;
                    }
                }
            }
            if (songsItem == null)//需获取实例
            {
                MusicDetailRoot musicDetailRoot = await SongService.GetMusicDetail_GetAsync(playingSongId.ToString());
                if (musicDetailRoot == null || musicDetailRoot.Songs == null || musicDetailRoot.Songs.Count == 0)
                {
                    NotifyPopup.ShowError("获取音乐失败");
                    return false;
                }
                songsItem = musicDetailRoot.Songs.Last();
            }
            PlayingSong = songsItem;
            SongUrlRoot songUrlRoot = SongService.GetMusicUrl(songsItem.Id);
            if(songUrlRoot == null)
            {
                NotifyPopup.ShowError("获取播放地址失败");
                return false;
            }
            PlayingUrlRoot = songUrlRoot;

            PlayedSongId.Remove(playingSongId);
            PlayedSongId.Add(playingSongId);

            PlayingAlbum = await AlbumService.GetAlbumAsync(songsItem.Al.Id);
            if (PlayingAlbum == null)
            {
                NotifyPopup.ShowError("获取专辑失败");
                return false;
            }
            PlayingAlbumBitmapImage = await FileHelper.DownloadFile(new Uri(PlayingAlbum.Album.PicUrl + "?param=200y200"));

            IsPlayingSong = true;
            PlayingSongId = playingSongId;
            OnPlayingChanged?.Invoke(PlayingSongId, songUrlRoot.Data.First().Url);
            return true;
        }

        public static async Task<bool> ChangePlayingRadioAsync(long playingRadioId, List<RadioSongItem> radioSongItems)
        {
            if (IsPlayingSong && MediaTimelineController != null && MediaTimelineController.Position.TotalSeconds != 0)
            {
                _ = SongService.MarkPlayDurationAsync(PlayingSong.Id, PlayingListId, (long)MediaTimelineController.Position.TotalSeconds);
            }
            IsPlayingSong = false;
            PlayingRadioId = playingRadioId;
            if(radioSongItems!=null)
            {
                PlayingRadioList = radioSongItems;
                PlayedRadioId.Clear();
                PlayingListToBaseObject(PlayingRadioList);
            }
            else
            {
                foreach (var item in PlayingList)
                {
                    if (item.Id == playingRadioId)
                    {
                        item.IsPlaying = true;
                    }
                    else
                    {
                        item.IsPlaying = false;
                    }
                }
            }
            PlayedRadioId.Remove(playingRadioId);
            PlayedRadioId.Add(playingRadioId);
            PlayingRadio = PlayingRadioList.FirstOrDefault(p=>p.Id == playingRadioId);
            if(PlayingRadio == null)
            {
                NotifyPopup.ShowError("未找到电台信息");
                return false;
            }
            PlayingAlbumBitmapImage = await FileHelper.DownloadFile(new Uri(PlayingRadio.CoverUrl + "?param=200y200"));
            SongUrlRoot songUrlRoot = SongService.GetMusicUrl(PlayingRadioId);
            if (songUrlRoot == null)
            {
                NotifyPopup.ShowError("获取播放地址失败");
                return false;
            }
            PlayingUrlRoot = songUrlRoot;
            OnPlayingChanged?.Invoke(PlayingRadioId, songUrlRoot.Data.First().Url);
            return true;
        }

        /// <summary>
        /// 从本地xml加载播放信息
        /// </summary>
        public static async Task<bool> Load()
        {
            string path = ConfigService.Folder.Path + "/playinginfo.xml";
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("playinginfo.xml") is StorageFile)
            {
                XmlSerializerHelper.DeserializeFromXml(path, typeof(PlayingInfoToSave));
                return true;
            }
            return false;
        }
        /// <summary>
        /// 保存播放信息到本地xml
        /// </summary>
        public static void Save()
        {
            XmlSerializerHelper.SerializeToXml(ConfigService.Folder.Path + "/playinginfo.xml", new PlayingInfoToSave());
        }

        /// <summary>
        /// 歌曲、电台共用的播放列表
        /// </summary>
        public static ObservableCollection<MusicBase> PlayingList = new ObservableCollection<MusicBase>();

        /// <summary>
        /// 将歌曲/电台列表转换到共用的播放列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void PlayingListToBaseObject<T>(List<T> list)
        {
            PlayingList.Clear();
            if(list!=null)
            {
                if(list.First().GetType()==typeof(MusicItem))
                {
                    foreach (var temp in list)
                        PlayingList.Add(temp as MusicItem);
                }
                else
                {
                    foreach (var temp in list)
                        PlayingList.Add((temp as RadioSongItem).MainSong);
                }
            }
        }

        /// <summary>
        /// 播放记录是否包含播放列表所有的项
        /// </summary>
        /// <returns></returns>
        public static bool IsPlayedSongContainAllPlayingListSong(List<long> ls)
        {
            if (ls == null)
                return false;
            foreach(var temp in PlayingList)
            {
                if (!ls.Contains(temp.Id))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 播放顺序
        /// </summary>
        public static PlayOrderStateEnum PlayOrderState = PlayOrderStateEnum.顺序播放;
        /// <summary>
        /// 多媒体控制器
        /// </summary>
        public static MediaTimelineController MediaTimelineController;

        public static void PlayNext()
        {
            if(IsPlayingSong)
            {
                PlayNextSongs();
            }
            else
            {
                PlayNextRadio();
            }
        }
        public static void PlayLast()
        {
            if (IsPlayingSong)
            {
                PlayLastSongs();
            }
            else
            {
                PlayLastRadio();
            }
        }
        /// <summary>
        /// 播放下一首
        /// </summary>
        public static async void PlayNextSongs()
        {
            long willPlayId = 0;
            if (PlayingList == null|| PlayingList.Count == 0)
                return;
            int index = PlayingService.PlayingList.IndexOf(PlayingService.PlayingList.FirstOrDefault(p => p.Id == PlayingService.PlayingSong.Id));
            switch (PlayOrderState)
            {
                case PlayOrderStateEnum.顺序播放:
                    {
                        if (index == PlayingSongList.Count - 1)//播完，停止
                        {
                            MediaTimelineController.Start();
                            MediaTimelineController.Pause();
                            return;
                        }
                        willPlayId = PlayingService.PlayingList[++index].Id;
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        if (index == PlayingSongList.Count - 1)//播完，回到第一个
                        {
                            index = -1;
                        }
                        willPlayId = PlayingService.PlayingList[++index].Id;
                    }
                    break;
                case PlayOrderStateEnum.随机播放:
                    {
                        if (IsPlayedSongContainAllPlayingListSong(PlayedSongId))//播完，停止
                        {
                            MediaTimelineController.Start();
                            MediaTimelineController.Pause();
                            return;
                        }
                        Random rd = new Random();
                        while (true)
                        {
                            int i = rd.Next(0, PlayingService.PlayingList.Count - 1);
                            if (!PlayingService.PlayedSongId.Contains(PlayingService.PlayingList[i].Id))
                            {
                                willPlayId = PlayingService.PlayingList[i].Id;
                                break;
                            }
                        }
                    }
                    break;
                case PlayOrderStateEnum.单曲循环:
                    {
                        MediaTimelineController.Start();
                        return;
                    }
            }
            for (int i = 0; i < 5; i++)
            {
                if (await ChangePlayingSongAsync(willPlayId, PlayingListId, null,null))
                    break;
            }
            NotifyPopup notifyPopup = new NotifyPopup("多次获取音乐失败，停止播放");
        }

        /// <summary>
        /// 播放上一首
        /// </summary>
        public static async void PlayLastSongs()
        {
            if (PlayedSongId!=null&&PlayedSongId.Count > 1)//上一个是当前歌曲，所以要上上一个
            {
                PlayedSongId.Remove(PlayedSongId.Last());//移出当前
                for (int i = 0; i < 5; i++)
                {
                    if (await ChangePlayingSongAsync(PlayedSongId.Last(), PlayingListId, null, null))
                        break;
                }
                NotifyPopup notifyPopup = new NotifyPopup("多次获取音乐失败，停止播放");
            }
        }

        /// <summary>
        /// 播放下一首
        /// </summary>
        public static async void PlayNextRadio()
        {
            long willPlayId = 0;
            if (PlayingList == null || PlayingList.Count == 0)
                return;
            int index = PlayingList.IndexOf(PlayingList.FirstOrDefault(p => p.Id == PlayingRadio.MainSong.Id));
            switch (PlayOrderState)
            {
                case PlayOrderStateEnum.顺序播放:
                    {
                        if (index == PlayingRadioList.Count - 1)//播完，停止
                        {
                            MediaTimelineController.Start();
                            MediaTimelineController.Pause();
                            return;
                        }
                        willPlayId = PlayingList[++index].Id;
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        if (index == PlayingRadioList.Count - 1)//播完，回到第一个
                        {
                            index = -1;
                        }
                        willPlayId = PlayingList[++index].Id;
                    }
                    break;
                case PlayOrderStateEnum.随机播放:
                    {
                        if (IsPlayedSongContainAllPlayingListSong(PlayedRadioId))//播完，停止
                        {
                            MediaTimelineController.Start();
                            MediaTimelineController.Pause();
                            return;
                        }
                        Random rd = new Random();
                        while (true)
                        {
                            int i = rd.Next(0, PlayingList.Count - 1);
                            if (!PlayedRadioId.Contains(PlayingList[i].Id))
                            {
                                willPlayId = PlayingList[i].Id;
                                break;
                            }
                        }
                    }
                    break;
                case PlayOrderStateEnum.单曲循环:
                    {
                        MediaTimelineController.Start();
                        return;
                    }
            }
            for (int i = 0; i < 5; i++)
            {
                if (await ChangePlayingRadioAsync(willPlayId, PlayingRadioList))
                    break;
            }
            NotifyPopup notifyPopup = new NotifyPopup("多次获取音乐失败，停止播放");
        }

        /// <summary>
        /// 播放上一首
        /// </summary>
        public static async void PlayLastRadio()
        {
            if (PlayedRadioId!=null&&PlayedRadioId.Count > 1)//上一个是当前电台，所以要上上一个
            {
                PlayedRadioId.Remove(PlayedRadioId.Last());//移出当前
                for (int i = 0; i < 5; i++)
                {
                    if (await ChangePlayingRadioAsync(PlayedRadioId.Last(), PlayingRadioList))
                        break;
                }
                NotifyPopup notifyPopup = new NotifyPopup("多次获取音乐失败，停止播放");
            }
        }

        #region new
        public static HashSet<long> FavoriteMusics = new HashSet<long>();
        //public static void Play(long musicId)
        //{
        //    string url = GetSongMediaUrl(musicId);
        //    OnPlayingChanged?.Invoke(musicId,url);
        //}
        ///// <summary>
        ///// 128k
        ///// </summary>
        ///// <param name="musicId"></param>
        ///// <returns></returns>
        //public static string GetSongMediaUrl(long musicId)
        //{
        //    return $"https://music.163.com/song/media/outer/url?id={musicId}.mp3";//不走song/url避免403
        //}
        public delegate void PlayingChanged(long id,string url);
        public static event PlayingChanged OnPlayingChanged;
        public static void AddFavorite(long id)
        {
            FavoriteMusics.Add(id);
            OnFavoriteChanged?.Invoke(id, true);
        }
        public static void RemoveFavorite(long id)
        {
            FavoriteMusics.Remove(id);
            OnFavoriteChanged?.Invoke(id, false);
        }
        public delegate void FavoriteChanged(long id,bool isFavorite);
        public static event FavoriteChanged OnFavoriteChanged;
        #endregion
    }

    /// <summary>
    /// 供序列化保存使用的播放信息类,自动关联PlayingService
    /// </summary>
    [Serializable]
    public class PlayingInfoToSave
    {
        public MusicItem PlayingSong
        {
            get { return PlayingService.PlayingSong; }
            set { PlayingService.PlayingSong = value; }
        }
        public RadioSongItem PlayingRadio
        {
            get { return PlayingService.PlayingRadio; }
            set { PlayingService.PlayingRadio = value; }
        }
        public List<MusicItem> PlayingSongList
        {
            get { return PlayingService.PlayingSongList; }
            set { PlayingService.PlayingSongList = value; }
        }
        public List<RadioSongItem> PlayingRadioList
        {
            get { return PlayingService.PlayingRadioList; }
            set { PlayingService.PlayingRadioList = value; }
        }

        public SongUrlRoot PlayingSongUrlRoot
        {
            get { return PlayingService.PlayingUrlRoot; }
            set { PlayingService.PlayingUrlRoot = value; }
        }
        public AlbumRoot PlayingAlbum
        {
            get { return PlayingService.PlayingAlbum; }
            set { PlayingService.PlayingAlbum = value; }
        }
        public long PlayingListId
        {
            get { return PlayingService.PlayingListId; }
            set { PlayingService.PlayingListId = value; }
        }
        public ObservableCollection<MusicBase> PlayingListBaseObjects
        {
            get { return PlayingService.PlayingList; }
            set { PlayingService.PlayingList = value; }
        }
        public PlayOrderStateEnum PlayOrderState
        {
            get { return PlayingService.PlayOrderState; }
            set { PlayingService.PlayOrderState = value; }
        }
        public bool IsPlayingSong
        {
            get { return PlayingService.IsPlayingSong; }
            set { PlayingService.IsPlayingSong = value; }
        }
        public List<long> PlayedSongId
        {
            get { return PlayingService.PlayedSongId; }
            set { PlayingService.PlayedSongId = value??new List<long>(); }
        }

        public List<long> PlayedRadioId
        {
            get { return PlayingService.PlayedRadioId; }
            set { PlayingService.PlayedRadioId = value ?? new List<long>(); }
        }

        public double Volume
        {
            get { return PlayingService.Volume; }
            set { PlayingService.Volume = value; }
        }
    }
}
