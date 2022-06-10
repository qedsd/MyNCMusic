using MyNCMusic.Models;
using MyNCMusic.MyUserControl;
using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class PlayedRecord : Page
    {
        public PlayedRecord()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            Loaded += PlayedRecord_Loaded;
        }

        private async void PlayedRecord_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressBar_Loading.Visibility = Visibility.Visible;
            RecordData recordData = await Task.Run(() => RecordDataService.GetRecordData(ConfigService.Uid, 1));
            if (recordData.WeekData != null)
            {
                ListBox_Week.ItemsSource = recordData.WeekData;
            }
            else
            {
                NotifyPopup notifyPopup = new NotifyPopup("获取最近一周记录失败");
                notifyPopup.Show();
            }
            ProgressBar_Loading.Visibility = Visibility.Collapsed;
        }

        private async void Button_Artists_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            List<Artist> artists = ((CADataItem)button.DataContext).Artists as List<Artist>;
            if (artists.Count == 1)
            {
                ProgressBar_Loading.Visibility = Visibility.Visible;
                ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(artists.First().Id));
                ProgressBar_Loading.Visibility = Visibility.Collapsed;
                if (artistBaseDetailRoot == null)
                    return;
                Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
            }
        }

        private async void ListBox_Artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Artist artist = ((ListBox)sender).SelectedItem as Artist;
            if (artist == null)
                return;
            ProgressBar_Loading.Visibility = Visibility.Visible;
            ArtistBaseDetailRoot artistBaseDetailRoot = await Task.Run(() => ArtistService.GetArtistBaseDetail(artist.Id));
            ProgressBar_Loading.Visibility = Visibility.Collapsed;
            if (artistBaseDetailRoot == null)
                return;
            Frame.Navigate(typeof(ArtistHome), artistBaseDetailRoot);
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            if (pivotItem.Tag.ToString() == "1")
            {
                if (ListBox_All.ItemsSource == null)
                {
                    ProgressBar_Loading.Visibility = Visibility.Visible;
                    RecordData recordData = await Task.Run(() => RecordDataService.GetRecordData(ConfigService.Uid, 0));
                    if (recordData.AllData != null)
                    {
                        ListBox_All.ItemsSource = recordData.AllData;
                    }
                    else
                    {
                        NotifyPopup notifyPopup = new NotifyPopup("获取全部记录失败");
                        notifyPopup.Show();
                    }
                    ProgressBar_Loading.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void Button_Album_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            long id = (button.DataContext as RecordDataItem).Song.Al.Id;
            ProgressBar_Loading.Visibility = Visibility.Visible;
            AlbumRoot albumRoot=await Task.Run(()=>AlbumService.GetAlbum(id));
            ProgressBar_Loading.Visibility = Visibility.Collapsed;
            Frame.Navigate(typeof(AlbumDetail), albumRoot);
        }

        private async void ListBox_Click(object sender, DoubleTappedRoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            ProgressBar_Loading.Visibility = Visibility.Visible;
            long id= (listBox.SelectedItem as RecordDataItem).Song.Id;
            // MusicDetailRoot musicDetailRoot=await Task.Run(()=>SongService.GetMusicDetail_Get(id.ToString()));
            //if (musicDetailRoot == null || musicDetailRoot.songs == null)
            //{
            //    ProgressBar_Loading.Visibility = Visibility.Collapsed;
            //    return;
            //}
            //SongUrlRoot songUrlRoot = await Task.Run(() => SongService.GetMusicUrl(musicDetailRoot.songs.First().Id));
            //if (songUrlRoot == null)
            //{
            //    ProgressBar_Loading.Visibility = Visibility.Collapsed;
            //    return;
            //}
            //ProgressBar_Loading.Visibility = Visibility.Collapsed;
            ////修改播放列表
            //(Application.Current as App).myMainPage.currentPlayList.Clear();
            //(Application.Current as App).myMainPage.currentPlayList.Add(musicDetailRoot.songs.First());
            ////修改mainpage以触发修改正在播放的音乐
            //(Application.Current as App).myMainPage.ChnagePlayingSong(musicDetailRoot.songs.First(), songUrlRoot, (musicDetailRoot.songs.First().Id + 4) * -1);
            await PlayingService.ChangePlayingSong(id,null);
            ProgressBar_Loading.Visibility = Visibility.Collapsed;
        }
    }
}
