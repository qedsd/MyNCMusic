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
using Windows.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using MyNCMusic.Helper;
using MyNCMusic.MyUserControl;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class CompactOverlayViewModel
    {
        public Symbol PlayStstusSymbol { get; set; } = Symbol.Play;
        public Brush MainBackgroundBrush { get; set; }
        public ImageBrush MainImageBrush;
        public string MusicName { get; set; }
        public LyricStr PrevLyric { get; set; }
        public LyricStr CurLyric { get; set; }
        public LyricStr NextLyric { get; set; }
        public string FavoriteSymbolText { get; set; }
        public bool IsPlayingSong { get; set; }

        public CompactOverlayViewModel()
        {
            MainImageBrush = new ImageBrush();
            PlayingService.MediaTimelineController.StateChanged += MediaTimelineController_StateChanged;
            PlayingService.OnPlayingChanged += PlayingService_OnPlayingChanged;
            PlayingViewModel.OnUpdateCompactLyric += PlayingViewModel_OnUpdateCompactLyric;
            PlayingService.OnFavoriteChanged += PlayingService_OnFavoriteChanged;
            IsPlayingSong = PlayingService.IsPlayingSong;
            PlayStstusSymbol = Symbol.Pause;
            MusicName = PlayingService.IsPlayingSong?PlayingService.PlayingSong.Name : PlayingService.PlayingRadio.Name;
            ChangeMainBackgroundImage();
            if (PlayingService.IsPlayingSong)
            {
                if (PlayingService.PlayingSong.IsFavorite)
                {
                    FavoriteSymbolText = "\xE00B";
                }
                else
                {
                    FavoriteSymbolText = "\xE006";
                }
            }
        }

        private void PlayingService_OnFavoriteChanged(long id, bool isFavorite)
        {
            if (PlayingService.IsPlayingSong && id == PlayingService.PlayingSong.Id)
            {
                if (isFavorite)
                {
                    FavoriteSymbolText = "\xE00B";
                }
                else
                {
                    FavoriteSymbolText = "\xE006";
                }
            }
        }

        private async void PlayingViewModel_OnUpdateCompactLyric(LyricStr[] lyricStrs)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High,
            () =>
            {
                PrevLyric = lyricStrs[0];
                CurLyric = lyricStrs[1];
                NextLyric = lyricStrs[2];
            });
        }

        private void PlayingService_OnPlayingChanged(long id, string url)
        {
            IsPlayingSong = PlayingService.IsPlayingSong;
            if (PlayingService.IsPlayingSong)
            {
                MusicName = PlayingService.PlayingSong.Name;
                if (PlayingService.IsPlayingSong && id == PlayingService.PlayingSong.Id)
                {
                    if (PlayingService.FavoriteMusics.Contains(id))
                    {
                        FavoriteSymbolText = "\xE00B";
                    }
                    else
                    {
                        FavoriteSymbolText = "\xE006";
                    }
                }
            }
            else
            {
                MusicName = PlayingService.PlayingRadio.Name;
            }
            ChangeMainBackgroundImage();
        }
        /// <summary>
        /// 读取本地专辑图片修改背景图
        /// </summary>
        private async void ChangeMainBackgroundImage()
        {
            //判断Local是否有文件
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(ConfigService.ImageFilename) is StorageFile localFile)//本地有专辑图片，读取
            {
                //WriteableBitmap writeableBitmap = await FileHelper.OpenWriteableBitmapFile(localFile);
                MainImageBrush.ImageSource = PlayingService.PlayingAlbumBitmapImage;
                MainImageBrush.Stretch = Stretch.UniformToFill;
                MainBackgroundBrush = MainImageBrush;
            }
            else//本地无专辑图片
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
                MainBackgroundBrush = solidColorBrush;
            }
        }
        private async void MediaTimelineController_StateChanged(Windows.Media.MediaTimelineController sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High,
            () =>
            {
                if (sender.State == MediaTimelineControllerState.Running)
                {
                    PlayStstusSymbol = Symbol.Pause;
                }
                else
                {
                    PlayStstusSymbol = Symbol.Play;
                }
            });
        }

        public ICommand StopOrPlayCommand => new DelegateCommand(() =>
        {
            if(PlayingService.MediaTimelineController.State == MediaTimelineControllerState.Running)
            {
                PlayingService.MediaTimelineController.Pause();
            }
            else
            {
                PlayingService.MediaTimelineController.Resume();
            }
        });
        public ICommand PrevCommand => new DelegateCommand(() =>
        {
            PlayingService.PlayLast();
        });
        public ICommand NextCommand => new DelegateCommand(() =>
        {
            PlayingService.PlayNext();
        });

        public ICommand ChangeFavoriteCommand => new DelegateCommand(async() =>
        {
            if (PlayingService.PlayingSong == null)
            {
                NotifyPopup.ShowError("无法获取音乐信息");
                return;
            }
            if (PlayingService.PlayingSong.IsFavorite)//取消喜欢
            {
                if (await SongService.LoveOrDontLoveSongAsync(PlayingService.PlayingSong.Id, false) == true)
                {
                    NotifyPopup.ShowSuccess("已取消喜欢");
                    PlayingService.PlayingSong.IsFavorite = false;
                    Services.PlayingService.RemoveFavorite(PlayingService.PlayingSong.Id);
                }
                else
                {
                    NotifyPopup.ShowError("操作失败");
                }
            }
            else//添加为喜欢的
            {
                if (await SongService.LoveOrDontLoveSongAsync(PlayingService.PlayingSong.Id, true) == true)
                {
                    NotifyPopup.ShowSuccess("已添加为喜欢");
                    PlayingService.PlayingSong.IsFavorite = true;
                    Services.PlayingService.AddFavorite(PlayingService.PlayingSong.Id);
                }
                else
                {
                    NotifyPopup.ShowError("操作失败");
                }
            }
        });
    }
}
