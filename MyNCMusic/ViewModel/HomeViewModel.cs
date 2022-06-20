using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.Services;
using MyNCMusic.Views;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class HomeViewModel
    {
        public string NickName { get; set; }
        public BitmapImage AvatarImage { get; set; }
        public HomeViewModel()
        {
            Init();
        }
        private async void Init()
        {
            //账号信息
            Controls.WaitingPopup.Show();
            await ConfigService.LoadConfig();
            Controls.WaitingPopup.Hide();
            Controls.WaitingPopup.Show();
            if (!await CheckLoginStatusAsync())
            {
                Controls.WaitingPopup.Hide();
                MyUserControl.NotifyPopup.ShowError("请先登陆");
                SettingCommand.Execute(null);
                return;
            }
            Controls.WaitingPopup.Hide();
            if (LoginRoot == null || LoginRoot.code != 200)
            {
                ConfigService.Uid = -1;
                MyUserControl.NotifyPopup.ShowError("登陆失败");
                return;
            }
            CookieHelper.WriteCookiesToDisk(ConfigService.Folder.Path + "/" + CookieHelper.SavedFileName, Http.cookies);
            ConfigService.Uid = LoginRoot.account.id;
            AvatarImage = new BitmapImage(new Uri(LoginRoot.profile.avatarUrl));
            NickName = LoginRoot.profile.nickname;

            //喜欢的歌曲
            Controls.WaitingPopup.Show();
            var favoriteSongsRoot = await SongService.GetFavoriteSongsAsync();
            Controls.WaitingPopup.Hide();
            if (favoriteSongsRoot != null)
            {
                PlayingService.FavoriteMusics.Clear();
                favoriteSongsRoot.Ids.ForEach(p =>
                {
                    PlayingService.FavoriteMusics.Add(p);
                });
            }
            Home.Instance.NavigateTo(typeof(Recommendation));
        }

        /// <summary>
        /// 检查登陆状态
        /// </summary>
        /// <returns></returns>
        private async Task<LoginStatus> GetLoginStatusAsync()
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + "/login/status");
            return result == null ? null : JsonConvert.DeserializeObject<LoginStatus>(result);
        }
        private LoginRoot LoginRoot;
        private async Task<bool> CheckLoginStatusAsync()
        {
            if (Http.cookies != null && Http.cookies.GetCookies(new Uri(ConfigService.ApiUri + "/login")) != null && Http.cookies.GetCookies(new Uri(ConfigService.ApiUri + "/login")).Count != 0)//存在cookies，检查登陆状态
            {
                var status = await GetLoginStatusAsync();
                if (status != null && status.Data.account != null)
                {
                    LoginRoot = status.Data;
                    return status.Data.code == 200;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public ICommand RecommendCommand => new DelegateCommand(() =>
        {
            Home.Instance.NavigateTo(typeof(Recommendation));
        });

        public ICommand MyPlaylistCommand => new DelegateCommand(() =>
        {
            if (ConfigService.Uid >= 0)
            {
                Home.Instance.NavigateTo(typeof(MyMusicList));
            }
            else
            {
                MyUserControl.NotifyPopup.ShowError("请先登陆");
            }
        });


        public ICommand MyCollectionCommand => new DelegateCommand(() =>
        {
            if (ConfigService.Uid >= 0)
            {
                Home.Instance.NavigateTo(typeof(MyCollection));
            }
            else
            {
                MyUserControl.NotifyPopup.ShowError("请先登陆");
            }
        });


        public ICommand MyRadioCommand => new DelegateCommand(() =>
        {
            if (ConfigService.Uid >= 0)
            {
                Home.Instance.NavigateTo(typeof(RadioPage));
            }
            else
            {
                MyUserControl.NotifyPopup.ShowError("请先登陆");
            }
        });


        public ICommand HistoryCommand => new DelegateCommand(() =>
        {
            if (ConfigService.Uid >= 0)
            {
                Home.Instance.NavigateTo(typeof(PlayedRecord));
            }
            else
            {
                MyUserControl.NotifyPopup.ShowError("请先登陆");
            }
        });

        public ICommand SettingCommand => new DelegateCommand(() =>
        {
            Home.Instance.NavigateTo(typeof(SettingPage), null, new DrillInNavigationTransitionInfo());
        });
    }
}
