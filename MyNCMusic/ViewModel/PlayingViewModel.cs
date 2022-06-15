using MyNCMusic.Controls;
using MyNCMusic.Models;
using MyNCMusic.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class PlayingViewModel
    {
        public BitmapImage AlbumImage { get; set; }
        public string MusicName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public List<Artist> Artists { get; set; }
        public List<LyricStr> LyricStrs { get; set; }
        public List<CommentItem> HotComments { get; set; }
        public List<CommentItem> AllComments { get; set; }
        public List<MusicItem> SimiSongs { get; set; }
        public int CommentCount { get; set; }
        public bool IsPlayingSong { get; set; }
        private int pivotIndex;
        /// <summary>
        /// 0 歌词 1评论 2相似
        /// </summary>
        public int PivotIndex
        {
            get=> pivotIndex;
            set
            {
                pivotIndex = value;
                switch (value)
                {
                    case 1:
                        {
                            if (AllComments == null)
                            {
                                if (PlayingService.IsPlayingSong)
                                {
                                    UpdateSongComment();
                                }
                                else
                                {
                                    UpDateRadioComment();
                                }
                            }
                        }
                        break;
                    case 2:
                        {
                            if (PlayingService.IsPlayingSong && SimiSongs == null)
                            {
                                UpdateSimiSongs();
                            }
                        }
                        break;
                }
            }
        }
        public PlayingViewModel()
        {
            PlayingService.OnPlayingChanged += PlayingService_OnPlayChanged;
            PlayingService.MediaTimelineController.PositionChanged += MediaTimelineController_PositionChanged;
            Update();
        }

        private void MediaTimelineController_PositionChanged(Windows.Media.MediaTimelineController sender, object args)
        {
            ChangeLyricPosition(sender.Position.TotalMilliseconds);
        }
        /// <summary>
        /// MediaTimelineController 250ms间隔，无可避免歌词延迟
        /// </summary>
        /// <param name="totalMilliseconds"></param>
        public async void ChangeLyricPosition(double totalMilliseconds)
        {
            if (LyricStrs == null || LyricStrs.Count == 0)
                return;
            int index = await Task.Run(() => SearchPerfectTime(totalMilliseconds));
            if (index < 0)
                return;
            //下一句歌词距当前时间大于100ms先不显示
            if (LyricStrs[index].DateTime.TimeOfDay.TotalMilliseconds - totalMilliseconds > 100)
                return;
            OnUpdatePlayingLyric?.Invoke(index);
            if(OnUpdateCompactLyric!=null)
            {
                LyricStr[] lyricStrArray = new LyricStr[3];
                //前一句
                if (index == 0)
                {
                    lyricStrArray[0] = new LyricStr();
                } 
                else
                {
                    lyricStrArray[0] = LyricStrs[index - 1];
                }
                //当前
                lyricStrArray[1] = LyricStrs[index];
                //后一句
                if (index == LyricStrs.Count - 1)
                {
                    lyricStrArray[2] = new LyricStr();
                }
                else
                {
                    lyricStrArray[2] = LyricStrs[index + 1];
                }
                OnUpdateCompactLyric.Invoke(lyricStrArray);
            }
        }
        public delegate void UpdatePlayingLyric(int index);
        public static event UpdatePlayingLyric OnUpdatePlayingLyric;
        public delegate void UpdateCompactLyric(LyricStr[] lyricStrs);
        public static event UpdateCompactLyric OnUpdateCompactLyric;
        /// <summary>
        /// 从歌词lyricStrs中寻找下一句歌词
        /// 二分查找
        /// </summary>
        /// <param name="totalMilliseconds"></param>
        /// <returns></returns>
        int SearchPerfectTime(double totalMilliseconds)
        {
            List<double> ranges = new List<double>();
            int low = 0, high = LyricStrs.Count - 1, mid;
            double range = LyricStrs[LyricStrs.Count / 2].DateTime.TimeOfDay.TotalMilliseconds > totalMilliseconds ? LyricStrs[LyricStrs.Count / 2].DateTime.TimeOfDay.TotalMilliseconds - totalMilliseconds : totalMilliseconds - LyricStrs[LyricStrs.Count / 2].DateTime.TimeOfDay.TotalMilliseconds;
            while (low <= high) //当前区间存在元素时循环
            {
                mid = (low + high) / 2;
                if (low == high)
                {
                    return low;
                }
                double rangeTemp_mid = Range(LyricStrs[mid].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds);

                //range = rangeTemp_mid;
                //判断前后数据变化
                if (mid == 0)//即只剩下0和1位置
                {
                    if (Range(LyricStrs[mid].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds) > Range(LyricStrs[mid + 1].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds))
                    {
                        return mid + 1;
                    }
                    else
                    {
                        return mid;
                    }
                }
                else
                {
                    double rangeTemp_previous = Range(LyricStrs[mid - 1].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds);
                    double rangeTemp_next = Range(LyricStrs[mid + 1].DateTime.TimeOfDay.TotalMilliseconds, totalMilliseconds);
                    if (rangeTemp_mid < rangeTemp_previous && rangeTemp_mid < rangeTemp_next)
                    {
                        return mid;
                    }
                    if (rangeTemp_previous > rangeTemp_next)
                        low = mid + 1;
                    else
                        high = mid - 1;
                }
            }
            return -1;
        }
        private double Range(double d1, double d2)
        {
            return d1 > d2 ? d1 - d2 : d2 - d1;
        }

        private void PlayingService_OnPlayChanged(long id, string url)
        {
            Update();
        }
        private void Update()
        {
            AlbumImage = PlayingService.PlayingAlbumBitmapImage;
            IsPlayingSong = PlayingService.IsPlayingSong;
            HotComments = null;
            AllComments = null;
            SimiSongs = null;
            if (PlayingService.IsPlayingSong)
                UpdateSong();
            else
                UpdateRadio();
            switch(PivotIndex)
            {
                case 1:
                    {
                        if(PlayingService.IsPlayingSong)
                        {
                            UpdateSongComment();
                        }
                        else
                        {
                            UpDateRadioComment();
                        }
                    }break;
               case 2:
                    {
                        if (PlayingService.IsPlayingSong)
                        {
                            UpdateSimiSongs();
                        }
                    }
                    break;
            }
        }
        private async void UpdateSong()
        {
            MusicName = PlayingService.PlayingSong.Name;
            ArtistName = PlayingService.PlayingSong.Ar.First().Name;
            AlbumName = PlayingService.PlayingSong.Al.Name;
            Artists = PlayingService.PlayingSong.Ar;
            LyricRoot lyricRoot = await LyricService.GetLyricAsync(PlayingService.PlayingSong.Id);
            if (lyricRoot == null)
            {
                LyricStrs = null;
            }
            else
            {
                LyricStrs = LyricService.GetLyricStrs(lyricRoot);
                if (LyricStrs != null)
                {
                    //歌词滚动到第一个？
                    // ListBox_lyric.ScrollIntoView(lyricStrs.First());
                }
                else
                {
                    LyricStrs = null;
                }
            }
            
        }
        async void UpdateSongComment()
        {
            Controls.WaitingPopup.Show();
            CommentRoot commentRoot = await CommentService.GetSongsCommentAsync(PlayingService.PlayingSong.Id);
            Controls.WaitingPopup.Hide();
            if (commentRoot == null)
            {
                CommentCount = 0;
                HotComments = null;
                AllComments = null;
            }
            else
            {
                CommentCount = commentRoot.Total;
                HotComments = commentRoot.HotComments;
                AllComments = commentRoot.Comments;
            }
            //ScrollViewer_comment.ChangeView(null, 0, null);
        }
        async void UpdateSimiSongs()
        {
            SimiSongsRoot simiSongsRoot = await SongService.GetSimiSongsAsync(PlayingService.PlayingSong.Id);
            if (simiSongsRoot == null)
            {
                SimiSongs = null;
            }
            else
            {
                //此处获取的专辑信息为album，需手动赋值给al，歌手信息为artistsm，需改为ar
                foreach (var temp in simiSongsRoot.Songs)
                {
                    temp.Al = temp.Album;
                    temp.Ar = temp.Artists;
                }
                SimiSongs = simiSongsRoot.Songs;
            }
        }

        private void UpdateRadio()
        {
            MusicName = PlayingService.PlayingRadio.Name;
            ArtistName = PlayingService.PlayingRadio.Dj.Nickname;
            AlbumName = PlayingService.PlayingRadio.Radio.Name;
            LyricStrs = null;
        }
        async void UpDateRadioComment()
        {
            CommentRoot commentRoot = await CommentService.GetRadioCommentAsync(PlayingService.PlayingRadio.Id);
            if (commentRoot == null)
            {
                CommentCount = 0;
                HotComments = null;
                AllComments = null;
            }
            else
            {
                CommentCount = commentRoot.Total;
                HotComments = commentRoot.HotComments;
                AllComments = commentRoot.Comments;
            }
        }

        public ICommand IntoCompactOverlayModeCommand => new DelegateCommand(() =>
        {
            NavigateService.NavigateToCompactOverlayMode();
        });

        public ICommand CheckArtistCommand => new DelegateCommand<Artist>(async(artist) =>
        {
            WaitingPopup.Show();
            if(artist == null)
            {
                artist = PlayingService.PlayingSong.Ar.First();
            }
            ArtistBaseDetailRoot artistBaseDetailRoot = await ArtistService.GetArtistBaseDetailAsync(artist.Id);
            NavigateService.NavigateToArtistAsync(artistBaseDetailRoot);
            WaitingPopup.Hide();
        });

        public ICommand CheckAlbumCommand => new DelegateCommand(() =>
        {
            WaitingPopup.Show();
            NavigateService.NavigateToAlbumAsync(PlayingService.PlayingAlbum);
            WaitingPopup.Hide();
        });
        /// <summary>
        /// 收藏
        /// </summary>
        public ICommand AddToPlaylistCommand => new DelegateCommand(async() =>
        {
            await new ContentDialogs.AddToPlaylist().ShowAsync();
        });

        /// <summary>
        /// 播放相似单曲
        /// </summary>
        /// <param name="music"></param>
        public async void PlaySimiMusic(MusicItem music = null)
        {
            if (music == null)
            {
                music = SimiSongs?.FirstOrDefault();
            }
            if (music != null)
            {
                PlayingService.PlayingListId = music.Al.Id;
                await PlayingService.ChangePlayingSong(music.Id, SimiSongs, music);
            }
        }
    }
}
