using MyNCMusic.Models;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MyNCMusic.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RadioDetail : Page
    {
        List<RadioSongItem> radioSongItems;
        public RadioDetail()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);  //将传过来的数据 类型转换一下　
            if (e.Parameter == null)
                return;
            //var list = (List<Object>)e.Parameter;
            //djRadiosItem = list.First() as DjRadiosItem;
            radioSongItems = e.Parameter as List<RadioSongItem>;
            LoadLayout();
        }

        void LoadLayout()
        {
            Image_Radio.Source =  new BitmapImage(new Uri(radioSongItems.First().Radio.PicUrl));
            TextBlock_RadioName.Text = radioSongItems.First().Radio.Name;
            Button_User.Content = radioSongItems.First().Dj.Nickname;
            TextBlock_Des.Text = radioSongItems.First().Radio.Desc;
            TextBlock_songsCount.Text = radioSongItems.Count.ToString();
            ListBox_RadioDetail.ItemsSource = radioSongItems;
        }

        private void Button_Sub_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_PlayAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AutoSuggestBox_search_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text == "")
                ListBox_RadioDetail.ItemsSource = radioSongItems;
            else
            {
                var list = radioSongItems.FindAll(p => p.Name.Contains(sender.Text));
                if (list != null && list.Count != 0)
                {
                    ListBox_RadioDetail.ItemsSource = list;
                }
            }
        }


        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void ListBox_RadioDetail_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            await PlayingService.ChangePlayingRadio((((ListBox)sender).SelectedItem as RadioSongItem).MainSong.Id, radioSongItems);
        }
    }
}
