using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using MyNCMusic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyNCMusic
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MediaPlayer _mediaPlayer;
        MediaSource _mediaSource;
        MediaPlaybackItem _mediaPlaybackItem;
        public ImageBrush mainImageBrush;
        MediaTimelineController _mediaTimelineController;//时间控制器
        public static FavoriteSongsRoot favoriteSongsRoot;
        public static SolidColorBrush mainSolidColorBrush;//文字foreground颜色
        public static SolidColorBrush backgroundBrush;//背景图片主颜色
        bool isSliderChangedFromAuto = false;
        TimeSpan _duration;//进度条长度
        public Stopwatch playDurationStopwatch;//当前歌曲播放时长
        public MainPage()
        {
            this.InitializeComponent();
            //_mediaPlayer = new MediaPlayer();
            //backgroundBrush = new SolidColorBrush(Colors.Black);
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //Loaded += MainPage_Loaded;
            //(Application.Current as App).myMainPage = this;
            //_mediaTimelineController = PlayingService.MediaTimelineController;
            //_mediaTimelineController.PositionChanged += _mediaTimelineController_PositionChanged;
            //_mediaTimelineController.StateChanged += _mediaTimelineController_StateChanged;
            //_mediaPlayer.TimelineController = _mediaTimelineController;
            //_mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;
            //_mediaPlayer.SourceChanged += _mediaPlayer_SourceChanged;


            ////接管系统播放音频控制
            //_mediaPlayer.CommandManager.NextBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            //_mediaPlayer.CommandManager.PreviousBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            //_mediaPlayer.CommandManager.PreviousReceived += CommandManager_PreviousReceived;
            //_mediaPlayer.CommandManager.NextReceived += CommandManager_NextReceived;
            //mainImageBrush = new ImageBrush();
            //mainSolidColorBrush = new SolidColorBrush(Colors.White);
            //playDurationStopwatch = PlayingService.PlayDurationStopwatch;

            //PlayingService.PlayingSongList = new List<MusicItem>();
            //PlayingService.PlayingRadioList = new List<RadioSongItem>();
            //PlayingService.OnPlayingSongChanged += PlayingService_OnPlayingSongChanged;
            //PlayingService.OnPlayingRadioChanged += PlayingService_OnPlayingRadioChanged;
            //GetSetting();

            //设置标题栏
            var tiWtleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            tiWtleBar.BackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonBackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonForegroundColor = Colors.White;
            tiWtleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonHoverBackgroundColor = Colors.LightGray;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(MyTitleBar);
        }



        private async void PlayingService_OnPlayingRadioChanged()
        {
            ChangeImage();
            ChangePlayBar(PlayingService.PlayingAlbumBitmapImage, PlayingService.PlayingRadio.Name, PlayingService.PlayingRadio.Dj.Nickname, PlayingService.PlayingRadio.Name, PlayingService.PlayingRadio.MainSong.Duration / 1000);
            if ((Application.Current as App).playingPage != null)
                (Application.Current as App).playingPage.LoadLayout();
            if ((Application.Current as App).compactOverlayPage != null)
                (Application.Current as App).compactOverlayPage.UpdateLayout();
            if (PlayingService.PlayingSongUrlRoot.Data.First().Url == null)
            {
                NotifyPopup notifyPopup = new NotifyPopup("播放地址错误");
                notifyPopup.Show();
                //跳过当前到下一首
                PlayingService.PlayNextRadio();
                return;
            }
            _mediaSource = await Task.Run(() => MediaSource.CreateFromUri(new Uri(PlayingService.PlayingSongUrlRoot.Data.First().Url)));
            _mediaSource.OpenOperationCompleted += _mediaSource_OpenOperationCompleted;
            _mediaPlaybackItem = new MediaPlaybackItem(_mediaSource);
            _mediaPlayer.Source = _mediaPlaybackItem;
            _mediaTimelineController.Start();

            //修改SMTC 显示的元数据
            MediaItemDisplayProperties props = _mediaPlaybackItem.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Music;
            props.MusicProperties.Title = PlayingService.PlayingRadio.Name;
            props.MusicProperties.Artist = PlayingService.PlayingRadio.Dj.Nickname;
            props.Thumbnail = RandomAccessStreamReference.CreateFromFile(await ApplicationData.Current.LocalFolder.TryGetItemAsync(ConfigService.ImageFilename) as StorageFile);
            _mediaPlaybackItem.ApplyDisplayProperties(props);
        }

        ObservableCollection<MusicBase> playingListBaseObjects = PlayingService.PlayingList;

        private  async void PlayingService_OnPlayingSongChanged()
        {
            ChangeImage();
            ChangePlayBar(PlayingService.PlayingSong,PlayingService.PlayingAlbumBitmapImage, PlayingService.PlayingSong.Name, PlayingService.PlayingSong.Ar.First().Name, PlayingService.PlayingSong.Al.Name, PlayingService.PlayingSong.Dt / 1000);
            if ((Application.Current as App).playingPage != null)
                (Application.Current as App).playingPage.LoadLayout();
            if ((Application.Current as App).compactOverlayPage != null)
                (Application.Current as App).compactOverlayPage.UpdateLayout();
            if (PlayingService.PlayingSongUrlRoot.Data.First().Url == null)
            {
                NotifyPopup notifyPopup = new NotifyPopup("播放地址错误");
                notifyPopup.Show();
                //跳过当前到下一首
                PlayingService.PlayNextSongs();
                return;
            }
            _mediaSource = await Task.Run(() => MediaSource.CreateFromUri(new Uri(PlayingService.PlayingSongUrlRoot.Data.First().Url)));
            _mediaSource.OpenOperationCompleted += _mediaSource_OpenOperationCompleted;
            _mediaPlaybackItem = new MediaPlaybackItem(_mediaSource);
            _mediaPlayer.Source = _mediaPlaybackItem;
            _mediaTimelineController.Start();
            //SetLastPlayedSong();

            //修改SMTC 显示的元数据

            MediaItemDisplayProperties props = _mediaPlaybackItem.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Music;
            props.MusicProperties.Title = PlayingService.PlayingSong.Name;
            props.MusicProperties.Artist = PlayingService.PlayingSong.Ar.First().Name;
            props.Thumbnail = RandomAccessStreamReference.CreateFromFile(await ApplicationData.Current.LocalFolder.TryGetItemAsync(ConfigService.ImageFilename) as StorageFile);
            _mediaPlaybackItem.ApplyDisplayProperties(props);
        }

        private void _mediaPlayer_SourceChanged(MediaPlayer sender, object args)
        {
            Thread.Sleep(100);//确保打卡请求到的TotalMilliseconds是有效的而不是被Restart恢复到0了
            playDurationStopwatch.Restart();
        }

        private async void _mediaTimelineController_StateChanged(MediaTimelineController sender, object args)
        {

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (sender.State == MediaTimelineControllerState.Running)
                {
                    playDurationStopwatch.Start();
                    SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
                }
                else
                {
                    playDurationStopwatch.Stop();
                    SymbolIcon_stopOrPlay.Symbol = Symbol.Play;
                }
            });
        }

        private void CommandManager_NextReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerNextReceivedEventArgs args)
        {
            if (PlayingService.IsPlayingSong)
                PlayingService.PlayNextSongs();
            else
                PlayingService.PlayNextRadio();
        }

        private void CommandManager_PreviousReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPreviousReceivedEventArgs args)
        {
            if (PlayingService.IsPlayingSong)
                PlayingService.PlayLastSongs();
            else
                PlayingService.PlayLastRadio();
        }

        //监控修改播放进度条
        private async void _mediaTimelineController_PositionChanged(MediaTimelineController sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                isSliderChangedFromAuto = true;
                Slider_play.Value = sender.Position.TotalSeconds;
                
                TextBlock_currentTime.Text = OtherHelper.GetDt((int)sender.Position.TotalSeconds);
                if ((Application.Current as App).playingPage != null)
                    (Application.Current as App).playingPage.ChangeLyricPosition(sender.Position.TotalMilliseconds);
            });
        }

        private async void _mediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _mediaTimelineController.Pause();
                if (PlayingService.IsPlayingSong)
                    PlayingService.PlayNextSongs();
                else
                    PlayingService.PlayNextRadio();
            });
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Frame_all.Navigate(typeof(Home));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(MyTitleBar);
        }

        /// <summary>
        /// 读取本地专辑图片修改背景图
        /// </summary>
        async void ChangeImage()
        {
            //判断Local是否有文件
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(ConfigService.ImageFilename) is StorageFile localFile)//本地有专辑图片，读取
            {
                WriteableBitmap writeableBitmap = await FileHelper.OpenWriteableBitmapFile(localFile);
                SolidColorBrush solidColorBrush = new SolidColorBrush(GetColor.GetMajorColor(writeableBitmap));
                mainImageBrush.ImageSource = writeableBitmap;
                mainImageBrush.Stretch = Stretch.UniformToFill;
                mainGrid.Background = mainImageBrush;

                solidColorBrush.Color = OtherHelper.ChangeColor(solidColorBrush.Color, (float)0.4);
                backgroundBrush = solidColorBrush;
            }
            else//本地无专辑图片
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Gray);
                mainGrid.Background = solidColorBrush;
                backgroundBrush = solidColorBrush;
            }
        }

        /// <summary>
        /// 获取上一次播放的信息
        /// </summary>
        async void GetLastPlayingInfo()
        {
            ChangeImage();
            PlayingService.PlayingAlbumBitmapImage = await FileHelper.ReadLoaclBitmapImage(ConfigService.ImageFilename);
            if (await PlayingService.Load())
            {
                Slider_Volume.Value = PlayingService.Volume * 100;
                if (PlayingService.IsPlayingSong)
                {
                    if(PlayingService.PlayingSong!=null)
                        ChangePlayBar(PlayingService.PlayingSong, PlayingService.PlayingAlbumBitmapImage, PlayingService.PlayingSong.Name, PlayingService.PlayingSong.Ar.First().Name, PlayingService.PlayingSong.Al.Name, PlayingService.PlayingSong.Dt / 1000, false);
                }
                else
                {
                    if(PlayingService.PlayingRadio!=null)
                        ChangePlayBar(PlayingService.PlayingAlbumBitmapImage, PlayingService.PlayingRadio.Name, PlayingService.PlayingRadio.Dj.Nickname, PlayingService.PlayingRadio.Name, PlayingService.PlayingRadio.MainSong.Duration / 1000);
                }
                UpDatePlayOrderStateIcon();
                if (PlayingService.PlayingSongUrlRoot != null)
                {
                    SongUrlRoot songUrlRoot = SongService.GetMusicUrl(PlayingService.PlayingSongUrlRoot.Data.First().Id);
                    //_mediaSource = await Task.Run(() => MediaSource.CreateFromUri(new Uri(PlayingService.PlayingSongUrlRoot.data.First().url)));
                    _mediaSource = await Task.Run(() => MediaSource.CreateFromUri(new Uri(songUrlRoot.Data.First().Url)));
                    _mediaSource.OpenOperationCompleted += _mediaSource_OpenOperationCompleted;
                    _mediaSource.StateChanged += _mediaSource_StateChanged;
                    _mediaPlaybackItem = new MediaPlaybackItem(_mediaSource);
                    _mediaPlayer.Source = _mediaPlaybackItem;
                    
                }
                
            }
        }

        private void _mediaSource_StateChanged(MediaSource sender, MediaSourceStateChangedEventArgs args)
        {
            
        }


        /// <summary>
        /// 启动时获取应用设置
        /// </summary>
        void GetSetting()
        {
            try
            {
                ConfigService.LoadConfig();
                GetLastPlayingInfo();
            }
            catch (Exception) { }
        }

        private async void _mediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            _duration = sender.Duration.GetValueOrDefault();
            //_mediaTimelineController.Duration = _duration;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Slider_play.Maximum = _duration.TotalSeconds;
                Slider_play.StepFrequency = 1;
            });
        }

        async void ChangePlayBar(MusicItem song,BitmapImage bitmapImage, string musicName, string artistName, string albumName, int maximum, bool isStartPlaying = true)
        {
            Image_playingAlbum.Source = bitmapImage;//修改专辑图片
            TextBlcok_musicName.Text = musicName;
            TextBlcok_artistName.Text = artistName;
            TextBlcok_albumName.Text = albumName;
            TextBlock_lengthTime.Text = await Task.Run(() => OtherHelper.GetDt(maximum));
            if (isStartPlaying)
                SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
            if (song.IsFavorite)
            {
                TextBlock_isOrnotFavorite.Text = "\xE00B";
            }
            else
            {
                TextBlock_isOrnotFavorite.Text = "\xE006";
            }
        }

        /// <summary>
        /// 播放电台时使用修改底部播放条
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <param name="musicName"></param>
        /// <param name="artistName"></param>
        /// <param name="albumName"></param>
        /// <param name="maximum"></param>
        /// <param name="isStartPlaying"></param>
        async void ChangePlayBar(BitmapImage bitmapImage, string musicName, string artistName, string albumName, int maximum, bool isStartPlaying = true)
        {
            Image_playingAlbum.Source = bitmapImage;//修改专辑图片
            TextBlcok_musicName.Text = musicName;
            TextBlcok_artistName.Text = artistName;
            TextBlcok_albumName.Text = albumName;
            TextBlock_lengthTime.Text = await Task.Run(() => OtherHelper.GetDt(maximum));
            if (isStartPlaying)
                SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
        }

        private void Button_stopOrPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaSource == null)
                return;
            var state = _mediaPlayer.PlaybackSession.PlaybackState;
            if (state == MediaPlaybackState.Playing)//改为暂停
            {
                _mediaTimelineController.Pause();
            }
            else
            {
                _mediaTimelineController.Resume();
            }
        }



        /// <summary>
        /// 播放列表双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListBox_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            PlayingSongBaseObject song = listBox.SelectedItem as PlayingSongBaseObject;
            if(PlayingService.IsPlayingSong)
            {
                await PlayingService.ChangePlayingSong(song.Id);
            }
            else
            {
                await PlayingService.ChangePlayingRadio(song.Id);
            }
        }

        /// <summary>
        /// 点击修改播放顺序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_playOrderState_Click(object sender, RoutedEventArgs e)
        {
            switch(PlayingService.PlayOrderState)
            {
                case PlayOrderStateEnum.顺序播放:
                    {
                        PlayingService.PlayOrderState = PlayOrderStateEnum.列表循环;
                        SymbolIcon_playOrderState.Symbol = Symbol.RepeatAll;
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        PlayingService.PlayOrderState = PlayOrderStateEnum.随机播放;
                        SymbolIcon_playOrderState.Symbol = Symbol.Shuffle;
                    }
                    break;
                case PlayOrderStateEnum.随机播放:
                    {
                        PlayingService.PlayOrderState = PlayOrderStateEnum.单曲循环;
                        SymbolIcon_playOrderState.Symbol = Symbol.RepeatOne;
                    }
                    break;
                case PlayOrderStateEnum.单曲循环:
                    {
                        PlayingService.PlayOrderState = PlayOrderStateEnum.顺序播放;
                        SymbolIcon_playOrderState.Symbol = Symbol.AlignLeft;
                    }
                    break;
            }
        }

        /// <summary>
        /// 更新播放顺序
        /// </summary>
        void UpDatePlayOrderStateIcon()
        {
            switch (PlayingService.PlayOrderState)
            {
                case PlayOrderStateEnum.顺序播放:
                    {
                        SymbolIcon_playOrderState.Symbol = Symbol.AlignLeft;
                        
                    }
                    break;
                case PlayOrderStateEnum.列表循环:
                    {
                        SymbolIcon_playOrderState.Symbol = Symbol.RepeatAll;
                        
                    }
                    break;
                case PlayOrderStateEnum.随机播放:
                    {
                        SymbolIcon_playOrderState.Symbol = Symbol.Shuffle;
                        
                    }
                    break;
                case PlayOrderStateEnum.单曲循环:
                    {
                        SymbolIcon_playOrderState.Symbol = Symbol.RepeatOne;
                    }
                    break;
            }
        }

        private void Slider_play_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isSliderChangedFromAuto)
            {
                isSliderChangedFromAuto = false;
                return;
            }
            _mediaTimelineController.Position = TimeSpan.FromSeconds(e.NewValue);
            //_mediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        private void Button_previous_Click(object sender, RoutedEventArgs e)
        {
            if(PlayingService.IsPlayingSong)
                PlayingService.PlayLastSongs();
            else
                PlayingService.PlayLastRadio();
        }

        private void Button_next_Click(object sender, RoutedEventArgs e)
        {
            if (PlayingService.IsPlayingSong)
                PlayingService.PlayNextSongs();
            else
                PlayingService.PlayNextRadio();
        }

        private void Button_playInfo_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).homepage!=null&&(Application.Current as App).homepage.Frame.CanGoBack)
                (Application.Current as App).homepage.Frame.GoBack();
            else
            {
                if (PlayingService.PlayingSong == null)
                    return;
                Frame_all.Navigate(typeof(PlayingPage));
            }
        }

        public delegate void IsOrNotFavoriteChanged();
        //与委托相关联的事件
        public event IsOrNotFavoriteChanged OnIsOrNotFavoriteChanged;
        //事件触发函数
        private void WhenIsOrNotFavoriteChange()
        {
            OnIsOrNotFavoriteChanged?.Invoke();
        }

        public async void Button_isOrNotFavorite_Click(object sender, RoutedEventArgs e)
        {
            if(!PlayingService.IsPlayingSong)
            {
                NotifyPopup notifyPopup = new NotifyPopup("无法对电台节目进行此操作");
                notifyPopup.Show();
                return;
            }
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
                    TextBlock_isOrnotFavorite.Text = "\xE006";
                    PlayingService.PlayingSong.IsFavorite = false;
                    favoriteSongsRoot.Ids.Remove(PlayingService.PlayingSong.Id);
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
                    NotifyPopup notifyPopup = new NotifyPopup("已添加为喜欢", "\xE00B",Colors.MediumSeaGreen);
                    notifyPopup.Show();
                    TextBlock_isOrnotFavorite.Text = "\xE00B";
                    PlayingService.PlayingSong.IsFavorite = true;
                    favoriteSongsRoot.Ids.Add(PlayingService.PlayingSong.Id);
                }
                else
                {
                    NotifyPopup notifyPopup = new NotifyPopup("操作失败", "\xE10A");
                    notifyPopup.Show();
                }
            }
            WhenIsOrNotFavoriteChange();
        }

        /// <summary>
        /// 开启迷你置顶模式
        /// </summary>
        public async void IntoCompactOverlayMode()
        {
            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(340, 160);
            if(await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions))
                Frame.Navigate(typeof(CompactOverlayPage));
        }

        private void Button_playList_Click(object sender, RoutedEventArgs e)
        {
            var temp = PlayingService.PlayingList.FirstOrDefault(p => p.IsPlaying);
            if(temp!=null)
                ListBox_playList.ScrollIntoView(temp);
        }

        private void Flyout_Opened(object sender, object e)
        {

        }

        private void Slider_Volume_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            //每滚动一次固定值120？向上为正，下为负
            if(e.GetCurrentPoint(Slider_Volume).Properties.MouseWheelDelta>0)
            {
                Slider_Volume.Value += 5;
            }
            else
            {
                Slider_Volume.Value -= 5;
            }
        }
    }
}
