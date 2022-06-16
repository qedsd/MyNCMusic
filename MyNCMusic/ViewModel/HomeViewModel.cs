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
            if(ConfigService.PhoneOrEmail == null || ConfigService.PhoneOrEmail == "")
            {
                return;
            }
            LoginRoot loginRoot = await LoginAccountAsync();
            Controls.WaitingPopup.Hide();
            if (loginRoot == null || loginRoot.code != 200)
            {
                ConfigService.Uid = -1;
                MyUserControl.NotifyPopup.ShowError("登陆失败");
                return;
            }
            CookieHelper.WriteCookiesToDisk(ConfigService.Folder.Path + "/" + CookieHelper.SavedFileName, Http.cookies);
            ConfigService.Uid = loginRoot.account.id;
            AvatarImage = new BitmapImage(new Uri(loginRoot.profile.avatarUrl));
            NickName = loginRoot.profile.nickname;

            //喜欢的歌曲
            Controls.WaitingPopup.Show();
            var favoriteSongsRoot = await SongService.GetFavoriteSongsAsync();
            Controls.WaitingPopup.Hide();
            if (favoriteSongsRoot != null)
            {
                favoriteSongsRoot.Ids.ForEach(p =>
                {
                    PlayingService.FavoriteMusics.Add(p);
                });
            }
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        /// <returns></returns>
        public static async Task<LoginRoot> LoginAccountAsync()
        {
            try
            {
                if (Http.cookies != null && Http.cookies.GetCookies(new Uri(ConfigService.ApiUri + "/login")) != null && Http.cookies.GetCookies(new Uri(ConfigService.ApiUri + "/login")).Count != 0)//存在cookies，检查登陆状态
                {
                    var status = await GetLoginStatusAsync();
                    if (status != null && status.Data.account != null)
                        return status.Data;
                }
            }
            catch (NullReferenceException)//上一次请求出错后记录了错误的cookie，再次读取会引发null错误
            {
                Http.cookies = null;
            }
            if (Http.cookies == null)
                Http.cookies = new System.Net.CookieContainer();
            string result = null;
            if (ConfigService.PhoneOrEmail.Contains('@'))
            {
                result = await Http.GetAsync(ConfigService.ApiUri + @"/login?email=" + ConfigService.PhoneOrEmail + "&md5_password=" + ConfigService.Password);
            }
            else
            {
                result = await Http.GetAsync(ConfigService.ApiUri + @"/login/cellphone?phone=" + ConfigService.PhoneOrEmail + "&md5_password=" + ConfigService.Password);
            }
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<LoginRoot>(result);
            }
            catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 检查登陆状态
        /// </summary>
        /// <returns></returns>
        static async Task<LoginStatus> GetLoginStatusAsync()
        {
            string result = await Http.GetAsync(ConfigService.ApiUri + "/login/status");
            return result == null ? null : JsonConvert.DeserializeObject<LoginStatus>(result);
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
