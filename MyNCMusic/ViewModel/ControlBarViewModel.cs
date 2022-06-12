﻿using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class ControlBarViewModel
    {
        #region 属性
        public BitmapImage AlbumImage { get; set; }
        public string MusicName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        /// <summary>
        /// 正常播放导致的PlayedSeconds变化
        /// </summary>
        private bool isPlayedSecondsAutoChange = true;
        private double playedSeconds;
        /// <summary>
        /// 绑定Slider的Value
        /// 有可能是用户操作控件导致的，也可能是正常播放变化
        /// </summary>
        public double PlayedSeconds
        {
            get => playedSeconds;
            set
            {
                playedSeconds = value;
                if (isPlayedSecondsAutoChange)//正常播放变化
                {
                    isPlayedSecondsAutoChange = false;
                    return;
                }
                else//手动拖拽
                {
                    if (MediaTimelineController != null)
                    {
                        MediaTimelineController.Position = TimeSpan.FromSeconds(value);
                    }
                }
            }
        }
        public double TotalSeconds { get; set; }
        public bool IsFavorite { get; set; }
        public SolidColorBrush MainForegroundSolidColorBrush { get; set; }
        /// <summary>
        /// 电台不可以设为喜欢
        /// </summary>
        public bool IsShowFavorite { get; set; }
        private bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                PlayStatusSymbol = !isPlaying ? Symbol.Play : Symbol.Pause;
            }
        }
        public Symbol PlayStatusSymbol { get; set; } = Symbol.Pause;
        public Symbol PlayOrderSymbol { get; set; }
        public string FavoriteSymbolText { get; set; }
        /// <summary>
        /// 播客媒体播放控制器
        /// </summary>
        public MediaPlayer MediaPlayer { get;private set; }
        /// <summary>
        /// 时间控制器
        /// </summary>
        public MediaTimelineController MediaTimelineController { get; private set; }

        public ObservableCollection<MusicBase> PlayingList { get; set; } = PlayingService.PlayingList;

        private MediaSource MediaSource;
        /// <summary>
        /// 用于系统音乐播放控件显示
        /// </summary>
        private MediaPlaybackItem MediaPlaybackItem;
        #endregion
        public static ControlBarViewModel Instance;
        public ControlBarViewModel()
        {
            Instance = this;
            MediaPlayer = new MediaPlayer();
            MediaTimelineController = new MediaTimelineController();
            PlayingService.MediaTimelineController = MediaTimelineController;
            MediaTimelineController.PositionChanged += MediaTimelineController_PositionChanged;
            MediaTimelineController.StateChanged += MediaTimelineController_StateChanged;
            MediaPlayer.TimelineController = MediaTimelineController;
            MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            MediaPlayer.SourceChanged += MediaPlayer_SourceChanged;
            PlayingService.OnPlayingChanged += PlayingService_OnPlayChanged;
            UpdatePlayOrder(PlayOrder);
        }

        private void PlayingService_OnPlayChanged(long id,string url)
        {
            Play(url, PlayingService.PlayingSong.Name, PlayingService.PlayingSong.Ar.First().Name, PlayingService.PlayingSong.Al.Name);
        }

        private void MediaPlayer_SourceChanged(MediaPlayer sender, object args)
        {
            
        }

        private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MediaTimelineController.Pause();
                if (PlayingService.IsPlayingSong)
                    PlayingService.PlayNextSongs();
                else
                    PlayingService.PlayNextRadio();
            });
        }

        private async void MediaTimelineController_StateChanged(MediaTimelineController sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                IsPlaying = sender.State == MediaTimelineControllerState.Running;
            });
        }

        private async void MediaTimelineController_PositionChanged(MediaTimelineController sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                isPlayedSecondsAutoChange = true;
                PlayedSeconds = sender.Position.TotalSeconds;
            });
        }

        #region 事件
        public ICommand PlayingInfoCommand => new DelegateCommand<string>((p) =>
        {
            Views.Home.Instance.NavigateToPlayingPage();
        });
        public ICommand FavoriteCommand => new DelegateCommand(async() =>
        {
            if (PlayingService.PlayingSong == null)
            {
                NotifyPopup notifyPopup = new NotifyPopup("无法获取音乐信息");
                notifyPopup.Show();
                return;
            }
            if (PlayingService.PlayingSong.IsFavorite)//取消喜欢
            {
                if (await SongService.LoveOrDontLoveSongAsync(PlayingService.PlayingSong.Id, false) == true)
                {
                    NotifyPopup notifyPopup = new NotifyPopup("已取消喜欢", "\xE00C");
                    notifyPopup.Show();
                    FavoriteSymbolText = "\xE006";
                    PlayingService.PlayingSong.IsFavorite = false;
                    Services.PlayingService.FavoriteMusics.Remove(PlayingService.PlayingSong.Id);
                }
                else
                {
                    NotifyPopup notifyPopup = new NotifyPopup("操作失败", "\xE10A");
                    notifyPopup.Show();
                }
            }
            else//添加为喜欢的
            {
                if (await SongService.LoveOrDontLoveSongAsync(PlayingService.PlayingSong.Id, true) == true)
                {
                    NotifyPopup notifyPopup = new NotifyPopup("已添加为喜欢", "\xE00B", Windows.UI.Colors.MediumSeaGreen);
                    notifyPopup.Show();
                    FavoriteSymbolText = "\xE00B";
                    PlayingService.PlayingSong.IsFavorite = true;
                    Services.PlayingService.FavoriteMusics.Add(PlayingService.PlayingSong.Id);
                }
                else
                {
                    NotifyPopup notifyPopup = new NotifyPopup("操作失败", "\xE10A");
                    notifyPopup.Show();
                }
            }
        });
        public ICommand PreviousMusicCommand => new DelegateCommand(() =>
        {
            PlayingService.PlayLastSongs();
        });
        public ICommand PauseOrPlayCommand => new DelegateCommand(() =>
        {
            if (MediaSource == null)
                return;
            var state = MediaPlayer.PlaybackSession.PlaybackState;
            if (state == MediaPlaybackState.Playing)//改为暂停
            {
                MediaTimelineController.Pause();
                IsPlaying = false;
            }
            else
            {
                MediaTimelineController.Resume();
                IsPlaying = true;
            }
        });
        public ICommand NextMusicCommand => new DelegateCommand(() =>
        {
            PlayingService.PlayNextSongs();
        });
        public ICommand ChangePlayOrderCommand => new DelegateCommand(() =>
        {
            ChangePlayOrder(PlayOrder);
        });

        public ICommand ChangePlayingId => new DelegateCommand<MusicBase>(async(item) =>
        {
            if(item!=null)
            {
                if (PlayingService.IsPlayingSong)
                {
                    await PlayingService.ChangePlayingSong(item.Id);
                }
                else
                {
                    await PlayingService.ChangePlayingRadio(item.Id);
                }
            }
        });
        #endregion
        private PlayOrderStateEnum PlayOrder = PlayingService.PlayOrderState;
        /// <summary>
        /// 按输入播放顺序修改新播放顺序
        /// </summary>
        /// <param name="playOrder"></param>
        private void ChangePlayOrder(PlayOrderStateEnum playOrder)
        {
            switch (playOrder)
            {
                case PlayOrderStateEnum.顺序播放:
                    {
                        PlayOrder = PlayOrderStateEnum.列表循环;
                        //PlayOrderSymbol = Symbol.RepeatAll;
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        PlayOrder = PlayOrderStateEnum.随机播放;
                        //PlayOrderSymbol = Symbol.Shuffle;
                    }
                    break;
                case PlayOrderStateEnum.随机播放:
                    {
                        PlayOrder = PlayOrderStateEnum.单曲循环;
                        //PlayOrderSymbol = Symbol.RepeatOne;
                    }
                    break;
                case PlayOrderStateEnum.单曲循环:
                    {
                        PlayOrder = PlayOrderStateEnum.顺序播放;
                        //PlayOrderSymbol = Symbol.AlignLeft;
                    }
                    break;
            }
            UpdatePlayOrder(playOrder);
        }
        /// <summary>
        /// 更新为指定播放顺序
        /// </summary>
        /// <param name="playOrder"></param>
        private void UpdatePlayOrder(PlayOrderStateEnum playOrder)
        {
            PlayOrder = playOrder;
            switch (playOrder)
            {
                case PlayOrderStateEnum.顺序播放:
                    {
                        PlayOrderSymbol = Symbol.AlignLeft;
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        PlayOrderSymbol = Symbol.RepeatAll;
                    }
                    break;
                case PlayOrderStateEnum.随机播放:
                    {
                        PlayOrderSymbol = Symbol.Shuffle;
                    }
                    break;
                case PlayOrderStateEnum.单曲循环:
                    {
                        PlayOrderSymbol = Symbol.RepeatOne;
                    }
                    break;
            }
        }

        private string lastPlayedUrl;
        public async void Play(string url,string musicName,string artistName, string albumName)
        {
            if (lastPlayedUrl != url)//非重复播放
            {
                MusicName = musicName;
                ArtistName = artistName;
                AlbumName = albumName;

                MediaSource = await Task.Run(() => MediaSource.CreateFromUri(new Uri(url)));
                MediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
                MediaPlaybackItem = new MediaPlaybackItem(MediaSource);
                MediaPlayer.Source = MediaPlaybackItem;
                MediaTimelineController.Start();

                //左下角专辑图片
                //AlbumImage = await Helper.FileHelper.ReadLoaclBitmapImage(ConfigService.ImageFilename);
                AlbumImage = PlayingService.PlayingAlbumBitmapImage;

                //修改SMTC 显示的元数据
                MediaItemDisplayProperties props = MediaPlaybackItem.GetDisplayProperties();
                props.Type = Windows.Media.MediaPlaybackType.Music;
                props.MusicProperties.Title = PlayingService.PlayingSong.Name;
                props.MusicProperties.Artist = PlayingService.PlayingSong.Ar.First().Name;
                props.Thumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(await Windows.Storage.ApplicationData.Current.LocalFolder.TryGetItemAsync(ConfigService.ImageFilename) as Windows.Storage.StorageFile);
                MediaPlaybackItem.ApplyDisplayProperties(props);
            }
            lastPlayedUrl = url;
            MediaTimelineController.Start();
            IsPlaying = true;
        }
        private async void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            var duration = sender.Duration.GetValueOrDefault();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                TotalSeconds = duration.TotalSeconds;
            });
        }
    }
}
