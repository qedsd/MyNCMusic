using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class RadioPage : Page
    {
        public RadioPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += Radio_Loaded;
        }

        private async void Radio_Loaded(object sender, RoutedEventArgs e)
        {
            DjRadio djRadio=await DjRadioService.GetUserCreatedRadio(ConfigService.Uid);
            if(djRadio==null)
            {
                NotifyPopup.ShowError("获取失败");
            }
            else
            {
                CreatedRadioList.ItemsSource = djRadio.DjRadios;
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((Pivot)sender).SelectedIndex == 1&&SubRadioList.ItemsSource==null)
            {
                Controls.WaitingPopup.Show();
                DjRadio djRadio = await DjRadioService.GetUserSublistRadioAsync();
                Controls.WaitingPopup.Hide();
                if(djRadio!=null)
                {
                    SubRadioList.ItemsSource = djRadio.DjRadios;
                }
                else
                {
                    NotifyPopup.ShowError("获取失败");
                }
            }
        }

        private async void CreatedRadioList_OnChangedRadio(DjRadiosItem Radio)
        {
            Controls.WaitingPopup.Show();
            RadioPrograms radioPrograms = await DjRadioService.GetRadioSongItemAsync(Radio.Id);
            Controls.WaitingPopup.Hide();
            if (radioPrograms == null)
            {
                NotifyPopup.ShowError("获取失败");
            }
            else
            {
                NavigateService.NavigateToRadioDetail(radioPrograms.Programs);
            }
        }
    }
}
