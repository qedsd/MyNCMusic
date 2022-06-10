using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home : Page
    {
        public static Home Instance;
        public Home()
        {
            DataContext = new ViewModel.HomeViewModel();
            Instance = this;
            this.InitializeComponent();
            (Application.Current as App).homepage = this;
            Loaded += Home_Loaded;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private string NickName;
        private string AvatarImgIdStr;
        private async void Home_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Home_Loaded;//仅第一次启动需要
            ProgressRing_initState.IsActive = true;
            int state = await InitAsync();
            switch (state)
            {
                case 1:
                    {
                        ProgressRing_initState.IsActive = false;
                        TextBlock_initState.Text = "登录失败";
                        NotifyPopup notifyPopup = new NotifyPopup("登陆失败");
                        notifyPopup.Show();
                    }
                    break;
                case 2:
                    {
                        ProgressRing_initState.IsActive = false;
                        TextBlock_initState.Text = "获取喜欢音乐失败";
                        NotifyPopup notifyPopup = new NotifyPopup("获取喜欢歌曲列表失败");
                        notifyPopup.Show();
                    }
                    break;
                default:
                    {
                        ProgressRing_initState.IsActive = false;
                        TextBlock_initState.Text = "初始化完成";
                        ImageEx_user.Source = AvatarImgIdStr;//设为用户头像
                        TextBlock_UserName.Text = NickName;
                    }
                    break;
            }
            Frame_main.Navigate(typeof(Recommendation));
        }

        async Task<int> InitAsync()
        {
            LoginRoot loginRoot = LoginHelper.LoginAccount();
            if(loginRoot==null|| loginRoot.code!=200)
            {
                ConfigService.Uid = -1;
                return 1;
            }
            CookieHelper.WriteCookiesToDisk(ConfigService.Folder.Path+"/"+CookieHelper.SavedFileName,Http.cookies);
            ConfigService.Uid = loginRoot.account.id;
            AvatarImgIdStr = loginRoot.profile.avatarUrl;
            NickName = loginRoot.profile.nickname;
            //获取喜欢的歌曲
            var favoriteSongsRoot = await SongService.GetFavoriteSongsAsync();
            if (favoriteSongsRoot == null)
            {
                return 2;
            }
            else
            {
                favoriteSongsRoot.Ids.ForEach(p =>
                {
                    Services.PlayingService.FavoriteMusics.Add(p);
                });
            }
            return 0;
        }

        private void Button_recommendation_Click(object sender, RoutedEventArgs e)
        {
            Frame_main.Navigate(typeof(Recommendation));
        }

        private void Button_myMusicList_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigService.Uid >= 0)
                Frame_main.Navigate(typeof(MyMusicList));
            else
            {
                NotifyPopup notifyPopup = new NotifyPopup("请先登录");
                notifyPopup.Show();
            }
         }

        private void Button_myCollection_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigService.Uid >= 0)
                Frame_main.Navigate(typeof(MyCollection));
            else
            {
                NotifyPopup notifyPopup = new NotifyPopup("请先登录");
                notifyPopup.Show();
            }
        }

        private void Button_setting_Click(object sender, RoutedEventArgs e)
        {
            Frame_main.Navigate(typeof(SettingPage),null, new DrillInNavigationTransitionInfo());
        }

        private void Button_History_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigService.Uid >= 0)
                Frame_main.Navigate(typeof(PlayedRecord));
            else
            {
                NotifyPopup notifyPopup = new NotifyPopup("请先登录");
                notifyPopup.Show();
            }
        }

        private void Button_Radio_Click(object sender, RoutedEventArgs e)
        {
            if (ConfigService.Uid >= 0)
                Frame_main.Navigate(typeof(Radio));
            else
            {
                NotifyPopup notifyPopup = new NotifyPopup("请先登录");
                notifyPopup.Show();
            }
        }



        #region new
        /// <summary>
        /// 显示播放界面
        /// </summary>
        public void NavigateToPlayingPage()
        {
            MainFrame.Navigate(typeof(PlayingPage));
        }
        public void NavigateTo([In] Type sourcePageType, [In] object parameter, [In] NavigationTransitionInfo infoOverride)
        {
            Frame_main.Navigate(sourcePageType, parameter, infoOverride);
        }
        public void NavigateTo([In] Type sourcePageType, [In] object parameter)
        {
            Frame_main.Navigate(sourcePageType, parameter);
        }
        public void NavigateTo([In] Type sourcePageType)
        {
            Frame_main.Navigate(sourcePageType);
        }
        #endregion
    }
}
