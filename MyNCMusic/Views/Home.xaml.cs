using MyNCMusic.Helper;
using MyNCMusic.Model;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static bool isFirstTimeLoad = true;
        public static SolidColorBrush mainSolidColorBrush;
        public Home()
        {
            mainSolidColorBrush = MainPage.mainSolidColorBrush;
            this.InitializeComponent();
            (Application.Current as App).homepage = this;
            Loaded += Home_Loaded;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        string nickName;
        string avatarImgIdStr;
        private async void Home_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isFirstTimeLoad)
                return;
            isFirstTimeLoad = false;
            //AcrylicBrush_mainFrame.TintColor = MainPage.backgroundBrush.Color;//acrylic背景颜色
            TextBox_account.Text = ConfigService.PhoneOrEmail;
            PasswordBox_password.Password = ConfigService.Password;
            TextBox_serverIP.Text = ConfigService.ApiUri.ToString();
            ProgressRing_initState.IsActive = true;
            int state = await Task.Run(() => Init());
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
                        ImageEx_user.Source = avatarImgIdStr;//设为用户头像
                        TextBlock_UserName.Text = nickName;
                    }
                    break;
            }
            Frame_main.Navigate(typeof(Recommendation));
        }

        int Init()
        {
            LoginRoot loginRoot = LoginHelper.LoginAccount();
            if(loginRoot==null|| loginRoot.code!=200)
            {
                ConfigService.Uid = -1;
                return 1;
            }
            CookieHelper.WriteCookiesToDisk(ConfigService.Folder.Path+"/"+CookieHelper.SavedFileName,Http.cookies);
            ConfigService.Uid = loginRoot.account.id;
            avatarImgIdStr = loginRoot.profile.avatarUrl;
            nickName = loginRoot.profile.nickname;
            //获取喜欢的歌曲
            MainPage.favoriteSongsRoot = SongService.GetFavoriteSongs();
            if (MainPage.favoriteSongsRoot == null)
            {
                
                return 2;
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
            //await ContentDialog_setting.ShowAsync();
            Frame_main.Navigate(typeof(SettingPage),null, new DrillInNavigationTransitionInfo());
        }

        private async void ContentDialog_setting_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (TextBox_account.Text == "" || PasswordBox_password.Password == ""|| TextBox_serverIP.Text=="")
            {
                args.Cancel = true;
                return;
            }
            ConfigService.ApiUri= TextBox_serverIP.Text;
            ConfigService.PhoneOrEmail = TextBox_account.Text;
            ConfigService.Password = PasswordBox_password.Password;
            ConfigService.SaveConfig();
            await CoreApplication.RequestRestartAsync(String.Empty);
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
    }
}
