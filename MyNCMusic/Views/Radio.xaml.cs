using MyNCMusic.Model;
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
    public sealed partial class Radio : Page
    {
        public Radio()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += Radio_Loaded;
        }

        private async void Radio_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar_Loading.Visibility = Visibility.Visible;
            DjRadio djRadio=await DjRadioService.GetUserCreatedRadio(ConfigService.Uid);
            if(djRadio==null)
            {
                NotifyPopup notifyPopup = new NotifyPopup("获取失败");
                notifyPopup.Show();
            }
            else
            {
                ListBox_Created.ItemsSource = djRadio.DjRadios;
            }
            ProgressBar_Loading.Visibility = Visibility.Collapsed;
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            if (pivotItem.Tag.ToString() == "1"&& ListBox_Sublist.ItemsSource==null)
            {
                ProgressBar_Loading.Visibility = Visibility.Visible;
                DjRadio djRadio = await DjRadioService.GetUserSublistRadio();
                if (djRadio == null)
                {
                    NotifyPopup notifyPopup = new NotifyPopup("获取失败");
                    notifyPopup.Show();
                }
                else
                {
                    ListBox_Sublist.ItemsSource = djRadio.DjRadios;
                }
                ProgressBar_Loading.Visibility = Visibility.Collapsed;
            }
        }

        private async void ListBox_Click(object sender, TappedRoutedEventArgs e)
        {
            var seletedItem = ((ListBox)sender).SelectedItem as DjRadiosItem;
            ProgressBar_Loading.Visibility = Visibility.Visible;
            RadioPrograms radioPrograms = await DjRadioService.GetRadioSongItem(seletedItem.Id);
            if (radioPrograms == null)
            {
                ProgressBar_Loading.Visibility = Visibility.Collapsed;
                NotifyPopup notifyPopup = new NotifyPopup("获取失败");
                notifyPopup.Show();
                return;
            }
            ProgressBar_Loading.Visibility = Visibility.Collapsed;
            Frame.Navigate(typeof(RadioDetail), radioPrograms.Programs);
        }
    }
}
