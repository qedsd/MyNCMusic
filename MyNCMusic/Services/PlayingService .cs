using MyNCMusic.Helper;
using MyNCMusic.Model;
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
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.Services
{
    public static class PlayingService
    {
        /// <summary>
        /// 我创建的歌单
        /// </summary>
        public static ObservableCollection<PlaylistItem> PlaylistItems_Created;
        /// <summary>
        /// 我订阅的歌单
        /// </summary>
        public static ObservableCollection<PlaylistItem> PlaylistItems_Subscribed;


        /// <summary>
        /// 上一次播放的歌曲列表
        /// </summary>
        public static List<SongsItem> PlayedSongList;

        public static List<SongsItem> _PlayingSongList;
        /// <summary>
        /// 当前播放的歌曲列表
        /// </summary>
        public static List<SongsItem> PlayingSongList
        {
            get { return _PlayingSongList; }
            set
            {
                PlayedSongList = PlayingSongList;
                _PlayingSongList = value;
            }
        }

        /// <summary>
        /// 上一次播放的电台列表
        /// </summary>
        public static List<RadioSongItem> PlayedRadioList;

        public static List<RadioSongItem> _PlayingRadioList;
        /// <summary>
        /// 当前播放的电台列表
        /// </summary>
        public static List<RadioSongItem> PlayingRadioList
        {
            get { return _PlayingRadioList; }
            set
            {
                PlayedRadioList = PlayingRadioList;
                _PlayingRadioList = value;
            }
        }

        /// <summary>
        /// 正在播放的歌曲的ID
        /// </summary>
        public static long PlayingSongId;

        /// <summary>
        /// 正在播放的电台节目的ID
        /// </summary>
        public static long PlayingRadioId;

        /// <summary>
        /// 上次播放的歌曲
        /// </summary>
        public static SongsItem PlayedSong;

        /// <summary>
        /// 上次播放的电台
        /// </summary>
        public static RadioSongItem PlayedRadio;

        public static SongsItem _PlayingSong;
        /// <summary>
        /// 当前播放歌曲
        /// </summary>
        public static SongsItem PlayingSong
        {
            get { return _PlayingSong; }
            set
            {
                PlayedSong = PlayingSong;
                _PlayingSong = value;
            }
        }

        public static RadioSongItem _PlayingRadio;
        /// <summary>
        /// 当前播放电台
        /// </summary>
        public static RadioSongItem PlayingRadio
        {
            get { return _PlayingRadio; }
            set
            {
                PlayedRadio = PlayingRadio;
                _PlayingRadio = value;
            }
        }

        /// <summary>
        /// 当前播放的歌曲的专辑
        /// </summary>
        public static AlbumRoot PlayingAlbum;

        /// <summary>
        /// 当前播放歌曲URL
        /// </summary>
        public static SongUrlRoot PlayingSongUrlRoot;

        /// <summary>
        /// 播放过的歌曲的ID
        /// </summary>
        public static List<long> PlayedSongId;

        /// <summary>
        /// 播放过的电台的ID
        /// </summary>
        public static List<long> PlayedRadioId;

        /// <summary>
        /// 当前播放时长
        /// </summary>
        public static Stopwatch PlayDurationStopwatch = new Stopwatch();

        /// <summary>
        /// 上次播放的歌单/专辑id
        /// </summary>
        public static long PlayedListId;

        public static long _PlayingListId;
        /// <summary>
        /// 当前播放的歌单/专辑ID
        /// </summary>
        public static long PlayingListId
        {
            get { return _PlayingListId; }
            set
            {
                PlayedListId = PlayingListId;
                _PlayingListId = value;
            }
        }

        /// <summary>
        /// 当前播放歌曲专辑Image
        /// </summary>
        public static BitmapImage PlayingAlbumBitmapImage;

        public delegate void PlayingSongChanged();
        //与委托相关联的事件
        public static event PlayingSongChanged OnPlayingSongChanged;
        //事件触发函数
        private static void WhenPlayingSongChange()
        {
            OnPlayingSongChanged?.Invoke();
        }

        public static bool IsPlayingSong;

        public static double Volume = 1;

        /// <summary>
        /// 更改播放歌曲入口函数
        /// </summary>
        /// <param name="playingSongId"></param>
        /// <param name="songsItems"></param>
        /// <param name="songsItem"></param>
        /// <returns></returns>
        public static async Task<bool> ChangePlayingSong(long playingSongId, List<SongsItem> songsItems, SongsItem songsItem = null)
        {
            IsPlayingSong = true;
            PlayingSongId = playingSongId;
            PlayingSongList = songsItems;
            bool b;
            if (songsItem == null)
                b=await PreparePlayingSong(PlayingSongId);
            else
                b=await PreparePlayingSong(PlayingSongId, songsItem);
            if (!b)
            {
                NotifyPopup notifyPopup = new NotifyPopup("获取音乐失败");
                notifyPopup.Show();
            }
            else
            {

                PlayingListToBaseObject(PlayingSongList);
                WhenPlayingSongChange();
            }
            return b;
        }
        public static async Task<bool> ChangePlayingSong(long playingSongId, SongsItem songsItem = null)
        {
            IsPlayingSong = true;
            PlayingSongId = playingSongId;
            PlayingSongList = PlayingSongList;
            
            bool b;
            if (songsItem == null)
                b = await PreparePlayingSong(PlayingSongId);
            else
                b = await PreparePlayingSong(PlayingSongId, songsItem);
            PlayingListToBaseObject(PlayingSongList);
            WhenPlayingSongChange();
            return b;
        }

        public delegate void PlayingRadioChanged();
        //与委托相关联的事件
        public static event PlayingRadioChanged OnPlayingRadioChanged;
        //事件触发函数
        private static void WhenPlayingRadioChange()
        {
            OnPlayingRadioChanged?.Invoke();
        }
        public static async Task<bool> ChangePlayingRadio(long playingRadioId, List<RadioSongItem> radioSongItems=null)
        {
            IsPlayingSong = false;
            PlayingRadioId = playingRadioId;
            if(radioSongItems!=null)
            {
                PlayingRadioList = radioSongItems;
            }
            else
            {
                PlayingRadioList = PlayingRadioList;
            }
            bool b=await PreparePlayingRadio();
            PlayingListToBaseObject(PlayingRadioList);
            WhenPlayingRadioChange();
            return b;
        }

        

        public delegate void PlayingRadioListChanged();
        //与委托相关联的事件
        public static event PlayingRadioListChanged OnPlayingRadioListChanged;
        //事件触发函数
        private static void WhenPlayingRadioListChange()
        {
            OnPlayingRadioListChanged?.Invoke();
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
        /// 开始播放前获取相关信息准备
        /// </summary>
        /// <param name="playingSongId"></param>
        /// <param name="songsItem"></param>
        /// <returns></returns>
        public static async Task<bool> PreparePlayingSong(long playingSongId, SongsItem songsItem = null)
        {
            if (songsItem == null)//需获取实例
            {
                MusicDetailRoot musicDetailRoot = await Task.Run(() => SongService.GetMusicDetail_Get(playingSongId.ToString()));
                if (musicDetailRoot == null || musicDetailRoot.songs == null || musicDetailRoot.songs.Count == 0)
                {
                    return false;
                }
                songsItem = musicDetailRoot.songs.Last();
                //判断是否为喜欢歌曲
                if (MainPage.favoriteSongsRoot != null)
                {
                    if (MainPage.favoriteSongsRoot.ids.Find(p => p.Equals(songsItem.Id)) != 0)
                        songsItem.isFavorite = true;
                }
            }
            SongUrlRoot songUrlRoot = SongService.GetMusicUrl(songsItem.Id);
            
            if (songUrlRoot == null)
                return false;
            PlayingSong = songsItem;
            PlayingSongUrlRoot = songUrlRoot;
            var playingSong = PlayingService.PlayingSongList.FirstOrDefault(p => p.Id == PlayingService.PlayingSong.Id);
            if(playingSong==null)//将要播放的歌曲不在当前播放列表
            {
                PlayingSongList = new List<SongsItem>() { PlayingSong };
                playingSong = PlayingSong;
            }
            if(PlayedSongId!=null)
            {
                if (PlayingSongList != PlayedSongList)//不同一个播放列表需清空列表
                {
                    PlayedSongId.Clear();
                }
            }
            else
            {
                PlayedSongId = new List<long>();
            }
            PlayedSongId.Remove(playingSong.Id);//删除重复的，避免死循环
            PlayedSongId.Add(playingSong.Id);
            if (PlayingService.PlayedSong != null)//听歌打卡
            {
                PlayingService.PlayDurationStopwatch.Stop();
                await Task.Run(() => SongService.MarkPlayDuration(PlayedSong.Id, PlayedListId, PlayDurationStopwatch.ElapsedMilliseconds / 1000));
            }

            //获取专辑
            PlayingAlbum = await Task.Run(() => AlbumService.GetAlbum(songsItem.al.id));
            if (PlayingAlbum == null)
                return false;
            PlayingService.PlayingAlbumBitmapImage = await FileHelper.DownloadFile(new Uri(PlayingAlbum.album.picUrl + "?param=200y200"));

            if(PlayedSong!=null&& PlayedSongList!=null)
                PlayedSongList.FirstOrDefault(p=>p.Id== PlayedSong.Id).IsPlaying = false;
            PlayingSongList.FirstOrDefault(p => p.Id == PlayingSong.Id).IsPlaying = true;

            return true;

            
        }

        public static async Task<bool> PreparePlayingRadio()
        {
            SongUrlRoot songUrlRoot = SongService.GetMusicUrl(PlayingRadioId);
            if (songUrlRoot == null)
                return false;
            PlayingSongUrlRoot = songUrlRoot;
            var playingRadio = PlayingRadioList.FirstOrDefault(p => p.MainSong.Id == PlayingRadioId);
            if (playingRadio == null)//将要播放的电台不在当前播放列表
            {
                PlayingRadioList = new List<RadioSongItem>() { PlayingRadio };
                playingRadio = PlayingRadio;
            }
            if (PlayedRadioId != null)
            {
                if (PlayingRadioList != PlayedRadioList)//不同一个播放列表需清空列表
                {
                    PlayedRadioId.Clear();
                }
            }
            else
            {
                PlayedRadioId = new List<long>();
            }
            PlayedRadioId.Remove(playingRadio.MainSong.Id);//删除重复的，避免死循环
            PlayedRadioId.Add(playingRadio.MainSong.Id);
            PlayingRadio = playingRadio;


            PlayingAlbumBitmapImage = await FileHelper.DownloadFile(new Uri(PlayingRadio.CoverUrl + "?param=200y200"));

            if (PlayedRadio != null&& PlayedRadioList!=null)
                PlayedRadioList.FirstOrDefault(p => p.Id == PlayedRadio.Id).MainSong.IsPlaying = false;
            PlayingRadioList.FirstOrDefault(p => p.Id == PlayingRadio.Id).MainSong.IsPlaying = true;
            return true;
        }

        /// <summary>
        /// 歌曲、电台共用的播放列表
        /// </summary>
        public static ObservableCollection<PlayingSongBaseObject> PlayingListBaseObjects = new ObservableCollection<PlayingSongBaseObject>();

        /// <summary>
        /// 将歌曲/电台列表转换到共用的播放列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void PlayingListToBaseObject<T>(List<T> list)
        {
            PlayingListBaseObjects.Clear();
            if(list!=null)
            {
                if(list.First().GetType()==typeof(SongsItem))
                {
                    foreach (var temp in list)
                        PlayingListBaseObjects.Add(temp as PlayingSongBaseObject);
                }
                else
                {
                    foreach (var temp in list)
                        PlayingListBaseObjects.Add((temp as RadioSongItem).MainSong as PlayingSongBaseObject);
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
            foreach(var temp in PlayingListBaseObjects)
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
        public static MediaTimelineController MediaTimelineController = new MediaTimelineController();

        /// <summary>
        /// 播放下一首
        /// </summary>
        public static async void PlayNextSongs()
        {
            long willPlayId = 0;
            if (PlayingListBaseObjects==null||PlayingListBaseObjects.Count == 0)
                return;
            int index = PlayingService.PlayingListBaseObjects.IndexOf(PlayingService.PlayingListBaseObjects.FirstOrDefault(p => p.Id == PlayingService.PlayingSong.Id));
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
                        willPlayId = PlayingService.PlayingListBaseObjects[++index].Id;
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        if (index == PlayingSongList.Count - 1)//播完，回到第一个
                        {
                            index = -1;
                        }
                        willPlayId = PlayingService.PlayingListBaseObjects[++index].Id;
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
                            int i = rd.Next(0, PlayingService.PlayingListBaseObjects.Count - 1);
                            if (!PlayingService.PlayedSongId.Contains(PlayingService.PlayingListBaseObjects[i].Id))
                            {
                                willPlayId = PlayingService.PlayingListBaseObjects[i].Id;
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
                if (await ChangePlayingSong(willPlayId, PlayingSongList))
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
                    if (await ChangePlayingSong(PlayedSongId.Last(), PlayingService.PlayedSongList))
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
            if (PlayingListBaseObjects == null || PlayingListBaseObjects.Count == 0)
                return;
            int index = PlayingListBaseObjects.IndexOf(PlayingListBaseObjects.FirstOrDefault(p => p.Id == PlayingRadio.MainSong.Id));
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
                        willPlayId = PlayingListBaseObjects[++index].Id;
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        if (index == PlayingRadioList.Count - 1)//播完，回到第一个
                        {
                            index = -1;
                        }
                        willPlayId = PlayingListBaseObjects[++index].Id;
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
                            int i = rd.Next(0, PlayingListBaseObjects.Count - 1);
                            if (!PlayedRadioId.Contains(PlayingListBaseObjects[i].Id))
                            {
                                willPlayId = PlayingListBaseObjects[i].Id;
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
                if (await ChangePlayingRadio(willPlayId, PlayingRadioList))
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
                    if (await ChangePlayingRadio(PlayedRadioId.Last(), PlayedRadioList))
                        break;
                }
                NotifyPopup notifyPopup = new NotifyPopup("多次获取音乐失败，停止播放");
            }
        }

    }
    
    /// <summary>
    /// 供序列化保存使用的播放信息类,自动关联PlayingService
    /// </summary>
    [Serializable]
    public class PlayingInfoToSave
    {
        public SongsItem PlayingSong
        {
            get { return PlayingService.PlayingSong; }
            set { PlayingService.PlayingSong = value; }
        }
        public RadioSongItem PlayingRadio
        {
            get { return PlayingService.PlayingRadio; }
            set { PlayingService.PlayingRadio = value; }
        }
        public List<SongsItem> PlayingSongList
        {
            get { return PlayingService.PlayingSongList; }
            set { PlayingService.PlayingSongList = value; }
        }
        public List<RadioSongItem> PlayingRadioList
        {
            get { return PlayingService.PlayingRadioList; }
            set { PlayingService.PlayingRadioList = value; }
        }
        public SongsItem PlayedSong
        {
            get { return PlayingService.PlayedSong; }
            set { PlayingService.PlayedSong = value; }
        }
        public RadioSongItem PlayedRadio
        {
            get { return PlayingService.PlayedRadio; }
            set { PlayingService.PlayedRadio = value; }
        }
        public SongUrlRoot PlayingSongUrlRoot
        {
            get { return PlayingService.PlayingSongUrlRoot; }
            set { PlayingService.PlayingSongUrlRoot = value; }
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
        public ObservableCollection<PlayingSongBaseObject> PlayingListBaseObjects
        {
            get { return PlayingService.PlayingListBaseObjects; }
            set { PlayingService.PlayingListBaseObjects = value; }
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
        public List<SongsItem> PlayedSongList
        {
            get { return PlayingService.PlayedSongList; }
            set { PlayingService.PlayedSongList = value??new List<SongsItem>(); }
        }
        public List<RadioSongItem> PlayedRadioList
        {
            get { return PlayingService.PlayedRadioList; }
            set { PlayingService.PlayedRadioList = value??new List<RadioSongItem>(); }
        }

        public double Volume
        {
            get { return PlayingService.Volume; }
            set { PlayingService.Volume = value; }
        }
    }
}
