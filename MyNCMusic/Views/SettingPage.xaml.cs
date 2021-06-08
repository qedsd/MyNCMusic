using MyNCMusic.Helper;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public string Version;
        public SettingPage()
        {
            Version = String.Format("{0}.{1}.{2}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private async void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_account.Text == "" || PasswordBox_password.Password == "" || TextBox_serverIP.Text == "")
            {
                NotifyPopup notifyPopup = new NotifyPopup("?");
                notifyPopup.Show();
                return;
            }
            ConfigService.ApiUri = TextBox_serverIP.Text;
            ConfigService.PhoneOrEmail = TextBox_account.Text;
            ConfigService.Password = OtherHelper.Encrypt(PasswordBox_password.Password);
            ConfigService.SaveConfig();
            await CoreApplication.RequestRestartAsync(String.Empty);
        }
    }
}
