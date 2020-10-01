using MyNCMusic.Model;
using MyNCMusic.MyUserControl;
using MyNCMusic.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        MediaPlayer _mediaPlayer;
        MediaSource _mediaSource;
        MediaPlaybackItem _mediaPlaybackItem;
        public ImageBrush mainImageBrush;
        public AlbumRoot _album;
        public BitmapImage _AlbumBitmapImage;
        MediaTimelineController _mediaTimelineController;//时间控制器

        public ObservableCollection<SongsItem> currentPlayList;

        public static SongsItem songsItem;
        public static SongUrlRoot songUrlRoot;
        public static FavoriteSongsRoot favoriteSongsRoot;
        public static long PlayingListId;//1日推 2随机50喜欢 +3相似
        public static SolidColorBrush mainSolidColorBrush;//文字foreground颜色
        public static SolidColorBrush backgroundBrush;//背景图片主颜色

        public static bool isTaskDone=true;
        int playOrderState = 0;//0顺序播放，1循环列表播放，2随机播放，3单曲循环
        public List<int> playHistoryIndex;
        bool isSliderChangedFromAuto = false;
        static Object locker = new object();
        TimeSpan _duration;//进度条长度
        public MainPage()
        {
            
            backgroundBrush = new SolidColorBrush(Colors.Black);
            _AlbumBitmapImage = new BitmapImage();
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += MainPage_Loaded;
            (Application.Current as App).myMainPage = this;
            _mediaPlayer = new MediaPlayer();
            _mediaTimelineController = new MediaTimelineController();
            _mediaTimelineController.PositionChanged += _mediaTimelineController_PositionChanged;
            _mediaTimelineController.StateChanged += _mediaTimelineController_StateChanged;
            _mediaPlayer.TimelineController = _mediaTimelineController;
            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;

            //接管系统播放音频控制
            _mediaPlayer.CommandManager.NextBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            _mediaPlayer.CommandManager.PreviousBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            _mediaPlayer.CommandManager.PreviousReceived += CommandManager_PreviousReceived;
            _mediaPlayer.CommandManager.NextReceived += CommandManager_NextReceived;

            currentPlayList = new ObservableCollection<SongsItem>();
            playHistoryIndex = new List<int>();
            mainImageBrush = new ImageBrush();
            mainSolidColorBrush = new SolidColorBrush(Colors.White);

            //playingAlbumBitmapImage = new BitmapImage(new Uri(""));
            //playingAlbumBitmapImage.ImageOpened += PlayingAlbumBitmapImage_ImageOpened;
            //b.ImageSource = playingAlbumBitmapImage;
            GetSetting();
            ChangeImage();
            GetLastPlayedSong();

            //Test();
            //设置标题栏
            var tiWtleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            tiWtleBar.BackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonBackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            tiWtleBar.ButtonHoverBackgroundColor = Colors.LightGray;
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(MyTitleBar);
        }

        private async void _mediaTimelineController_StateChanged(MediaTimelineController sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (sender.State == MediaTimelineControllerState.Running)
                    SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
                else
                    SymbolIcon_stopOrPlay.Symbol = Symbol.Play;
            });
        }

        private void CommandManager_NextReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerNextReceivedEventArgs args)
        {
            if (playHistoryIndex.Count == 0)
                return;
            int index = currentPlayList.IndexOf(songsItem);
            PlayNextSongs(index);
        }

        private void CommandManager_PreviousReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPreviousReceivedEventArgs args)
        {
            if (playHistoryIndex.Count > 1)
            {
                playHistoryIndex.Remove(playHistoryIndex.Last());
                PlayNextSongs(playHistoryIndex.Last(), true);
            }
        }

        //监控修改播放进度条
        private async void _mediaTimelineController_PositionChanged(MediaTimelineController sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                isSliderChangedFromAuto = true;
                Slider_play.Value = sender.Position.TotalSeconds;
                TextBlock_currentTime.Text = MyClassManager.GetDt((int)sender.Position.TotalSeconds);
                if ((Application.Current as App).playingPage != null)
                    (Application.Current as App).playingPage.ChangeLyricPosition(sender.Position.TotalMilliseconds);
            });
        }

        private async void _mediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            //int index = currentPlayList.IndexOf(songsItem);
            //playHistoryIndex.Add(index);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PlayNextSongs(playHistoryIndex.Last());
            });
            
            //switch (playOrderState)
            //{
            //    case 0:
            //        {
            //            //int index=currentPlayList.IndexOf(songsItem);
            //            if (index == currentPlayList.Count)//播完，停止
            //            {
            //                SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
            //                isTaskDone = true;
            //                return;
            //            }
            //            songsItem = currentPlayList[++index];
            //        }
            //        break;
            //    case 1:
            //        {
            //            //int index = currentPlayList.IndexOf(songsItem);
            //            if (index == currentPlayList.Count)//播完，回到第一个
            //            {
            //                index = -1;
            //            }
            //            songsItem = currentPlayList[++index];
            //        }
            //        break;
            //    case 2:
            //        {
            //            Random rd = new Random();
            //            while(true)
            //            {
            //                int i = rd.Next(0, currentPlayList.Count - 1);
            //                if(playHistoryIndex.Count== currentPlayList.Count)//播完，停止
            //                {
            //                    SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
            //                    isTaskDone = true;
            //                    return;
            //                }
            //                if (playHistoryIndex.First()==i)
            //                {
            //                    songsItem = currentPlayList[i];
            //                    break;
            //                }
            //                if(playHistoryIndex.IndexOf(i)!=0)//第一个不相等时返回0即为找不到
            //                {
            //                    songsItem = currentPlayList[i];
            //                    break;
            //                }
            //            }
            //        }
            //        break;
            //    case 3:
            //        {
            //            _mediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
            //            _mediaPlayer.Play();
            //        }
            //        break;
            //}
            //if (songsItem == null)
            //    return;
            //SongUrlRoot songUrlRootT = MyClassManager.GetMusicUrl(songsItem.id);
            //if (songUrlRootT == null)
            //    return;
            //songUrlRoot = songUrlRootT;
            //ChnagePlayingSong(songsItem, songUrlRoot);
        }
        public async void PlayNextSongs(int index,bool certain=false)
        {
            SongsItem songsItem_temp = null;
            if (certain)
                songsItem_temp = currentPlayList[index];
            else
            {
                switch (playOrderState)
                {
                    case 0:
                        {
                            //int index=currentPlayList.IndexOf(songsItem);
                            if (index == currentPlayList.Count-1)//播完，停止
                            {
                                //SymbolIcon_stopOrPlay.Symbol = Symbol.Play;
                                //isTaskDone = true;
                                //_mediaTimelineController.Position = TimeSpan.FromMilliseconds(0);
                                _mediaTimelineController.Start();
                                _mediaTimelineController.Pause();
                                return;
                            }
                            songsItem_temp = currentPlayList[++index];
                        }
                        break;
                    case 1:
                        {
                            //int index = currentPlayList.IndexOf(songsItem);
                            if (index == currentPlayList.Count-1)//播完，回到第一个
                            {
                                index = -1;
                            }
                            songsItem_temp = currentPlayList[++index];
                        }
                        break;
                    case 2:
                        {
                            Random rd = new Random();
                            while (true)
                            {
                                int i = rd.Next(0, currentPlayList.Count - 1);
                                if (index == -1)
                                {
                                    break;
                                }
                                if (playHistoryIndex.Count == currentPlayList.Count)//播完，停止
                                {
                                    //SymbolIcon_stopOrPlay.Symbol = Symbol.Play;
                                    //isTaskDone = true;
                                    //_mediaTimelineController.Position = TimeSpan.FromMilliseconds(0);
                                    _mediaTimelineController.Start();
                                    _mediaTimelineController.Pause();
                                    return;
                                }
                                if (playHistoryIndex.First() == i)
                                {
                                    songsItem_temp = currentPlayList[i];
                                    break;
                                }
                                if (playHistoryIndex.IndexOf(i) != 0)//第一个不相等时返回0即为找不到
                                {
                                    songsItem_temp = currentPlayList[i];
                                    break;
                                }
                            }
                        }
                        break;
                    case 3:
                        {
                            //if (index == -1)
                            //{
                            //    songsItem_temp = currentPlayList[0];
                            //}
                            //_mediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
                            //_mediaPlayer.Play();
                            _mediaTimelineController.Start();
                            return;
                        }
                }
            }
            if (songsItem_temp == null)
                return;
            SongUrlRoot songUrlRoot_temp = await Task.Run(() => MyClassManager.GetMusicUrl(songsItem_temp.id));
            if (songUrlRoot_temp == null)
                return;
            //songUrlRoot = songUrlRootT;
            ChnagePlayingSong(songsItem_temp, songUrlRoot_temp);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Frame_all.Navigate(typeof(Home));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(MyTitleBar);
        }


        async void ChangeImage()
        {
            //ImageBrush b = new ImageBrush();

            //byte[]  by=await MyClassManager.DownloadFile(new Uri(""));
            //SolidColorBrush solidColorBrush=await GetMajorColorAndBlur(by, b);

            //BitmapImage bitmapImageawait =await MyClassManager.DownloadFile(new Uri(""));
            StorageFile localFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync(MyClassManager.imageFilename) as StorageFile;//判断Local是否有文件
            if (null != localFile)//本地有专辑图片，读取
            {
                //StorageFile localFile = await StorageFile.GetFileFromPathAsync(MyClassManager.folder.Path + "\\" + MyClassManager.imageFilename);//读取文件
                WriteableBitmap writeableBitmap = await MyClassManager.OpenWriteableBitmapFile(localFile);
                SolidColorBrush solidColorBrush = new SolidColorBrush(GetColor.GetMajorColor(writeableBitmap));
                mainImageBrush.ImageSource = writeableBitmap;
                mainImageBrush.Stretch = Stretch.UniformToFill;
                mainGrid.Background = mainImageBrush;

                solidColorBrush.Color = MyClassManager.ChangeColor(solidColorBrush.Color, (float)0.4);
                //if((solidColorBrush.Color.R+ solidColorBrush.Color.G+ solidColorBrush.Color.B)>650)
                //    mainSolidColorBrush = new SolidColorBrush(Colors.Black);
                //else
                //    mainSolidColorBrush = new SolidColorBrush(Colors.White);
                //Grid_playBar.Background = solidColorBrush;
                backgroundBrush = solidColorBrush;
                //AcrylicBrush_mainFrame.TintColor = solidColorBrush.Color;
            }
            else//本地无专辑图片
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Gray);
                mainGrid.Background = solidColorBrush;
                backgroundBrush = solidColorBrush;
            }
        }
        async void GetLastPlayedSong()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["LastSongName"] != null)
            {
                TextBlcok_musicName.Text = localSettings.Values["LastSongName"].ToString();
            }
            if (localSettings.Values["LastArtistName"] != null)
            {
                TextBlcok_artistName.Text = localSettings.Values["LastArtistName"].ToString();
            }
            if (localSettings.Values["LastAlbumName"] != null)
            {
                TextBlcok_albumName.Text = localSettings.Values["LastAlbumName"].ToString();
            }
            if(localSettings.Values["LastSongId"] != null)
            {
                SongsItem songsItem = new SongsItem();
                songsItem.id = (int)localSettings.Values["LastSongId"];
                
                MusicDetailRoot musicDetailRoot = await Task.Run(() => MyClassManager.GetMusicDetail(songsItem.id.ToString()));
                if (musicDetailRoot == null || musicDetailRoot.songs == null)
                {
                    return;
                }
                SongUrlRoot songUrlRoot = await Task.Run(() => MyClassManager.GetMusicUrl(songsItem.id));
                if (songUrlRoot == null)
                {
                    return;
                }
                if (localSettings.Values["IsFavorite"] != null && (bool)localSettings.Values["IsFavorite"] == true)
                    musicDetailRoot.songs.First().isFavorite = true;
                else
                    musicDetailRoot.songs.First().isFavorite = false;
                currentPlayList.Add(musicDetailRoot.songs.First());
                ChnagePlayingSong(musicDetailRoot.songs.First(), songUrlRoot,false);
            }
        }
        void SetLastPlayedSong()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["LastSongName"] = songsItem.name;
            localSettings.Values["LastArtistName"] = songsItem.ar.First().name;
            localSettings.Values["LastAlbumName"] = songsItem.al.name;
            localSettings.Values["LastSongId"] = songsItem.id;
            localSettings.Values["IsFavorite"] = songsItem.isFavorite;
        }
        void GetSetting()
        {
            //获取账号、服务器地址
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["ServerIP"] != null)
            {
                MyClassManager.apiUri = localSettings.Values["ServerIP"].ToString();
            }
            if (localSettings.Values["PhoneOrEmail"] != null)
            {
                MyClassManager.phoneOrEmail = (string)localSettings.Values["PhoneOrEmail"];
            }
            if (localSettings.Values["Password"] != null)
            {
                MyClassManager.password = localSettings.Values["Password"].ToString();
            }
            if (localSettings.Values["Uid"] != null)
            {
                MyClassManager.uid = (long)localSettings.Values["Uid"];
            }
        }
        async System.Threading.Tasks.Task<SolidColorBrush> GetMajorColorAndBlur(byte[] b, ImageBrush backgroundBrush)
        {
            WriteableBitmap wb = new WriteableBitmap(1000, 1500);
            using (IRandomAccessStream iras = b.AsBuffer().AsStream().AsRandomAccessStream())
            {
                await wb.SetSourceAsync(iras);
            }
            //高斯模糊
            BlurEffect be = new BlurEffect(wb);
            backgroundBrush.ImageSource = await be.ApplyFilter(10);//高斯模糊等级可以自己定义
            //取主色调并应用到TagsTextBlock
            return new SolidColorBrush(GetColor.GetMajorColor(wb));
        }
        async System.Threading.Tasks.Task<SolidColorBrush> GetMajorColorAndBlur(string url, ImageBrush backgroundBrush)
        {
            WriteableBitmap wb = new WriteableBitmap(1000, 1500);
            HttpClient hc = new HttpClient();
            byte[] b = await hc.GetByteArrayAsync(url);
            using (IRandomAccessStream iras = b.AsBuffer().AsStream().AsRandomAccessStream())
            {
                await wb.SetSourceAsync(iras);
            }
            //高斯模糊
            BlurEffect be = new BlurEffect(wb);
            backgroundBrush.ImageSource = await be.ApplyFilter(10);//高斯模糊等级可以自己定义
            //取主色调并应用到TagsTextBlock
            return new SolidColorBrush(GetColor.GetMajorColor(wb));
        }

        /// <summary>
        /// 修改播放歌曲
        /// 嗯，change手快打错了，就这样吧
        /// </summary>
        /// <param name="songsItem"></param>
        /// <param name="songUrlRoot"></param>
        /// <param name="isStartPlaying"></param>
        public async void ChnagePlayingSong(SongsItem songsItem, SongUrlRoot songUrlRoot,bool isStartPlaying=true)
        {
            //添加读取歌曲进历史记录最后
            int index = currentPlayList.IndexOf(songsItem);
            playHistoryIndex.Remove(index);
            playHistoryIndex.Add(index);

            if(MainPage.songsItem!=null)
                MainPage.songsItem.isPlaying = false;
            songsItem.isPlaying = true;

            MainPage.songsItem = songsItem;
            MainPage.songUrlRoot = songUrlRoot;
            _mediaTimelineController.Pause();
            //_mediaPlayer.Pause();
            //获取专辑
            _album=await Task.Run(()=>MyClassManager.GetMAlbum(songsItem.al.id));
            if (_album == null)
                return;
            _AlbumBitmapImage = await MyClassManager.DownloadFile(new Uri(_album.album.blurPicUrl+ "?param=200y200"));
            ChangeImage();
            ChangePlayBar(_AlbumBitmapImage, songsItem.name, songsItem.ar.First().name, songsItem.al.name, songsItem.dt/1000,isStartPlaying);
            if((Application.Current as App).playingPage!=null)
                (Application.Current as App).playingPage.LoadLayout();
            if ((Application.Current as App).compactOverlayPage != null)
                (Application.Current as App).compactOverlayPage.UpdateLayout_name(songsItem.name);
            if (songUrlRoot.data.First().url == null)
            {
                NotifyPopup notifyPopup = new NotifyPopup("播放地址错误（VIP专属？)");
                notifyPopup.Show();
                return;
            }
            _mediaSource = await Task.Run(() => MediaSource.CreateFromUri(new Uri(songUrlRoot.data.First().url)));
            _mediaSource.OpenOperationCompleted += _mediaSource_OpenOperationCompleted;
            _mediaPlaybackItem = new MediaPlaybackItem(_mediaSource);
            //_mediaPlayer.Source = _mediaSource;//设置MediaPlaybackItem后无法直接设_mediaSource为mediaPlayer.Source 
            _mediaPlayer.Source = _mediaPlaybackItem;//而应该以MediaPlaybackItem为源
            //_mediaPlayer.Play();
            if (isStartPlaying)
                _mediaTimelineController.Start();
            SetLastPlayedSong();

            //修改SMTC 显示的元数据
            
            MediaItemDisplayProperties props = _mediaPlaybackItem.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Music;
            props.MusicProperties.Title = songsItem.name;
            props.MusicProperties.Artist = songsItem.ar.First().name;
            props.Thumbnail = RandomAccessStreamReference.CreateFromFile(await ApplicationData.Current.LocalFolder.TryGetItemAsync(MyClassManager.imageFilename) as StorageFile);
            _mediaPlaybackItem.ApplyDisplayProperties(props);
        }

        private async void _mediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            _duration = sender.Duration.GetValueOrDefault();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Slider_play.Maximum = _duration.TotalSeconds;
                Slider_play.StepFrequency = 1;
            });
        }

        //void ChangePlayBar(WriteableBitmap writeableBitmap,string musicName, string artistName, string albumName,int progressBarValue,int maximum)
        //{
        //    Image_playingAlbum.Source = writeableBitmap;//修改专辑图片
        //    TextBlcok_musicName.Text = musicName;
        //    TextBlcok_artistName.Text = artistName;
        //    TextBlcok_albumName.Text = albumName;
        //    //Slider_play.Maximum = maximum;
        //    //Slider_play.Value = progressBarValue;
        //    //TextBlock_currentTime.Text = progressBarValue.ToString();
        //    //TextBlock_lengthTime.Text = maximum.ToString();
        //    SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
        //}
        async void ChangePlayBar(BitmapImage bitmapImage, string musicName, string artistName, string albumName, int maximum, bool isStartPlaying = true)
        {
            Image_playingAlbum.Source = bitmapImage;//修改专辑图片
            TextBlcok_musicName.Text = musicName;
            TextBlcok_artistName.Text = artistName;
            TextBlcok_albumName.Text = albumName;
            //Slider_play.Maximum = maximum;

            //Slider_play.Value = progressBarValue;
            //TextBlock_currentTime.Text = MyClassManager.GetDt(progressBarValue);
            TextBlock_lengthTime.Text = await Task.Run(() => MyClassManager.GetDt(maximum));
            if(isStartPlaying)
                SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
            if (songsItem.isFavorite)
            {
                TextBlock_isOrnotFavorite.Text = "\xE00B";
            }
            else
            {
                TextBlock_isOrnotFavorite.Text = "\xE006";
            }
            //if (isTaskDone)
            //{
            //    isTaskDone = false;
            //    Task.Run(() => ChangePosition());
            //}
        }

        private void Button_stopOrPlay_Click(object sender, RoutedEventArgs e)
        {
            //var t = _mediaPlayer.PlaybackSession.Position;
            var state = _mediaPlayer.PlaybackSession.PlaybackState;
            if (state == MediaPlaybackState.Playing)
            {
                //_mediaPlayer.Pause();
                _mediaTimelineController.Pause();
                //SymbolIcon_stopOrPlay.Symbol = Symbol.Play;
                //isTaskDone = true;
            }
            else if (state == MediaPlaybackState.Paused)
            {
                //_mediaPlayer.Play();
                //_mediaTimelineController.Start();
                //if (_mediaTimelineController.Position.TotalMilliseconds == _duration.TotalSeconds)
                //    _mediaTimelineController.Start();
                //else
                    _mediaTimelineController.Resume();
                //SymbolIcon_stopOrPlay.Symbol = Symbol.Pause;
                //isTaskDone = false;
            }
        }


        /// <summary>
        /// 后台线程修改播放进度条
        /// 弃用
        /// </summary>
        //async void ChangePosition()
        //{
        //    while (!isTaskDone)
        //    {
        //        isSliderChangedFromAuto = true;
        //        if (_mediaPlayer.PlaybackSession.Position.TotalSeconds != _mediaPlayer.PlaybackSession.NaturalDuration.TotalSeconds)
        //        {
        //            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //            {
        //                Slider_play.Value = _mediaPlayer.PlaybackSession.Position.TotalSeconds;
        //                TextBlock_currentTime.Text = MyClassManager.GetDt((int)_mediaPlayer.PlaybackSession.Position.TotalSeconds);
        //            });
        //        }
        //        Thread.Sleep(400);
        //    }
        //}

        /// <summary>
        /// 播放列表双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            SongsItem songsItem = listBox.SelectedItem as SongsItem;
            SongUrlRoot songUrlRoot = MyClassManager.GetMusicUrl(songsItem.id);
            if (songUrlRoot == null)
                return;
            ChnagePlayingSong(songsItem, songUrlRoot);
        }

        private void Button_playOrderState_Click(object sender, RoutedEventArgs e)
        {
            switch(playOrderState)
            {
                case 0:
                    {
                        playOrderState = 1;
                        SymbolIcon_playOrderState.Symbol = Symbol.RepeatAll;
                    }
                    break;
                case 1:
                    {
                        playOrderState = 2;
                        SymbolIcon_playOrderState.Symbol = Symbol.Shuffle;
                    }
                    break;
                case 2:
                    {
                        playOrderState = 3;
                        SymbolIcon_playOrderState.Symbol = Symbol.RepeatOne;
                    }
                    break;
                case 3:
                    {
                        playOrderState = 1;
                        SymbolIcon_playOrderState.Symbol = Symbol.AlignLeft;
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
            if (playHistoryIndex.Count == 0|| playHistoryIndex.Count == 1)
                return;
            //var lastSong = currentPlayList.FirstOrDefault(p => p.id == playHistoryIndex.Last());
            //if (lastSong == null)
            //    return;
            playHistoryIndex.Remove(playHistoryIndex.Last());
            PlayNextSongs(playHistoryIndex.Last(),true);
        }

        private void Button_next_Click(object sender, RoutedEventArgs e)
        {
            if (playHistoryIndex.Count == 0)
                return;
            int index = currentPlayList.IndexOf(songsItem);
            PlayNextSongs(index);
        }

        private void Button_playList_Click(object sender, RoutedEventArgs e)
        {
            ListBox_playList.ItemsSource = currentPlayList;
        }

        private void Button_playInfo_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App).homepage!=null&&(Application.Current as App).homepage.Frame.CanGoBack)
                (Application.Current as App).homepage.Frame.GoBack();
            else
            {
                if (songsItem == null)
                    return;
                Frame_all.Navigate(typeof(PlayingPage));
            }
        }

        private async void Button_isOrNotFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (songsItem == null)
            {
                NotifyPopup notifyPopup = new NotifyPopup("无法获取音乐信息");
                notifyPopup.Show();
                return;
            }
            if (songsItem.isFavorite)//取消喜欢
            {
                if (await Task.Run(() => MyClassManager.LoveOrDontLove_songs(songsItem.id, false) == true))
                {
                    NotifyPopup notifyPopup = new NotifyPopup("已取消喜欢", "\xE00C");
                    notifyPopup.Show();
                    TextBlock_isOrnotFavorite.Text = "\xE006";
                    songsItem.isFavorite = false;
                    favoriteSongsRoot.ids.Remove(songsItem.id);
                }
                else
                {
                    NotifyPopup notifyPopup = new NotifyPopup("操作失败", "\xE10A");
                    notifyPopup.Show();
                }
            }
            else//添加为喜欢的
            {
                if (await Task.Run(() => MyClassManager.LoveOrDontLove_songs(songsItem.id, true) == true))
                {
                    NotifyPopup notifyPopup = new NotifyPopup("已添加为喜欢", "\xE00B",Colors.MediumSeaGreen);
                    notifyPopup.Show();
                    TextBlock_isOrnotFavorite.Text = "\xE00B";
                    songsItem.isFavorite = true;
                    favoriteSongsRoot.ids.Add(songsItem.id);
                }
                else
                {
                    NotifyPopup notifyPopup = new NotifyPopup("操作失败", "\xE10A");
                    notifyPopup.Show();
                }
            }
        }

        public async void IntoCompactOverlayMode()
        {
            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(340, 160);
            //if (await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay))
            //    Frame.Navigate(typeof(CompactOverlayPage),songsItem.name);
            
            if(await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions))
                Frame.Navigate(typeof(CompactOverlayPage), songsItem.name);
        }
    }
}
